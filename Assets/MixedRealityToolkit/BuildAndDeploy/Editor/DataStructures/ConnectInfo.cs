// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.Build.DataStructures
{
    [Serializable]
    public struct ConnectInfo
    {
        public ConnectInfo(string ip, string user, string password, string machineName = "")
        {
            IP = ip;
            User = user;
            Password = password;
            MachineName = string.IsNullOrEmpty(machineName) ? ip : machineName;
        }

        public string IP;
        public string User;
        public string Password;
        public string MachineName;
    }
}