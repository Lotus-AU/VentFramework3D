using System;
using SG.Airlock;
using SG.Airlock.Network;
using VentLib.Logging;
using VentLib.Logging.Default;
using Object = UnityEngine.Object;

namespace VentLib.Vanilla.Behaviour;

public static class PlayerKillBehaviour
{
    private static readonly StandardLogger _log = LoggerFactory.GetLogger<StandardLogger>(typeof(NetworkRunnerBehaviour));
    private static NetworkedKillBehaviour _killBehaviour = null!;
    private static AirlockPeer _airlockPeer = null!;
    
    private static bool VerifyInstance()
    {
        if (IsVerified()) return true;
        _killBehaviour = Object.FindObjectOfType<NetworkedKillBehaviour>();
        _airlockPeer = Object.FindObjectOfType<AirlockPeer>();
        return IsVerified();

        bool IsVerified() => _killBehaviour != null && Object.IsNativeObjectAlive(_killBehaviour) &&
                             _airlockPeer != null && Object.IsNativeObjectAlive(_airlockPeer);
    }
    
    public static NetworkedKillBehaviour GetStateManager() => VerifyInstance() ? _killBehaviour : null!;
    public static AirlockPeer GetPeer() => VerifyInstance() ? _airlockPeer : null!;
    
    /// <summary>
    /// Kills a player and disregards kill cooldown, distance, and other factors.
    /// </summary>
    /// <param name="killer">The player who is going to kill.</param>
    /// <param name="victim">The player who is going to die.</param>
    /// <param name="wasVigilanteKill">If the kill should be classified as coming from Vigilante.</param>
    public static void RPC_KillPlayer(PlayerState killer, PlayerState victim, bool wasVigilanteKill)
    {
        if (VerifyInstance()) // Verify to make sure we have an instance of NetworkedKillBehaviour.
        {
            if (killer == null)
            {
                _log.Exception(new NullReferenceException($"'killer' parameter is null in RPC_KillPlayer."));
                return;
            }
            if (victim == null)
            {
                _log.Exception(new NullReferenceException($"'victim' parameter is null in RPC_KillPlayer."));
                return;
            }

            _killBehaviour.KillPlayer(_airlockPeer, victim, killer.PlayerId, wasVigilanteKill);
        }
        else _log.Exception(new NullReferenceException($"'_killBehaviour' is null in RPC_KillPlayer."));
    }
    
    /// <summary>
    /// Plays a green blood splatter from Infection LTM.
    /// </summary>
    /// <param name="killer">The player who is infecting.</param>
    /// <param name="victim">The player who was infected.</param>
    public static void RPC_InfectEffect(PlayerState killer, PlayerState victim)
    {
        if (VerifyInstance()) // Verify to make sure we have an instane of NetworkedKillBehaviour.
        {
            // Just some null checks.
            if (killer == null)
            {
                _log.Exception(new NullReferenceException($"'killer' parameter is null in RPC_InfectEffect."));
                return;
            }
            if (victim == null)
            {
                _log.Exception(new NullReferenceException($"'victim' parameter is null in RPC_InfectEffect."));
                return;
            }

            _killBehaviour.RPC_InfectVFX(victim.PlayerId, killer.PlayerId);
        }
        else _log.Exception(new NullReferenceException($"'_killBehaviour' is null in RPC_InfectEffect."));
    }

    /// <summary>
    /// Plays a blood splatter when getting killed.
    /// </summary>
    /// <param name="killer">The target to play splat for.</param>
    public static void RPC_KillEffect(PlayerState killer)
    {
        if (VerifyInstance()) // Verify to make sure we have an instane of NetworkedKillBehaviour.
        {
            // Just some null checks.
            if (killer == null)
            {
                _log.Exception(new NullReferenceException($"'killer' parameter is null in RPC_InfectEffect."));
                return;
            }

            _killBehaviour.RPC_KillerVFX(killer.PlayerId);
        }
        else _log.Exception(new NullReferenceException($"'_killBehaviour' is null in RPC_KillEffect."));
        
    }
}