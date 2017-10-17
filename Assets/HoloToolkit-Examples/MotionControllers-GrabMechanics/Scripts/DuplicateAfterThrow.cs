// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    public class DuplicateAfterThrow : MonoBehaviour
    {
        public GameObject ThrowObject;
        private Vector3 startPos;
        private Quaternion startRot;
        private Color startColor;

        protected virtual void OnEnable()
        {
            grabbable.OnReleased += SpawnDuplicate;
        }

        protected virtual void OnDisable()
        {
            grabbable.OnReleased -= SpawnDuplicate;
        }

        protected virtual void Awake()
        {
            grabbable = GetComponent<BaseGrabbable>();
        }

        private void Start()
        {
            startPos = transform.position;
            startRot = transform.rotation;
            startColor = GetComponent<Renderer>().material.color;
            ThrowObject = gameObject;
        }

        /// <summary>
        /// For the demo only - if we throw an object, we respawn it at its initial location with the same throw properties as the previous one.
        /// This way a user can try out throw a few times
        /// </summary>
        /// <param name="baseGrab"></param>
        private void SpawnDuplicate(BaseGrabbable baseGrab)
        {
            GameObject thrown = Instantiate(ThrowObject, startPos, Quaternion.identity);
            thrown.GetComponent<ThrowableObject>().ZeroGravityThrow = GetComponent<BaseThrowable>().ZeroGravityThrow;
            thrown.GetComponent<ThrowableObject>().ThrowMultiplier = GetComponent<BaseThrowable>().ThrowMultiplier;
            thrown.GetComponent<Renderer>().material.color = startColor;
            thrown.GetComponent<Rigidbody>().useGravity = true;
            thrown.transform.rotation = startRot;
        }

        private BaseGrabbable grabbable;
    }
}
