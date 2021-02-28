// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    [ExecuteInEditMode]
    public class GridObjectCollectionSizeFitter : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The GridObjectColletion to fit")]
        private GridObjectCollection GridObjectCollection;

        [SerializeField]
        [Range(0.001f, .5f)]
        [Tooltip("Scale factor for the border. Stored in local Z scale.")]
        private float BorderScale = 0.02f;

        [SerializeField]
        [Range(0, .1f)]
        [Tooltip("Total margin added to left and right")]
        private float HorizontalMargin = 0.0f;

        [SerializeField]
        [Range(0, .1f)]
        [Tooltip("Total margin added to top and bottom")]
        private float VerticalMargin = 0.0f;

        [SerializeField]
        [Range(0.001f, .1f)]
        [Tooltip("Distance behind GridObjectCollection")]
        private float Distance = 0.001f;

        private void Start()
        {
            if (GridObjectCollection)
            {
                GridObjectCollection.OnCollectionUpdated = (BaseObjectCollection box) => UpdateLayout();
                UpdateLayout();
            }
        }

        private void OnValidate()
        {
            if (GridObjectCollection)
            {
                GridObjectCollection.OnCollectionUpdated = (BaseObjectCollection box) => UpdateLayout();
                UpdateLayout();
            }
        }

        private void UpdateLayout()
        {
            if (GridObjectCollection)
            {
                transform.localPosition = new Vector3(GridObjectCollection.Center.x, GridObjectCollection.Center.y, GridObjectCollection.Distance + Distance);
                transform.localScale = new Vector3(GridObjectCollection.Width + HorizontalMargin, GridObjectCollection.Height + VerticalMargin, BorderScale);
            }
        }
    }
}