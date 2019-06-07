using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public abstract class SpatialLocalizationSession<TSpatialLocalizationSettings> : DisposableBase, ISpatialLocalizationSession where TSpatialLocalizationSettings : ISpatialLocalizationSettings
    {
        protected TSpatialLocalizationSettings Settings { get; }

        protected SpatialLocalizationSession(TSpatialLocalizationSettings settings)
        {
            Settings = settings;
        }

        public abstract Task<ISpatialCoordinate> LocalizeAsync(CancellationToken cancellationToken);
    }
}