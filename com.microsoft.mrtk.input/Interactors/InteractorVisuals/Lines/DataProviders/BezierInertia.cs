// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Represents a parametric curve of an inertial type movement. The parametric curve positions are updated each frame
    /// before rendering line visuals, using a linear interpolation function, until the positions reach their specified targets.
    /// </summary>
    [RequireComponent(typeof(BezierDataProvider))]
    [AddComponentMenu("MRTK/Input/Bezier Inertia")]
    public class BezierInertia : MonoBehaviour
    {        
        [SerializeField]
        [Tooltip("A set parametric curve points.")]
        private BezierDataProvider bezier;

        [SerializeField]
        [Tooltip("The inertial value of the inertial movement.")]
        private float inertia = 15f;

        [SerializeField]
        [Tooltip("The dampen value of the inertial movement.")]
        private float dampen = 6f;

        [SerializeField]
        [Tooltip("The scalar value to apply to the time delta when updating the parametric curve point positions.")]
        private float seekTargetStrength = 5f;

        [SerializeField]
        [Tooltip("The target value for the first parametric curve point position.")]
        private Vector3 p1Target = new Vector3(0, 0, 0.33f);

        [SerializeField]
        [Tooltip("The target value for the second parametric curve point position.")]
        private Vector3 p2Target = new Vector3(0, 0, 0.66f);

        private Vector3 p1Velocity;
        private Vector3 p1Position;
        private Vector3 p1Offset;

        private Vector3 p2Velocity;
        private Vector3 p2Position;
        private Vector3 p2Offset;

        private void OnEnable()
        {
            bezier = gameObject.EnsureComponent<BezierDataProvider>();
            p1Position = bezier.GetPoint(1);
            p2Position = bezier.GetPoint(2);

            Application.onBeforeRender += OnBeforeRenderLineVisual;
        }


        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRenderLineVisual;
        }

        private void OnBeforeRenderLineVisual()
        {
            Vector3 p1BasePoint = bezier.GetPoint(1);
            Vector3 p2BasePoint = bezier.GetPoint(2);

            p1Offset = p1BasePoint - p1Position;
            p2Offset = p2BasePoint - p2Position;

            Vector3 p1WorldTarget = bezier.LineTransform.TransformPoint(p1Target);
            Vector3 p2WorldTarget = bezier.LineTransform.TransformPoint(p2Target);

            p1Offset += p1WorldTarget - p1Position;
            p2Offset += p2WorldTarget - p2Position;

            p1Velocity = Vector3.Lerp(p1Velocity, p1Offset, Time.deltaTime * inertia);
            p1Velocity = Vector3.Lerp(p1Velocity, Vector3.zero, Time.deltaTime * dampen);

            p2Velocity = Vector3.Lerp(p2Velocity, p2Offset, Time.deltaTime * inertia);
            p2Velocity = Vector3.Lerp(p2Velocity, Vector3.zero, Time.deltaTime * dampen);

            p1Position += p1Velocity;
            p2Position += p2Velocity;

            p1Position = Vector3.Lerp(p1Position, p1WorldTarget, seekTargetStrength * Time.deltaTime);
            p2Position = Vector3.Lerp(p2Position, p2WorldTarget, seekTargetStrength * Time.deltaTime);

            bezier.SetPoint(1, p1Position);
            bezier.SetPoint(2, p2Position);
        }
    }
}
