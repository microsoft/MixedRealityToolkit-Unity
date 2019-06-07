using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public interface ISpatialLocalizationSettings
    {
        void Serialize(BinaryWriter writer);
    }
}