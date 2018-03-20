// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using MixedRealityToolkit.Common.Extensions;

namespace MixedRealityToolkit.UX.ToolTips
{
    [RequireComponent(typeof(MeshFilter))]

    /// <summary>
    /// Connects a ToolTip to a target
    /// Maintains that connection even if the target moves
    /// </summary>
    public class ToolTipConnector : MonoBehaviour
    {
        public enum ConnectorFollowType
        {
            AnchorOnly,             // The anchor will follow the target - pivot remains unaffected
            PositionOnly,           // Anchor and pivot will follow target position, but not rotation
            PositionAndYRotation,   // Anchor and pivot will follow target like it's parented, but only on Y axis
            PositionAndRotation,    // Anchor and pivot will follow target like it's parented
        }

        public enum ConnectorOrientType
        {
            OrientToObject,     // Tooltip will maintain anchor-pivot relationship relative to target object
            OrientToCamera,     // Tooltip will maintain anchor-pivot relationship relative to camera
        }

        public enum ConnnectorPivotMode
        {
            Manual,         // Tooltip pivot will be set manually
            Automatic,      // Tooltip pivot will be set relative to object/camera based on specified direction and line length
        }

        public enum ConnectorPivotDirection
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

        [SerializeField]
        private ToolTip toolTip;

        [SerializeField]
        private ConnectorFollowType connectorFollowType = ConnectorFollowType.AnchorOnly;

        [SerializeField]
        private ConnnectorPivotMode pivotMode = ConnnectorPivotMode.Manual;

        [SerializeField]
        private ConnectorPivotDirection pivotDirection = ConnectorPivotDirection.North;

        [SerializeField]
        private ConnectorOrientType pivotDirectionOrient = ConnectorOrientType.OrientToObject;

        [SerializeField]
        private Vector3 manualPivotDirection = Vector3.up;

        [SerializeField]
        private Vector3 manualPivotLocalPosition = Vector3.up;

        [SerializeField]
        [Range(0f, 2f)]
        private float pivotDistance = 0.25f;

        private void OnEnable()
        {
            if (!FindToolTip())
            {
                return;
            }

            ManualPivotLocalPosition = transform.InverseTransformPoint (toolTip.PivotPosition);
        }

        private bool FindToolTip()
        {
            toolTip = gameObject.EnsureComponent<ToolTip>();
            return toolTip != null;
        }

