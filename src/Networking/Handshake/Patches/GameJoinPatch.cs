using System.Reflection;
using Fusion;
using Fusion.Photon.Realtime;
using HarmonyLib;
using SG.Airlock.Network;
using VentLib.Logging;
using VentLib.Networking.RPC.Patches;
using VentLib.Utilities.Extensions;
using VentLib.Version;
using VentLib.Version.BuiltIn;

namespace VentLib.Networking.Handshake.Patches;

[HarmonyPatch(typeof(SpawnManager), nameof(SpawnManager.PlayerJoined))]
internal static class GameJoinPatch
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(GameJoinPatch));
    public static void Prefix(SpawnManager __instance, PlayerRef player)
    {
        if (player != XRRigExtensions.LocalPlayer().PState._playerId) return;
        HandleRpcPatch.AumUsers.Clear();
        foreach (Assembly assembly in Vents.RegisteredAssemblies.Keys)
        {
            Vents.RegisteredAssemblies[assembly] = VentControlFlag.AllowedReceiver | VentControlFlag.AllowedSender;
            Vents.BlockedReceivers[assembly] = null;
            Vents.VersionControl.PassedClients.Clear();
            Vents.VersionControl.PlayerVersions.Clear();
            Vents.VersionControl.PlayerVersions[0] = VersionControl.Instance.Version ?? new NoVersion();
        }
        log.Info("Refreshed Assembly Flags", "VentLib");
    }
}