// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This component works on the content of a ScrollRect window! It will
    /// 'curve' the position and orientation of contents based on their
    /// position within the ScrollRect's viewport.
    /// 
    /// This component should be placed on the "Content" GameObject of a Scroll
    /// View, as it uses OnTransformChildrenChanged, and works directly on the
    /// children of the GameObject it's assigned to. This component will not
    /// look great on items that are vertically large, as items are curved at a
    /// GameObject level. Their contents will remain flat.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Scroll Rect Curve")]
    public class ScrollRectCurve : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("A link to the ScrollRect this component is curving. If unassigned, this component will find the first ScrollRect in its parent chain.")]
        private ScrollRect scrollRect;

        [SerializeField]
        [Tooltip("How much should the UI elements be curved, in local units? This is the distance from the content base position, to the curvature's maximum Z distance from the base position.")]
        private float curveDepth = 20;

        void Awake()
        {
            if (scrollRect == null)
            {
                scrollRect = gameObject.GetComponentInParent<ScrollRect>(true);
            }
            scrollRect.onValueChanged.AddListener(a => UpdatePositions());
        }

        void OnTransformChildrenChanged() => UpdatePositions();
        void OnValidate()                 => UpdatePositions();

        void UpdatePositions()
        {
            if (scrollRect == null) { return; }

            float height  = scrollRect.viewport.rect.yMin - scrollRect.viewport.rect.yMax;
            float height2 = height * height;
            float top     = ((RectTransform)transform).rect.yMax + transform.localPosition.y;

            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform tr = (RectTransform)transform.GetChild(i);
                float         y  = (tr.localPosition.y + tr.rect.center.y) + top;

                // This uses a parabola for the curve (tested alongside a
                // cosine curve, the parabola looked better), and the
                // derivative of that curve to determine the tangent/normal for
                // building the orientation.
                //
                // Graph for this here:
                // https://www.desmos.com/calculator/sfwaxipupr
                float curve   = -(4 * curveDepth * y * (-height + y)) / height2;
                float tangent = -(4 * curveDepth * (height - 2 * y)) / height2;

                // Correct for the pivot point being in the top left corner,
                // rotation should happen around the center of the item. This
                // math finds the z axis translation of the center of the UI
                // element caused by the rotation around the pivot point.
                //
                // Both these lines are equivalent, the Sqrt version is a
                // reduced version of the first, and should be faster than 2
                // trig calls.

                // float offset = Mathf.Sin(Mathf.PI - (Mathf.Atan(1.0f / tangent) + Mathf.PI / 2.0f)) * (tr.rect.height / 2.0f) * Mathf.Sign(tangent);
                float offset = (tr.rect.height / 2.0f) / Mathf.Sqrt(1.0f / (tangent*tangent) + 1) * Mathf.Sign(tangent);

                Vector3 pos = tr.localPosition;
                tr.localPosition = new Vector3(pos.x, pos.y, (curve - offset) - curveDepth );
                tr.localRotation = Quaternion.LookRotation(new Vector3(0, tangent, 1), Vector3.up);
            }
        }
    }
}
