using System.Reflection;
using Fusion;
using Fusion.Photon.Realtime;
using Il2CppSystem;
using SG.Airlock.Network;
using SG.Airlock.UI.TitleScreen;
using UnityEngine;
using VentLib.Logging.Default;
using VentLib.Utilities.Harmony.Attributes;

namespace VentLib.Networking;

/// <summary>
/// Adds client mods to end of code so regulars can't match up.
/// </summary>
internal class CodeManager
{
    private static string _codeExtension = string.Empty;

    public static void AddAssemblyToCode(Assembly assembly)
    {
        _codeExtension += $"_{assembly.GetName().Name}";
    }

    [QuickPrefix(typeof(NetworkRunner), nameof(NetworkRunner.StartGame))]
    public static void StartGamePrefix(NetworkRunner __instance, ref StartGameArgs args)
    {
        if (_codeExtension == string.Empty) return;
        args.SessionName += _codeExtension;
        NoDepLogger.Debug(args.SessionName);
    }
    
    [QuickPrefix(typeof(AirlockPeer), nameof(AirlockPeer.DirectJoinFindGame))]
    public static void FindGamePrefix(AirlockPeer __instance, ref string gameName)
    {
        if (_codeExtension == string.Empty) return;
        gameName += _codeExtension;
    }
}