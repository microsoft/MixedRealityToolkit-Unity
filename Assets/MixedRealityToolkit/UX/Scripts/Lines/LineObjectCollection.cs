// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.UX.Lines
{
    public class LineObjectCollection : MonoBehaviour
    {
        public List<Transform> Objects = new List<Transform>();

        [Range(-2f, 2f)]
        public float DistributionOffset = 0f;
        [Range(0f, 2f)]
        public float LengthOffset = 0f;
        [Range(0f, 2f)]
        public float ScaleOffset = 0f;
        [Range(0.001f, 2f)]
        public float ScaleMultiplier = 1f;
        [Range(0.001f, 2f)]
        public float PositionMultiplier = 1f;

        public float DistributionOffsetPerObject
        {
            get
            {
                return 1f / Objects.Count;
            }
        }

        public AnimationCurve ObjectScale = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public AnimationCurve ObjectPosition = AnimationCurve.Linear(0f, 0f, 1f, 0f);

        public bool FlipRotation = false;

        public Vector3 RotationOffset = Vector3.zero;

        public Vector3 PositionOffset = Vector3.zero;

        public RotationTypeEnum RotationTypeOverride = RotationTypeEnum.None;

        public PointDistributionTypeEnum DistributionType = PointDistributionTypeEnum.None;

        [Header("Object Placement")]
        public StepModeEnum StepMode = StepModeEnum.Interpolated;

        [SerializeField]
        private LineBase source;

        [SerializeField]
        private Transform transformHelper;

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

        // Convenience functions
        public float GetOffsetFromObjectIndex(int index, bool wrap = true)
        {
            if (Objects.Count == 0)
            {
                return 0;
            }

            if (wrap)
            {
                index = WrapIndex(index, Objects.Count);
            }
            else
            {
                index = Mathf.Clamp(index, 0, Objects.Count - 1);
            }

            return (1f / Objects.Count * (index + 1));
        }

        public int GetNextObjectIndex(int index, bool wrap = true)
        {
            if (Objects.Count == 0)
            {
                return 0;
            }

            index++;

            if (wrap)
            {
                return WrapIndex(index, Objects.Count);
            }
            else
            {
                return Mathf.Clamp(index, 0, Objects.Count - 1);
            }
        }

        public int GetPrevObjectIndex(int index, bool wrap = true)
        {
            if (Objects.Count == 0)
            {
                return 0;
            }

            index--;

            if (wrap)
            {
                return WrapIndex(index, Objects.Count);
            }
            else
            {
                return Mathf.Clamp(index, 0, Objects.Count - 1);
            }
        }

        public void Update()
        {
            UpdateCollection();
        }

        public void UpdateCollection()
        {
            if (Source == null)
            {
                return;
            }

            if (transformHelper == null)
            {
                transformHelper = transform.Find("TransformHelper");
                if (transformHelper == null)
                {
                    transformHelper = new GameObject("TransformHelper").transform;
                    transformHelper.parent = transform;
                }
            }

            switch (StepMode)
            {
                case StepModeEnum.FromSource:
                    break;

                case StepModeEnum.Interpolated:
                    for (int i = 0; i < Objects.Count; i++)
                    {
                        if (Objects[i] == null)
                        {
                            continue;
                        }

                        float normalizedDistance = Mathf.Repeat(((float)i / Objects.Count) + DistributionOffset, 1f);
                        Objects[i].position = Source.GetPoint(normalizedDistance);
                        Objects[i].rotation = Source.GetRotation(normalizedDistance, RotationTypeOverride);

                        transformHelper.localScale = Vector3.one;
                        transformHelper.position = Objects[i].position;
                        transformHelper.localRotation = Quaternion.identity;
                        Transform tempParent = Objects[i].parent;
                        Objects[i].parent = transformHelper;
                        transformHelper.localEulerAngles = RotationOffset;
                        Objects[i].parent = tempParent;
                        Objects[i].transform.localScale = Vector3.one * ObjectScale.Evaluate(Mathf.Repeat(ScaleOffset + normalizedDistance, 1f)) * ScaleMultiplier;
                    }
                    break;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            UpdateCollection();
        }
#endif

        private static int WrapIndex(int index, int numObjects)
        {
            return ((index % numObjects) + numObjects) % numObjects;
        }
    }
}