// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility component to play an input AnimationClip using the input simulation service.
    /// </summary>
    internal class InputAnimator : MonoBehaviour
    {
        [SerializeField]
        private AnimationClip clip;
        public AnimationClip Clip => clip;

        private float localTime = 0.0f;
        public float LocalTime => localTime;

        public void Awake()
        {
            localTime = 0.0f;
        }

        public void Update()
        {
            localTime += Time.deltaTime;
        }
    }
}