// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Lines;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    [ExecuteInEditMode]
    public class LineObjectSwarm : LineBase
    {
        private const int RandomValueResolution = 1024;
        private readonly FastSimplexNoise noise = new FastSimplexNoise();

        [SerializeField]
        private Transform[] objects;

        private Vector3[] swarmPoints;

        [Range(0, 100)]
        [SerializeField]
        private int seed = 0;

        public int Seed
        {
            get { return seed; }
            set
            {
                if (value < 0)
                {
                    seed = 0;
                }
                else if (value > 100)
                {
                    seed = 100;
                }
                else
                {
                    seed = value;
                }

                randomPosition = new System.Random(seed);
            }
        }

        private System.Random randomPosition;

        [Header("Noise Settings")]

        [SerializeField]
        private float scaleMultiplier = 10f;

        [SerializeField]
        private float speedMultiplier = 1f;

        [SerializeField]
        private float strengthMultiplier = 0.5f;

        [SerializeField]
        private Vector3 axisStrength = Vector3.one;

        [SerializeField]
        private Vector3 axisSpeed = Vector3.one;

        [SerializeField]
        private Vector3 axisOffset = Vector3.zero;

        [Header("Swarm Settings")]

        [Range(0f, 1f)]
        [SerializeField]
        private float normalizedDistance = 0f;

        public float NormalizedDistance
        {
            get { return normalizedDistance; }
            set
            {
                if (value < 0f)
                {
                    normalizedDistance = 0f;
                }
                else if (value > 1f)
                {
                    normalizedDistance = 1f;
                }
                else
                {
                    normalizedDistance = value;
                }
            }
        }

        [SerializeField]
        private Vector3 swarmScale = Vector3.one;

        public Vector3 SwarmScale
        {
            get { return swarmScale; }
            set { swarmScale = value; }
        }

        [SerializeField]
        private LineRotationType rotationTypeOverride = LineRotationType.None;

        public LineRotationType RotationTypeOverride
        {
            get { return rotationTypeOverride; }
            set { rotationTypeOverride = value; }
        }

        [SerializeField]
        private bool swarmVelocities = true;

        public bool SwarmVelocities
        {
            get { return swarmVelocities; }
            set { swarmVelocities = value; }
        }

        [SerializeField]
        private float velocityBlend = 0.5f;

        public float VelocityBlend
        {
            get { return velocityBlend; }
            set { velocityBlend = value; }
        }

        [SerializeField]
        private Vector3 rotationOffset = Vector3.zero;

        public Vector3 RotationOffset
        {
            get { return rotationOffset; }
            set { rotationOffset = value; }
        }

        public override int PointCount => objects.Length;

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (pointIndex > objects.Length || pointIndex < 0)
            {
                Debug.LogError("invalid point index");
                return;
            }

            objects[pointIndex].position = point;
        }

        protected override Vector3 GetPointInternal(float normalizedLength)
        {
            return GetPointInternal(Mathf.RoundToInt(normalizedLength * objects.Length));
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            if (pointIndex > objects.Length || pointIndex < 0)
            {
                Debug.LogError("invalid point index");
                return Vector3.zero;
            }

            return objects[pointIndex].position;
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

        private Vector3 GetRandomPoint()
        {
            Vector3 randomPoint = Vector3.one;
            randomPoint.x = (float)randomPosition.Next(-RandomValueResolution, RandomValueResolution) / (RandomValueResolution * 2);
            randomPoint.y = (float)randomPosition.Next(-RandomValueResolution, RandomValueResolution) / (RandomValueResolution * 2);
            randomPoint.z = (float)randomPosition.Next(-RandomValueResolution, RandomValueResolution) / (RandomValueResolution * 2);
            return Vector3.Scale(randomPoint, swarmScale);
        }

        private void OnValidate()
        {
            randomPosition = new System.Random(seed);
        }

        private void Awake()
        {
            randomPosition = new System.Random(seed);
        }

        private void Update()
        {
            UpdateCollection();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        private void UpdateCollection()
        {
            if (objects == null)
            {
                return;
            }

            if (swarmPoints == null || swarmPoints.Length != objects.Length)
            {
                swarmPoints = new Vector3[objects.Length];
            }

            Vector3 linePoint = GetPoint(normalizedDistance);
            Quaternion lineRotation = GetRotation(normalizedDistance, rotationTypeOverride);

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] == null)
                {
                    continue;
                }

                Vector3 point = transform.TransformVector(GetRandomPoint());
                point.x = (float)(point.x + (noise.Evaluate((point.x + axisOffset.x) * scaleMultiplier, Time.unscaledTime * axisSpeed.x * speedMultiplier)) * axisStrength.x * strengthMultiplier);
                point.y = (float)(point.y + (noise.Evaluate((point.y + axisOffset.y) * scaleMultiplier, Time.unscaledTime * axisSpeed.y * speedMultiplier)) * axisStrength.y * strengthMultiplier);
                point.z = (float)(point.z + (noise.Evaluate((point.z + axisOffset.z) * scaleMultiplier, Time.unscaledTime * axisSpeed.z * speedMultiplier)) * axisStrength.z * strengthMultiplier);

                SetPointInternal(i, point + linePoint);

                if (swarmVelocities)
                {
                    Vector3 velocity = swarmPoints[i] - point;
                    objects[i].rotation = Quaternion.Lerp(lineRotation, Quaternion.LookRotation(velocity, Vector3.up), velocityBlend);
                }
                else
                {
                    objects[i].rotation = lineRotation;
                }

                objects[i].Rotate(rotationOffset);

                swarmPoints[i] = point;
            }
        }
    }
}
