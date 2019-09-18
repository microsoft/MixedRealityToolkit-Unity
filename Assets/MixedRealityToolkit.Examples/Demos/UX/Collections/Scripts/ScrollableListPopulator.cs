using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental
{
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
            get => scrollCollection;
            set => scrollCollection = value;
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
            get => numItems;
            set => numItems = value;
        }

        /// <summary>
        /// Demonstrate lazy loading 
        /// </summary>
        [SerializeField]
        [Tooltip("Demonstrate lazy loading")]
        private bool lazyLoad;

        public bool LazyLoad
        {
            get { return lazyLoad; }
            set { lazyLoad = value; }
        }


        /// <summary>
        /// Number of items to load each frame during lazy load 
        /// </summary>
        [SerializeField]
        [Tooltip("Number of items to load each frame during lazy load")]
        private int itemsPerFrame = 3;

        public int ItemsPerFrame
        {
            get { return itemsPerFrame; }
            set { itemsPerFrame = value; }
        }


        private IEnumerator loadOverTime;

        /// <summary>
        /// Indeterminate loader to hide / show for <see cref="LazyLoad"/> 
        /// </summary>
        [SerializeField]
        [Tooltip("Indeterminate loader to hide / show for LazyLoad")]
        private GameObject loader;

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
                newScroll.transform.localPosition = Vector3.zero;
                newScroll.transform.localRotation = Quaternion.identity;
                newScroll.SetActive(false);
                scrollCollection = newScroll.AddComponent<ScrollingObjectCollection>();

                //prevent the scrolling collection from running until we're done dynamically populating it.
                scrollCollection.SetUpAtRuntime = false;
                scrollCollection.CellHeight = 0.06f;
                scrollCollection.CellWidth = 0.06f;
                scrollCollection.Tiers = 3;
                scrollCollection.ViewableArea = 4;
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

                scrollCollection.UpdateCollection();
            }
            else
            {
                if(loader != null)
                {
                    loader.SetActive(true);
                }

                loadOverTime = UpdateListOverTime(loader, itemsPerFrame);
                StartCoroutine(loadOverTime);
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

            loaderViz.SetActive(false);

            scrollCollection.gameObject.SetActive(true);
            scrollCollection.UpdateCollection();
        }

        private void MakeItem(GameObject item, Transform newItemParent)
        {
            GameObject g = Instantiate(item);

            g.transform.parent = newItemParent.transform;

            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.SetActive(true);

        }

    }

}