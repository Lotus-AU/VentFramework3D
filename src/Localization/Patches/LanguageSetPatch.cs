using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SG.Airlock.Localization;
using SG.Airlock.TitleScreen;
using SG.Airlock.UI;
using SG.Airlock.UI.TitleScreen;
using VentLib.Logging;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Harmony.Attributes;

namespace VentLib.Localization.Patches;

[HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.SetLanguage))]
public class LanguageSetPatch
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(LanguageSetPatch));
    private static List<Action<bool>> _actions = new();
    private static bool listenForChanges;
    
    internal static string CurrentLanguage = "English";
    
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(LocalizationManager __instance, LanguageKey newLanguage)
    {
        if (!listenForChanges) return;
        log.Info($"Loaded Language: {newLanguage.SystemLanguage.ToString()}");
        CurrentLanguage = newLanguage.SystemLanguage.ToString();
        Localizer.Localizers.Values.ForEach(l => l.CurrentLanguage = CurrentLanguage);
        Localizer.Reload();

        _actions.ForEach(a => a.Invoke(false));
    }

    [QuickPrefix(typeof(TitleStartup), nameof(TitleStartup.Start))]
    private static void StartPrefix(TitleStartup __instance)
    {
        listenForChanges = true;
        CurrentLanguage = __instance._currentLanguage.Value.SystemLanguage.ToString();
        log.Info($"Loaded Language: {CurrentLanguage}");
        Localizer.Localizers.Values.ForEach(l => l.CurrentLanguage = CurrentLanguage);
        Localizer.Reload();

        _actions.ForEach(a => a.Invoke(true));
    }

    /// <summary>
    /// Add a callback to be run when the Language changes.
    /// </summary>
    /// <param name="action">The function to run. The bool is whether this is the first time.</param>
    public static void AddCallback(Action<bool> action) => _actions.Add(action);
}