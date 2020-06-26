// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.WindowsDevicePortal
{
    [Serializable]
    public class InterfaceInfo
    {
        public string Description;
        public string GUID;
        public int Index;
        public NetworkProfileInfo[] ProfilesList;
    }
}