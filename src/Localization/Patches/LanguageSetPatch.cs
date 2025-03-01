using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SG.Airlock.UI;
using VentLib.Logging;
using VentLib.Utilities.Extensions;

namespace VentLib.Localization.Patches;

[HarmonyPatch(typeof(UILanguageSelector), nameof(UILanguageSelector.SetLanguage))]
internal class LanguageSetPatch
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(LanguageSetPatch));
    internal static string CurrentLanguage;
    
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix(UILanguageSelector __instance)
    {
        var lang = __instance.GetLanguageKey(__instance._currentIndex);
        
        log.Info($"Loaded Language: {lang.SystemLanguage}");
        CurrentLanguage = lang.SystemLanguage.ToString();
        Localizer.Localizers.Values.ForEach(l => l.CurrentLanguage = CurrentLanguage);
    }
}