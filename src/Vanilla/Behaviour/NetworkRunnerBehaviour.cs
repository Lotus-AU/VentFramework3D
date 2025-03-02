using System;
using Fusion;
using SG.Airlock;
using UnityEngine;
using SG.Airlock.Network;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using VentLib.Logging;
using VentLib.Logging.Default;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace VentLib.Vanilla.Behaviour;

public static class NetworkRunnerBehaviour
{
    private static readonly StandardLogger _log = LoggerFactory.GetLogger<StandardLogger>(typeof(NetworkRunnerBehaviour));
    private static AirlockNetworkRunner _networkRunner = null!;

    private static bool VerifyInstance()
    {
        if (_networkRunner != null && Object.IsNativeObjectAlive(_networkRunner)) return true;
        _networkRunner = Object.FindObjectOfType<AirlockNetworkRunner>();
        return _networkRunner != null && Object.IsNativeObjectAlive(_networkRunner);
    }
    
    public static AirlockNetworkRunner GetManager() => VerifyInstance() ? _networkRunner : null!;
    
    /// <summary>
    /// Gets available network input from a player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static NetworkInput? GetInputFromPlayer(PlayerState player)
    {
        if (VerifyInstance()) // Verify instance to avoid null.
        {
            // Null checks.
            if (player == null)
            {
                _log.Exception($"'player' array is null or empty in GetInputFromPlayer.");
                return null;
            }

            // Get Input from Network Runner.
            Il2CppSystem.Nullable<NetworkInput> input = _networkRunner.GetRawInputForPlayer(player.PlayerId);

            if (input == null) return null;

            return input.Value;
        }

        return null;
    }
    
    /// <summary>
    /// Spawns a Network Object from a baked NetworkObject or a prefab with a NetworkObject component.
    /// </summary>
    /// <param name="obj">The object to spawn.</param>
    /// <param name="position">The target position to be placed. (Must have <see cref="Fusion.NetworkTransform">NetworkTransform</see> Component)</param>
    /// <param name="rotation">The target rotation to be aimed. (Must have <see cref="Fusion.NetworkTransform">NetworkTransform</see> Component)</param>
    /// <param name="playerGivenInputAuthority">The player who can control this object.</param>
    /// <param name="syncPhysics">Whether physics for this object syncs with other clients.</param>
    public static void Spawn(NetworkObject obj, Vector3 position, Quaternion rotation, PlayerState playerGivenInputAuthority, bool syncPhysics = true)
    {
        if (!VerifyInstance()) // Verify instance to avoid errors.
            return;
        
        // Null checks. Default inputAuthority to host if PlayerState is null.
        int inputAuthority = 9;
        if (playerGivenInputAuthority != null)
        {
            inputAuthority = playerGivenInputAuthority.PlayerId;
        }

        // Only objects with a NetworkTransform can be moved anywhere.
        if (obj.GetComponent<NetworkTransform>() == null)
        {
            position = obj.transform.position;
            rotation = obj.transform.rotation;
        }

        // Run the spawn method with some other junk lol.
        _networkRunner.Spawn(
            obj,
            new Il2CppSystem.Nullable<Vector3>(position),
            new Il2CppSystem.Nullable<Quaternion>(rotation),
            new Il2CppSystem.Nullable<PlayerRef>(inputAuthority),
            new NetworkRunner.OnBeforeSpawned(System.IntPtr.Zero),
            new Il2CppSystem.Nullable<NetworkObjectPredictionKey>(),
            syncPhysics);
    }
    
    /// <summary>
    /// Shutdown the NetworkRunner, effectively closing a lobby if you are in one.
    /// </summary>
    /// <param name="deleteObject">Whether to delete the NetworkRunner.</param>
    /// <param name="reason">The reason to shut down.</param>
    /// <param name="force">If the game should forcibly close.</param>
    public static void Shutdown(bool deleteObject, ShutdownReason reason, bool force)
    {
        if (VerifyInstance()) // Verify instance to prevent null.
            _networkRunner.Shutdown(deleteObject, reason, force);
        
    }

