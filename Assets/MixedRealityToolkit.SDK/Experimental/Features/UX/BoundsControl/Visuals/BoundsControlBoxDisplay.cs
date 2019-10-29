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
    public class BoundsControlBoxDisplay
    {
        // Game object used to display the box. Parented to the rig root
        private GameObject boxDisplay;

        private BoundsControlBoxDisplayConfiguration config;

        internal BoundsControlBoxDisplay(BoundsControlBoxDisplayConfiguration configuration)
        {
            Debug.Assert(configuration != null, "Can't create BoundsControlBoxDisplay without valid configuration");
            config = configuration;
        }

        internal void AddBoxDisplay(Transform parent, Vector3 currentBoundsExtents, FlattenModeType flattenAxis)
        {
            if (config.BoxMaterial != null)
            {
                // this has to be cube even in flattened mode as flattened box display can still have a thickness of flattenAxisDisplayScale
                boxDisplay = GameObject.CreatePrimitive(PrimitiveType.Cube); 
                GameObject.Destroy(boxDisplay.GetComponent<Collider>());
                boxDisplay.name = "bounding box";

                BoundsControlVisualUtils.ApplyMaterialToAllRenderers(boxDisplay, config.BoxMaterial);
                boxDisplay.transform.localScale = GetBoxDisplayScale(currentBoundsExtents, flattenAxis);
                boxDisplay.transform.parent = parent;
            }
        }

        private Vector3 GetBoxDisplayScale(Vector3 currentBoundsExtents, FlattenModeType flattenAxis)
        {
            // When a box is flattened one axis is normally scaled to zero, this doesn't always work well with visuals so we take 
            // that flattened axis and re-scale it to the flattenAxisDisplayScale.
            Vector3 displayScale = BoundsControlVisualUtils.FlattenBounds(currentBoundsExtents, flattenAxis, config.FlattenAxisDisplayScale);
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
                BoundsControlVisualUtils.ApplyMaterialToAllRenderers(boxDisplay, config.BoxGrabbedMaterial);
            }
        }

        internal void ResetVisibility(bool activate)
        {
            //set box display visibility
            if (boxDisplay != null)
            {
                boxDisplay.SetActive(activate);
                BoundsControlVisualUtils.ApplyMaterialToAllRenderers(boxDisplay, config.BoxMaterial);
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
