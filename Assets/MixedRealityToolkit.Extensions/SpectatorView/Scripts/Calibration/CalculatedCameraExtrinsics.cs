using Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    public class CalculatedCameraExtrinsics : CameraExtrinsics
    {
        public bool Succeeded;

        public CalculatedCameraExtrinsics() : base() { }

        public override string ToString()
        {
            return $"Succeeded: {Succeeded} {base.ToString()}";
        }
    }
}
