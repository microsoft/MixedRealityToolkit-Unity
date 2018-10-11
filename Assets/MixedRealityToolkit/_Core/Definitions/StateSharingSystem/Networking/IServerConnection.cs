using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Networking
{
    public interface IServerConnection
    {
        bool IsConnected { get; }
        bool AllClientsReady { get; }

        void StartServer();
        void SetAllClientsNotReady();
        void ForceDisconnect();
    }
}