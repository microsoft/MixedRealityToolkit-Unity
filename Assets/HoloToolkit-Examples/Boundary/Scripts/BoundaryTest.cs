// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.Boundary.Tests
{
    public class BoundaryTest : MonoBehaviour
    {
        private Material[] defaultMaterials = null;

        private void Start()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            BoundaryManager.Instance.RenderBoundary = true;
            BoundaryManager.Instance.RenderFloor = true;

            defaultMaterials = GetComponent<Renderer>().materials;
        
            if (BoundaryManager.Instance.ContainsObject(gameObject.transform.position))
            {
                Debug.LogFormat("Object is within established boundary. Position: {0}", gameObject.transform.position);

                for (int i = 0; i < defaultMaterials.Length; i++)
                {
                    // Color the cube green if object is within specified boundary.
                    Color highlightColor = Color.green;
                    defaultMaterials[i].SetColor("_Color", highlightColor);
                }
            }
#else
            BoundaryManager.Instance.RenderBoundary = false;
            BoundaryManager.Instance.RenderFloor = false;
#endif
        }

        private void OnDestroy()
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                Destroy(defaultMaterials[i]);
            }
        }
    }
}
