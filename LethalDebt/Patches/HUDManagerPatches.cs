using HarmonyLib;
using UnityEngine;

namespace LethalDebt.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatches
    {
        /*
         * Not needed currently but still nice to have it written in case I do need it
        [HarmonyPatch("ApplyPenalty")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Iterate through the IL code
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_1 && codes[i + 1].operand.ToString().Contains("groupCredits"))
                {
                    for (int j = 0; j < 4; j++)
                    {
                        codes[i + j].opcode = OpCodes.Nop;
                    }
                    break;
                }
            }

            return codes.AsEnumerable();
        }
        */

        [HarmonyPatch("ApplyPenalty")]
        [HarmonyPrefix]
        static bool CalculatePenaltyWithDebt(HUDManager __instance, int playersDead, int bodiesInsured)
        {
            if (/* TerminalPatches.terminal.groupCredits < 0 && */ Plugin.Instance.applyDeathPenaltyDebt.Value)
            {
                float num = 0.2f;
                int groupCredits = TerminalPatches.terminal.groupCredits;
                bodiesInsured = Mathf.Max(bodiesInsured, 0);
                for (int i = 0; i < playersDead - bodiesInsured; i++)
                {
                    TerminalPatches.terminal.groupCredits += (int)(groupCredits * num);
                }
                for (int j = 0; j < bodiesInsured; j++)
                {
                    TerminalPatches.terminal.groupCredits += (int)(groupCredits * (num / 2.5f));
                }
                __instance.statsUIElements.penaltyAddition.text = $"{playersDead} casualties: -{5f * 100f * (playersDead - bodiesInsured)}%\n({bodiesInsured} bodies recovered)";
                __instance.statsUIElements.penaltyTotal.text = $"DUE: ${groupCredits - TerminalPatches.terminal.groupCredits}";
                Plugin.mls.LogDebug($"New group credits after penalty: {TerminalPatches.terminal.groupCredits}");
                return false;
            }
            return true;
        }
    }
}
