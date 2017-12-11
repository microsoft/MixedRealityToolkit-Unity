using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.UX;

namespace HoloToolkit.Unity.Controllers
{
    public class TeleportPad : MonoBehaviour
    {
        private void OnEnable()
        {
            // Place ourselves into the scene
            transform.parent = null;
        }

        private void Update()
        {
            // Wait for teleportation to finish before making any changes
            if (PointerTeleportManager.Instance.State == PointerTeleportManager.TeleportStateEnum.Teleporting)
            {
                return;
            }

            if (pointer.InteractionEnabled)
            {
                switch (pointer.HitResult)
                {
                    case NavigationSurfaceResultEnum.None:
                    default:
                        animator.SetBool("Disabled", true);
                        break;

                    case NavigationSurfaceResultEnum.Invalid:
                        animator.SetBool("Disabled", false);
                        animator.SetBool("Valid", false);
                        break;

                    case NavigationSurfaceResultEnum.HotSpot:
                        animator.SetBool("Disabled", false);
                        animator.SetBool("Valid", true);
                        break;

                    case NavigationSurfaceResultEnum.Valid:
                        animator.SetBool("Disabled", false);
                        animator.SetBool("Valid", true);
                        break;
                }
            }
            else
            {
                animator.SetBool("Disabled", true);
                return;
            }

            transform.position = pointer.NavigationTarget;
            Quaternion rotation = Quaternion.identity;

            // This is kind of a cheat - if the result is valid we can be reasonably sure the dot will permit using the camera's forward
            if (pointer.HitResult == NavigationSurfaceResultEnum.Valid)
            {
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0f;
                rotation = Quaternion.LookRotation(forward.normalized, pointer.NavigationNormal);
            }
            else
            {
                // Otherwise, just use the navigation normal directly
                rotation = Quaternion.FromToRotation(Vector3.up, pointer.NavigationNormal);
            }

            // Smooth out rotation just a tad to prevent jarring transitions
            baseTransform.rotation = Quaternion.Lerp(baseTransform.rotation, rotation, 0.5f);

            // Point the arrow towards the target orientation
            arrowTransform.eulerAngles = new Vector3(0f, pointer.PointerOrientation, 0f);
        }

        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform arrowTransform;
        [SerializeField]
        private Transform baseTransform;
        [SerializeField]
        private NavigationPointer pointer;        
    }
}