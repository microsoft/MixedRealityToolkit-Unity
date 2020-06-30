// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.WindowsDevicePortal
{
    [Serializable]
    public class InstallStatus
    {
        public int Code;
        public string CodeText;
        public string Reason;
        public bool Success;
    }
}