// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Helper class to enable spatial localization between two entities on SpectatorView.
    /// </summary>
    /// <remarks>In the future this would move to SpatialLocalization in a better form, abstraction-wise.</remarks>
    public abstract class SpatialLocalizer : MonoBehaviour
    {
        public const string SpatialLocalizationMessageHeader = "LOCALIZE";

        /// <summary>
        /// The spatial coordinate service for sub class to instantiate and this helper base to rely on.
        /// </summary>
        protected abstract ISpatialCoordinateService SpatialCoordinateService { get; }

        protected readonly object lockObject = new object();

        private string typeName;

        [Tooltip("Toggle to enable troubleshooting logging.")]
        [SerializeField]
        protected bool debugLogging = false;

        /// <summary>
        /// The type name of this object instance.
        /// </summary>
        public string TypeName => typeName ?? (typeName = GetType().Name);

        /// <summary>
        /// Helper method for logging troubleshooting information.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="token">The <see cref="Guid"/> token representing the request.</param>
        protected void DebugLog(string message, Guid token)
        {
            if (debugLogging)
            {
                Debug.Log($"{TypeName} - {token}: {message}");
            }
        }

        /// <summary>
        /// Start a new spatial localization session, and return a token for that session.
        /// </summary>
        /// <param name="actAsHost">If true, this localizer will operate under the assumption that it is hosting the spatial coordinate system.</param>
        /// <param name="cancellationToken">The cancellation token to cancel this async operation.</param>
        /// <returns>The token representing this session, should be used for subsequent calls.</returns>
        internal abstract Task<Guid> InitializeAsync(bool actAsHost, CancellationToken cancellationToken);

        /// <summary>
        /// Deinitializes the session.
        /// </summary>
        /// <param name="actAsHost">If true, this localizer will operate under the assumption that it is hosting the spatial coordinate system.</param>
        /// <param name="token">The token representing the session.</param>
        internal abstract void Uninitialize(bool actAsHost, Guid token);

        /// <summary>
        /// Handles incoming message for the session.
        /// </summary>
        /// <param name="actAsHost">If true, this localizer will operate under the assumption that it is hosting the spatial coordinate system.</param>
        /// <param name="token">The token representing the session.</param>
        /// <param name="command">The command associated with the binary reader.</param>
        /// <param name="r">The binary reader for the message.</param>
        internal abstract void ProcessIncomingMessage(bool actAsHost, Guid token, string command, BinaryReader r);

        /// <summary>
        /// Attempts to localize the given session.
        /// </summary>
        /// <param name="actAsHost">If true, this localizer will operate under the assumption that it is hosting the spatial coordinate system.</param>
        /// <param name="token">The token representing the session.</param>
        /// <param name="writeAndSendMessage">A function that allows the spatialLocalizer to write and send content to other devices.</param>
        /// <param name="cancellationToken">The cancellation token to cancel this async operation.</param>
        /// <returns>The spatial coordinate when this operation is complete.</returns>
        internal abstract Task<ISpatialCoordinate> LocalizeAsync(bool actAsHost, Guid token, Action<Action<BinaryWriter>> writeAndSendMessage, CancellationToken cancellationToken);
    }
}
