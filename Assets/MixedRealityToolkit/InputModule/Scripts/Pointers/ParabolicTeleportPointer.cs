// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Attributes;
using MixedRealityToolkit.UX.Distorters;
using MixedRealityToolkit.UX.Lines;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Pointers
{
    [RequireComponent(typeof(ParabolaPhysical))]
    [RequireComponent(typeof(DistorterGravity))]
    public class ParabolicTeleportPointer : TeleportPointer
    {
        [Header("Pointer Control")]
        [SerializeField]
        [Tooltip("Pointers that you want to disable while teleporting")]
        private BaseControllerPointer[] disableWhileActive;

        //[Header("Parabola settings")]
        //[SerializeField]
        //private AnimationCurve parabolaVelocityCurve = AnimationCurve.Linear(-1f, 1f, 1f, 1f);

        //[SerializeField]
        //[Range(1f, 36f)]
        //private float parabolaVelocityMultiplier = 10f;

        [SerializeField]
        [DropDownComponent(true, true)]
        private ParabolaPhysical physicsParabolaDataMain;

        public override void OnPreRaycast()
        {
            UpdateParabola();

            base.OnPreRaycast();
        }

        private void UpdateParabola()
        {
            // Make sure our parabola only rotates on y/x axis
            // NOTE: Parabola's custom line transform field should be set to a transform OTHER than its gameObject's transform
            physicsParabolaDataMain.Direction = transform.forward + Vector3.up;
            physicsParabolaDataMain.LineTransform.eulerAngles = Vector3.zero;
        }

        public override void OnSelectPressed()
        {
            base.OnSelectPressed();

            for (int i = 0; i < disableWhileActive.Length; i++)
            {
                disableWhileActive[i].InteractionEnabled = false;
            }

            // Initiate teleportation
            PointerTeleportManager.Instance.InitiateTeleport(this);
        }

        public override void OnInputReleased()
        {
            base.OnInputReleased();

            for (int i = 0; i < disableWhileActive.Length; i++)
            {
                disableWhileActive[i].InteractionEnabled = true;
            }

            // Finish teleportation
            PointerTeleportManager.Instance.TryToTeleport();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                UpdateParabola();
            }
        }
    }
}
