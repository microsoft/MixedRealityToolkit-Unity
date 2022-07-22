// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Component for setting the min/max move values for ObjectManipulator
    /// or BoundsControl
    /// </summary>
    [AddComponentMenu("MRTK/Spatial Manipulation/Min Max Move Constraint")]
    public class MinMaxMoveConstraint : TransformConstraint
    {
        #region Properties

        [SerializeField]
        [Tooltip("Minimum position allowed")]
        private Vector3 minimumPosition = Vector3.one * 0.2f;

        /// <summary>
        /// Minimum position allowed
        /// </summary>
        public Vector3 MinimumPosition
        {
            get => minimumPosition;
            set => minimumPosition = value;
        }

        [SerializeField]
        [Tooltip("Maximum position allowed")]
        private Vector3 maximumPosition = Vector3.one * 2f;

        /// <summary>
        /// Maximum position allowed
        /// </summary>
        public Vector3 MaximumPosition
        {
            get => maximumPosition;
            set => maximumPosition = value;
        }
        [SerializeField]
        [Tooltip("Use local space do constraint move.")]
        private bool useLocalSpaceForConstraint = true;

        /// <summary>
        /// Gets or sets whether the constraints should be applied in local space of the object being manipulated or world space.
        /// </summary>
        public bool UseLocalSpaceForConstraint
        {
            get => useLocalSpaceForConstraint;
            set => useLocalSpaceForConstraint = value;
        }

        public override TransformFlags ConstraintType => TransformFlags.Move;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Clamps the transform move to the move limits set by min and max values:
        /// </summary>
        public override void ApplyConstraint(ref MixedRealityTransform transform)
        {
            Vector3 position = transform.Position;

            Transform parent = this.transform.parent;
            if (parent == null)
                parent = this.transform;

            if (useLocalSpaceForConstraint) //Use local space
                position = parent.InverseTransformPoint(transform.Position);

            position.x = Mathf.Clamp(position.x, minimumPosition.x, maximumPosition.x);
            position.y = Mathf.Clamp(position.y, minimumPosition.y, maximumPosition.y);
            position.z = Mathf.Clamp(position.z, minimumPosition.z, maximumPosition.z);

            if (useLocalSpaceForConstraint) //Use local space
                transform.Position = parent.TransformPoint(position);
            else
                transform.Position = position;
        }

        /// <summary>
        /// Draw limits on select object with MinMaxMoveConstraint
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;
            Vector3 offset = Vector3.zero;

            if (useLocalSpaceForConstraint) //Use local space
            {
                Transform parent = this.transform.parent;
                if (parent != null)
                {
                    offset = parent.position;
                    right = parent.right;
                    up = parent.up;
                    forward = parent.forward;
                }
            }

            //Draw X axis
            Gizmos.color = Color.red / 1.5f;
            Gizmos.DrawLine(offset, offset + right * minimumPosition.x);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(offset, offset + right * maximumPosition.x);

            //Draw Y axis
            Gizmos.color = Color.green / 1.5f;
            Gizmos.DrawLine(offset, offset + up * minimumPosition.y);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(offset, offset + up * maximumPosition.y);

            //Draw Z axis
            Gizmos.color = Color.blue / 1.5f;
            Gizmos.DrawLine(offset, offset + forward * minimumPosition.z);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(offset, offset + forward * maximumPosition.z);
        }

        #endregion Public Methods
    }
}
