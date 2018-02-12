// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.Examples.Grabbables
{
    /// <summary>
    /// Simple class to change the color of grabbable objects based on state
    /// </summary>
    public class GrabbableColor : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField]
        private Color colorOnContactSingle = Color.blue;

        [SerializeField]
        private Color colorOnContactMulti = Color.cyan;

        [SerializeField]
        private Color colorOnGrabSingle = Color.yellow;

        [SerializeField]
        private Color colorOnGrabMulti = Color.red;

        [Header("Objects")]
        [SerializeField]
        private Renderer targetRenderer;

        [SerializeField]
        private BaseGrabbable grabbable;

        private Color originalColor;
        private void Awake()
        {
            if (grabbable == null)
            {
                grabbable = GetComponent<BaseGrabbable>();
            }

            if (targetRenderer == null)
            {
                targetRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
            }

            originalColor = targetRenderer.material.color;
            grabbable.OnContactStateChange += RefreshColor;
            grabbable.OnGrabStateChange += RefreshColor;
        }

        private void RefreshColor(BaseGrabbable baseGrab)
        {
            Color finalColor = originalColor;

            switch (baseGrab.ContactState)
            {
                case GrabStateEnum.Inactive:
                    break;

                case GrabStateEnum.Multi:
                    finalColor = colorOnContactMulti;
                    break;

                case GrabStateEnum.Single:
                    finalColor = colorOnContactSingle;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (baseGrab.GrabState)
            {
                case GrabStateEnum.Inactive:
                    break;

                case GrabStateEnum.Multi:
                    finalColor = colorOnGrabMulti;
                    break;

                case GrabStateEnum.Single:
                    finalColor = colorOnGrabSingle;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            targetRenderer.material.color = finalColor;
        }
    }
}
