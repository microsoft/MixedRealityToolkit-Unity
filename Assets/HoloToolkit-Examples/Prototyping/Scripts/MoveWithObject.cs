// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Makes the assigned object follow and face another object.
    /// A potential use is moving a UI panel around, but is very flexible.
    /// 
    /// Add the reference object this object should follow and tell it to start running.
    /// To get this to follow the user around, add the camera as the reference object.
    /// Typical Use:
    /// Call Start Running();
    /// 
    /// Features:
    ///     - Independant adjustment speeds of position and rotation, feels really cool.
    ///     - Force the object to remain vertical or lock the x axis rotation.
    ///     - Link to an interactive object to add manual controls.
    ///     - Force the object to face to reference object or maintain it's existing direction
    ///     - Force the object to be in front or at the reference object's transform.forward.
    ///     - Add magnitism to bring the object closer to the reference object
    /// </summary>
    public class MoveWithObject : MonoBehaviour
    {
        [Tooltip("The game object this object will follow : Main Camera by default")]
        public GameObject ReferenceObject;

        [Tooltip("Auto start? or status")]
        public bool IsRunning = false;

        [Tooltip("translation speed : higher is faster")]
        public float LerpPositionSpeed = 1f;

        [Tooltip("rotation speed : higher is faster")]
        public float LerpRotationSpeed = 0.5f;

        [Tooltip("Lock the x axis if the object is set to face the reference object")]
        public bool KeepUpRight = false;

        [Tooltip("An game object containing an Interactive component to call on air-tap")]
        public GameObject ReferenceInteractive;

        [Tooltip("Does not center the object to the reference object's transform.forward vector")]
        public bool KeepStartingOffset = true;

        [Tooltip("Force the object to always face the reference object")]
        public bool FaceObject = true;

        [Tooltip("Force the object to keep relative to the reference object's transform.forward")]
        public bool KeepInFront = true;

        [Tooltip("Magnitism speed to move closer to the reference object")]
        public float Magnetism = 0;

        [Tooltip("Minimum distance to stay away from the reference object if magnitism is used")]
        public float MagnetismPaddingDistance = 1f;

        // the position different between the objects position and the reference object's transform.forward
        private Vector3 mOffsetDirection;

        // this object's direction
        private Vector3 mDirection;

        // the offset rotation at start
        private Quaternion mOffsetRotation;

        // the offset distance at start
        private float mOffsetDistance = 0;

        // the amount of magnitism to apply
        private float mMagnetismPercentage = 1;

        private Vector3 mNormalzedOffsetDirection;

        /// <summary>
        /// set the reference object if not set already
        /// </summary>
        private void Awake()
        {
            if (ReferenceObject == null)
            {
                ReferenceObject = CameraCache.Main.gameObject;
            }
        }

        // start the object following the reference object
        public void StartRunning()
        {

            if (ReferenceObject == null)
                ReferenceObject = CameraCache.Main.gameObject;

            mOffsetDirection = this.transform.position - ReferenceObject.transform.position;
            mOffsetDistance = mOffsetDirection.magnitude;
            mDirection = ReferenceObject.transform.forward.normalized;
            mNormalzedOffsetDirection = mOffsetDirection.normalized;
            mOffsetRotation = Quaternion.FromToRotation(mDirection, mNormalzedOffsetDirection);
            IsRunning = true;

            mMagnetismPercentage = 1;

            if (ReferenceInteractive != null)
            {
                InputManager.Instance.PushModalInputHandler(ReferenceInteractive);
            }
        }

        /// <summary>
        /// stop the object from following
        /// </summary>
        public void StopRunning()
        {

            IsRunning = false;

            if (ReferenceInteractive != null)
            {
                InputManager.Instance.PopModalInputHandler();
            }
        }

        /// <summary>
        /// update the position of the object based on the reference object and configuration
        /// </summary>
        /// <param name="position"></param>
        /// <param name="time"></param>
        protected virtual void UpdatePosition(Vector3 position, float time)
        {
            // update the position
            this.transform.position = Vector3.Lerp(this.transform.position, position, LerpPositionSpeed * time);

            // rotate to face the reference object
            if (FaceObject)
            {
                Quaternion forwardRotation = Quaternion.LookRotation(this.transform.position - ReferenceObject.transform.position);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, forwardRotation, LerpRotationSpeed * time);
            }

            // lock the x axis
            if (KeepUpRight)
            {
                Quaternion upRotation = Quaternion.FromToRotation(this.transform.up, Vector3.up);
                this.transform.rotation = upRotation * this.transform.rotation;
            }

        }

        /// <summary>
        /// Animate!
        /// </summary>
        protected virtual void Update()
        {
            if (IsRunning)
            {
                Vector3 newDirection = ReferenceObject.transform.forward;

                // move the object in front of the reference object
                if (KeepInFront)
                {
                    if (KeepStartingOffset)
                    {
                        newDirection = Vector3.Normalize(mOffsetRotation * ReferenceObject.transform.forward);
                    }
                }
                else
                {
                    newDirection = mNormalzedOffsetDirection;
                    // could we allow drifting?
                }

                // move toward the reference object
                if (Magnetism > 0)
                {

                    float magnetismDelta = MagnetismPaddingDistance / (mOffsetDistance * mMagnetismPercentage) - 1;
                    if (Mathf.Abs(magnetismDelta * 100) > 0.01f)
                    {
                        mMagnetismPercentage += Time.deltaTime * magnetismDelta * Magnetism;
                    }
                }

                Vector3 lerpPosition = ReferenceObject.transform.position + newDirection * mOffsetDistance * mMagnetismPercentage;

                UpdatePosition(lerpPosition, Time.deltaTime);
            }
        }
    }
}
