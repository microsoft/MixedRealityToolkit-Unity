// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity;

namespace HoloToolkit.UX.ToolTips
{
    [RequireComponent(typeof(MeshFilter))]

    /// <summary>
    /// Connects a ToolTip to a target
    /// Maintains that connection even if the target moves
    /// </summary>
    public class ToolTipConnector : MonoBehaviour
    {
        [SerializeField]
        private GameObject target;

        /// <summary>
        /// The GameObject to which the tooltip is connected
        /// </summary>
        public GameObject Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }

        [SerializeField]
        private ToolTip toolTip;

        [SerializeField]
        private ConnectorFollowType connectorFollowType = ConnectorFollowType.AnchorOnly;
        /// <summary>
        /// getter /setter for the follow style of the tooltip connector
        /// </summary>
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

        [SerializeField]
        private ConnnectorPivotMode pivotMode = ConnnectorPivotMode.Manual;
        /// <summary>
        /// is the connector pivot set manually or automatically
        /// </summary>
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

        [SerializeField]
        private ConnectorPivotDirection pivotDirection = ConnectorPivotDirection.North;
        /// <summary>
        /// getter/setter for the direction of the connector
        /// </summary>
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

        [SerializeField]
        private ConnectorOrientType pivotDirectionOrient = ConnectorOrientType.OrientToObject;
        /// <summary>
        /// orientation style for connector
        /// </summary>
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

        [SerializeField]
        private Vector3 manualPivotDirection = Vector3.up;
        /// <summary>
        /// getter/setter for direction of pivot manually set
        /// </summary>
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

        [SerializeField]
        private Vector3 manualPivotLocalPosition = Vector3.up;
        /// <summary>
        /// getter/setter for local pivot position
        /// </summary>
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

        [SerializeField]
        [Range(0f, 2f)]
        private float pivotDistance = 0.25f;
        /// <summary>
        /// Set Distance from object that Tooltip pivots around.
        /// </summary>
        public float PivotDistance
        {
            get
            {
                return pivotDistance;
            }
            set
            {
                pivotDistance = Mathf.Min( 2.0f, Mathf.Max( 0,value));
            }
        }

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

        private void UpdatePosition()
        {
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

                case ConnectorFollowType.Position:
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
                                relativeTo) * PivotDistance;
                            break;

                        case ConnnectorPivotMode.Manual:
                            // Do nothing
                            break;
                    }
                    break;

                case ConnectorFollowType.YRotation:
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
                            Vector3 localPosition = GetDirectionFromPivotDirection(PivotDirection, ManualPivotDirection, relativeTo) * PivotDistance;
                            toolTip.PivotPosition = Target.transform.position + localPosition;
                            break;

                        case ConnnectorPivotMode.Manual:
                            // Do nothing
                            break;
                    }
                    break;

                case ConnectorFollowType.XRotation:
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
                                relativeTo) * PivotDistance;
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