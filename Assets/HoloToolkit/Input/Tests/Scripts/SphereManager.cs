// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests 
{
    public class SphereManager : MonoBehaviour 
    {
        [SerializeField]
        private SphereKeywords[] spheres;

        public void ResetAll() 
        {
            for (int i = 0; i < spheres.Length; i++) 
            {
                spheres[i].ResetColor();
            }
        }
    }
}
