using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

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
            if (TerminalPatches.terminal.groupCredits < 0)
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
                    TerminalPatches.terminal.groupCredits += (int)((float)groupCredits * (num / 2.5f));
                }
                __instance.statsUIElements.penaltyAddition.text = string.Format("{0} casualties: -{1}%\n({2} bodies recovered)", playersDead, 5f * 100f * (float)(playersDead - bodiesInsured), bodiesInsured);
                __instance.statsUIElements.penaltyTotal.text = string.Format("DUE: ${0}", groupCredits - TerminalPatches.terminal.groupCredits);
                Debug.Log(string.Format("New group credits after penalty: {0}", TerminalPatches.terminal.groupCredits));
                return false;
            }
            return true;
        }
    }
}
