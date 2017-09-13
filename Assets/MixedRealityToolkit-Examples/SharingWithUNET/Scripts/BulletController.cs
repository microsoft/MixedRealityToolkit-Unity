// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.SharingWithUNET
{
    /// <summary>
    /// Controls little bullets fired into the world.
    /// </summary>
    public class BulletController : MonoBehaviour
    {
        private void Start()
        {
            // The bullet's transform should be in local space to the Shared Anchor.
            // Make the shared anchor the parent, but we don't want the transform to try
            // to 'preserve' the position, so we set false in SetParent.
            transform.SetParent(SharedCollection.Instance.transform, false);

            // The rigid body has a velocity that needs to be transformed into 
            // the shared coordinate system.
            Rigidbody rb = GetComponentInChildren<Rigidbody>();
            rb.velocity = transform.parent.TransformDirection(rb.velocity);
        }
    }
}
