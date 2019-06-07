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
    public abstract class SpatialLocalizer<TSpatialLocalizationSettings, TSpatialCoordinateKey> : MonoBehaviour, ISpatialLocalizer where TSpatialLocalizationSettings : ISpatialLocalizationSettings
    {
        /// <summary>
        /// The spatial coordinate service for sub class to instantiate and this helper base to rely on.
        /// </summary>
        protected abstract ISpatialCoordinateService<TSpatialCoordinateKey> SpatialCoordinateService { get; }

        public abstract Guid SpatialLocalizerID { get; }

        protected readonly object lockObject = new object();

        private string typeName;

        [Tooltip("Toggle to enable troubleshooting logging.")]
        [SerializeField]
        protected bool debugLogging = false;

        /// <summary>
        /// The type name of this object instance.
        /// </summary>
        private string TypeName => typeName ?? (typeName = GetType().Name);

        /// <summary>
        /// Helper method for logging troubleshooting information.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="token">The <see cref="Guid"/> token representing the request.</param>
        protected void DebugLog(string message)
        {
            if (debugLogging)
            {
                Debug.Log($"{TypeName}: {message}");
            }
        }

        protected virtual void Start()
        {
            if (!SpatialCoordinateSystemManager.IsInitialized)
            {
                Debug.LogError($"{TypeName} - Failed to register spatial localizer with the {nameof(SpatialCoordinateSystemManager)}");
                return;
            }

            SpatialCoordinateSystemManager.Instance.RegisterSpatialLocalizer(this);
        }

        protected virtual void OnDestroy()
        {
            if (SpatialCoordinateSystemManager.IsInitialized)
            {
                SpatialCoordinateSystemManager.Instance.UnregisterSpatialLocalizer(this);
            }
        }

        public abstract bool TryDeserializeSettings(BinaryReader reader, out TSpatialLocalizationSettings settings);

        public abstract ISpatialLocalizationSession CreateLocalizationSession(TSpatialLocalizationSettings settings);

        public abstract bool TryGetKnownCoordinate(TSpatialLocalizationSettings settings, out ISpatialCoordinate coordinate);

        bool ISpatialLocalizer.TryDeserializeSettings(BinaryReader reader, out ISpatialLocalizationSettings settings)
        {
            if (TryDeserializeSettings(reader, out TSpatialLocalizationSettings specificSettings))
            {
                settings = specificSettings;
                return true;
            }
            else
            {
                settings = default(ISpatialLocalizationSettings);
                return false;
            }
        }

        ISpatialLocalizationSession ISpatialLocalizer.CreateLocalizationSession(ISpatialLocalizationSettings settings)
        {
            if (settings is TSpatialLocalizationSettings specificSettings)
            {
                return CreateLocalizationSession(specificSettings);
            }
            else
            {
                throw new ArgumentException(nameof(settings));
            }
        }

        bool ISpatialLocalizer.TryGetKnownCoordinate(ISpatialLocalizationSettings settings, out ISpatialCoordinate coordinate)
        {
            if (settings is TSpatialLocalizationSettings specificSettings)
            {
                return TryGetKnownCoordinate(specificSettings, out coordinate);
            }
            else
            {
                throw new ArgumentException(nameof(settings));
            }
        }
    }
}
