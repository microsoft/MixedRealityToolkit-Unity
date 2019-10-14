using Microsoft.MixedReality.Toolkit.UI.Experimental.BoundsControlTypes;
using System;

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.Experimental
{
    [Serializable]
    /// <summary> 
    /// Used to display the bounding box attached to the rig root of a <see cref="BoundsControl"/>
    /// </summary>
    public class BoundsControlBoxDisplay
    {
        [Header("Box Display")]

        [SerializeField]
        [Tooltip("Material used to display the bounding box. If set to null no bounding box will be displayed")]
        private Material boxMaterial = null;

        /// <summary>
        /// Material used to display the bounding box. If set to null no bounding box will be displayed
        /// </summary>
        public Material BoxMaterial
        {
            get { return boxMaterial; }
            set
            {
                if (boxMaterial != value)
                {
                    boxMaterial = value;
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material used to display the bounding box when grabbed. If set to null no change will occur when grabbed.")]
        private Material boxGrabbedMaterial = null;

        /// <summary>
        /// Material used to display the bounding box when grabbed. If set to null no change will occur when grabbed.
        /// </summary>
        public Material BoxGrabbedMaterial
        {
            get { return boxGrabbedMaterial; }
            set
            {
                if (boxGrabbedMaterial != value)
                {
                    boxGrabbedMaterial = value;
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("When an axis is flattened what value to set that axis's scale to for display.")]
        private float flattenAxisDisplayScale = 0.0f;

        /// <summary>
        /// When an axis is flattened what value to set that axis's scale to for display.
        /// </summary>
        public float FlattenAxisDisplayScale
        {
            get { return flattenAxisDisplayScale; }
            set
            {
                if (flattenAxisDisplayScale != value)
                {
                    flattenAxisDisplayScale = value;
                    configurationChanged.Invoke();
                }
            }
        }

        internal protected UnityEvent configurationChanged = new UnityEvent();

        // Game object used to display the bounding box. Parented to the rig root
        private GameObject boxDisplay;

        internal void AddBoxDisplay(Transform parent, Vector3 currentBoundsExtents, FlattenModeType flattenAxis)
        {
            if (boxMaterial != null)
            {
                bool isFlattened = flattenAxis != FlattenModeType.DoNotFlatten;

                boxDisplay = GameObject.CreatePrimitive(isFlattened ? PrimitiveType.Quad : PrimitiveType.Cube);
                GameObject.Destroy(boxDisplay.GetComponent<Collider>());
                boxDisplay.name = "bounding box";

                BoundsControlVisualUtils.ApplyMaterialToAllRenderers(boxDisplay, boxMaterial);

                boxDisplay.transform.localScale = GetBoxDisplayScale(currentBoundsExtents, flattenAxis);
                boxDisplay.transform.parent = parent;
            }
        }

        private Vector3 GetBoxDisplayScale(Vector3 currentBoundsExtents, FlattenModeType flattenAxis)
        {
            // When a box is flattened one axis is normally scaled to zero, this doesn't always work well with visuals so we take 
            // that flattened axis and re-scale it to the flattenAxisDisplayScale.
            Vector3 displayScale = currentBoundsExtents;
            displayScale.x = (flattenAxis == FlattenModeType.FlattenX) ? flattenAxisDisplayScale : displayScale.x;
            displayScale.y = (flattenAxis == FlattenModeType.FlattenY) ? flattenAxisDisplayScale : displayScale.y;
            displayScale.z = (flattenAxis == FlattenModeType.FlattenZ) ? flattenAxisDisplayScale : displayScale.z;

            return 2.0f * displayScale;
        }

        internal void UpdateVisibilityInInspector(HideFlags desiredFlags)
        {
            if (boxDisplay != null)
            {
                boxDisplay.hideFlags = desiredFlags;
            }
        }

        internal void SetHighlighted()
        {
            //update the box material to the grabbed material
            if (boxDisplay != null)
            {
                BoundsControlVisualUtils.ApplyMaterialToAllRenderers(boxDisplay, boxGrabbedMaterial);
            }
        }

        internal void ResetVisibility(bool activate)
        {
            //set box display visibility
            if (boxDisplay != null)
            {
                boxDisplay.SetActive(activate);
                BoundsControlVisualUtils.ApplyMaterialToAllRenderers(boxDisplay, boxMaterial);
            }
        }

        internal void Update(Transform parent, Vector3 boundsExtents, FlattenModeType flattenAxis)
        {
            if (boxDisplay != null)
            {
                Vector3 rootScale = parent.lossyScale;
                Vector3 invRootScale = new Vector3(1.0f / rootScale.x, 1.0f / rootScale.y, 1.0f / rootScale.z);
                // Compute the local scale that produces the desired world space size
                boxDisplay.transform.localScale = Vector3.Scale(GetBoxDisplayScale(boundsExtents, flattenAxis), invRootScale);
            }
        }
    }
}
