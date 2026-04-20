using System.Runtime.CompilerServices;
using MoreShipUpgrades.API;

namespace LethalDebt
{
    internal class LGUHandler
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AllowOverspending()
        {
            UpgradeApi.TurnOnOverspending();
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void DisallowOverspending()
        {
            UpgradeApi.TurnOffOverspending();
        }
    }
}
