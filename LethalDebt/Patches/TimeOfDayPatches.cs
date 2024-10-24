using HarmonyLib;

namespace LethalDebt.Patches
{
    internal class TimeOfDayPatches
    {
        [HarmonyPatch(typeof(TimeOfDay), "SetNewProfitQuota")]
        [HarmonyPrefix]
        static bool FirePlayers(TimeOfDay __instance)
        {
            if (Plugin.Instance.enabled && TerminalPatches.terminal.groupCredits < 0)
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
    }
}
