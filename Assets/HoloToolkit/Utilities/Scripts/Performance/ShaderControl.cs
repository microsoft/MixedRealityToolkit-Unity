using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class ShaderControl : MonoBehaviour
    {
        [SerializeField]
        private Material[] materialList = new Material[1];

        private int currentShaderLevel = 0;
        private Renderer objectRenderer;

        private List<Material> createdMaterials = new List<Material>();

        private void Awake()
        {
            objectRenderer = GetComponent<Renderer>();
            if (materialList.Length > 0 && materialList[0] == null)
            {
                materialList[0] = objectRenderer.material;
            }

            AdaptivePerformance.Instance.OnPerformanceBucketChanged.AddListener(PerformanceBucketChanged);
            ApplyBucket(AdaptivePerformance.Instance.GetCurrentBucket());
        }

        private void PerformanceBucketChanged(PerformanceBucket perfBucket)
        {
            ApplyBucket(perfBucket);
        }

        private void ApplyBucket(PerformanceBucket bucket)
        {
            var newShaderLevel = bucket.ShaderLevel;
            while (newShaderLevel > 0
                && (newShaderLevel > materialList.Length
                    || materialList[newShaderLevel] == null))
            {
                --newShaderLevel;
            }
            if (newShaderLevel != currentShaderLevel &&
                newShaderLevel >= 0 &&
                newShaderLevel < materialList.Length &&
                materialList[newShaderLevel] != null &&
                objectRenderer != null)
            {
                // to create an instance do Instantiate()
                // objectRenderer.material = Instantiate(materialList[newShaderLevel]);
                // createdMaterials.Add(objectRenderer.material);
                objectRenderer.material = materialList[newShaderLevel];
                currentShaderLevel = newShaderLevel;
                Debug.LogFormat("[ShaderControl.PerformanceBucketChanged ({0})]", newShaderLevel);
            }
        }

        private void OnDestroy()
        {
            AdaptivePerformance.Instance.OnPerformanceBucketChanged.RemoveListener(PerformanceBucketChanged);

            foreach (var mat in createdMaterials)
            {
                Destroy(mat);
            }
        }
    }
}

