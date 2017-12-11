using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity.UX;
using HoloToolkit.Unity;

namespace HoloToolkit.Unity.UX
{
    [Serializable]
    public enum NavigationSurfaceResultEnum
    {
        None,
        Valid,
        Invalid,
        HotSpot,
    }

    [UseWith(typeof(LineBase))]
    [UseWith(typeof(LineRendererBase))]
    [UseWith(typeof(NavigationPointerInput))]
    [RequireSingleton(typeof(PointerTeleportManager))]
    [RequireComponent(typeof(DistorterGravity))]
    public class NavigationPointer : LinePointer
    {
        [SerializeField]
        protected float minValidDot = 0.2f;

        [Header("Navigation Colors")]
        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Green, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient lineColorHotSpot;
        [SerializeField]
        [GradientDefault(GradientDefaultAttribute.ColorEnum.Red, GradientDefaultAttribute.ColorEnum.White, 1f, 0.5f)]
        protected Gradient lineColorInvalid;

        [Header("Layers & Tags")]
        [SerializeField]
        [Tooltip("Layers that are considered 'valid' for navigation")]
        protected LayerMask validLayers = 1; // Default
        [SerializeField]
        [Tooltip("Layers that are considered 'invalid' for navigation")]
        protected LayerMask invalidLayers = 1 << 2; // Ignore raycast

        protected INavigationHotSpot targetHotSpot;

        protected override void OnEnable()
        {
            base.OnEnable();

            lineBase = GetComponent<LineBase>();
            distorterGravity = GetComponent<DistorterGravity>();
            lineBase.AddDistorter(distorterGravity);
            lineRenderers = lineBase.GetComponentsInChildren<LineRendererBase>();
        }

        /// The position of the navigation target
        public virtual Vector3 NavigationTarget
        {
            get
            {
                if (!InteractionEnabled)
                {
                    return Vector3.zero;
                }

                if (HitResult == NavigationSurfaceResultEnum.HotSpot)
                {
                    return targetHotSpot.Position;
                }
                else
                {
                    return Result.End.Point;
                }
            }
        }

        /// The normal of the navigation target
        public virtual Vector3 NavigationNormal
        {
            get
            {
                if (!InteractionEnabled)
                {
                    return Vector3.up;
                }

                if (HitResult == NavigationSurfaceResultEnum.HotSpot)
                {
                    return targetHotSpot.Normal;
                }
                else
                {
                    return Result.End.Normal;
                }
            }
        }

        /// The Y rotation of the target in world-space degrees.
        public override float PointerOrientation
        {
            get
            {
                if (!InteractionEnabled)
                {
                    return 0f;
                }

                if (HitResult == NavigationSurfaceResultEnum.HotSpot && targetHotSpot.OverrideTargetOrientation)
                {
                    return targetHotSpot.TargetOrientation;
                }
                else
                {
                    // Use the camera orientation by default
                    return Camera.main.transform.eulerAngles.y - pointerOrientation;
                }
            }
            set {
                // Store pointer orientation as the difference between camera and input
                pointerOrientation = value;
            }
        }

        public override bool InteractionEnabled
        {
            get
            {
                return selectPressed || base.InteractionEnabled;
            }
        }

        public override void OnSelectPressed()
        {
            selectPressed = true;

            switch (HitResult)
            {
                case NavigationSurfaceResultEnum.Invalid:
                case NavigationSurfaceResultEnum.None:
                    return;

                default:
                    break;
            }

            PointerTeleportManager.Instance.InitiateTeleport(this);
        }

        public override void OnSelectReleased()
        {
            selectPressed = false;

            PointerTeleportManager.Instance.TryToTeleport();
        }

        /// The result of our hit
        public NavigationSurfaceResultEnum HitResult { get; protected set; }

        public Gradient GetColor(NavigationSurfaceResultEnum targetResult)
        {
            switch (targetResult)
            {
                case NavigationSurfaceResultEnum.None:
                default:
                    return lineColorNoTarget;

                case NavigationSurfaceResultEnum.Valid:
                    return lineColorValid;

                case NavigationSurfaceResultEnum.Invalid:
                    return lineColorInvalid;

                case NavigationSurfaceResultEnum.HotSpot:
                    return lineColorHotSpot;
            }
        }

