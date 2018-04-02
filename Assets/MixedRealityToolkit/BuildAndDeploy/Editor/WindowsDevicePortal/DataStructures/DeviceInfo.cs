using System;
using System.Collections.Generic;
using MixedRealityToolkit.Common.RestUtility;

namespace MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class DeviceInfo
    {
        public string IP;
        public string User;
        public string Password;
        public string MachineName;
        [NonSerialized]
        public string CsrfToken;

        [NonSerialized]
        private Dictionary<string, string> authorization;

        public Dictionary<string, string> Authorization => authorization ?? (authorization = new Dictionary<string, string> { { "Authorization", Rest.GetBasicAuthentication(User, Password) } });

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
