// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UX;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("MRTK/Examples/Toggle Collection Object Activate")]
    public class ToggleCollectionObjectActivate : MonoBehaviour
    {
        [Tooltip("The ToggleCollection to listen to.")]
        [SerializeField]
        private ToggleCollection toggleCollection;

        [Tooltip("Array of Objects to be controlled by the toggle collection")]
        [SerializeField]
        private GameObject[] targetObjects;

        private void Start()
        {
            Set(toggleCollection.CurrentIndex);
            toggleCollection.OnToggleSelected.AddListener((toggleSelectedIndex) => Set(toggleSelectedIndex));
        }

        private void Set(int index)
        {
            for (int i = 0; i < targetObjects.Length; i++)
            {
                targetObjects[i].SetActive(i == index);
            }
        }
    }
}
