using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public interface ISpatialLocalizer
    {
        Guid SpatialLocalizerID { get; }

        bool TryDeserializeSettings(BinaryReader reader, out ISpatialLocalizationSettings settings);

        ISpatialLocalizationSession CreateLocalizationSession(ISpatialLocalizationSettings settings);

        bool TryGetKnownCoordinate(ISpatialLocalizationSettings settings, out ISpatialCoordinate coordinate);
    }
}