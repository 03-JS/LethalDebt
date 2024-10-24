using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalDebt.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatches
    {
        public static Terminal terminal;
        private static Color terminalCreditsColor;

        [HarmonyPatch("LoadNewNodeIfAffordable")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Iterate through the IL code to find the code that prevents players from buying things they can't afford
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldarg_0 &&
                    codes[i + 1].opcode == OpCodes.Ldfld &&
                    codes[i + 1].operand.ToString().Contains("groupCredits"))
                {
                    for (int j = 0; j < 20; j++)
                    {
                        codes[i + j].opcode = OpCodes.Nop; // Adjust index for the block
                    }
                    Plugin.Instance.LogToConsole("Purchasing restrictions removed", "debug");
                    break;
                }
            }

            // Iterate through the IL code to find the Math.Clamp calls and remove the 0 limit from both
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_0 && codes[i + 1].opcode == OpCodes.Ldc_I4 && Convert.ToInt64(codes[i + 1].operand) == 10000000)
                {
                    codes[i].opcode = OpCodes.Ldc_I4;
                    codes[i].operand = Plugin.DEBT_LIMIT;
                    Plugin.Instance.LogToConsole("New min credit limit set", "debug");
                    // break;
                }
            }

            return codes.AsEnumerable();
        }

        [HarmonyPatch("LoadNewNodeIfAffordable")]
        [HarmonyPrefix]
        static bool PreventGriefing(TerminalNode node, Terminal __instance, ref int ___totalCostOfItems)
        {
            if (!Plugin.Instance.enabled)
            {
                if (__instance.groupCredits < ___totalCostOfItems && (node.buyVehicleIndex == -1 || !__instance.hasWarrantyTicket))
                {
                    __instance.LoadNewNode(__instance.terminalNodes.specialNodes[2]);
                    return false;
                }
            }
            if (__instance.groupCredits - ___totalCostOfItems <= Plugin.DEBT_LIMIT)
            {
                __instance.LoadNewNode(__instance.terminalNodes.specialNodes[2]);
                return false;
            }
            return true;
        }

        [HarmonyPatch("LoadNewNodeIfAffordable")]
        [HarmonyPostfix]
        static void ChangeCreditsColorAfterPurchase(Terminal __instance)
        {
            Utils.ChangeTerminalCreditsColor(Plugin.Instance.debtColor.Value);
        }
        
        [HarmonyPatch("BeginUsingTerminal")]
        [HarmonyPostfix]
        static void ChangeCreditsColorOnTerminalOpen(Terminal __instance)
        {
            if (terminal.groupCredits >= 0) terminalCreditsColor = terminal.topRightText.color;
            Utils.ChangeTerminalCreditsColor(Plugin.Instance.debtColor.Value);
        }

        // Not really terminal patches but they kinda count
        [HarmonyPatch(typeof(DepositItemsDesk), "SellItemsClientRpc")]
        [HarmonyPostfix]
        static void ChangeCreditsColorAfterSellingClientRpc()
        {
            Plugin.Instance.LogToConsole("Credits color reverted to default", "debug");
            terminal.topRightText.color = terminalCreditsColor;
        }

        [HarmonyPatch(typeof(Terminal), "Start")]
        [HarmonyPostfix]
        static void GetTerminal(Terminal __instance)
        {
            terminal = __instance;
            terminalCreditsColor = terminal.topRightText.color;
            Utils.ChangeTerminalCreditsColor(Plugin.Instance.debtColor.Value);
        }
    }
}
