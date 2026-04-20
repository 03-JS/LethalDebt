using System.Collections;
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
        
        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return CorroutineInstance.StartCoroutine(routine);
        }

        private static MonoBehaviour CorroutineInstance
        {
            get
            {
                if (_corroutineInstance == null)
                {
                    _corroutineInstance = new GameObject("CoroutineManager").AddComponent<CoroutineManagerBehaviour>();
                }
                return _corroutineInstance;
            }
        }

        private static MonoBehaviour _corroutineInstance;

        internal class CoroutineManagerBehaviour : MonoBehaviour { }
    }
}
