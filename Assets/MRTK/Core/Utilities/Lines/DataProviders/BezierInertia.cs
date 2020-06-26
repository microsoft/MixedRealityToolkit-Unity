// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    [ExecuteAlways]
    [AddComponentMenu("Scripts/MRTK/Core/BezierInertia")]
    public class BezierInertia : MonoBehaviour
    {
        [SerializeField]
        private BezierDataProvider bezier;
        [SerializeField]
        private float inertia = 4f;
        [SerializeField]
        private float dampen = 5f;
        [SerializeField]
        private float seekTargetStrength = 1f;

        [SerializeField]
        private Vector3 p1Target = new Vector3(0, 0, 0.25f);
        [SerializeField]
        private Vector3 p2Target = new Vector3(0, 0, 0.75f);

        private Vector3 p1Velocity;
        private Vector3 p1Position;
        private Vector3 p1Offset;

        private Vector3 p2Velocity;
        private Vector3 p2Position;
        private Vector3 p2Offset;

        private void Start()
        {
            bezier = gameObject.EnsureComponent<BezierDataProvider>();
            p1Position = bezier.GetPoint(1);
            p2Position = bezier.GetPoint(2);
        }

        private void Update()
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

            p1Position = p1Position + p1Velocity;
            p2Position = p2Position + p2Velocity;

            p1Position = Vector3.Lerp(p1Position, p1WorldTarget, seekTargetStrength * Time.deltaTime);
            p2Position = Vector3.Lerp(p2Position, p2WorldTarget, seekTargetStrength * Time.deltaTime);

            bezier.SetPoint(1, p1Position);
            bezier.SetPoint(2, p2Position);
        }
    }
}