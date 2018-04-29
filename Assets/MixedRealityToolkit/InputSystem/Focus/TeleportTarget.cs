// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.EventData;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Focus
{
    public class TeleportTarget : InteractableHighlight, ITeleportTarget
    {
        [SerializeField]
        [Header("Hot Spot Options")]
        private bool overrideOrientation = false;

        public Vector3 Position => transform.position;

        public Vector3 Normal => transform.up;

        public bool IsActive => isActiveAndEnabled;

        public bool OverrideTargetOrientation => overrideOrientation;

        public float TargetOrientation => transform.eulerAngles.y;

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
