using System;
using SG.Airlock;
using SG.Airlock.Network;
using SG.Airlock.Settings;
using VentLib.Logging;
using VentLib.Vanilla.Enums;
using Object = UnityEngine.Object;

namespace VentLib.Vanilla.Behaviour;

public static class MatchSettingsBehaviour
{
    private static readonly StandardLogger _log = LoggerFactory.GetLogger<StandardLogger>(typeof(NetworkRunnerBehaviour));
    private static MatchCustomizationSettings _settings = null!;
    
    private static bool VerifyInstance()
    {
        if (_settings != null && Object.IsNativeObjectAlive(_settings)) return true;
        _settings = Object.FindObjectOfType<MatchCustomizationSettings>();
        return _settings != null && Object.IsNativeObjectAlive(_settings);
    }
    
    public static MatchCustomizationSettings GetManager() => VerifyInstance() ? _settings : null!;
    
    /// <summary>
    /// Resets the Match Settings to the default settings.
    /// </summary>
    public static void ResetToDefault()
    {
        if (VerifyInstance()) // Verify instance incase null.
            _settings.SetSettingsDefaults();
    }
    
    /// <summary>
    /// Updates a match setting based on the parameters.
    /// </summary>
    /// <param name="boolName">The setting to look for.</param>
    /// <param name="value">The new value to be changed to.</param>
    public static void UpdateSetting(VanillaBoolName boolName, bool value)
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            string settingName = "MatchCustomization_" + boolName.ToString();
            foreach (BoolSettingsItem setting in _settings.LocalBoolSettings)
            {
                if (setting.name == settingName) setting.SetValue(value);
            }
        }
    }

    /// <summary>
    /// Updates a match setting based on the parameters.
    /// </summary>
    /// <param name="floatName">The setting to look for.</param>
    /// <param name="value">The new value to be changed to.</param>
    public static void UpdateSetting(VanillaFloatName floatName, float value)
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            string settingName = "MatchCustomization_" + floatName.ToString();
            foreach (FloatSettingsItem setting in _settings.LocalFloatSettings)
            {
                if (setting.name == settingName) setting.SetValue(value);
            }
        }
    }

    /// <summary>
    /// Updates a match setting based on the parameters.
    /// </summary>
    /// <param name="intName">The setting to look for.</param>
    /// <param name="value">The new value to be changed to.</param>
    public static void UpdateSetting(VanillaIntName intName, int value)
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            string settingName = "MatchCustomization_" + intName.ToString();
            foreach (IntSettingsItem setting in _settings.LocalIntSettings)
            {
                if (setting.name == settingName) setting.SetValue(value);
            }
        }
    }

    /// <summary>
    /// Adds a custom setting, used for mods.
    /// </summary>
    /// <param name="settingName">The setting to add.</param>
    /// <param name="value">The new value to be changed to.</param>
    public static BoolSettingsItem AddCustomSetting(string settingName, bool value)
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            BoolSettingsItem setting = new();
            setting.name = settingName;
            setting.SetValue(value);

            _settings.LocalBoolSettings.Add(setting);
            return setting;
        }
        return null!;
    }

    /// <summary>
    /// Adds a custom setting, used for mods.
    /// </summary>
    /// <param name="settingName">The setting to add.</param>
    /// <param name="value">The new value to be changed to.</param>
    public static FloatSettingsItem AddCustomSetting(string settingName, float value)
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            FloatSettingsItem setting = new();
            setting.name = settingName;
            setting.SetValue(value);

            _settings.LocalFloatSettings.Add(setting);
            return setting;
        }
        return null!;
    }

    /// <summary>
    /// Adds a custom setting, used for mods.
    /// </summary>
    /// <param name="settingName">The setting to add.</param>
    /// <param name="value">The new value to be changed to.</param>
    public static IntSettingsItem AddCustomSetting(string settingName, int value)
    {
        if (VerifyInstance()) // Verify instance incase null.
        {
            IntSettingsItem setting = new();
            setting.name = settingName;
            setting.SetValue(value);

            _settings.LocalIntSettings.Add(setting);
            return setting;
        }
        return null!;
    }

    /// <summary>
    /// Gets the value of a setting.
    /// </summary>
    /// <param name="boolName">The setting to look for.</param>
    public static bool GetSettingBool(VanillaBoolName boolName)
    {
        string settingName = "MatchCustomization_" + boolName.ToString();
        if (VerifyInstance()) // Verify instance incase null.
        {
            foreach (BoolSettingsItem setting in _settings.LocalBoolSettings)
            {
                if (setting.name == settingName) return setting.GetValue();
            }
        }
        throw new NullReferenceException($"No Bool setting has the name: '{settingName}'");
    }

    /// <summary>
    /// Gets the value of a setting.
    /// </summary>
    /// <param name="floatName">The setting to look for.</param>
    public static float GetSettingFloat(VanillaFloatName floatName)
    {
        string settingName = "MatchCustomization_" + floatName.ToString();
        if (VerifyInstance()) // Verify instance incase null.
        {
            foreach (FloatSettingsItem setting in _settings.LocalFloatSettings)
            {
                if (setting.name == settingName) return setting.GetValue();
            }
        }
        throw new NullReferenceException($"No Float setting has the name: '{settingName}'");
    }

    /// <summary>
    /// Gets the value of a setting.
    /// </summary>
    /// <param name="intName">The setting to look for.</param>
    public static int GetSettingInt(VanillaIntName intName)
    {
        string settingName = "MatchCustomization_" + intName.ToString();
        if (VerifyInstance()) // Verify instance incase null.
        {
            foreach (IntSettingsItem setting in _settings.LocalIntSettings)
            {
                if (setting.name == settingName) return setting.GetValue();
            }
        }
        throw new NullReferenceException($"No Int setting has the name: '{settingName}'");
    }
}