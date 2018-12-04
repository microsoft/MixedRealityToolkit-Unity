using UnityEngine;

namespace Pixie.AnchorControl
{
    public interface IAnchorMatrixSource
    {
        bool GetAnchorMatrix(string anchorID, out Matrix4x4 matrix);
    }
}