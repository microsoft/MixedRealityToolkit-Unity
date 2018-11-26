// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    [ExecuteInEditMode]
    public class LineObjectFollower : MonoBehaviour
    {
        public Transform Object;

        [Header("Follow Settings")]
        [Range(0f, 1f)]
        public float NormalizedDistance = 0f;

        [SerializeField]
        private LineBase source = null;

        private void Update()
        {
            Vector3 linePoint = source.GetPoint(NormalizedDistance);
            Object.position = linePoint;
        }
    }
}