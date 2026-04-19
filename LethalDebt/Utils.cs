using LethalDebt.Patches;
using UnityEngine;

namespace LethalDebt
{
    internal class Utils
    {
        public static void SetCreditsColorToDebt()
        {
            if (TerminalPatches.terminal.groupCredits < 0)
            {
                ColorUtility.TryParseHtmlString(Plugin.Instance.debtColor.Value, out Color color);
                TerminalPatches.terminal.topRightText.color = color;
            }
        }
    }
}
