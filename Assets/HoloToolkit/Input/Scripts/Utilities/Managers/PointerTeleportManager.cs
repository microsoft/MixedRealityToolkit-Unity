// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class PointerTeleportManager : Singleton<PointerTeleportManager>
    {
        private enum TeleportStateEnum
        {
            None,           // No action
            Disabled,       // Can't take action
            Initiating,     // Choosing a valid position and rotation
            Ready,          // Has a valid position and rotation
            Teleporting,    // Teleporting to target position
        }

        private enum TeleportPointerBehaviorEnum
        {
            Override,   // If another pointer initiates teleportation, overrides any in progress
            Locked,     // First pointer to initiate teleportation stays
        }

        [SerializeField]
        private Transform playBounds;

        [SerializeField]
        private float teleportDuration = 0.25f;

        [SerializeField]
        private TeleportPointerBehaviorEnum pointerBehavior = TeleportPointerBehaviorEnum.Override;

        [SerializeField]
        private Transform helperTransform;

        private TeleportPointer currentPointer;

        private Vector3 startPosition;
        private Vector3 startRotation;

        private Vector3 targetPosition;
        private Vector3 targetRotation;

        private TeleportStateEnum state = TeleportStateEnum.None;

        private void Start()
        {
            if (helperTransform == null)
            {
                helperTransform = transform.Find("HelperTransform");
                if (helperTransform == null)
                {
                    helperTransform = new GameObject("HelperTransform").transform;
                    helperTransform.parent = transform;
                }
            }
        }

        /// <summary>
        /// Try to initiate a Teleport, and cancel if already attempting to do so.
        /// </summary>
        public void TryToTeleport()
        {
            switch (state)
            {
                case TeleportStateEnum.Ready:
                case TeleportStateEnum.Teleporting:
                    // Proceed with the teleport
                    state = TeleportStateEnum.Teleporting;
                    break;
                default:
                    // Cancel the teleport
                    state = TeleportStateEnum.None;
                    break;
            }
        }

        public void InitiateTeleport(TeleportPointer newPointer)
        {
            if (state != TeleportStateEnum.None &&
                pointerBehavior == TeleportPointerBehaviorEnum.Override)
            {
                return;
            }

            currentPointer = newPointer;
            StartCoroutine(TeleportOverTime());
        }

        private IEnumerator TeleportOverTime()
        {
            // If the play bounds transform is null, use the parent of the main camera
            if (playBounds == null)
            {
                playBounds = CameraCache.Main.transform.parent;
            }

            Debug.Assert(playBounds != null, "Can't teleport without a valid play bounds transform.");

            state = TeleportStateEnum.Initiating;

            InputManager.Instance.RaiseTeleportIntent(currentPointer);

            while (isActiveAndEnabled)
            {
                // Use the pointer to choose a target position
                while (state == TeleportStateEnum.Initiating || state == TeleportStateEnum.Ready)
                {
                    switch (currentPointer.TeleportSurfaceResult)
                    {
                        case TeleportSurfaceResult.HotSpot:
                        case TeleportSurfaceResult.Valid:
                            targetPosition = currentPointer.TeleportTargetPosition;
                            targetRotation = new Vector3(0f, currentPointer.PointerOrientation, 0f);
                            state = TeleportStateEnum.Ready;
                            break;
                        default:
                            state = TeleportStateEnum.Initiating;
                            break;
                    }

                    yield return null;
                }

                // If the state has been set to teleporting
                // Do the teleport
                if (state == TeleportStateEnum.Teleporting)
                {
                    InputManager.Instance.RaiseTeleportStarted(currentPointer);

                    Debug.Assert(helperTransform != null, "Can't teleport without a valid helper transform!");

                    // Move the helper transform to the position and rotation of the camera
                    startPosition = CameraCache.Main.transform.position;
                    startPosition.y = playBounds.transform.position.y;
                    startRotation = CameraCache.Main.transform.eulerAngles;
                    startRotation.x = 0f;
                    startRotation.z = 0f;

                    helperTransform.position = startPosition;
                    helperTransform.eulerAngles = startRotation;

                    // Briefly parent our play bounds under our helper transform
                    Transform playBoundsParent = playBounds.parent;
                    playBounds.parent = helperTransform;

                    float startTime = Time.unscaledTime;
                    while (Time.unscaledTime < startTime + teleportDuration)
                    {
                        float normalizedTime = (Time.unscaledTime - startTime) / teleportDuration;
                        helperTransform.position = Vector3.Lerp(startPosition, targetPosition, normalizedTime);
                        helperTransform.rotation = Quaternion.Lerp(
                                Quaternion.Euler(startRotation),
                                Quaternion.Euler(targetRotation),
                                normalizedTime);

                        // TODO do effects here

                        yield return null;
                    }

                    // Set the play bounds to the final position
                    helperTransform.position = targetPosition;
                    helperTransform.eulerAngles = targetRotation;

                    // Un-parent the play bounds
                    playBounds.parent = playBoundsParent;

                    InputManager.Instance.RaiseTeleportCompleted(currentPointer);
                }
                else
                {
                    InputManager.Instance.RaiseTeleportCanceled(currentPointer);
                }

                // Now that we're done with teleporting, reset state
                // (Don't override disabled state)
                if (state != TeleportStateEnum.Disabled)
                {
                    state = TeleportStateEnum.None;
                }

                yield return null;
            }
        }
    }
}
