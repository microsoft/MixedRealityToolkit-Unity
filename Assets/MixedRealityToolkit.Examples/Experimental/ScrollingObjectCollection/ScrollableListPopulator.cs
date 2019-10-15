// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    /// <summary>
    /// Simple demonstration of how to instantiate a <see cref="Microsoft.MixedReality.Toolkit.Experimental.UI.ScrollingObjectCollection"/> as well as use lazy loading to mitigate the perf cost of a large list of items.
    /// </summary>
    public class ScrollableListPopulator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The ScrollingObjectCollection to populate, if left empty. the populator will create on your behalf.")]
        private ScrollingObjectCollection scrollCollection;

        /// <summary>
        /// The ScrollingObjectCollection to populate, if left empty. the populator will create on your behalf.
        /// </summary>
        public ScrollingObjectCollection ScrollCollection
        {
            get { return scrollCollection; }
            set { scrollCollection = value; }
        }

        [SerializeField]
        [Tooltip("Object to duplicate in ScrollCollection")]
        private GameObject dynamicItem;

        /// <summary>
        /// Object to duplicate in <see cref="ScrollCollection"/>. 
        /// </summary>
        public GameObject DynamicItem
        {
            get { return dynamicItem; }
            set { dynamicItem = value; }
        }

        [SerializeField]
        [Tooltip("Number of items to generate")]
        private int numItems;

        /// <summary>
        /// Number of items to generate
        /// </summary>
        public int NumItems
        {
            get { return numItems; }
            set { numItems = value; }
        }

        [SerializeField]
        [Tooltip("Demonstrate lazy loading")]
        private bool lazyLoad;

        /// <summary>
        /// Demonstrate lazy loading 
        /// </summary>
        public bool LazyLoad
        {
            get { return lazyLoad; }
            set { lazyLoad = value; }
        }

        [SerializeField]
        [Tooltip("Number of items to load each frame during lazy load")]
        private int itemsPerFrame = 3;

        /// <summary>
        /// Number of items to load each frame during lazy load 
        /// </summary>
        public int ItemsPerFrame
        {
            get { return itemsPerFrame; }
            set { itemsPerFrame = value; }
        }

        [SerializeField]
        [Tooltip("Indeterminate loader to hide / show for LazyLoad")]
        private GameObject loader;

        /// <summary>
        /// Indeterminate loader to hide / show for <see cref="LazyLoad"/> 
        /// </summary>
        public GameObject Loader
        {
            get { return loader; }
            set { loader = value; }
        }

        private void OnEnable()
        {
            //make sure we find a collection
            if (scrollCollection == null)
            {
                scrollCollection = GetComponentInChildren<ScrollingObjectCollection>();
            }
        }

        public void MakeScrollingList()
        {
            if (scrollCollection == null)
            {
                GameObject newScroll = new GameObject("Scrolling Object Collection");
                newScroll.transform.parent = transform;
                newScroll.transform.localPosition = new Vector3(-0.0178f, 0.0165f, -0.0047f);
                newScroll.transform.localRotation = Quaternion.identity;
                newScroll.SetActive(false);
                scrollCollection = newScroll.AddComponent<ScrollingObjectCollection>();

                //prevent the scrolling collection from running until we're done dynamically populating it.
                scrollCollection.SetUpAtRuntime = false;
                scrollCollection.CellHeight = 0.032f;
                scrollCollection.CellWidth = 0.032f;
                scrollCollection.Tiers = 3;
                scrollCollection.ViewableArea = 5;
                scrollCollection.DragTimeThreshold = 0.75f;
                scrollCollection.HandDeltaMagThreshold = 0.8f;
                scrollCollection.TypeOfVelocity = ScrollingObjectCollection.VelocityType.FalloffPerItem;
            }

            if (!lazyLoad)
            {
                for (int i = 0; i < numItems; i++)
                {
                    MakeItem(dynamicItem, scrollCollection.transform);
                }
                scrollCollection.gameObject.SetActive(true);
                scrollCollection.UpdateCollection();
            }
            else
            {
                if(loader != null)
                {
                    loader.SetActive(true);
                }

                StartCoroutine(UpdateListOverTime(loader, itemsPerFrame));
            }
        }

        private IEnumerator UpdateListOverTime(GameObject loaderViz, int instancesPerFrame)
        {
            int currItemCount = 0;

            while (currItemCount < numItems)
            {
                for (int i = 0; i < instancesPerFrame; i++)
                {
                    MakeItem(dynamicItem, scrollCollection.transform);

                    currItemCount++;
                }

                yield return null;
            }

            //Now that the list is populated, hide the loader and show the list
            loaderViz.SetActive(false);
            scrollCollection.gameObject.SetActive(true);

            //Finally, manually call UpdateCollection to set up the collection
            scrollCollection.UpdateCollection();
        }

        private void MakeItem(GameObject item, Transform newItemParent)
        {
            GameObject g = Instantiate(item);

            g.transform.parent = newItemParent.transform;

            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
        }

    }

}