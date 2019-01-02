// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Utilities;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// Connects a ToolTip to a target
    /// Maintains that connection even if the target moves
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class ToolTipConnector : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The GameObject to which the tooltip is connected")]
        private GameObject target;

        /// <summary>
        /// The GameObject to which the tooltip is connected
        /// </summary>
        public GameObject Target
        {
            get { return target; }
            set { target = value; }
        }

        [SerializeField]
        private ToolTip toolTip;

        private bool IsTooltipValid
        {
            get
            {
                toolTip = gameObject.EnsureComponent<ToolTip>();
                return toolTip != null;
            }
        }

        [SerializeField]
        [Tooltip("The follow style of the tooltip connector")]
        private ConnectorFollowType connectorFollowType = ConnectorFollowType.AnchorOnly;

        /// <summary>
        /// The follow style of the tooltip connector
        /// </summary>
        public ConnectorFollowType ConnectorFollowingType
        {
            get { return connectorFollowType; }
            set { connectorFollowType = value; }
        }

        [SerializeField]
        [Tooltip("Is the connector pivot set manually or automatically?")]
        private ConnectorPivotModeType pivotMode = ConnectorPivotModeType.Manual;

        /// <summary>
        /// Is the connector pivot set manually or automatically?
        /// </summary>
        public ConnectorPivotModeType PivotMode
        {
            get { return pivotMode; }
            set { pivotMode = value; }
        }

        [SerializeField]
        [Tooltip("The direction of the connector")]
        private ConnectorPivotDirectionType pivotDirection = ConnectorPivotDirectionType.North;

        /// <summary>
        /// The direction of the connector
        /// </summary>
        public ConnectorPivotDirectionType PivotDirection
        {
            get { return pivotDirection; }
            set { pivotDirection = value; }
        }

        [SerializeField]
        [Tooltip("orientation style for connector")]
        private ConnectorOrientType pivotDirectionOrient = ConnectorOrientType.OrientToObject;

        /// <summary>
        /// Orientation style for connector
        /// </summary>
        public ConnectorOrientType PivotDirectionOrient
        {
            get { return pivotDirectionOrient; }
            set { pivotDirectionOrient = value; }
        }

        [SerializeField]
        [Tooltip("The direction of the manual pivot.")]
        private Vector3 manualPivotDirection = Vector3.up;

        /// <summary>
        /// The direction of the manual pivot.
        /// </summary>
        public Vector3 ManualPivotDirection
        {
            get { return manualPivotDirection; }
            set { manualPivotDirection = value; }
        }

        [SerializeField]
        private Vector3 manualPivotLocalPosition = Vector3.up;

        /// <summary>
        /// getter/setter for local pivot position
        /// </summary>
        public Vector3 ManualPivotLocalPosition
        {
            get { return manualPivotLocalPosition; }
            set { manualPivotLocalPosition = value; }
        }

        [SerializeField]
        [Range(0f, 2f)]
        [Tooltip("Set Distance from object that Tooltip pivots around.")]
        private float pivotDistance = 0.25f;

        /// <summary>
        /// Set Distance from object that Tooltip pivots around.
        /// </summary>
        public float PivotDistance
        {
            get { return pivotDistance; }
            set { pivotDistance = Mathf.Min(2.0f, Mathf.Max(0, value)); }
        }

        private void OnEnable()
        {
            if (!IsTooltipValid)
            {
                return;
            }

            ManualPivotLocalPosition = transform.InverseTransformPoint(toolTip.PivotPosition);
        }

        private void UpdatePosition()
        {
            if (!IsTooltipValid)
            {
                return;
            }

            if (target == null)
            {
                return;
            }

            switch (connectorFollowType)
            {
                case ConnectorFollowType.AnchorOnly:
                default:
                    // Set the position of the anchor to the target's position
                    // And do nothing else
                    toolTip.Anchor.transform.position = target.transform.position;
                    break;

                case ConnectorFollowType.Position:
                    // Move the entire tooltip transform while maintaining the anchor position offset
                    toolTip.transform.position = target.transform.position;
                    switch (PivotMode)
                    {
                        case ConnectorPivotModeType.Automatic:
                            Transform relativeTo = null;
                            switch (PivotDirectionOrient)
                            {
                                case ConnectorOrientType.OrientToCamera:
                                    relativeTo = CameraCache.Main.transform;
                                    break;

                                case ConnectorOrientType.OrientToObject:
                                    relativeTo = target.transform;
                                    break;
                            }

                            toolTip.PivotPosition = target.transform.position + GetDirectionFromPivotDirection(
                                PivotDirection,
                                ManualPivotDirection,
                                relativeTo) * PivotDistance;
                            break;

                        case ConnectorPivotModeType.Manual:
                            // Do nothing
                            break;
                    }
                    break;

                case ConnectorFollowType.YRotation:
                    // Set the transform of the entire tool tip
                    // Set the pivot relative to target/camera
                    toolTip.transform.position = target.transform.position;
                    Vector3 eulerAngles = target.transform.eulerAngles;
                    eulerAngles.x = 0f;
                    eulerAngles.z = 0f;
                    toolTip.transform.eulerAngles = eulerAngles;

                    switch (PivotMode)
                    {
                        case ConnectorPivotModeType.Automatic:
                            Transform relativeTo = null;
                            switch (PivotDirectionOrient)
                            {
                                case ConnectorOrientType.OrientToCamera:
                                    relativeTo = CameraCache.Main.transform;
                                    break;

                                case ConnectorOrientType.OrientToObject:
                                    relativeTo = target.transform;
                                    break;
                            }
                            Vector3 localPosition = GetDirectionFromPivotDirection(PivotDirection, ManualPivotDirection, relativeTo) * PivotDistance;
                            toolTip.PivotPosition = target.transform.position + localPosition;
                            break;

                        case ConnectorPivotModeType.Manual:
                            // Do nothing
                            break;
                    }
                    break;

                case ConnectorFollowType.XRotation:
                    // Set the transform of the entire tool tip
                    // Set the pivot relative to target/camera
                    toolTip.transform.position = target.transform.position;
                    toolTip.transform.rotation = target.transform.rotation;
                    switch (PivotMode)
                    {
                        case ConnectorPivotModeType.Automatic:
                            Transform relativeTo = null;
                            switch (PivotDirectionOrient)
                            {
                                case ConnectorOrientType.OrientToCamera:
                                    relativeTo = CameraCache.Main.transform;
                                    break;

                                case ConnectorOrientType.OrientToObject:
                                    relativeTo = target.transform;
                                    break;
                            }
                            toolTip.PivotPosition = target.transform.position + GetDirectionFromPivotDirection(
                                PivotDirection,
                                ManualPivotDirection,
                                relativeTo) * PivotDistance;
                            break;

                        case ConnectorPivotModeType.Manual:
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

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            UpdatePosition();
        }

        /// <summary>
        /// Computes the director of the connector 
        /// </summary>
        /// <param name="pivotDirection">enum describing director of connector pivot</param>
        /// <param name="manualPivotDirection">is the pivot set manually</param>
        /// <param name="relativeTo">Transform that describes the frame of reference of the pivot</param>
        /// <returns>a vector describing the pivot direction in world space</returns>
        public static Vector3 GetDirectionFromPivotDirection(ConnectorPivotDirectionType pivotDirection, Vector3 manualPivotDirection, Transform relativeTo)
        {
            Vector3 dir = Vector3.zero;

            switch (pivotDirection)
            {
                case ConnectorPivotDirectionType.North:
                    dir = Vector3.up;
                    break;

                case ConnectorPivotDirectionType.Northeast:
                    dir = Vector3.Lerp(Vector3.up, Vector3.right, 0.5f).normalized;
                    break;

                case ConnectorPivotDirectionType.East:
                    dir = Vector3.right;
                    break;

                case ConnectorPivotDirectionType.Southeast:
                    dir = Vector3.Lerp(Vector3.down, Vector3.right, 0.5f).normalized;
                    break;

                case ConnectorPivotDirectionType.South:
                    dir = Vector3.down;
                    break;

                case ConnectorPivotDirectionType.Southwest:
                    dir = Vector3.Lerp(Vector3.down, Vector3.left, 0.5f).normalized;
                    break;

                case ConnectorPivotDirectionType.West:
                    dir = Vector3.left;
                    break;

                case ConnectorPivotDirectionType.Northwest:
                    dir = Vector3.Lerp(Vector3.up, Vector3.left, 0.5f).normalized;
                    break;

                case ConnectorPivotDirectionType.InFront:
                    dir = Vector3.forward;
                    break;

                case ConnectorPivotDirectionType.Manual:
                    dir = manualPivotDirection.normalized;
                    break;
            }

            return relativeTo.TransformDirection(dir);
        }
    }
}