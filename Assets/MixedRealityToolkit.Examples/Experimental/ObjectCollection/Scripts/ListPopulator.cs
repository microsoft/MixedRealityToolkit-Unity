using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
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
            get
            {
                return scrollCollection;
            }
            set
            {
                scrollCollection = value;
            }
        }

        [SerializeField]
        [Tooltip("The material to apply to the primative for the list")]
        private Material itemMaterial;

        /// <summary>
        /// The material to apply to the primative for the list
        /// </summary>
        public Material ItemMaterial
        {
            get
            {
                return itemMaterial;
            }
            set
            {
                itemMaterial = value;
            }
        }

        [SerializeField]
        [Tooltip("Number of items to generate")]
        private int numItems;

        /// <summary>
        /// The material to apply to the primative for the list
        /// </summary>
        public int NumItems
        {
            get
            {
                return numItems;
            }
            set
            {
                numItems = value;
            }
        }

        [SerializeField]
        [Tooltip("Default scale of each item in the list")]
        private Vector3 itemScale = new Vector3(0.04f, 0.04f, 0.04f);

        /// <summary>
        /// Default scale of each item in the list
        /// </summary>
        public Vector3 ItemScale
        {
            get
            {
                return itemScale;
            }
            set
            {
                itemScale = value;
            }
        }

        private void OnEnable()
        {
            //make sure we find a collection
            if(scrollCollection == null)
            {
                scrollCollection = GetComponentInChildren<ScrollingObjectCollection>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (scrollCollection == null) { return; }

            for (int i = 0; i < numItems; i++)
            {
                GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);

                if (itemMaterial != null)
                {
                    item.GetComponent<Renderer>().sharedMaterial = itemMaterial;
                }

                item.AddComponent<NearInteractionTouchable>();

                item.transform.parent = scrollCollection.transform;

                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
                item.transform.localScale = itemScale;
            }

            scrollCollection.UpdateCollection();

        }

    }
}
