using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AnchorControl
{
    public interface IAnchorMatrixSource
    {
        bool GetAnchorMatrix(string anchorID, out Matrix4x4 matrix);
    }
}