using System;
using System.Collections.Generic;
using System.Linq;
using SG.Airlock;
using SG.Airlock.Sabotage;
using UnityEngine.SceneManagement;
using VentLib.Logging;
using VentLib.Logging.Default;
using Object = UnityEngine.Object;

namespace VentLib.Vanilla.Behaviour;

public static class GameStateBehaviour
{
    private static readonly StandardLogger _log = LoggerFactory.GetLogger<StandardLogger>(typeof(NetworkRunnerBehaviour));
    private static GameStateManager _stateManager = null!;

    private static bool VerifyInstance()
    {
        if (_stateManager != null && Object.IsNativeObjectAlive(_stateManager)) return true;
        _stateManager = Object.FindObjectOfType<GameStateManager>();
        return _stateManager != null && Object.IsNativeObjectAlive(_stateManager);
    }
    
    public static GameStateManager GetManager() => VerifyInstance() ? _stateManager : null!;
    
    /// <summary>
    /// Opens the lobby doors.
    /// </summary>
    public static void RPC_OpenLobbyDoors()
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            _stateManager.RPC_ToggleLobbyDoors(false);
        }
    }

    /// <summary>
    /// Closes the lobby doors.
    /// </summary>
    public static void RPC_CloseLobbyDoors()
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            _stateManager.RPC_ToggleLobbyDoors(true);
        }
    }

    /// <summary>
    /// If true, the game is in a state where players can do tasks.
    /// </summary>
    /// <returns></returns>
    public static bool InTasksState()
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            return _stateManager.InTaskState();
        }
        return false;
    }

    /// <summary>
    /// If true, the game is in a meeting.
    /// </summary>
    /// <returns></returns>
    public static bool InVotingState()
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            return _stateManager.InVotingState();
        }
        return false;
    }

    /// <summary>
    /// If true, the game is not in progress.
    /// </summary>
    /// <returns></returns>
    public static bool InPreGameState()
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            return _stateManager.InLobbyState();
        }
        return false;
    }

    /// <summary>
    /// If true, the client is currently doing the tutorial.
    /// </summary>
    /// <returns></returns>
    public static bool InTutorial()
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            return _stateManager.InTutorialMode();
        }
        return false;
    }

    /// <summary>
    /// Calls a sabotage.
    /// </summary>
    /// <param name="sabotage">The type of sabotage to call.</param>
    public static void RPC_CallSabotage(Enums.VanillaSabotage sabotage)
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            SabotageManager sabotageManager = Object.FindObjectOfType<SabotageManager>(); // Get SabotageManager for calling sabotages.

            if (sabotage == Enums.VanillaSabotage.Reactor)
            {
                if (SceneManager.GetActiveScene().name.Contains("Polus")) // Polus flipped sabotages, I don't know why.
                    sabotageManager.RPC_SendSabotageToAll(2, -1);
                else 
                    sabotageManager.RPC_SendSabotageToAll(0, -1);
            }
            if (sabotage == Enums.VanillaSabotage.Lights) sabotageManager.RPC_SendSabotageToAll(1, -1);
            
            if (sabotage == Enums.VanillaSabotage.Oxygen)
            {
                if (SceneManager.GetActiveScene().name.Contains("Polus")) // Polus flipped sabotages, I don't know why.
                    sabotageManager.RPC_SendSabotageToAll(0, -1);
                else 
                    sabotageManager.RPC_SendSabotageToAll(2, -1);
            }
        } else _log.Exception(new NullReferenceException($"'_stateManager' parameter is null in RPC_CallSabotage."));
    }
    
    /// <summary>
    /// Gets a list of all players currently connected.
    /// </summary>
    /// <returns>A list of <see cref="SG.Airlock.PlayerState"/> representing players.</returns>
    public static List<PlayerState> GetConnectedPlayers(bool ignoreSpectators = false)
    {
        List<PlayerState> result = new List<PlayerState>(); // Create a list for our connected players.

        foreach (PlayerState player in Object.FindObjectsOfType<PlayerState>())
        {
            if (player == null) continue; // Incase the code decides to be awkward. It's happened to me before lol.
            if (!player.IsConnected) continue; // Incase player hasn't connected.
            if (player.IsSpectating && ignoreSpectators) continue; // Ignore people who have joined mid-match.

            result.Add(player);
        }

        return result;
    }

    public static PlayerState GetPlayerState(int playerId) => GetConnectedPlayers().FirstOrDefault(ps => ps.PlayerId == playerId)!;
}