using System.Collections.Generic;
using System.Linq;
using Fusion;
using HarmonyLib;
using SG.Airlock.Network;
using UnityEngine;
using VentLib.Networking.RPC;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using VentLib.Version;
using VentLib.Version.BuiltIn;

namespace VentLib.Networking.Handshake.Patches;

[HarmonyPatch(typeof(SpawnManager), nameof(SpawnManager.PlayerJoined))]
internal static class PlayerJoinPatch
{
    private const uint VersionCheck = (uint)VentCall.VersionCheck;
    internal static readonly HashSet<int> WaitSet = new();
    private static ModRPC _modRPC = Vents.FindRPC(VersionCheck, typeof(VersionCheck), nameof(Handshake.VersionCheck.SendVersion))!;

    internal static void Postfix(SpawnManager __instance, [HarmonyArgument(0)] PlayerRef player)
    {
        if (!XRRigExtensions.LocalPlayer().IsHost()) return;
        VersionControl vc = VersionControl.Instance;
        if (!vc.Handshake) return;
        
        ModRPC rpc = Vents.FindRPC(VersionCheck, AccessTools.Method(typeof(VersionCheck), nameof(Handshake.VersionCheck.RequestVersion)))!;
        WaitSet.Add(player);
        Async.Schedule(() =>
        {
            rpc.Send([player]);
            if (vc.ResponseTimer <= 0) return;
            Async.Schedule(() =>
            {
                if (!WaitSet.Contains(player)) return;
                int targetPlayerId = player.PlayerId;
                Vents.LastSenders[VersionCheck] = Object.FindObjectsOfType<NetworkedLocomotionPlayer>().FirstOrDefault(p => p.PlayerID.PlayerId == targetPlayerId)!;
                _modRPC.InvokeTrampoline(new NoVersion());
            }, vc.ResponseTimer);
        }, NetUtils.DeriveDelay(1.5f));
    }
}