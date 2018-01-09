// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Animates the scale of an object based on it's distance to the distance object. Could be used for scaling UI based on the users position.
    /// Move closer and the object scale down, move away and the object scales up.
    /// </summary>
    public class ScaleByDistance : MonoBehaviour
    {
        [Tooltip("The object's distance to scale against, default: Main Camera")]
        public GameObject ReferenceObject;

        [Tooltip("The object to scale")]
        public GameObject TargetObject;

        [Tooltip("A game object that contains an Interactive to handle air taps")]
        public GameObject ReferenceInteractive;

        [Tooltip("How far away should the object be at 100%")]
        public float ScaleDistance = 1;

        [Tooltip("Auto start? or status")]
        public bool IsScaling;

        [Tooltip("scaling speed : higher is faster")]
        public float ScaleSpeed = 3;

        [Tooltip("Minimum scale")]
        public float MinimumScale = 0.3f;

        // the cached start scale
        private Vector3 mStartScale;
        // the current scale through the transformation
        private float mCurrentScale = 1;
        // scale difference
        private float mDeltaScale;
        // the cached starting difference
        private float mStartDistance;

        /// <summary>
        /// Set the TargetObject and the ReferenceObject if not set already
        /// </summary>
        void Start()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            if (ReferenceObject == null)
            {
                ReferenceObject = CameraCache.Main.gameObject;
            }
        }
        
        /// <summary>
        /// Start the scaling animation based on distance
        /// </summary>
        public void StartRunning(bool state = false)
        {
            mStartScale = TargetObject.transform.localScale;
            mStartDistance = Vector3.Distance(TargetObject.transform.position, ReferenceObject.transform.position);
            IsScaling = true;

            if (!state)
            {
                mCurrentScale = mDeltaScale;
            }

            if (ReferenceInteractive != null)
            {
                InputManager.Instance.PushModalInputHandler(ReferenceInteractive);
            }
        }

        /// <summary>
        /// Stop the animation
        /// </summary>
        public void StopRunning()
        {

            if (ReferenceInteractive != null)
            {
                InputManager.Instance.PopModalInputHandler();
            }
            IsScaling = false;
        }

        // set the scale value
        void Update()
        {
            if (IsScaling)
            {
                float ratio = (Vector3.Distance(TargetObject.transform.position, ReferenceObject.transform.position) - mStartDistance) / ScaleDistance;
                mDeltaScale = Mathf.Max(mCurrentScale + ratio, MinimumScale);
                Vector3 targetScale = mStartScale * mDeltaScale;
                TargetObject.transform.localScale = Vector3.Lerp(TargetObject.transform.localScale, targetScale, Time.deltaTime * ScaleSpeed);

            }
        }
    }
}
