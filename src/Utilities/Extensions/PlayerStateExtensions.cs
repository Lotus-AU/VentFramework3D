using System;
using SG.Airlock;

namespace VentLib.Utilities.Extensions;

public static class PlayerStateExtensions
{
    /// <summary>
    /// Sets the username of a player.
    /// </summary>
    /// <param name="player">The player to change.</param>
    /// <param name="name">The new name.</param>
    public static void RPC_SetNetworkName(this PlayerState player, string name)
    {
        if (player == null) throw new NullReferenceException($"'player' is null in RPC_SetNetworkName.");
        player.NetworkName.Value = name;
    }
    /// <summary>
    /// Sets the color of a player.
    /// </summary>
    /// <param name="player">The player to change.</param>
    /// <param name="id">The new color.</param>
    public static void RPC_SetColorID(this PlayerState player, int id)
    {
        if (player == null) throw new NullReferenceException($"'player' is null in RPC_SetColorID.");
        player.ColorId = id;
    }

    /// <summary>
    /// Sets the hat of a specific player.
    /// </summary>
    /// <param name="player">The player to change.</param>
    /// <param name="id">The new hat.</param>
    public static void RPC_SetHatID(this PlayerState player, int id)
    {
        if (player == null) throw new NullReferenceException($"'player' is null in RPC_SetColorID.");
        player.HatId = id;
    }

    /// <summary>
    /// Sets the gloves of a player.
    /// </summary>
    /// <param name="player">The player to change.</param>
    /// <param name="id">The new gloves.</param>
    public static void RPC_SetHandsID(this PlayerState player, int id)
    {
        if (player == null) throw new NullReferenceException($"'player' is null in RPC_SetColorID.");
        player.HandsId = id;
    }

    /// <summary>
    /// Sets the skin of a player.
    /// </summary>
    /// <param name="player">The player to change.</param>
    /// <param name="id">The new skin.</param>
    public static void RPC_SetSkinID(PlayerState player, int id)
    {
        if (player == null) throw new NullReferenceException($"'player' is null in RPC_SetColorID.");
        player.SkinId = id;
    }

    /// <summary>
    /// Revives a player if they are dead.
    /// </summary>
    /// <param name="player">The player to change.</param>
    public static void RPC_RevivePlayer(PlayerState player)
    {
        if (player == null) throw new NullReferenceException($"'player' is null in RPC_SetColorID.");
        player.IsAlive = true;
    }

    /// <summary>
    /// Shows the target player a death cutscene.
    /// </summary>
    /// <param name="player">The player to change.</param>
    /// <param name="killer"></param>
    /// <param name="wasVigilanteKill"></param>
    public static void RPC_ShowDeathCutscene(PlayerState player, PlayerState killer, bool wasVigilanteKill)
    {
        if (killer == null) throw new NullReferenceException($"'player' is null in RPC_SetColorID.");
        if (player == null) throw new NullReferenceException($"'player' is null in RPC_SetColorID.");

        player.RPC_ShowDeathAnim(player.PlayerId, killer.PlayerId, wasVigilanteKill);
    }

    /// <summary>
    /// Spawns in a fake player.
    /// </summary>
    /// <param name="player">The player to spawn.</param>
    /// <param name="color">The player's color.</param>
    /// <param name="hands">The player's glove type.</param>
    /// <param name="hat">The player's hat.</param>
    /// <param name="skin">The player's skin.</param>
    /// <param name="name">The player's name.</param>
    public static void RPC_Spawn(this PlayerState player, int color, int hands, int hat, int skin, string name)
    {
        // Null checks.
        if (player == null) throw new NullReferenceException($"'player' is null in RPC_SetColorID.");

        player.LocomotionPlayer.RPC_SpawnInitialization(color, hands, hat, skin, name, "FAKE PLAYER", "FAKE PLAYER", "FAKE PLAYER", true);
    }

    /// <summary>
    /// Spawns a player's dead body.
    /// </summary>
    /// <param name="player">The player's body to spawn.</param>
    public static void RPC_SpawnBody(this PlayerState player)
    {
        if (player == null) throw new NullReferenceException($"'player' is null in RPC_SetColorID.");
        player.LocomotionPlayer.SpawnBody();
    }
}