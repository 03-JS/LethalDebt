using HarmonyLib;
using Unity.Netcode;

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
