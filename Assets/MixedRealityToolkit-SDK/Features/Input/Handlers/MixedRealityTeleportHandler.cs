// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Teleport;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.TeleportSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.SDK.UX.Pointers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Input.Handlers
{
    /// <summary>
    /// SDK component handling teleportation when a user focuses this <see cref="GameObject"/> and triggers the teleport action.
    /// </summary>
    public class MixedRealityTeleportHandler : BaseFocusHandler, IMixedRealityTeleportTarget, IMixedRealityTeleportHandler
    {
        private static IMixedRealityTeleportSystem teleportSystem = null;
        protected static IMixedRealityTeleportSystem TeleportSystem => teleportSystem ?? (teleportSystem = MixedRealityManager.Instance.GetManager<IMixedRealityTeleportSystem>());

        #region IMixedRealityFocusHandler Implementation

        /// <inheritdoc />
        public override void OnBeforeFocusChange(FocusEventData eventData)
        {
            base.OnBeforeFocusChange(eventData);

            if (!(eventData.Pointer is TeleportPointer)) { return; }

            if (eventData.NewFocusedObject == gameObject)
            {
                eventData.Pointer.TeleportTarget = this;

                if (eventData.Pointer.IsInteractionEnabled)
                {
                    TeleportSystem.RaiseTeleportCanceled(eventData.Pointer, this);
                    TeleportSystem.RaiseTeleportRequest(eventData.Pointer, this);
                }
            }
            else if (eventData.OldFocusedObject == gameObject)
            {
                eventData.Pointer.TeleportTarget = null;

                if (eventData.Pointer.IsInteractionEnabled)
                {
                    TeleportSystem.RaiseTeleportCanceled(eventData.Pointer, this);
                }
            }
        }

        #endregion IMixedRealityFocusHandler Implementation

        #region IMixedRealityTeleportTarget Implementation

        /// <inheritdoc />
        public Vector3 Position => transform.position;

        /// <inheritdoc />
        public Vector3 Normal => transform.up;

        /// <inheritdoc />
        public bool IsActive => isActiveAndEnabled;

        [SerializeField]
        [Tooltip("Should the destination orientation be overridden? " +
                 "Useful when you want to orient the user in a specific direction when they teleport to this position. " +
                 "Override orientation is the transform forward of the GameObject this component is attached to.")]
        private bool overrideOrientation = false;

        /// <inheritdoc />
        public bool OverrideTargetOrientation => overrideOrientation;

        /// <inheritdoc />
        public float TargetOrientation => transform.eulerAngles.y;

        /// <inheritdoc />
        public GameObject GameObjectReference => gameObject;

        #endregion IMixedRealityTeleportTarget Implementation

        #region IMixedRealityTeleportHandler Implementation

        public void OnTeleportRequest(TeleportEventData eventData)
        {
        }

        public void OnTeleportStarted(TeleportEventData eventData)
        {
        }

        public void OnTeleportCompleted(TeleportEventData eventData)
        {
        }

        public void OnTeleportCanceled(TeleportEventData eventData)
        {
        }

        #endregion IMixedRealityTeleportHandler Implementation

        private void OnDrawGizmos()
        {
            Gizmos.color = IsActive ? Color.green : Color.red;
            Gizmos.DrawLine(Position + (Vector3.up * 0.1f), Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f));
            Gizmos.DrawSphere(Position + (Vector3.up * 0.1f) + (transform.forward * 0.1f), 0.01f);
        }

    }
}