    /// <summary>
    /// Same as Shutdown() but with an exception parameter.
    /// </summary>
    /// <param name="exception">The exception to show.</param>
    public static void ShutdownWithException(Il2CppSystem.Exception exception)
    {
        if (VerifyInstance()) // Verify instance to prevent null.
            _networkRunner.ShutdownAndBuildResult(exception);
        
    }

    /// <summary>
    /// Change the Scene of a lobby for all clients and host.
    /// </summary>
    /// <param name="newScene">The scene to change to.</param>
    public static void ChangeScene(SceneRef newScene)
    {
        if (VerifyInstance()) // Verify instance to prevent null.
            _networkRunner.SetActiveScene(newScene);
    }

    /// <summary>
    /// Sends byte data to the host/server.
    /// </summary>
    /// <param name="data">The data to send.</param>
    public static void SendDataToHost(IEnumerable<byte> data) => SendDataToHost(data.ToArray());

    /// <summary>
    /// Sends byte data to the host/server.
    /// </summary>
    /// <param name="data">The data to send (in bytes).</param>
    public static void SendDataToHost(byte[] data)
    {
        if (VerifyInstance()) // Verify instance to prevent null.
        {
            // Null checks.
            if (data.Length == 0)
            {
                _log.Exception($"'data' array is null or empty in SendDataToHost.");
                return;
            }

            // Convert the data to Il2CppStructArray and send it.
            Il2CppStructArray<byte> il2CppData = new Il2CppStructArray<byte>(data);
            _networkRunner.SendReliableDataToServer(il2CppData);
        }
    }
    
    /// <summary>
    /// When LocalPlayer is host, you can send data to a specific client.
    /// </summary>
    /// <param name="receiver">The player who receives this information.</param>
    /// <param name="data"></param>
    public static void SendDataToClientAsHost(PlayerState receiver, IEnumerable<byte> data) => SendDataToClientAsHost(receiver, data.ToArray());

    /// <summary>
    /// When LocalPlayer is host, you can send data to a specific client.
    /// </summary>
    /// <param name="receiver">The player who receives this information.</param>
    /// <param name="data">The data to send.</param>
    public static void SendDataToClientAsHost(PlayerState receiver, byte[] data)
    {
        if (!XRRigExtensions.LocalPlayer().IsHost())
        {
            _log.Exception($"Cannot send info to Clients while not Host! Caller: {Mirror.GetCaller()?.Name}");
            return;
        }
        if (VerifyInstance()) // Verify instance to prevent null.
        {
            // Null checks.
            if (receiver == null)
            {
                _log.Exception($"'receiver' array is null or empty in SendDataToClientAsHost.");
                return;
            }

            if (receiver.PlayerId == XRRigExtensions.LocalPlayer().PState.PlayerId)
            {
                _log.Exception($"Cannot send info to Host, while being Host! Caller: {Mirror.GetCaller()?.Name}");
                return;
            }

            Il2CppStructArray<byte> il2CppData = new Il2CppStructArray<byte>(data);
            _networkRunner.SendReliableDataToPlayer(receiver.PlayerId, il2CppData);
        }
    }
    
    /// <summary>
    /// Gets the ping of a client.
    /// </summary>
    /// <param name="player">The player to get ping for.</param>
    /// <returns>Ping in  ms.</returns>
    public static double GetPingForClient(PlayerState player)
    {
        if (VerifyInstance()) // Verify instance incase null.
            return _networkRunner.Simulation.GetPlayerRtt(player.PlayerId);
        return 0;
    }

    /// <summary>
    /// Gets the ping of the LocalPlayer.
    /// </summary>
    /// <returns>Ping in ms.</returns>
    public static double GetPing()
    {
        if (VerifyInstance()) // Verify instance incase null.
            return _networkRunner.Simulation.GetPlayerRtt(_networkRunner.LocalPlayer);
        return 0;
    }
}