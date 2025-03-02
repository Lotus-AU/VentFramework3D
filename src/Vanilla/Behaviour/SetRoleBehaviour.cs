using System;
using Il2CppSystem.Collections.Generic;
using SG.Airlock;
using SG.Airlock.Roles;
using VentLib.Logging;
using VentLib.Logging.Default;
using Object = UnityEngine.Object;


namespace VentLib.Vanilla.Behaviour;

public class SetRoleBehaviour
{
    private static readonly StandardLogger _log = LoggerFactory.GetLogger<StandardLogger>(typeof(NetworkRunnerBehaviour));
    private static RoleManager _roleManager = null!;

    private static bool VerifyInstance()
    {
        if (_roleManager != null && Object.IsNativeObjectAlive(_roleManager)) return true;
        _roleManager = Object.FindObjectOfType<RoleManager>();
        return _roleManager != null && Object.IsNativeObjectAlive(_roleManager);
    }
    
    public static RoleManager GetManager() => VerifyInstance() ? _roleManager : null!;
    
    /// <summary>
    /// Sets the role of a player.
    /// </summary>
    /// <param name="player">The player to change role for.</param>
    /// <param name="newRole">The new role.</param>
    /// <param name="publicChange">Whether to send the new role to every client.</param>
    public static void RPC_SetRole(PlayerState player, GameRole newRole, bool publicChange)
    {
        if (!VerifyInstance()) // Verify instance to prevent null.
            return;
        
        if (player == null) throw new NullReferenceException("'player' is null in RPC_SetRole.");
        if (publicChange)
            player.RPC_KnownGameRole(newRole); // Sends the player's role to all clients.
        else
            _roleManager.AlterPlayerRole(newRole, player.PlayerId); // Keeps the player's role hidden to other clients but the host.
    }
}