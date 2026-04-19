using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Unity.Netcode;

namespace LethalDebt
{
    internal class NetworkHelper : NetworkBehaviour
    {
        public static NetworkHelper Instance;

        void Start()
        {
            Instance = this;
        }

        [ClientRpc]
        public void EnableClientRpc()
        {
            Plugin.mls.LogDebug("The host has Lethal Debt!");
            Plugin.Instance.enabled = true;
        }
    }
}
