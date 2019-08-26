using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental
{
    public class ScrollableListPopulator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The ScrollingObjectCollection to populate")]
        private ScrollingObjectCollection scrollCollection;

        /// <summary>
        /// The ScrollingObjectCollection to populate
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

        private void OnEnable()
        {
            //make sure we find a collection
            if (scrollCollection == null)
            {
                scrollCollection = GetComponentInChildren<ScrollingObjectCollection>();
            }

            if (scrollCollection == null) { return; }

            for (int i = 0; i < numItems; i++)
            {
                GameObject item = Instantiate(dynamicItem);

                item.transform.parent = scrollCollection.transform;

                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
                item.SetActive(true);
            }
        }

        private void Start()
        {
            scrollCollection.UpdateCollection();
        }

    }

}