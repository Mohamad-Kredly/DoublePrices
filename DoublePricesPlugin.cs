using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace DoublePrices
{

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class DoublePricesPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "SupermarketTogether.plugins.DoublePrices";
        private const string PluginName = "double-prices";
        private const string VersionString = "1.1.0";

        // Config entry key strings
        // These will appear in the config file created by BepInEx and can also be used
        // by the OnSettingsChange event to determine which setting has changed.
        public static string KeyboardShortcutDoublePriceKey = "Double price toggle";
        public static string KeyboardShortcutRoundDownSwitchKey = "Round down switch";
        public static string KeyboardShortcutRoundDownToggleKey = "Round down toggle";

        public static string roundDownKey = "Round down";
        public static string NearestFiveKey = "Round down to nearest 0.05";
        public static string NearestTenKey = "Round down to nearest 0.10";

        // Configuration entries. Static, so can be accessed directly elsewhere in code via
        // e.g.
        // float myFloat = fuck_coolPlugin.FloatExample.Value;
        // TODO Change this code or remove the code if not required.
        public static ConfigEntry<KeyboardShortcut> KeyboardShortcutDoublePrice;
        public static ConfigEntry<KeyboardShortcut> KeyboardShortcutRoundDownSwitch;
        public static ConfigEntry<KeyboardShortcut> KeyboardShortcutRoundDownToggle;
        public static ConfigEntry<bool> roundDown;
        public static ConfigEntry<bool> NearestFive;
        public static ConfigEntry<bool> NearestTen;

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        public static bool doublePrice = true;

        public static bool notify = false;
        public static string notificationType;

        /// <summary>
        /// Initialise the configuration settings and patch methods
        /// </summary>
        private void Awake()
        {
            // Double Price setting

            KeyboardShortcutDoublePrice = Config.Bind("General",
            KeyboardShortcutDoublePriceKey,
                new KeyboardShortcut(KeyCode.Q));

            // Round down settings

            roundDown = Config.Bind("Round Down", roundDownKey, false);
            NearestFive = Config.Bind("Round Down", NearestFiveKey, true);
            NearestTen = Config.Bind("Round Down", NearestTenKey, false);

            // Round down shortcuts settings

            KeyboardShortcutRoundDownSwitch = Config.Bind("Round Down Shortucts",
            KeyboardShortcutRoundDownSwitchKey,
                new KeyboardShortcut(KeyCode.Q, KeyCode.LeftControl));

            KeyboardShortcutRoundDownToggle = Config.Bind("Round Down Shortucts",
            KeyboardShortcutRoundDownToggleKey,
                new KeyboardShortcut(KeyCode.Q, KeyCode.LeftControl, KeyCode.LeftShift));

            // Apply all of our patches
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");

            // Sets up our static Log, so it can be used elsewhere in code.
            // .e.g.
            // DoublePricesPlugin.Log.LogDebug("Debug Message to BepInEx log file");
            Log = Logger;
        }

        /// <summary>
        /// Code executed every frame. See below for an example use case
        /// to detect keypress via custom configuration.
        /// </summary>
        private void Update()
        {
            if (KeyboardShortcutDoublePrice.Value.IsDown())
            {
                doublePrice = !doublePrice;
                notificationType = "priceToggle";
                notify = true;
            }
            else if (KeyboardShortcutRoundDownSwitch.Value.IsDown())
            {
                if (NearestTen.Value)
                {
                    NearestTen.Value = false;
                    NearestFive.Value = true;
                }
                else
                {
                    NearestTen.Value = true;
                    NearestFive.Value = false;
                }
                notificationType = "roundDownSwitch";
                notify = true;
            }
            else if (KeyboardShortcutRoundDownToggle.Value.IsDown())
            {
                roundDown.Value = !roundDown.Value;
                notificationType = "roundDownToggle";
                notify = true;
            }
        }

        private void ConfigSettingChanged(object sender, System.EventArgs e)
        {
            SettingChangedEventArgs settingChangedEventArgs = e as SettingChangedEventArgs;

            // Check if null and return
            if (settingChangedEventArgs == null)
            {
                return;
            }

            // Turn off nearest 10 if both true
            if (settingChangedEventArgs.ChangedSetting.Definition.Key == NearestFiveKey || settingChangedEventArgs.ChangedSetting.Definition.Key == NearestTenKey)
            {
                if (NearestTen.Value && NearestFive.Value)
                    NearestTen.Value = false;
            }
        }
    }
}
