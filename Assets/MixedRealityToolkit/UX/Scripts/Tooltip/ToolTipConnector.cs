//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace MixedRealityToolkit.UX.ToolTips
{
    /// <summary>
    /// Connects a ToolTip to a target
    /// Maintains that connection even if the target moves
    /// </summary>
    public class ToolTipConnector : MonoBehaviour
    {
        public enum FollowTypeEnum
        {
            AnchorOnly,             // The anchor will follow the target - pivot remains unaffected
            PositionOnly,           // Anchor and pivot will follow target position, but not rotation
            PositionAndYRotation,   // Anchor and pivot will follow target like it's parented, but only on Y axis
            PositionAndRotation,    // Anchor and pivot will follow target like it's parented
        }

        public enum OrientTypeEnum
        {
            OrientToObject,     // Tooltip will maintain anchor-pivot relationship relative to target object
            OrientToCamera,     // Tooltip will maintain anchor-pivot relationship relative to camera
        }

        public enum PivotModeEnum
        {
            Manual,         // Tooltip pivot will be set manually
            Automatic,      // Tooltip pivot will be set relative to object/camera based on specified direction and line length
        }

        public enum PivotDirectionEnum
        {
            Manual,         // Direction will be specified manually
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest,
            InFront,
        }
        
        public GameObject Target;

        public FollowTypeEnum FollowType = FollowTypeEnum.AnchorOnly;
        public PivotModeEnum PivotMode = PivotModeEnum.Manual;
        public PivotDirectionEnum PivotDirection = PivotDirectionEnum.North;
        public OrientTypeEnum PivotDirectionOrient = OrientTypeEnum.OrientToObject;
        public Vector3 ManualPivotDirection = Vector3.up;
        public Vector3 ManualPivotLocalPosition = Vector3.up;
        [Range(0f, 2f)]
        public float PivotDistance = 0.25f;

        private void OnEnable()
        {
            if (!FindToolTip())
                return;

            ManualPivotLocalPosition = transform.InverseTransformPoint (toolTip.PivotPosition);
        }

        private bool FindToolTip()
        {
            if (toolTip == null)
            {
                toolTip = GetComponent<ToolTip>();
            }
            if (toolTip == null)
            {
                return false;
            }

            return true;
        }

        private void UpdatePosition() {

            if (!FindToolTip())
                return;

            if (Target == null)
                return;

            switch (FollowType)
            {
                case FollowTypeEnum.AnchorOnly:
                default:
                    // Set the position of the anchor to the target's position
                    // And do nothing else
                    toolTip.Anchor.transform.position = Target.transform.position;
                    break;

                case FollowTypeEnum.PositionOnly:
                    // Move the entire tooltip transform while maintaining the anchor position offset
                    toolTip.transform.position = Target.transform.position;
                    switch (PivotMode)
                    {
                        case PivotModeEnum.Automatic:
                            Transform relativeTo = null;
                            switch (PivotDirectionOrient)
                            {
                                case OrientTypeEnum.OrientToCamera:
                                    relativeTo = Camera.main.transform;//Veil.Instance.HeadTransform;
                                    break;

                                case OrientTypeEnum.OrientToObject:
                                    relativeTo = Target.transform;
                                    break;
                            }
                            toolTip.PivotPosition = Target.transform.position + GetDirectionFromPivotDirection(
                                PivotDirection,
                                ManualPivotDirection,
                                relativeTo) * PivotDistance;
                            break;

                        case PivotModeEnum.Manual:
                            // Do nothing
                            break;
                    }
                    break;

                case FollowTypeEnum.PositionAndYRotation:
                    // Set the transform of the entire tool tip
                    // Set the pivot relative to target/camera
                    toolTip.transform.position = Target.transform.position;
                    Vector3 eulerAngles = Target.transform.eulerAngles;
                    eulerAngles.x = 0f;
                    eulerAngles.z = 0f;
                    toolTip.transform.eulerAngles = eulerAngles;
                    switch (PivotMode)
                    {
                        case PivotModeEnum.Automatic:
                            Transform relativeTo = null;
                            switch (PivotDirectionOrient)
                            {
                                case OrientTypeEnum.OrientToCamera:
                                    relativeTo = Camera.main.transform;//Veil.Instance.HeadTransform;
                                    break;

                                case OrientTypeEnum.OrientToObject:
                                    relativeTo = Target.transform;
                                    break;
                            }
                            Vector3 localPosition = GetDirectionFromPivotDirection(PivotDirection, ManualPivotDirection, relativeTo) * PivotDistance;
                            toolTip.PivotPosition = Target.transform.position + localPosition;
                            break;

                        case PivotModeEnum.Manual:
                            // Do nothing
                            break;
                    }
                    break;

                case FollowTypeEnum.PositionAndRotation:
                    // Set the transform of the entire tool tip
                    // Set the pivot relative to target/camera
                    toolTip.transform.position = Target.transform.position;
                    toolTip.transform.rotation = Target.transform.rotation;
                    switch (PivotMode)
                    {
                        case PivotModeEnum.Automatic:
                            Transform relativeTo = null;
                            switch (PivotDirectionOrient)
                            {
                                case OrientTypeEnum.OrientToCamera:
                                    relativeTo = Camera.main.transform;//Veil.Instance.HeadTransform;
                                    break;

                                case OrientTypeEnum.OrientToObject:
                                    relativeTo = Target.transform;
                                    break;
                            }
                            toolTip.PivotPosition = Target.transform.position + GetDirectionFromPivotDirection(
                                PivotDirection,
                                ManualPivotDirection,
                                relativeTo) * PivotDistance;
                            break;

                        case PivotModeEnum.Manual:
                            // Do nothing
                            break;
                    }
                    break;
            }
        }

        private void Update()
        {
            UpdatePosition();
        }

        [SerializeField]
        private ToolTip toolTip;

        private void OnDrawGizmos ()
        {
            if (Application.isPlaying)
                return;
            
            UpdatePosition();
        }

        public static Vector3 GetDirectionFromPivotDirection (PivotDirectionEnum pivotDirection, Vector3 manualPivotDirection, Transform relativeTo)
        {
            Vector3 dir = Vector3.zero;
            switch (pivotDirection)
            {
                case PivotDirectionEnum.North:
                    dir = Vector3.up;
                    break;

                case PivotDirectionEnum.NorthEast:
                    dir = Vector3.Lerp(Vector3.up, Vector3.right, 0.5f).normalized;
                    break;

                case PivotDirectionEnum.East:
                    dir = Vector3.right;
                    break;

                case PivotDirectionEnum.SouthEast:
                    dir = Vector3.Lerp(Vector3.down, Vector3.right, 0.5f).normalized;
                    break;

                case PivotDirectionEnum.South:
                    dir = Vector3.down;
                    break;

                case PivotDirectionEnum.SouthWest:
                    dir = Vector3.Lerp(Vector3.down, Vector3.left, 0.5f).normalized;
                    break;

                case PivotDirectionEnum.West:
                    dir = Vector3.left;
                    break;

                case PivotDirectionEnum.NorthWest:
                    dir = Vector3.Lerp(Vector3.up, Vector3.left, 0.5f).normalized;
                    break;

                case PivotDirectionEnum.InFront:
                    dir = Vector3.forward;
                    break;

                case PivotDirectionEnum.Manual:
                    dir = manualPivotDirection.normalized;
                    break;
            }

            return relativeTo.TransformDirection(dir);
        }
    }
}