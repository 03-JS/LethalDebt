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
            Plugin.mls.LogDebug("Network Helper has been added");
        }

        [HarmonyPatch(typeof(GameNetworkManager), "SaveGameValues")]
        [HarmonyPostfix]
        static void SaveDeadline(GameNetworkManager __instance)
        {
            ES3.Save("LethalDebt_Deadline", Plugin.Instance.deadline, __instance.currentSaveFileName);
        }
        
        [HarmonyPatch(typeof(GameNetworkManager), "ResetSavedGameValues")]
        [HarmonyPostfix]
        static void ResetDeadline(GameNetworkManager __instance)
        {
            ES3.Save("LethalDebt_Deadline", Plugin.Instance.quotaDeadline.Value, __instance.currentSaveFileName);
        }
    }
}
