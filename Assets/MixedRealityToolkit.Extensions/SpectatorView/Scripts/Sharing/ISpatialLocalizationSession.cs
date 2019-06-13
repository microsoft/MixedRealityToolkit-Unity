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
    public interface ISpatialLocalizationSession : IDisposable
    {
        Task<ISpatialCoordinate> LocalizeAsync(CancellationToken cancellationToken);

        void OnDataReceived(BinaryReader reader);
    }
}