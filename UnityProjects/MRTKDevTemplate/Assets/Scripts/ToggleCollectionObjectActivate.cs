// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UX;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{

    public class ToggleCollectionObjectActivate : MonoBehaviour
    {
        [Tooltip("The ToggleCollection")]
        [SerializeField]
        private ToggleCollection toggleCollection;

        [Tooltip("Array of Objects to be controlled by the toggle collection")]
        [SerializeField]
        private GameObject[] targetObjects;

        /// <summary>
        /// The ToggleCollection for this color changer.
        /// </summary>
        public ToggleCollection ToggleCollection
        {
            get => toggleCollection;
            set => toggleCollection = value;
        }

        private void Start()
        {
            for(int i = 0; i < targetObjects.Length; i++)
            {
                if(ToggleCollection.CurrentIndex != i)
                {
                    targetObjects[i].SetActive(false);
                }
            }
        }
    }
}
