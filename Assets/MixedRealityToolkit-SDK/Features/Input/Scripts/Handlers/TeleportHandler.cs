// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Input.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public class TeleportHandler : BaseFocusHandler, ITeleportTarget
    {
        [SerializeField]
        [Tooltip("Should the destination orientation be overridden? " +
                 "Useful when you want to orient the user in a specific direction when they teleport to this position. " +
                 "Override orientation is the transform forward of the GameObject this component is attached to.")]
        private bool overrideOrientation = false;

        /// <inheritdoc />
        public Vector3 Position => transform.position;

        /// <inheritdoc />
        public Vector3 Normal => transform.up;

        /// <inheritdoc />
        public bool IsActive => isActiveAndEnabled;

        /// <inheritdoc />
        public bool OverrideTargetOrientation => overrideOrientation;

        /// <inheritdoc />
        public float TargetOrientation => transform.eulerAngles.y;

        /// <inheritdoc />
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
