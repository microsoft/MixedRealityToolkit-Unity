using HoloToolkit.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public class PointerTeleportManager : Singleton<PointerTeleportManager>
    {
        public enum TeleportStateEnum
        {
            None,           // No action
            Disabled,       // Can't take action
            Initiating,     // Choosing a position and rotation
            Ready,          // Has a valid position and rotation
            Teleporting,    // Teleporting to target position
        }

        public enum TeleportPointerBehaviorEnum
        {
            Override,   // If another pointer initiates telportation, overrides any in progress
            Locked,     // First pointer to initiate telportation stays
        }

        public Action<ControllerPointerBase> OnTeleportInitiate;
        public Action<ControllerPointerBase> OnTeleportCancel;
        public Action<ControllerPointerBase> OnTeleportBegin;
        public Action<ControllerPointerBase> OnTeleportEnd;

        [SerializeField]
        private Transform playBounds;
        [SerializeField]
        private float teleportDuration = 0.25f;
        [SerializeField]
        private TeleportPointerBehaviorEnum pointerBehavior = TeleportPointerBehaviorEnum.Override;
        [SerializeField]
        private Transform helperTransform;

        private NavigationPointer currentPointer;
        private float orientationOffset = 0f;

        private Vector3 startPosition;
        private Vector3 startRotation;

        private Vector3 targetPosition;
        private Vector3 targetRotation;
        private TeleportStateEnum state;

        public TeleportStateEnum State
        {
            get
            {
                return state;
            }
        }

        public void InitiateTeleport(NavigationPointer newPointer)
        {
            if (state != TeleportStateEnum.None)
            {
                return;
            }

            currentPointer = newPointer;
            StartCoroutine(TeleportOverTime());
        }

        public void TryToTeleport()
        {
            Debug.Log("Trying to teleport with state " + state);
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

        private IEnumerator TeleportOverTime()
        {
            // If the play bounds transform is null, use the parent of the main camera
            if (playBounds == null)
            {
                playBounds = Camera.main.transform.parent;
            }

            Debug.Assert(playBounds != null, "Can't teleport without a valid playbounds transform.");

            state = TeleportStateEnum.Initiating;
            orientationOffset = 0f;

            if (OnTeleportInitiate != null)
            {
                OnTeleportInitiate(currentPointer);
            }

            while (isActiveAndEnabled)
            {
                // Use the pointer to choose a target position
                while (state == TeleportStateEnum.Initiating || state == TeleportStateEnum.Ready)
                {                    
                    switch (currentPointer.HitResult)
                    {
                        case NavigationSurfaceResultEnum.HotSpot:
                        case NavigationSurfaceResultEnum.Valid:
                            targetPosition = currentPointer.NavigationTarget;
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
                if (state == TeleportStateEnum.Teleporting) {

                    // Let everyone know we're about to begin teleporting
                    if (OnTeleportBegin != null)
                    {
                        OnTeleportBegin(currentPointer);
                    }

                    FindHelperTransform();

                    // Move the helper transform to the position and rotation of the camera
                    startPosition = Camera.main.transform.position;
                    startPosition.y = playBounds.transform.position.y;
                    startRotation = Camera.main.transform.eulerAngles;
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
                        helperTransform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(targetRotation), normalizedTime);
                        // TODO do effects here
                        yield return null;
                    }

                    // Set the play bounds to the final position
                    helperTransform.position = targetPosition;
                    helperTransform.eulerAngles = targetRotation;

                    // Un-parent the play bounds
                    playBounds.parent = playBoundsParent;

                    if (OnTeleportEnd != null)
                    {
                        OnTeleportEnd(currentPointer);
                    }
                }
                else
                {
                    if (OnTeleportCancel != null)
                    {
                        OnTeleportCancel(currentPointer);
                    }
                }

                // Now that we're done with teleporting, reset state
                // (Don't override disabled state)
                if (state != TeleportStateEnum.Disabled)
                {
                    state = TeleportStateEnum.None;
                }
                
                yield return null;

            }
            yield break;
        }

        private void FindHelperTransform()
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

    }
}