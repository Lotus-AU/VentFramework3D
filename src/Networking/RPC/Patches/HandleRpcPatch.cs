using System.Collections.Generic;
using Fusion;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using VentLib.Networking.Handshake;

namespace VentLib.Networking.RPC.Patches;

[HarmonyPatch(typeof(NetworkRunner), nameof(NetworkRunner.Fusion_Simulation_ICallbacks_OnReliableData))]
public class HandleRpcPatch
{
    internal static HashSet<string> AumUsers = new();
    private const uint VersionCheck = (uint)VentCall.VersionCheck;
    private static ModRPC _modRPC = Vents.FindRPC(VersionCheck, typeof(VersionCheck), nameof(Handshake.VersionCheck.SendVersion))!;

    public static bool Prefix(NetworkRunner __instance, [HarmonyArgument(0)] PlayerRef player, [HarmonyArgument(1)] Il2CppStructArray<byte> dataArray)
    {
        string rpcStr = string.Empty;
        if (!rpcStr.StartsWith(NetworkRules.VentSignature)) return true;
        RpcManager.HandleRpc(player, new MessageReader(rpcStr));
        return false;
    }
}