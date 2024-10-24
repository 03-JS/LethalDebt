using LethalDebt.Patches;
using UnityEngine;

namespace LethalDebt
{
    internal class Utils
    {
        public static void ChangeTerminalCreditsColor(string htmlColor)
        {
            if (TerminalPatches.terminal.groupCredits < 0)
            {
                ColorUtility.TryParseHtmlString(htmlColor, out Color color);
                TerminalPatches.terminal.topRightText.color = color;
            }
        }
    }
}
