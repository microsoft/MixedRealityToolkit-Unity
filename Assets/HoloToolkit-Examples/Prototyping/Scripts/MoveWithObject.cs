// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Examples.Prototyping
{
    public class MoveWithObject : MonoBehaviour
    {

        public GameObject ReferenceObject;
        public bool IsRunning = false;
        public float LerpPositionSpeed = 1f;
        public float LerpRotationSpeed = 0.5f;
        public bool KeepUpRight = false;
        public GameObject ReferenceInteractive;
        public bool KeepStartingOffset = true;
        public bool FaceObject = true;
        public bool KeepInFront = true;
        public float Magnetism = 0;
        public float MagnetismPaddingDistance = 1f;

        private Vector3 mOffsetDirection;
        private Vector3 mDirection;
        private Quaternion mOffsetRotation;
        private float mOffsetDistance = 0;
        private float mMagnetismPercentage = 1;

        private Vector3 mNormalzedOffsetDirection;

        private NetworkIdentity mNetworkRoot = null;

        private void Awake()
        {
            if (ReferenceObject == null)
            {
                ReferenceObject = Camera.main.gameObject;
            }

            mNetworkRoot = GetComponentInParent<NetworkIdentity>();
        }

        public void StartRunning()
        {

            if (ReferenceObject == null)
                ReferenceObject = Camera.main.gameObject;

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

        public void StopRunning()
        {

            IsRunning = false;

            if (ReferenceInteractive != null)
            {
                InputManager.Instance.PopModalInputHandler();
            }
        }

        protected virtual void UpdatePosition(Vector3 position, float time)
        {

            this.transform.position = Vector3.Lerp(this.transform.position, position, LerpPositionSpeed * time);

            if (FaceObject)
            {
                Quaternion forwardRotation = Quaternion.LookRotation(this.transform.position - ReferenceObject.transform.position);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, forwardRotation, LerpRotationSpeed * time);
            }

            if (KeepUpRight)
            {
                Quaternion upRotation = Quaternion.FromToRotation(this.transform.up, Vector3.up);
                this.transform.rotation = upRotation * this.transform.rotation;
            }

        }

        protected virtual void Update()
        {
            if (IsRunning)
            {
                Vector3 newDirection = ReferenceObject.transform.forward;
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
                    // coould we allow drifting?
                }

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
