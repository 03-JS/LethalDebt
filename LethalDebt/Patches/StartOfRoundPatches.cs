using BepInEx.Bootstrap;
using HarmonyLib;

namespace LethalDebt.Patches
{
    internal class StartOfRoundPatches
    {
        [HarmonyPatch(typeof(StartOfRound), "OnClientConnect")]
        [HarmonyPrefix]
        static void DisableClient(StartOfRound __instance)
        {
            if (!__instance.IsHost)
            {
                Plugin.Instance.enabled = false;
                if (Chainloader.PluginInfos.ContainsKey("com.malco.lethalcompany.moreshipupgrades")) LGUHandler.DisallowOverspending();
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "OnClientConnect")]
        [HarmonyPostfix]
        static void EnableClientDebt(StartOfRound __instance)
        {
            NetworkHelper.Instance.EnableClientRpc();
        }
    }
}
