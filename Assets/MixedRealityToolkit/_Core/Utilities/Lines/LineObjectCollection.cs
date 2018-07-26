// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Lines;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    [DisallowMultipleComponent]
    public class LineObjectCollection : LineBase
    {
        [SerializeField]
        private Transform[] objects = null;

        [Range(-2f, 2f)]
        [SerializeField]
        private float distributionOffset = 0f;

        [Range(0f, 2f)]
        [SerializeField]
        private float scaleOffset = 0f;

        [Range(0.001f, 2f)]
        [SerializeField]
        private float scaleMultiplier = 1f;

        public float DistributionOffsetPerObject => 1f / objects.Length;

        [SerializeField]
        private AnimationCurve objectScale = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [SerializeField]
        private Vector3 rotationOffset = Vector3.zero;

        [SerializeField]
        private LineRotationType rotationTypeOverride = LineRotationType.None;

        [Header("Object Placement")]

        [SerializeField]
        private StepMode stepMode = StepMode.Interpolated;

        [SerializeField]
        private Transform lineCollectionRoot;

        /// <inheritdoc />
        public override int PointCount => objects.Length;

        /// <inheritdoc />
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (pointIndex > objects.Length || pointIndex < 0)
            {
                Debug.LogError("invalid point index");
                return;
            }

            objects[pointIndex].position = point;
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedLength)
        {
            return GetPointInternal(Mathf.RoundToInt(normalizedLength * objects.Length));
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            if (pointIndex > objects.Length || pointIndex < 0)
            {
                Debug.LogError("invalid point index");
                return Vector3.zero;
            }

            return objects[pointIndex].position;
        }

        public float GetOffsetFromObjectIndex(int index, bool wrap = true)
        {
            if (objects.Length == 0)
            {
                return 0;
            }

            index = wrap ? WrapIndex(index, objects.Length) : Mathf.Clamp(index, 0, objects.Length - 1);

            return 1f / objects.Length * (index + 1);
        }

        public int GetNextObjectIndex(int index, bool wrap = true)
        {
            if (objects.Length == 0)
            {
                return 0;
            }

            index++;

            return wrap ? WrapIndex(index, objects.Length) : Mathf.Clamp(index, 0, objects.Length - 1);
        }

        public int GetPrevObjectIndex(int index, bool wrap = true)
        {
            if (objects.Length == 0)
            {
                return 0;
            }

            index--;

            return wrap ? WrapIndex(index, objects.Length) : Mathf.Clamp(index, 0, objects.Length - 1);
        }

        private void OnValidate()
        {
            if (lineCollectionRoot == null)
            {
                lineCollectionRoot = transform.Find("Line Object Collection");

                if (lineCollectionRoot == null)
                {
                    lineCollectionRoot = new GameObject("Line Object Collection").transform;
                    lineCollectionRoot.parent = transform;
                }
            }
        }

        public void Update()
        {
            UpdateCollection();
        }

        protected override float GetUnClampedWorldLengthInternal()
        {
            // Crude approximation
            // TODO optimize
            float distance = 0f;
            Vector3 last = GetUnClampedPoint(0f);

            for (int i = 1; i < 10; i++)
            {
                Vector3 current = GetUnClampedPoint((float)i / 10);
                distance += Vector3.Distance(last, current);
            }

            return distance;
        }

        protected override void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            UpdateCollection();
        }

        public void UpdateCollection()
        {
            if (stepMode == StepMode.Interpolated)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] == null)
                    {
                        continue;
                    }

                    float normalizedDistance = Mathf.Repeat(((float)i / objects.Length) + distributionOffset, 1f);
                    objects[i].position = GetPoint(normalizedDistance);
                    objects[i].rotation = GetRotation(normalizedDistance, rotationTypeOverride);
                    lineCollectionRoot.localScale = Vector3.one;
                    lineCollectionRoot.position = objects[i].position;
                    lineCollectionRoot.localRotation = Quaternion.identity;
                    Transform tempParent = objects[i].parent;
                    objects[i].parent = lineCollectionRoot;
                    lineCollectionRoot.localEulerAngles = rotationOffset;
                    objects[i].parent = tempParent;
                    objects[i].transform.localScale = Vector3.one * objectScale.Evaluate(Mathf.Repeat(scaleOffset + normalizedDistance, 1f)) * scaleMultiplier;
                }
            }
        }

        private static int WrapIndex(int index, int numObjects)
        {
            return ((index % numObjects) + numObjects) % numObjects;
        }
    }
}