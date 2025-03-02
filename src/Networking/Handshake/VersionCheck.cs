using System.Linq;
using HarmonyLib;
using SG.Airlock.Network;
using UnityEngine;
using VentLib.Logging;
using VentLib.Networking.RPC;
using VentLib.Networking.RPC.Attributes;
using VentLib.Utilities;
using VentLib.Networking.Handshake.Patches;
using VentLib.Version;
using VentLib.Version.BuiltIn;

namespace VentLib.Networking.Handshake;

public class VersionCheck
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(VersionCheck));
    // This is the sender version of this Rpc. In order to fully utilize it you must make your own handler.
    [VentRPC(VentCall.VersionCheck, RpcActors.Host, RpcActors.NonHosts)]
    public static void RequestVersion()
    {
        SendVersion(VersionControl.Instance.Version ?? new NoVersion());
    }

    [VentRPC(VentCall.VersionCheck, RpcActors.NonHosts, RpcActors.LastSender)]
    public static void SendVersion(Version.Version version)
    {
        NetworkedLocomotionPlayer? lastSender = Vents.GetLastSender((uint)VentCall.VersionCheck);
        if (lastSender == null) return;
        log.Info($"Received Version: \"{version.ToSimpleName()}\" from Player {lastSender.PState._cachedName}");
        VersionControl vc = VersionControl.Instance;

        if (lastSender != null)
            PlayerJoinPatch.WaitSet.Remove(lastSender.PlayerID);

        if (vc.HandshakeFilter == null)
        {
            log.Warn("Mod hasn't implemented handshake filter! Please do so!");
            return;
        }
        
        HandshakeResult action = vc.HandshakeFilter.Invoke(version);
        vc.VersionHandles
            .Where(h => h.Item1.HasFlag(action is HandshakeResult.PassDoNothing ? ReceiveExecutionFlag.OnSuccessfulHandshake : ReceiveExecutionFlag.OnFailedHandshake))
            .Do(h => h.Item2.Invoke(version, lastSender!));
        
        HandleAction(action, lastSender);
    }

    private static void HandleAction(HandshakeResult action, NetworkedLocomotionPlayer? player)
    {
        if (player == null) return;
        switch (action)
        {
            case HandshakeResult.DisableRPC:
                Vents.BlockClient(Vents.RootAssemby, player.PlayerID.PlayerId);
                break;
            case HandshakeResult.Kick:
                ModerationManager moderationManager = Object.FindObjectOfType<ModerationManager>();
                if (moderationManager != null) moderationManager.RPC_KickPlayer(player.PlayerID);
                break;
            case HandshakeResult.Ban:
                ModerationManager moderationManager2 = Object.FindObjectOfType<ModerationManager>();
                if (moderationManager2 != null) moderationManager2.RPC_KickPlayer(player.PlayerID); // no ban rpc :(
                break;
            case HandshakeResult.PassDoNothing:
                VersionControl.Instance.PassedClients.Add(player.PlayerID.PlayerId);
                break;
            case HandshakeResult.FailDoNothing:
            default:
                break;
        }
        
    }
}