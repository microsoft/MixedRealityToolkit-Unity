// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Boundary;
using UnityEngine;

namespace MixedRealityToolkit.Examples.Boundary
{
    public class BoundaryTest : MonoBehaviour
    {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        private Material[] defaultMaterials = null;

        private void Start()
        {
            BoundaryManager.Instance.RenderBoundary = true;
            BoundaryManager.Instance.RenderFloor = true;

            defaultMaterials = GetComponent<Renderer>().materials;
        
            if (BoundaryManager.Instance.ContainsObject(gameObject.transform.position))
            {
                Debug.LogFormat("Object {0} is within established boundary. Position: {1}", name, gameObject.transform.position);

                for (int i = 0; i < defaultMaterials.Length; i++)
                {
                    // Color the cube green if object is within specified boundary.
                    defaultMaterials[i].SetColor("_Color", Color.green);
                }
            }
            else
            {
                Debug.LogFormat("Object {0} is outside established boundary. Position: {1}", name, gameObject.transform.position);

                for (int i = 0; i < defaultMaterials.Length; i++)
                {
                    // Color the cube red if object is outside specified boundary.
                    defaultMaterials[i].SetColor("_Color", Color.red);
                }
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                Destroy(defaultMaterials[i]);
            }
        }
#endif
    }
}