        private void UpdatePosition() {

            if (!FindToolTip())
            {
                return;
            }

            if (Target == null)
            {
                return;
            }

            switch (connectorFollowType)
            {
                case ConnectorFollowType.AnchorOnly:
                default:
                    // Set the position of the anchor to the target's position
                    // And do nothing else
                    toolTip.Anchor.transform.position = Target.transform.position;
                    break;

                case ConnectorFollowType.PositionOnly:
                    // Move the entire tooltip transform while maintaining the anchor position offset
                    toolTip.transform.position = Target.transform.position;
                    switch (PivotingMode)
                    {
                        case ConnnectorPivotMode.Automatic:
                            Transform relativeTo = null;
                            switch (PivotDirectionOrient)
                            {
                                case ConnectorOrientType.OrientToCamera:
                                    relativeTo = Camera.main.transform;//Veil.Instance.HeadTransform;
                                    break;

                                case ConnectorOrientType.OrientToObject:
                                    relativeTo = Target.transform;
                                    break;
                            }
                            toolTip.PivotPosition = Target.transform.position + GetDirectionFromPivotDirection(
                                PivotDirection,
                                ManualPivotDirection,
                                relativeTo) * pivotDistance;
                            break;

                        case ConnnectorPivotMode.Manual:
                            // Do nothing
                            break;
                    }
                    break;

                case ConnectorFollowType.PositionAndYRotation:
                    // Set the transform of the entire tool tip
                    // Set the pivot relative to target/camera
                    toolTip.transform.position = Target.transform.position;
                    Vector3 eulerAngles = Target.transform.eulerAngles;
                    eulerAngles.x = 0f;
                    eulerAngles.z = 0f;
                    toolTip.transform.eulerAngles = eulerAngles;
                    switch (PivotingMode)
                    {
                        case ConnnectorPivotMode.Automatic:
                            Transform relativeTo = null;
                            switch (PivotDirectionOrient)
                            {
                                case ConnectorOrientType.OrientToCamera:
                                    relativeTo = Camera.main.transform;//Veil.Instance.HeadTransform;
                                    break;

                                case ConnectorOrientType.OrientToObject:
                                    relativeTo = Target.transform;
                                    break;
                            }
                            Vector3 localPosition = GetDirectionFromPivotDirection(PivotDirection, ManualPivotDirection, relativeTo) * pivotDistance;
                            toolTip.PivotPosition = Target.transform.position + localPosition;
                            break;

                        case ConnnectorPivotMode.Manual:
                            // Do nothing
                            break;
                    }
                    break;

                case ConnectorFollowType.PositionAndRotation:
                    // Set the transform of the entire tool tip
                    // Set the pivot relative to target/camera
                    toolTip.transform.position = Target.transform.position;
                    toolTip.transform.rotation = Target.transform.rotation;
                    switch (PivotingMode)
                    {
                        case ConnnectorPivotMode.Automatic:
                            Transform relativeTo = null;
                            switch (PivotDirectionOrient)
                            {
                                case ConnectorOrientType.OrientToCamera:
                                    relativeTo = Camera.main.transform;//Veil.Instance.HeadTransform;
                                    break;

                                case ConnectorOrientType.OrientToObject:
                                    relativeTo = Target.transform;
                                    break;
                            }
                            toolTip.PivotPosition = Target.transform.position + GetDirectionFromPivotDirection(
                                PivotDirection,
                                ManualPivotDirection,
                                relativeTo) * pivotDistance;
                            break;

                        case ConnnectorPivotMode.Manual:
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

        public ConnectorPivotDirection PivotDirection
        {
            get
            {
                return pivotDirection;
            }

            set
            {
                pivotDirection = value;
            }
        }

        public Vector3 ManualPivotDirection
        {
            get
            {
                return manualPivotDirection;
            }

            set
            {
                manualPivotDirection = value;
            }
        }

        public Vector3 ManualPivotLocalPosition
        {
            get
            {
                return manualPivotLocalPosition;
            }

            set
            {
                manualPivotLocalPosition = value;
            }
        }

        public ConnectorFollowType FollowingType
        {
            get
            {
                return connectorFollowType;
            }

            set
            {
                connectorFollowType = value;
            }
        }

        public ConnectorOrientType PivotDirectionOrient
        {
            get
            {
                return pivotDirectionOrient;
            }

            set
            {
                pivotDirectionOrient = value;
            }
        }

        public ConnnectorPivotMode PivotingMode
        {
            get
            {
                return pivotMode;
            }

            set
            {
                pivotMode = value;
            }
        }

        private void OnDrawGizmos ()
        {
            if (Application.isPlaying)
                return;
            
            UpdatePosition();
        }

        public static Vector3 GetDirectionFromPivotDirection (ConnectorPivotDirection pivotDirection, Vector3 manualPivotDirection, Transform relativeTo)
        {
            Vector3 dir = Vector3.zero;
            switch (pivotDirection)
            {
                case ConnectorPivotDirection.North:
                    dir = Vector3.up;
                    break;

                case ConnectorPivotDirection.NorthEast:
                    dir = Vector3.Lerp(Vector3.up, Vector3.right, 0.5f).normalized;
                    break;

                case ConnectorPivotDirection.East:
                    dir = Vector3.right;
                    break;

                case ConnectorPivotDirection.SouthEast:
                    dir = Vector3.Lerp(Vector3.down, Vector3.right, 0.5f).normalized;
                    break;

                case ConnectorPivotDirection.South:
                    dir = Vector3.down;
                    break;

                case ConnectorPivotDirection.SouthWest:
                    dir = Vector3.Lerp(Vector3.down, Vector3.left, 0.5f).normalized;
                    break;

                case ConnectorPivotDirection.West:
                    dir = Vector3.left;
                    break;

                case ConnectorPivotDirection.NorthWest:
                    dir = Vector3.Lerp(Vector3.up, Vector3.left, 0.5f).normalized;
                    break;

                case ConnectorPivotDirection.InFront:
                    dir = Vector3.forward;
                    break;

                case ConnectorPivotDirection.Manual:
                    dir = manualPivotDirection.normalized;
                    break;
            }

            return relativeTo.TransformDirection(dir);
        }
    }
}