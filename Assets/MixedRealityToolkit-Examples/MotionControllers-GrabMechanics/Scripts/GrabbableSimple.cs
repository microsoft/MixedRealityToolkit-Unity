// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Grabbables
{
    /// <summary>
    /// This type of grab makes the grabbed object follow the position and rotation of the grabber, but does not create a parent child relationship
    /// </summary>
    public class GrabbableSimple : BaseGrabbable
    {
        private Rigidbody _rigidbody;

        [SerializeField]
        private bool matchPosition = true;
        [SerializeField]
        private bool matchRotation = false;

        protected override void Start()
        {
            base.Start();
            _rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Specify the target and turn off gravity. Otherwise gravity will interfere with desired grab effect
        /// </summary>
        protected override void StartGrab(BaseGrabber grabber)
        {
            base.StartGrab(grabber);
            if (_rigidbody)
            {
                _rigidbody.useGravity = false;
            }
        }

        /// <summary>
        /// On release turn gravity back on the so the object falls and set the target back to null
        /// </summary>
        protected override void EndGrab()
        {
            if (_rigidbody)
            {
                _rigidbody.useGravity = true;
            }

            base.EndGrab();
        }

        protected override void OnGrabStay()
        {
            if (matchPosition)
            {
                transform.position = GrabberPrimary.GrabHandle.position;
            }

            if (matchRotation)
            {
                transform.rotation = GrabberPrimary.GrabHandle.rotation;
            }
        }

        // The next two functions provide basic behaviour. Extend from this base script in order to provide more specific functionality.

        protected override void AttachToGrabber(BaseGrabber grabber)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            _rigidbody.isKinematic = true;
            if (!activeGrabbers.Contains(grabber))
            {
                activeGrabbers.Add(grabber);
            }
        }

        protected override void DetachFromGrabber(BaseGrabber grabber)
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
        }
    }
}
