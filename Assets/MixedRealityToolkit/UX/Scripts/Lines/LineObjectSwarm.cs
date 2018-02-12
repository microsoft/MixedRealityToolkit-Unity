// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.UX.Lines
{
    [ExecuteInEditMode]
    public class LineObjectSwarm : MonoBehaviour
    {
        const int RandomValueResolution = 1024;

        public List<Transform> Objects = new List<Transform>();

        [Range(0, 100)]
        public int Seed = 0;

        [SerializeField]
        private LineBase source;

        [Header("Noise Settings")]
        public float ScaleMultiplier = 10f;
        public float SpeedMultiplier = 1f;
        public float StrengthMultiplier = 0.5f;
        public Vector3 AxisStrength = Vector3.one;
        public Vector3 AxisSpeed = Vector3.one;
        public Vector3 AxisOffset = Vector3.zero;

        private Vector3[] prevPoints;
        private System.Random randomPosition;
        private System.Random randomRotation;
        private FastSimplexNoise noise = new FastSimplexNoise();

        [Header("Swarm Settings")]
        [Range(0f, 1f)]
        public float NormalizedDistance = 0f;

        public Vector3 SwarmScale = Vector3.one;

        public AnimationCurve ObjectScale = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public AnimationCurve ObjectOffset = AnimationCurve.Linear(0f, 0f, 1f, 0f);

        public RotationTypeEnum RotationTypeOverride = RotationTypeEnum.None;

        public bool SwarmVelocities = true;

        public float VelocityBlend = 0.5f;

        public Vector3 RotationOffset = Vector3.zero;

        public Vector3 AxisScale = Vector3.one;

        public virtual LineBase Source
        {
            get
            {
                if (source == null)
                {
                    source = GetComponent<LineBase>();
                }
                return source;
            }
            set
            {
                source = value;
                if (source == null)
                {
                    enabled = false;
                }
            }
        }

        public Vector3 GetRandomPoint()
        {
            Vector3 randomPoint = Vector3.one;
            randomPoint.x = (float)randomPosition.Next(-RandomValueResolution, RandomValueResolution) / (RandomValueResolution * 2);
            randomPoint.y = (float)randomPosition.Next(-RandomValueResolution, RandomValueResolution) / (RandomValueResolution * 2);
            randomPoint.z = (float)randomPosition.Next(-RandomValueResolution, RandomValueResolution) / (RandomValueResolution * 2);

            return Vector3.Scale(randomPoint, SwarmScale);
        }

        public void Update()
        {
            UpdateCollection();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        public void UpdateCollection()
        {
            if (Source == null)
            {
                return;
            }

            if (prevPoints == null || prevPoints.Length != Objects.Count)
            {
                prevPoints = new Vector3[Objects.Count];
            }

            randomPosition = new System.Random(Seed);
            Vector3 linePoint = source.GetPoint(NormalizedDistance);
            Quaternion lineRotation = source.GetRotation(NormalizedDistance, RotationTypeOverride);

            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i] == null)
                {
                    continue;
                }

                Vector3 point = source.transform.TransformVector(GetRandomPoint());
                point.x = (float)(point.x + (noise.Evaluate((point.x + AxisOffset.x) * ScaleMultiplier, Time.unscaledTime * AxisSpeed.x * SpeedMultiplier)) * AxisStrength.x * StrengthMultiplier);
                point.y = (float)(point.y + (noise.Evaluate((point.y + AxisOffset.y) * ScaleMultiplier, Time.unscaledTime * AxisSpeed.y * SpeedMultiplier)) * AxisStrength.y * StrengthMultiplier);
                point.z = (float)(point.z + (noise.Evaluate((point.z + AxisOffset.z) * ScaleMultiplier, Time.unscaledTime * AxisSpeed.z * SpeedMultiplier)) * AxisStrength.z * StrengthMultiplier);

                Objects[i].position = point + linePoint;
                if (SwarmVelocities)
                {
                    Vector3 velocity = prevPoints[i] - point;
                    Objects[i].rotation = Quaternion.Lerp(lineRotation, Quaternion.LookRotation(velocity, Vector3.up), VelocityBlend);
                }
                else
                {
                    Objects[i].rotation = lineRotation;
                }
                Objects[i].Rotate(RotationOffset);

                prevPoints[i] = point;
            }
        }
    }
}
