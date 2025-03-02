using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using SG.Airlock;
using SG.Airlock.Network;
using UnityEngine;
using VentLib.Logging;
using VentLib.Networking.Batches;
using VentLib.Networking.Helpers;
using VentLib.Networking.RPC.Attributes;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using Object = UnityEngine.Object;

namespace VentLib.Networking.RPC;

internal static class RpcManager
{
    private static StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(RpcManager));
    private static readonly Dictionary<uint, object[]> BatchArgumentStorage = new();

    internal static void Register(ModRPC rpc)
    {
        if (Vents.BuiltinRPCs.Contains(rpc.CallId) && rpc.Attribute is not VentRPCAttribute)
            throw new ArgumentException($"RPC {rpc.CallId} shares an ID with a Builtin-VentLib RPC. Please choose a different ID. (Builtin-IDs: {Vents.BuiltinRPCs.StrJoin()})");

        if (!Vents.RpcBindings.ContainsKey(rpc.CallId))
            Vents.RpcBindings.Add(rpc.CallId, new List<ModRPC>());

        Vents.RpcBindings[rpc.CallId].Add(rpc);
    }

    internal static void HandleRpc(PlayerRef playerRef, MessageReader reader)
    {
        reader.Read<string>(); // Remove Signature
        int callId = reader.Read<int>();
        int targetPlayerId = reader.Read<int>();
        
        uint customId = reader.Read<uint>();
        RpcActors actor = (RpcActors)reader.Read<byte>();
        if (!CanReceive(actor)) return;
        int senderId = reader.ReadPacked<int>();

        NetworkedLocomotionPlayer? player = GameObject.Find("PlayerState (" + senderId + ")").GetComponent<PlayerState>().LocomotionPlayer;
        if (player != null) Vents.LastSenders[customId] = player;

        if (player != null && player.PState.PlayerId == XRRigExtensions.LocalPlayer().PState.PlayerId) return;
        string sender = "Client: " + (player == null ? "?" : $"{player.PState.NetworkName.Value} ({player.PState.PlayerId}");
        string receiverType = XRRigExtensions.LocalPlayer().IsHost() ? "Host" : "NonHost";
        log.Info($"Custom RPC Received ({customId}) from \"{sender}\" as {receiverType}");
        if (!Vents.RpcBindings.TryGetValue(customId, out List<ModRPC>? rpcs))
        {
            log.Warn($"Received Unknown RPC: {customId}");
            return;
        }

        if (rpcs.Count == 0) return;


        object[]? args = null;
        if (callId == 204)
        {
            if (!HandleBatch(reader, rpcs[0], out object[] batchArgs)) return;
            args = batchArgs;
        }

        foreach (ModRPC modRPC in rpcs)
        {
            // Cases in which the client is not the correct listener
            if (!CanReceive(actor, modRPC.Receivers)) continue;
            if (!Vents.CallingAssemblyFlag(modRPC.Assembly).HasFlag(VentControlFlag.AllowedReceiver)) continue;
            args ??= ParameterHelper.Cast(modRPC.Parameters, MessageReader.Get(reader));
            modRPC.InvokeTrampoline(args);
        }
    }

    private static bool HandleBatch(MessageReader reader, ModRPC rpc, out object[] args)
    {
        uint batchId = reader.Read<uint>();
        byte argumentAmount = reader.Read<byte>();
        BatchArgumentStorage.TryAdd(batchId, new object[argumentAmount]);

        args = BatchArgumentStorage[batchId];
        byte argumentIndex = reader.Read<byte>();
        byte batchMarker = reader.Read<byte>();
        log.Trace($"Handling Batch (ID={batchId}, Marker={batchMarker}, Index={argumentIndex}, Args={argumentAmount})", "BatchRPC");
        if (batchMarker == 4)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is BatchReader batchReaderArg)
                    args[i] = batchReaderArg.Initialize(rpc.Parameters[i]);
            }

            BatchArgumentStorage.Remove(batchId);
            return true;
        }

        
        if (args[argumentIndex] is not BatchReader batchReader) 
            args[argumentIndex] = reader.ReadDynamic(rpc.Parameters[argumentIndex]);
        else batchReader.Add(reader);
        return false;
    }

    private static bool CanReceive(RpcActors actor, RpcActors localActor = RpcActors.Everyone)
    {
        return actor switch
        {
            RpcActors.None => false,
            RpcActors.Host => XRRigExtensions.LocalPlayer().IsHost() && localActor is RpcActors.Host or RpcActors.NonHosts,
            RpcActors.NonHosts => !XRRigExtensions.LocalPlayer().IsHost() && localActor is RpcActors.Everyone or RpcActors.NonHosts,
            RpcActors.LastSender => localActor is RpcActors.Everyone or RpcActors.LastSender,
            RpcActors.Everyone => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

