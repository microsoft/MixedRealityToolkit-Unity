// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.HandPhysics
{
    /// <summary>
    /// Updates a rigidbody transform against another transform.
    /// </summary>
    public class JointKinematicBody : MonoBehaviour
    {
        /// <summary>
        /// The joint this component tracks.
        /// </summary>
        public Transform Joint { get; set; }

        /// <summary>
        /// What hand this component lives on.
        /// </summary>
        public Handedness HandednessType { get; set; }

        /// <summary>
        /// What joint this component lives on.
        /// </summary>
        public TrackedHandJoint JointType { get; set; }

        /// <summary>
        /// An event to subscribe to when the component get's enabled. Useful for tacking when the joint loses tracking.
        /// </summary>
        public Action<JointKinematicBody> OnEnableAction { get; set; }

        /// <summary>
        /// An event to subscribe to when the component get's disabled. Useful for tacking when the joint loses tracking.
        /// </summary>
        public Action<JointKinematicBody> OnDisableAction { get; set; }

        /// <summary>
        /// Updates the position of the <see cref="JointKinematicBody"/> based on <see cref="JointType"/>.
        /// </summary>
        public void UpdateState(bool active)
        {
            bool previousActiveState = gameObject.activeSelf;
            gameObject.SetActive(active);

            if (active)
            {
                transform.position = Joint.position;
                transform.rotation = Joint.rotation;
            }

            if (previousActiveState != active)
            {
                if (active)
                {
                    OnEnableAction?.Invoke(this);
                }
                else
                {
                    OnDisableAction?.Invoke(this);
                }
            }
        }
    }
}
