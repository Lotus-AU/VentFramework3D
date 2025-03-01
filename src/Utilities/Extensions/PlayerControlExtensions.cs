using Fusion;
using SG.Airlock.Network;
using SG.Airlock.XR;
using UnityEngine;
using VentLib.Version;
using VentLib.Version.BuiltIn;

namespace VentLib.Utilities.Extensions;

public static class PlayerControlExtensions
{
    public static bool IsHost(this XRRig player) => player != null && IsHost(player.PState.PlayerId);
    
    public static bool IsHost(this NetworkedLocomotionPlayer player) => player != null && IsHost(player._playerState.PlayerId);
    public static bool IsModded(this NetworkedLocomotionPlayer player) => player != null && IsModded(player._playerState.PlayerId);

    public static bool IsHost(this PlayerRef player)
    {
        AirlockNetworkRunner networkManager = Object.FindObjectOfType<AirlockNetworkRunner>();
        if (networkManager != null) return networkManager.IsHostPlayer(player);
        return false;
    }
    public static bool IsModded(this PlayerRef player) => VersionControl.Instance.GetPlayerVersion(player.PlayerId) is not NoVersion;
}