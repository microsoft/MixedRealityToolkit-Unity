// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Interface for graph authentication
    /// </summary>
    public interface IGraphAuthentication
    {
        /// <summary>
        /// Gets the cached token, if any.
        /// </summary>
        string AuthToken { get; }

        /// <summary>
        /// Gets a value indicating whether user is currently authenticated.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Get an access token for the graph scopes and app id. An attempt is first made to 
        /// acquire the token silently. If that fails, then we try to acquire the token by prompting the user.
        /// </summary>
        /// <returns>The async task.</returns>
        Task SignInAsync();

        /// <summary>
        /// Method to sign out the current user.
        /// </summary>
        void SignOut();
    }
}
