// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.WindowsDevicePortal
{
    [Serializable]
    public class WirelessNetworkInfo
    {
        public bool AlreadyConnected;
        public string AuthenticationAlgorithm;
        public int Channel;
        public string CipherAlgorithm;
        /// <summary>
        /// (0 | 1)
        /// </summary>
        public int Connectable;
        public string InfrastructureType;
        public bool ProfileAvailable;
        public string ProfileName;
        public string SSID;
        /// <summary>
        /// (0 | 1)
        /// </summary>
        public int SecurityEnabled;
        public int SignalQuality;
        public int[] BSSID;
        public string[] PhysicalTypes;
    }
}
