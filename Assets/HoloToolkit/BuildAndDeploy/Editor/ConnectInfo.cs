// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity
{
    public struct ConnectInfo
    {
        public ConnectInfo(string ip, string user, string password)
        {
            IP = ip;
            User = user;
            Password = password;
        }

        public string IP;
        public string User;
        public string Password;
    }
}