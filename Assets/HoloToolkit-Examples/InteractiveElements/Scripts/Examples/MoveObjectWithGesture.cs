// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.InteractiveElements
{
    public class MoveObjectWithGesture : MoveWithObject
    {

        public bool GestureWithGaze = true;
        public Vector3 GestureOffset;

        private Vector3 mPreviousGestureOffset = Vector3.zero;

        // Use this for initialization
        void Start()
        {
            GestureOffset = Vector3.zero;
        }

        protected override void UpdatePosition(Vector3 position, float time)
        {
            //base.UpdatePosition(position, time);
            
            Vector3 newPosition;
            if (GestureWithGaze)
            {
                newPosition = position + GestureOffset;
            }
            else
            {
                newPosition = transform.position + (GestureOffset - mPreviousGestureOffset);
                mPreviousGestureOffset = GestureOffset;
            }

            this.transform.position = Vector3.Lerp(this.transform.position, newPosition, LerpPositionSpeed * time);

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

        protected override void Update()
        {
            if (!IsRunning)
            {
                mPreviousGestureOffset = Vector3.zero;
            }

            base.Update();
        }
    }
}
