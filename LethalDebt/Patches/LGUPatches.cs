using HarmonyLib;
using System.Collections.Generic;
using MoreShipUpgrades.UI.Application;
using System.Linq;
using System.Reflection.Emit;
using MoreShipUpgrades.UI.TerminalNodes;

namespace LethalDebt.Patches
{
    internal class LGUPatches
    {
        [HarmonyPatch("MoreShipUpgrades.UI.Application.UpgradeStoreApplication", "BuyUpgrade")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_S && codes[i].operand.ToString().Equals("System.Boolean (6)") &&
                    codes[i + 1].opcode == OpCodes.Brfalse && codes[i + 2].opcode == OpCodes.Nop)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        codes[i + j].opcode = OpCodes.Nop;
                    }
                    Plugin.mls.LogDebug("Removed LGU purchasing restrictions");
                    break;
                }
            }

            return codes.AsEnumerable();
        }

        [HarmonyPatch("MoreShipUpgrades.UI.Application.UpgradeStoreApplication", "BuyUpgrade")]
        [HarmonyPrefix]
        static bool PreventClientGriefing(CustomTerminalNode node)
        {
            if (!Plugin.Instance.enabled && TerminalPatches.terminal.groupCredits < node.GetCurrentPrice()) return false;
            return true;
        }

        [HarmonyPatch("MoreShipUpgrades.UI.Application.UpgradeStoreApplication", "PurchaseUpgrade")]
        [HarmonyPostfix]
        static void UpdateCreditsColorAfterPurchase()
        {
            Utils.SetCreditsColorToDebt();
        }
    }
}
