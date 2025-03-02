using System.Collections.Generic;
using Fusion;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using VentLib.Logging.Default;
using VentLib.Networking.Handshake;
using VentLib.Utilities;

namespace VentLib.Networking.RPC.Patches;

[HarmonyPatch(typeof(NetworkRunner), nameof(NetworkRunner.Fusion_Simulation_ICallbacks_OnReliableData))]
public class HandleRpcPatch
{
    private const uint VersionCheck = (uint)VentCall.VersionCheck;
    private static ModRPC _modRPC = Vents.FindRPC(VersionCheck, typeof(VersionCheck), nameof(Handshake.VersionCheck.SendVersion))!;

    public static bool Prefix(NetworkRunner __instance, [HarmonyArgument(0)] PlayerRef player, [HarmonyArgument(1)] Il2CppStructArray<byte> dataArray)
    {
        string rpcStr = Converter.ByteArrayToString(dataArray);
        if (!rpcStr.StartsWith(NetworkRules.VentSignature)) return true;
        
        #if DEBUG
            NoDepLogger.Debug($"Rpc Recieved: {rpcStr} | Size: {dataArray.Length}");
        #endif
        
        RpcManager.HandleRpc(player, new MessageReader(rpcStr));
        return false;
    }
}