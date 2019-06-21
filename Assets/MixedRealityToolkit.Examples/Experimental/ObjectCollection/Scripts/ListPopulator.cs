using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Experimental
{
    public class ListPopulator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The ScrollingObjectCollection to generate")]
        private ScrollingObjectCollection scrollCollection;

        /// <summary>
        /// The ScrollingObjectCollection to generate
        /// </summary>
        public ScrollingObjectCollection ScrollCollection
        {
            get => scrollCollection;
            set => scrollCollection = value;
        }


        [SerializeField]
        [Tooltip("Object to duplicate in list")]
        private GameObject dynamicItem;

        /// <summary>
        /// Object to duplicate in list 
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
        /// The material to apply to the primative for the list
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
