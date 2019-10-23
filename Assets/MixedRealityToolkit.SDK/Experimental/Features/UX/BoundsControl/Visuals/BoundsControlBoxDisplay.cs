using Microsoft.MixedReality.Toolkit.UI.Experimental.BoundsControlTypes;
using System;

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI.Experimental
{
    /// <summary> 
    /// BoxDisplay can be used to attach a solid box visualization to a <see cref="BoundsControl"/>
    /// The box will only be rendered if a material is assigned
    /// </summary>
    [CreateAssetMenu(fileName = "BoundsControlBoxDisplay", menuName = "Mixed Reality Toolkit/Bounds Control/Box Display")]
    public class BoundsControlBoxDisplay : ScriptableObject
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

                boxDisplay = GameObject.CreatePrimitive(isFlattened ? PrimitiveType.Quad : PrimitiveType.Cube);// todo this is not correct - if we clamp the flatten axis to a configurable value then this must always be a cube
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
            Vector3 displayScale = BoundsControlVisualUtils.FlattenBounds(currentBoundsExtents, flattenAxis, flattenAxisDisplayScale);
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

        internal void UpdateDisplay(Vector3 boundsExtents, FlattenModeType flattenAxis)
        {
            if (boxDisplay != null)
            {
                Transform parent = boxDisplay.transform.parent;
                boxDisplay.transform.parent = null;
                boxDisplay.transform.localScale = GetBoxDisplayScale(boundsExtents, flattenAxis);
                boxDisplay.transform.parent = parent;
            }
        }
    }
}
