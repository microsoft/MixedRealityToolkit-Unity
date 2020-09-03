// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This component represents and ordered collection of UI elements. You
    /// can add to the UICollection by either dropping UI elements in the UI
    /// element this component is attached to in the Editor or by calling the
    /// AddItem(RectTransform) method. UI elements that are added to this
    /// collection via the Editor will be automatically arranged when this
    /// component executes.
    /// To use this component attach it to a UI element (a GameObject with a
    /// RectTransform component) such as an Image or Panel.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Experimental/UICollection")]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public class UICollection : MonoBehaviour
    {
        /// <summary>
        /// The maximum width that the collection should expand to. If the value is -1.0 then it will use
        /// the width specified by the RectTransform of this component's GameObject.
        /// </summary>
        [Experimental]
        [SerializeField] private float maxWidth = -1.0f;

        public float MaxWidth
        {
            get => maxWidth;
            set => maxWidth = value;
        }

        /// <summary>
        /// The maximum height that the collection should expand to. If the value is -1.0 then it will use
        /// the height specified by the RectTransform of this component's GameObject.
        /// </summary>
        [SerializeField] private float maxHeight = -1.0f;

        public float MaxHeight
        {
            get => maxHeight;
            set => maxHeight = value;
        }
        /// <summary>
        /// The amount of horizontal spacing (in pixels) to use between items in this collection.
        /// </summary>
        [SerializeField] private float horizontalSpacing = 0.0f;

        public float HorizontalSpacing
        {
            get => horizontalSpacing;
            set => horizontalSpacing = value;
        }
        /// <summary>
        /// The amount of vertical spacing (in pixels) to use between items in this collection.
        /// </summary>
        [SerializeField] private float verticalSpacing = 0.0f;

        public float VerticalSpacing
        {
            get => verticalSpacing;
            set => verticalSpacing = value;
        }
        /// <summary>
        /// A list of items in this collection. This list should not be modified directly. Instead
        /// use AddItem(RectTransform) and RemoveItem(RectTransform).
        /// </summary>
        public List<RectTransform> Items { get; private set; }

        /// <summary>
        /// Cached rect transform to use for collection
        /// </summary>
        private RectTransform rectTransform;

        private void Awake()
        {
            Items = new List<RectTransform>();
        }

        // Use this for initialization
        private void Start()
        {
            // Verify this is attached to a GameObject with a rect transform
            rectTransform = GetComponent<RectTransform>();

            if (!Application.isEditor) { return; }

            // Collect children items already added (likely added in the Editor)
            CollectItems();
            UpdateLayout();
        }

        private void Update()
        {
            if (!Application.isEditor) { return; }
            CollectItems();
            UpdateLayout();
        }

        /// <summary>
        /// Adds a UI element to the collection. This will cause the collection
        /// layout to update immediately.
        /// NOTE: The added item's RectTransform will get modified in order to layout properly in this collection.
        /// </summary>
        /// <param name="item">The UI element to add to the collection.</param>
        public void AddItem(RectTransform item)
        {
            Items.Add(item);

            item.SetParent(transform);
            item.transform.localScale = Vector3.one;
            item.position = Vector3.zero;
            item.anchoredPosition3D = Vector3.zero;

            UpdateLayout();
        }

        /// <summary>
        /// Removes a UI element from the collection. This will cause the collection
        /// layout to update immediately.
        /// NOTE: This method does not call Destroy removed items.
        /// </summary>
        /// <param name="item">The UI element to remove from the collection.</param>
        public void RemoveItem(RectTransform item)
        {
            Items.Remove(item);

            UpdateLayout();
        }

        /// <summary>
        /// Removes all UI elements added to the collection. This will cause the collection
        /// layout to update immediately.
        /// NOTE: This method does not call Destroy removed items.
        /// </summary>
        public void RemoveAllItems()
        {
            Items.Clear();

            UpdateLayout();
        }

        private void CollectItems()
        {
            Items.Clear();

            foreach (Transform childTransform in transform)
            {
                RectTransform childRect = childTransform.GetComponent<RectTransform>();
                if (childRect != null)
                {
                    AddItem(childRect);
                }
            }
        }

        protected virtual void UpdateLayout()
        {
            Rect rect = rectTransform.rect;

            Vector2 updatedSize = Vector2.zero;
            if (maxWidth < 0.0f)
            {
                // Set to the width of the panel
                updatedSize.x = rect.width;
            }
            else
            {
                // Set to the max width
                updatedSize.x = maxWidth;
            }

            if (maxHeight < 0.0f)
            {
                // Set to the height of the panel
                updatedSize.y = rect.height;
            }
            else
            {
                // Set to the max height
                updatedSize.y = maxHeight;
            }

            Vector2 currentOffset = Vector2.zero;
            Vector2 anchorVec = Vector2.up;

            float columnHeight = 0.0f;
            float maxPanelWidth = 0.0f;

            for (int i = 0; i < Items.Count; i++)
            {

                // Ensure the anchors and pivot are set properly for positioning in the UICollection
                Items[i].anchorMin = anchorVec;
                Items[i].anchorMax = anchorVec;
                Items[i].pivot = anchorVec;

                columnHeight = Mathf.Max(Items[i].rect.height, columnHeight);

                if (Items[i].rect.width + currentOffset.x > updatedSize.x)
                {
                    // Move to next column
                    currentOffset.y += columnHeight + verticalSpacing;
                    currentOffset.x = 0.0f;
                    columnHeight = Items[i].rect.height;

                    // Check to see if it can fit in the next column
                    if (Items[i].rect.height + currentOffset.y > updatedSize.y)
                    {
                        // Bail out... can't fit any more items!!!
                        break;
                    }
                }

                // Position item
                Items[i].anchoredPosition = new Vector2(currentOffset.x, -currentOffset.y);

                // Update current offset
                currentOffset.x += Items[i].rect.width + horizontalSpacing;

                maxPanelWidth = Mathf.Max(currentOffset.x - horizontalSpacing, maxPanelWidth);
            }

            // Update the panel size
            float finalWidth = maxWidth < 0.0f ? rect.width : maxPanelWidth;
            float finalHeight = maxHeight < 0.0f ? rect.height : columnHeight + currentOffset.y;
            rectTransform.sizeDelta = new Vector2(finalWidth, finalHeight);
        }
    }
}