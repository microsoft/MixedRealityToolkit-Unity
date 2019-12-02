// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// RectTransforms do not scale 3d objects (such as unit cubes) to fit within their bounds.
    /// This helper class will apply a scale to fit a unit cube into the bounds specified by the RectTransform.
    /// The Z component is scaled to the min of the X and Y components.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Scripts/MRTK/SDK/RectTransformCubeScaler")]
    public class RectTransformCubeScaler : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Vector2 prevRectSize = Vector2.zero;

        [SerializeField]
        [Tooltip("Object which will be scaled to fit in the bounds of RectTransform.  It will typically be the direct child of this game object.  It is assumed to have contents which fit into a unit cube prior to scaling applied by this component.  The object passed in here does not need to have a RectTransform; a standard Transform is sufficient.")]
        private Transform objectToScale = null;
        private Transform prevObjectToScale = null;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            var size = rectTransform.rect.size;

            if (prevRectSize != size
                || prevObjectToScale != objectToScale)
            {
                prevRectSize = size;
                prevObjectToScale = objectToScale;

                if (objectToScale != null)
                {
                    objectToScale.localScale = new Vector3(size.x, size.y, Mathf.Min(size.x, size.y));
                }
            }
        }
    }
}
