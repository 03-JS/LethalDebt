using HarmonyLib;

namespace LethalDebt.Patches
{
    internal class TimeOfDayPatches
    {
        [HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
        [HarmonyPrefix]
        static bool FirePlayers(TimeOfDay __instance)
        {
            if (Plugin.Instance.enabled && TerminalPatches.terminal.groupCredits < 0 && Plugin.Instance.currentQuota == Plugin.Instance.quotaDeadline.Value)
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
                Plugin.Instance.currentQuota = 1;
                return false;
            }
            HUDManager.Instance.DisplayTip("REMINDER", $"You have {Plugin.Instance.quotaDeadline.Value - Plugin.Instance.currentQuota} quota(s) left to pay off your debt!");
            Plugin.Instance.currentQuota++;
            return true;
        }
    }
}
