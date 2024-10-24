using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalDebt.Patches;
using System;
using System.Reflection;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace LethalDebt
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("BMX.LobbyCompatibility", DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.LethalDebt";
        private const string modName = "Lethal Debt";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);
        public static Plugin Instance;
        internal static ManualLogSource mls;

        // Networking
        public bool enabled;

        // Limits how far into the negatives you can go to avoid the credits overflowing
        public const int DEBT_LIMIT = -100000000;

        // Config
        public ConfigEntry<string> debtColor;

        void Awake()
        {
            if (Instance == null) Instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Lethal Debt is awake");

            enabled = true;

            // Netcode Patcher
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }

            GenerateConfig();
            ApplyPatches();
        }

        void ApplyPatches()
        {
            harmony.PatchAll(typeof(TerminalPatches));
            harmony.PatchAll(typeof(StartOfRoundPatches));
            harmony.PatchAll(typeof(TimeOfDayPatches));
            harmony.PatchAll(typeof(HUDManagerPatches));
            harmony.PatchAll(typeof(GameNetworkManagerPatches));
            if (Chainloader.PluginInfos.ContainsKey("com.malco.lethalcompany.moreshipupgrades")) harmony.PatchAll(typeof(LGUPatches));
            mls.LogInfo("Patches applied!");
        }

        void GenerateConfig()
        {
            debtColor = Config.Bind(
                "Customization", // Config section
                "Debt Color", // Key of this config
                "#ff0000", // Default value
                "Changes the color of the displayed credits in the terminal when they're in the negatives." +
                "\nMake sure you separate the different values with a comma and a blank space." // Description
            );
            debtColor.SettingChanged += (obj, args) =>
            {
                Utils.ChangeTerminalCreditsColor(debtColor.Value);
            };
        }

        public void LogToConsole(string message, string logType = "")
        {
            switch (logType.ToLower())
            {
                case "warn":
                    mls.LogWarning(message);
                    break;
                case "error":
                    mls.LogError(message);
                    break;
                case "debug":
                    mls.LogDebug(message);
                    break;
                default:
                    mls.LogInfo(message);
                    break;
            }
        }
    }
}
