using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace LethalDebt.Patches
{
    internal class TimeOfDayPatches
    {
        [HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
        [HarmonyPrefix]
        static bool FirePlayers(TimeOfDay __instance)
        {
            if (Plugin.Instance.enabled && TerminalPatches.terminal.groupCredits < 0 && __instance.timesFulfilledQuota == Plugin.Instance.quotaDeadline.Value - 1) // offset by 1 because it's a prefix and it runs before timesFulfilledQuota increases
            {
                GameNetworkManager.Instance.gameHasStarted = true;
                StartOfRound.Instance.firingPlayersCutsceneRunning = true;
                StartOfRound.Instance.FirePlayersAfterDeadlineClientRpc(new int[]
                {
                    StartOfRound.Instance.gameStats.daysSpent,
                    StartOfRound.Instance.gameStats.scrapValueCollected,
                    StartOfRound.Instance.gameStats.deaths,
                    StartOfRound.Instance.gameStats.allStepsTaken
                }, false);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
        [HarmonyPostfix]
        static void DisplayTip(TimeOfDay __instance)
        {
            if (__instance.timesFulfilledQuota != Plugin.Instance.quotaDeadline.Value - 1 && TerminalPatches.terminal.groupCredits < 0) Utils.StartCoroutine(DisplayDebtReminder(__instance));
        }
        
        static IEnumerator DisplayDebtReminder(TimeOfDay Instance)
        {
            yield return new WaitForSeconds(3f);
            HUDManager.Instance.DisplayTip("REMINDER", $"You have {Plugin.Instance.quotaDeadline.Value - Instance.timesFulfilledQuota} quota(s) left to pay off your debt!\nYou currently owe ${TerminalPatches.terminal.groupCredits * Plugin.Instance.debtMultiplier.Value}");
        }
    }
}
