// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using MixedRealityToolkit.Common.RestUtility;

namespace MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class DeviceInfo
    {
        // These fields are public to be serialized by the Unity Json Serializer Utility.
        #region Json Serialized Fields

        public string IP;
        public string User;
        public string Password;
        public string MachineName;

        #endregion Json Serialized Fields

        // These fields are public but NonSerialized because we don't want them serialized by the
        // Json Utility, but we also don't want their values overwritten when de-serialization happens.
        #region Json Overwriten Fields

        [NonSerialized]
        public string CsrfToken;

        [NonSerialized]
        private Dictionary<string, string> authorization;

        #endregion Json Overwriten Fields

        #region Properties

        public Dictionary<string, string> Authorization => authorization ?? (authorization = new Dictionary<string, string> { { "Authorization", Rest.GetBasicAuthentication(User, Password) } });

        public BatteryInfo BatteryInfo { get; set; }

        public PowerStateInfo PowerState { get; set; }

        #endregion Properties

        public DeviceInfo(string ip, string user, string password, string machineName = "")
        {
            IP = ip;
            User = user;
            Password = password;
            MachineName = string.IsNullOrEmpty(machineName) ? ip : machineName;
            CsrfToken = string.Empty;
        }
    }
}
