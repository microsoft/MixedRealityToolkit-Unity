using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public interface IPeerConnection
    {
        void SendData(Action<BinaryWriter> writeCallback);
    }

    public interface ISpatialLocalizer
    {
        Guid SpatialLocalizerId { get; }

        bool TryDeserializeSettings(BinaryReader reader, out ISpatialLocalizationSettings settings);

        bool TryCreateLocalizationSession(IPeerConnection peerConnection, ISpatialLocalizationSettings settings, out ISpatialLocalizationSession session);
    }
}