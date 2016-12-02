//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System.Collections.Generic;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Extensions methods for the Session class.
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Returns an unused username that can be used for this session.
        /// </summary>
        /// <param name="session">Session object for which this is being called.</param>
        /// <param name="baseName">Base name to use for the username.</param>
        /// <param name="excludedUserId">
        ///     User ID whose username excluded from the unique name check. 
        ///     If not specified, all users in the session will be taken into account to find
        ///     a unique name.
        /// </param>
        /// <returns></returns>
        public static string GetUnusedName(this Session session, string baseName, int excludedUserId = int.MaxValue)
        {
            List<string> nameList = new List<string>();
            return GetUnusedName(session, baseName, excludedUserId, nameList);
        }

        /// <summary>
        /// Returns an unused username that can be used for this session.
        /// </summary>
        /// <param name="session">Session object for which this is being called.</param>
        /// <param name="baseName">Base name to use for the username.</param>
        /// <param name="excludedUserId">
        ///     User ID whose username excluded from the unique name check. 
        ///     If not specified, all users in the session will be taken into account to find
        ///     a unique name.
        /// </param>
        /// <param name="cachedList">Cached list that can be provided to avoid extra memory allocations.
        /// </param>
        /// <returns></returns>
        public static string GetUnusedName(this Session session, string baseName, int excludedUserId, List<string> cachedList)
        {
            cachedList.Clear();

            for (int i = 0; i < session.GetUserCount(); i++)
            {
                using (var user = session.GetUser(i))
                using (var userName = user.GetName())
                {
                    string userNameString = userName.GetString();
                    if (user.GetID() != excludedUserId && userNameString.StartsWith(baseName))
                    {
                        cachedList.Add(userNameString);
                    }
                }
            }

            cachedList.Sort();

            int counter = 0;
            string currentName = baseName;
            while (cachedList.Contains(currentName))
            {
                currentName = baseName + (++counter);
            }

            return currentName;
        }
    }
}
