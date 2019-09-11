using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [Serializable]
    public class BoundingBoxHandlesBase
    {
        [SerializeField]
        [Tooltip("Material applied to hansdles when they are not in a grabbed state")]
        private Material handleMaterial;

        /// <summary>
        /// Material applied to handles when they are not in a grabbed state
        /// </summary>
        public Material HandleMaterial
        {
            get { return handleMaterial; }
            set
            {
                if (handleMaterial != value)
                {
                    handleMaterial = value;
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material applied to handles while they are a grabbed")]
        private Material handleGrabbedMaterial;

        /// <summary>
        /// Material applied to handles while they are a grabbed
        /// </summary>
        public Material HandleGrabbedMaterial
        {
            get { return handleGrabbedMaterial; }
            set
            {
                if (handleGrabbedMaterial != value)
                {
                    handleGrabbedMaterial = value;
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Prefab used to display this type of bounding box handle. If not set, default shape will be used (scale default: boxes, rotation default: spheres)")]
        GameObject handlePrefab = null;

        /// <summary>
        /// Prefab used to display this type of bounding box handle. If not set, default shape will be used (scale default: boxes, rotation default: spheres)
        /// </summary>
        public GameObject HandlePrefab
        {
            get { return handlePrefab; }
            set
            {
                if (handlePrefab != value)
                {
                    handlePrefab = value;
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        // [FormerlySerializedAs("cornerRadius")]
        [Tooltip("Size of the handle collidable")]
        private float handleSize = 0.016f; // 1.6cm default handle size

        /// <summary>
        /// Size of the handle collidable
        /// </summary>
        public float HandleSize
        {
            get { return handleSize; }
            set
            {
                if (handleSize != value)
                {
                    handleSize = value;
                    configurationChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Tooltip("Additional padding to apply to the handle collider to make handle easier to hit")]
        private Vector3 colliderPadding = new Vector3(0.016f, 0.016f, 0.016f);

        /// <summary>
        /// Additional padding to apply to the handle collider to make handle easier to hit
        /// </summary>
        public Vector3 ColliderPadding
        {
            get { return colliderPadding; }
            set
            {
                if (colliderPadding != value)
                {
                    colliderPadding = value;
                    configurationChanged.Invoke();
                }
            }
        }

        internal protected UnityEvent configurationChanged = new UnityEvent();
        internal protected UnityEvent visibilityChanged = new UnityEvent();
        //internal protected UnityEvent handlesCreated = new UnityEvent();

        internal void ResetHandleVisibility(bool isVisible)
        {
            if (handles != null)
            {
                for (int i = 0; i < handles.Count; ++i)
                {
                    handles[i].gameObject.SetActive(isVisible && IsVisible(handles[i]));
                    BoundingBoxHandleUtils.ApplyMaterialToAllRenderers(handles[i].gameObject, handleMaterial);
                }
            }
        }

        public virtual bool IsVisible(Transform handle)
        {
            // todo . check if we can remove implementation and force derived to implement
            return true;
        }

        public virtual bool IsHandleTypeActive()
        {
            // todo . check if we can remove implementation and force derived to implement
            return true;
        }

        internal protected List<Transform> handles = new List<Transform>();

        public IReadOnlyList<Transform> Handles
        {
            get { return handles; }
        }

        internal void SetHighlighted(Transform handleToHighlight)
        {
            BoundingBoxHandleUtils.SetHighlighted(handleToHighlight, handles, handleGrabbedMaterial);
        }

        internal void HandleIgnoreCollider(Collider handlesIgnoreCollider)
        {
            BoundingBoxHandleUtils.HandleIgnoreCollider(handlesIgnoreCollider, handles);
        }


        public virtual void Init()
        {
            handles = new List<Transform>();
        }

        internal void DestroyHandles()
        {
            if (handles != null)
            {
                foreach (Transform transform in handles)
                {
                    GameObject.Destroy(transform.gameObject);
                }

                handles.Clear();
            }
        }

        internal bool IsHandleType(Transform handle)
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                if (handle == handles[i])
                {
                    return true;
                }
            }

            return false;
        }


        public virtual HandleType GetHandleType()
        {
            return HandleType.None;
        }

        internal void ForEachHandle(Action<Transform> action)
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                action(handles[i]);
            }
        }

        public void SetMaterials()
        {
            if (handleMaterial == null /*&& handleMaterial != wireframeMaterial*/)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                Shader shader = Shader.Find("Mixed Reality Toolkit/Standard");

                handleMaterial = new Material(shader);
                handleMaterial.EnableKeyword("_InnerGlow");
                handleMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                handleMaterial.SetFloat("_InnerGlow", 1.0f);
                handleMaterial.SetFloatArray("_InnerGlowColor", color);
            }
            if (handleGrabbedMaterial == null && handleGrabbedMaterial != handleMaterial/* && handleGrabbedMaterial != wireframeMaterial*/)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                Shader shader = Shader.Find("Mixed Reality Toolkit/Standard");

                handleGrabbedMaterial = new Material(shader);
                handleGrabbedMaterial.EnableKeyword("_InnerGlow");
                handleGrabbedMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                handleGrabbedMaterial.SetFloat("_InnerGlow", 1.0f);
                handleGrabbedMaterial.SetFloatArray("_InnerGlowColor", color);
            }
        }

        //public void CreateHandles()
        //{

    }
}
