// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace MixedRealityToolkit.InputModule.Focus
{
    public class TeleportTarget : InteractableHighlight, ITeleportTarget
    {
        [Header("Hot Spot Options")]
        [SerializeField]
        private bool overrideOrientation;

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public Vector3 Normal
        {
            get { return transform.up; }
        }

        public bool IsActive
        {
            get { return isActiveAndEnabled; }
        }

        public bool OverrideTargetOrientation
        {
            get { return overrideOrientation; }
        }

        public float TargetOrientation
        {
            get { return transform.eulerAngles.y; }
        }

        public override void OnBeforeFocusChange(FocusEventData eventData)
        {
            base.OnBeforeFocusChange(eventData);

            if (eventData.NewFocusedObject == gameObject)
            {
                eventData.Pointer.TeleportTarget = this;
            }

            if (eventData.OldFocusedObject == gameObject)
            {
                eventData.Pointer.TeleportTarget = null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = IsActive ? Color.green : Color.red;
            Gizmos.DrawLine(Position + (Vector3.up * 0.1f), Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f));
            Gizmos.DrawSphere(Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f), 0.01f);
        }
    }
}
