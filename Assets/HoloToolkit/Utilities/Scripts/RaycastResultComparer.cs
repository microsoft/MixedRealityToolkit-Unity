using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity
{
    public class RaycastResultComparer : IComparer<RaycastResult>
    {
        public int Compare(RaycastResult left, RaycastResult right)
        {
            var result = CompareRaycastsByCanvasDepth(left, right);
            if (result != 0) { return result; }
            return CompareRaycastsByDistance(left, right);
        }

        private static int CompareRaycastsByCanvasDepth(RaycastResult left, RaycastResult right)
        {
            //Module is the graphic raycaster on the canvases.
            if (left.module.transform.IsParentOrChildOf(right.module.transform))
            {
                return left.depth.CompareTo(right.depth);
            }
            return 0;
        }

        private static int CompareRaycastsByDistance(RaycastResult left, RaycastResult right)
        {
            return right.distance.CompareTo(left.distance);
        }
    }
}
