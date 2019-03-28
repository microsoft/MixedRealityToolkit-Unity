// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing
{
    /// <summary>
    /// Interface implemented by classes that connect and disconnect a user from a shared appliation
    /// </summary>
    public interface IMatchMakingService
    {
        /// <summary>
        /// Connects a user to a shared application
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects a user from a shared application
        /// </summary>
        /// <returns></returns>
        bool Disconnect();

        /// <summary>
        /// Returns true if a user is currently connected to a shared application
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
    }
}