        public override void OnPreRaycast()
        {
            if (lineBase == null)
                return;

            // Make sure our array will hold
            if (rays == null || rays.Length != lineCastResolution)
                rays = new RayStep[lineCastResolution];

            // Set up our rays
            // Turn off gravity so we get accurate rays
            distorterGravity.enabled = false;

            float stepSize = 1f / rays.Length;
            Vector3 lastPoint = lineBase.GetUnclampedPoint(0f);
            Vector3 currentPoint = Vector3.zero;

            for (int i = 0; i < rays.Length; i++)
            {
                currentPoint = lineBase.GetUnclampedPoint(stepSize * (i + 1));
                rays[i] = new RayStep(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }

            // Re-enable gravity if we're looking at a hotspot
            distorterGravity.enabled = (HitResult == NavigationSurfaceResultEnum.HotSpot);
        }

        public override void OnPostRaycast()
        {
            // Use the results from the last update to set our HitResult
            float clearWorldLength = 0f;
            HitResult = NavigationSurfaceResultEnum.None;
            distorterGravity.enabled = false;
            targetHotSpot = null;

            if (InteractionEnabled)
            {
                lineBase.enabled = true;

                // If we hit something
                if (Result.End.Target != null)
                {
                    // Check if it's in our valid layers
                    if (((1 << Result.End.Target.layer) & validLayers.value) != 0)
                    {
                        // See if it's a hot spot
                        if (NavigationPointer.CheckForHotSpot(Result.End.Target, out targetHotSpot) && targetHotSpot.IsActive)
                        {
                            HitResult = NavigationSurfaceResultEnum.HotSpot;
                            // Turn on gravity, point it at hotspot
                            distorterGravity.WorldCenterOfGravity = targetHotSpot.Position;
                            distorterGravity.enabled = true;
                        }
                        else
                        {
                            // If it's NOT a hotspot, check if the hit normal is too steep 
                            // (Hotspots override dot requirements)
                            if (Vector3.Dot(Result.End.Normal, Vector3.up) < minValidDot)
                            {
                                HitResult = NavigationSurfaceResultEnum.Invalid;
                            }
                            else
                            {
                                HitResult = NavigationSurfaceResultEnum.Valid;
                            }
                        }
                    }
                    else if (((1 << Result.End.Target.layer) & invalidLayers) != 0)
                    {
                        HitResult = NavigationSurfaceResultEnum.Invalid;
                    }
                    else
                    {
                        HitResult = NavigationSurfaceResultEnum.None;
                    }

                    // Use the step index to determine the length of the hit
                    for (int i = 0; i <= Result.RayStepIndex; i++)
                    {
                        if (i == Result.RayStepIndex)
                        {
                            Debug.DrawLine(Result.StartPoint + Vector3.up * 0.1f, Result.End.Point + Vector3.up * 0.1f, (HitResult != NavigationSurfaceResultEnum.None) ? Color.yellow : Color.cyan);
                            // Only add the distance between the start point and the hit
                            clearWorldLength += Vector3.Distance(Result.StartPoint, Result.End.Point);
                        }
                        else if (i < Result.RayStepIndex)
                        {
                            // Add the full length of the step to our total distance
                            clearWorldLength += rays[i].length;
                        }
                    }

                    // Clamp the end of the parabola to the result hit's point
                    lineBase.LineEndClamp = lineBase.GetNormalizedLengthFromWorldLength(clearWorldLength, lineCastResolution);
                }
                else
                {
                    lineBase.LineEndClamp = 1f;
                }

                // Set the line color
                for (int i = 0; i < lineRenderers.Length; i++)
                {
                    lineRenderers[i].LineColor = GetColor(HitResult);
                }

            }
            else
            {
                lineBase.enabled = false;
            }
        }

        public static bool CheckForHotSpot(GameObject primeFocus, out INavigationHotSpot hotSpot)
        {
            hotSpot = null;

            if (primeFocus == null)
                return false;

            // First check the target directly
            hotSpot = primeFocus.GetComponent(typeof(INavigationHotSpot)) as INavigationHotSpot;
            if (hotSpot == null)
            {
                // Then check the attached rigidbody, just in case
                Collider c = primeFocus.GetComponent<Collider>();
                if (c != null && c.attachedRigidbody != null)
                {
                    hotSpot = c.attachedRigidbody.GetComponent(typeof(INavigationHotSpot)) as INavigationHotSpot;
                }
            }

            return hotSpot != null;
        }

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(NavigationPointer))]
        public new class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}