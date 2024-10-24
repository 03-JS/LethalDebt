using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;

namespace LethalDebt.Patches
{
    internal class GameNetworkManagerPatches
    {
        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        [HarmonyPostfix]
        private static void StartPatch(GameNetworkManager __instance)
        {
            __instance.gameObject.AddComponent<NetworkHelper>();
            __instance.gameObject.AddComponent<NetworkObject>();
            Plugin.Instance.LogToConsole("Network Helper has been added", "debug");
        }
    }
}
