// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Graph.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Graph
{
    /// <summary>
    /// Unity script that enables access to the Graph and uses MSAL for authentication.
    /// </summary>
    public class MsalGraphConnector : GraphConnector
    {
        /// <summary>
        /// Method invoked to create the authentication provider.
        /// </summary>
        /// <param name="graphAppId">The app id to access the Graph.</param>
        /// <returns>The authentication provider to use.</returns>
        protected override IGraphAuthentication CreateAuthenticationProvider(string graphAppId)
        {
            return new MsalGraphAuthentication(graphAppId);
        }
    }
}
