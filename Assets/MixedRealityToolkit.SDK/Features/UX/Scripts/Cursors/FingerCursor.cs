// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Cursor used to aide in near finger interactions.
    /// </summary>
    public class FingerCursor : BaseCursor
    {
        [Header("Ring Motion")]
        [SerializeField]
        [Tooltip("Should the cursor react to near grabbables.")]
        private bool checkForGrabbables = false;

        [SerializeField]
        [Tooltip("Positional offset from the finger's skin surface.")]
        private float skinSurfaceOffset = 0.01f;

        [SerializeField]
        [Tooltip("At what distance should the cursor align with the surface. (Should be < alignWithFingerDistance)")]
        private float alignWithSurfaceDistance = 0.1f;

        [Header("Ring Visualization")]
        [SerializeField]
        [Tooltip("Renderer representing the ring attached to the index finger using an MRTK/Standard material with the round corner feature enabled.")]
        protected Renderer indexFingerRingRenderer;

        private MaterialPropertyBlock materialPropertyBlock;
        private int proximityDistanceID;
        private readonly Quaternion fingerPadRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

        protected virtual void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            proximityDistanceID = Shader.PropertyToID("_Proximity_Distance_");
        }

        /// <summary>
        /// Override base behavior to align the cursor with the finger, else perform normal cursor transformations.
        /// </summary>
        protected override void UpdateCursorTransform() 
        {
            IMixedRealityNearPointer nearPointer = (IMixedRealityNearPointer)Pointer;

            // When the pointer has a IMixedRealityNearPointer interface we don't call base.UpdateCursorTransform because we handle 
            // cursor transformation a bit differently.
            if (nearPointer != null)
            {
                float deltaTime = UseUnscaledTime
                    ? Time.unscaledDeltaTime
                    : Time.deltaTime;

                Vector3 indexFingerPosition;
                Quaternion indexFingerRotation;
                if (!TryGetJoint(TrackedHandJoint.IndexTip, out indexFingerPosition, out indexFingerRotation))
                {
                    indexFingerPosition = transform.position;
                    indexFingerRotation = transform.rotation;
                }

                Vector3 indexKnucklePosition;
                if (!TryGetJoint(TrackedHandJoint.IndexKnuckle, out indexKnucklePosition, out _)) // knuckle rotation not used
                {
                    indexKnucklePosition = transform.position;
                }
                
                if (!nearPointer.IsInteractionEnabled)
                {
                    // If the pointer is disabled, make sure to turn the ring cursor off
                    // but still want show the proximity effect on bounding content
                    if (indexFingerRingRenderer != null)
                    {
                        UpdateVisuals(indexFingerRingRenderer, 1, false);
                    }
                }
                else if (nearPointer.IsNearObject)
                {
                    // If the pointer is near an object translate the primary ring to the index finger tip and rotate to surface normal if close.
                    // The secondary ring should be hidden.

                    float distance;
                    if (!nearPointer.TryGetDistanceToNearestSurface(out distance))
                    {
                        distance = float.MaxValue;
                    }

                    if (indexFingerRingRenderer != null)
                    {
                        TranslateToFinger(indexFingerRingRenderer.transform, deltaTime, indexFingerPosition, indexKnucklePosition);

                        Vector3 surfaceNormal;
                        if ((distance < alignWithSurfaceDistance) &&
                            nearPointer.TryGetNormalToNearestSurface(out surfaceNormal))
                        {
                            RotateToSurfaceNormal(indexFingerRingRenderer.transform, surfaceNormal, indexFingerRotation, distance);
                            TranslateFromTipToPad(indexFingerRingRenderer.transform, indexFingerPosition, indexKnucklePosition, surfaceNormal, distance);
                        }
                        else
                        {
                            RotateToFinger(indexFingerRingRenderer.transform, deltaTime, indexFingerRotation);
                        }

                        UpdateVisuals(indexFingerRingRenderer, distance, true);
                    }
                }
                else
                {
                    // If the pointer is near a grabbable object position and rotate the ring to the default, 
                    // else hide it.

                    float distance = 0;
                    bool nearGrabbable = checkForGrabbables && IsNearGrabbableObject(out distance);

                    if (indexFingerRingRenderer != null)
                    {
                        TranslateToFinger(indexFingerRingRenderer.transform, deltaTime, indexFingerPosition, indexKnucklePosition);
                        RotateToFinger(indexFingerRingRenderer.transform, deltaTime, indexFingerRotation);
                        UpdateVisuals(indexFingerRingRenderer, distance, nearGrabbable);
                    }
                }
            }
            else
            {
                base.UpdateCursorTransform();
            }
        }

        /// <summary>
        /// Applies material overrides to a ring renderer.
        /// </summary>
        /// <param name="ringRenderer">Renderer using an MRTK/Standard material with the round corner feature enabled.</param>
        /// <param name="distance">Distance between the ring and surface.</param>
        /// <param name="visible">Should the ring be visible?</param>
        protected virtual void UpdateVisuals(Renderer ringRenderer, float distance, bool visible)
        {
            ringRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetFloat(proximityDistanceID, visible ? distance : 1.0f);
            ringRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        /// <summary>
        /// Gets if the associated sphere pointer on this controller is near any grabbable objects.
        /// </summary>
        /// <returns>True if associated sphere pointer is near any grabbable objects, else false.</returns>
        /// <param name="dist">Out parameter gets the distance to the grabbable.</param>
        protected virtual bool IsNearGrabbableObject(out float dist)
        {
            var focusProvider = CoreServices.InputSystem?.FocusProvider;
            if (focusProvider != null)
            {
                var spherePointers = focusProvider.GetPointers<SpherePointer>();
                foreach (var spherePointer in spherePointers)
                {
                    if (spherePointer.Controller == Pointer.Controller)
                    {
                        var focusObject = focusProvider.GetFocusedObject(spherePointer);
                        if (focusObject != null)
                        {
                            return spherePointer.TryGetDistanceToNearestSurface(out dist);
                        }
                    }
                }
            }

            dist = -1;
            return false;
        }

        /// <summary>
        /// Tries and get's hand joints based on the current pointer.
        /// </summary>
        /// <param name="joint">The joint type to get.</param>
        /// <param name="position">Out parameter filled with joint position, otherwise 
        /// <see href="https://docs.unity3d.com/ScriptReference/Vector3-zero.html">Vector3.zero</see></param>
        /// <param name="rotation">Out parameter filled with joint rotation, otherwise 
        /// <see href="https://docs.unity3d.com/ScriptReference/Quaternion-identity.html">Quaternion.identity</see></param>
        protected bool TryGetJoint(TrackedHandJoint joint, out Vector3 position, out Quaternion rotation)
        {
            if (Pointer != null)
            {
                var hand = Pointer.Controller as IMixedRealityHand;
                if (hand != null)
                {
                    if (hand.TryGetJoint(joint, out MixedRealityPose handJoint))
                    {
                        position = handJoint.Position;
                        rotation = handJoint.Rotation;

                        return true;
                    }
                }
            }

            position = Vector3.zero;
            rotation = Quaternion.identity;

            return false;
        }

        private void TranslateToFinger(Transform target, float deltaTime, Vector3 fingerPosition, Vector3 knucklePosition)
        {
            var targetPosition = fingerPosition + (fingerPosition - knucklePosition).normalized * skinSurfaceOffset;
            target.position = Vector3.Lerp(target.position, targetPosition, deltaTime / PositionLerpTime);
        }

        private void RotateToFinger(Transform target, float deltaTime, Quaternion pointerRotation)
        {
            target.rotation = Quaternion.Lerp(target.rotation, pointerRotation, deltaTime / RotationLerpTime);
        }

        private void RotateToSurfaceNormal(Transform target, Vector3 surfaceNormal, Quaternion pointerRotation, float distance)
        {
            var t = distance / alignWithSurfaceDistance;
            var targetRotation = Quaternion.LookRotation(-surfaceNormal);
            target.rotation = Quaternion.Slerp(targetRotation, pointerRotation, t);
        }

        private void TranslateFromTipToPad(Transform target, Vector3 fingerPosition, Vector3 knucklePosition, Vector3 surfaceNormal, float distance)
        {
            var t = distance / alignWithSurfaceDistance;

            Vector3 tipNormal = (fingerPosition - knucklePosition).normalized;
            Vector3 tipPosition = fingerPosition + tipNormal * skinSurfaceOffset;
            Vector3 tipOffset = tipPosition - fingerPosition;

            // Check how perpindicular the finger normal is to the surface, so that the cursor will
            // not translate to the finger pad if the user is poking with a horizontal finger
            float fingerSurfaceDot = Vector3.Dot(tipNormal, -surfaceNormal);

            // Lerping an angular measurement from 0 degrees (default cursor position at tip of finger) to
            // 90 degrees (a new position on the fingertip pad) around the fingertip's X axis.
            Quaternion degreesRelative = Quaternion.AngleAxis((1f - t) * 90f * (1f - fingerSurfaceDot), indexFingerRingRenderer.transform.right);

            Vector3 tipToPadPosition = fingerPosition + degreesRelative * tipOffset;
            indexFingerRingRenderer.transform.position = tipToPadPosition;
        }
    }
}
