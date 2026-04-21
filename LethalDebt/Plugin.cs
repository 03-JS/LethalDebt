using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalDebt.Patches;
using System.Reflection;
using BepInEx.Bootstrap;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace LethalDebt
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("com.malco.lethalcompany.moreshipupgrades", DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.LethalDebt";
        private const string modName = "Lethal Debt";
        private const string modVersion = "1.1.0";

        private readonly Harmony harmony = new Harmony(modGUID);
        public static Plugin Instance;
        internal static ManualLogSource mls;

        // Networking
        public bool enabled;

        // Limits how far into the negatives you can go to avoid the credits overflowing
        public const int DEBT_LIMIT = -2147483647; // 32-bit integer limit
        
        // Config
        public ConfigEntry<string> debtColor;
        public ConfigEntry<float> debtMultiplier; 
        public ConfigEntry<int> quotaDeadline;
        public ConfigEntry<bool> applyDeathPenaltyDebt;

        void Awake()
        {
            if (Instance == null) Instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Lethal Debt is awake");

            enabled = true;
            if (Chainloader.PluginInfos.ContainsKey("com.malco.lethalcompany.moreshipupgrades")) LGUHandler.AllowOverspending();

            // Netcode Patcher
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.Name == "LGUPatches") continue;
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
                Utils.SetCreditsColorToDebt();
            };
            debtMultiplier = Config.Bind(
                "Customization", // Config section
                "Debt multiplier", // Key of this config
                1f, // Default value
                new ConfigDescription("How much money do you want to owe the Company?", new AcceptableValueRange<float>(1f, 4f)) // Description
            );
            quotaDeadline = Config.Bind(
                "Customization", // Config section
                "Deadline", // Key of this config
                1, // Default value
                new ConfigDescription("By which quota do you have to pay off your debt?", new AcceptableValueRange<int>(1, 3)) // Description
            );
            applyDeathPenaltyDebt = Config.Bind(
                "Customization", // Config section
                "Death penalty counts towards debt", // Key of this config
                true, // Default value
                "Can the amount subtracted by the death penalty make you go into debt?" // Description
            );
        }
    }
}
