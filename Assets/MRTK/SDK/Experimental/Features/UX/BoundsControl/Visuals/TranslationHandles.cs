// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControlTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl
{
    /// <summary>
    /// Translation handles for <see cref="BoundsControl"/> that are used for translating the
    /// Gameobject BoundsControl is attached to with near or far interaction
    /// </summary>
    public class TranslationHandles : HandlesBase
    {
        protected override HandlesBaseConfiguration BaseConfig => config;
        private TranslationHandlesConfiguration config;
        private FlattenModeType cachedFlattenAxis;

        internal TranslationHandles(TranslationHandlesConfiguration configuration)
        {
            Debug.Assert(configuration != null, "Can't create BoundsControlTranslationHandles without valid configuration");
            config = configuration;
            config.handlesChanged.AddListener(HandlesChanged);
            config.colliderTypeChanged.AddListener(UpdateColliderType);
        }

        ~TranslationHandles()
        {
            config.handlesChanged.RemoveListener(HandlesChanged);
            config.colliderTypeChanged.RemoveListener(UpdateColliderType);
        }

        private void UpdateColliderType()
        {
            foreach (var handle in handles)
            {
                // remove old colliders
                bool shouldCreateNewCollider = false;
                var oldBoxCollider = handle.GetComponent<BoxCollider>();
                if (oldBoxCollider != null && config.TranslationHandlePrefabColliderType == HandlePrefabCollider.Sphere)
                {
                    shouldCreateNewCollider = true;
                    Object.Destroy(oldBoxCollider);
                }

                var oldSphereCollider = handle.GetComponent<SphereCollider>();
                if (oldSphereCollider != null && config.TranslationHandlePrefabColliderType == HandlePrefabCollider.Box)
                {
                    shouldCreateNewCollider = true;
                    Object.Destroy(oldSphereCollider);
                }

                if (shouldCreateNewCollider)
                {
                    // attach new collider
                    var handleBounds = VisualUtils.GetMaxBounds(GetVisual(handle).gameObject);
                    var invScale = handleBounds.size.x == 0.0f ? 0.0f : config.HandleSize / handleBounds.size.x;
                    Vector3 colliderSizeScaled = handleBounds.size * invScale;
                    Vector3 colliderCenterScaled = handleBounds.center * invScale;
                    if (config.TranslationHandlePrefabColliderType == HandlePrefabCollider.Box)
                    {
                        BoxCollider collider = handle.gameObject.AddComponent<BoxCollider>();
                        collider.size = colliderSizeScaled;
                        collider.center = colliderCenterScaled;
                        collider.size += config.ColliderPadding;
                    }
                    else
                    {
                        SphereCollider sphere = handle.gameObject.AddComponent<SphereCollider>();
                        sphere.center = colliderCenterScaled;
                        sphere.radius = colliderSizeScaled.x * 0.5f;
                        sphere.radius += VisualUtils.GetMaxComponent(config.ColliderPadding);
                    }
                }

            }
        }

        internal const int NumFaces = 6;

        private Vector3[] faceCenters = new Vector3[NumFaces];
        private CardinalAxisType[] faceAxes;

        internal int GetTranslationHandleIdx(Transform handle)
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                if (handle == handles[i])
                {
                    return i;
                }
            }

            return handles.Count;
        }

        internal Vector3 GetFaceCenter(int index)
        {
            Debug.Assert(index >= 0 && index <= NumFaces, "Face center index out of bounds");
            return faceCenters[index];
        }

        internal CardinalAxisType GetAxisType(int index)
        {
            Debug.Assert(index >= 0 && index <= NumFaces, "Face axes index out of bounds");
            return faceAxes[index];
        }

        internal CardinalAxisType GetAxisType(Transform handle)
        {
            int index = GetTranslationHandleIdx(handle);
            return GetAxisType(index);
        }

        private void UpdateHandles()
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                handles[i].position = GetFaceCenter(i);
            }
        }

        internal void CalculateFaceCenters(ref Vector3[] boundsCorners)
        {
            if (boundsCorners != null && faceCenters != null)
            {
                for (int i = 0; i < faceCenters.Length; ++i)
                {
                    faceCenters[i] = VisualUtils.GetFaceCenterPosition(i, ref boundsCorners);
                }
            }

            UpdateHandles();
        }

        internal void InitEdgeAxis()
        {
            faceAxes = new CardinalAxisType[NumFaces];
            faceAxes[0] = CardinalAxisType.X;
            faceAxes[1] = CardinalAxisType.X;
            faceAxes[2] = CardinalAxisType.Z;
            faceAxes[3] = CardinalAxisType.Z;
            faceAxes[4] = CardinalAxisType.Y;
            faceAxes[5] = CardinalAxisType.Y;
        }

        internal virtual void Reset(bool areHandlesActive, FlattenModeType flattenAxis)
        {
            IsActive = areHandlesActive;
            cachedFlattenAxis = flattenAxis;
            if (IsActive)
            {
                ResetHandles();
                int[] flattenedHandles = VisualUtils.GetFlattenedIndices(flattenAxis);
                if (flattenedHandles != null)
                {
                    for (int i = 0; i < flattenedHandles.Length; ++i)
                    {
                        handles[flattenedHandles[i]].gameObject.SetActive(false);
                    }
                }
            }
        }

        internal void Create(ref Vector3[] boundsCorners, Transform parent)
        {
            faceCenters = new Vector3[6];
            CalculateFaceCenters(ref boundsCorners);
            InitEdgeAxis();
            CreateHandles(parent);
        }

        private void CreateHandles(Transform parent)
        {
            for (int i = 0; i < faceCenters.Length; ++i)
            {
                GameObject faceCenter = new GameObject();
                faceCenter.name = "faceCenter_" + i.ToString();
                faceCenter.transform.position = faceCenters[i];
                faceCenter.transform.parent = parent;

                Bounds faceCenterBounds = CreateVisual(i, faceCenter);
                float maxDim = VisualUtils.GetMaxComponent(faceCenterBounds.size);
                float invScale = maxDim == 0.0f ? 0.0f : config.HandleSize / maxDim;
                VisualUtils.AddComponentsToAffordance(faceCenter, new Bounds(faceCenterBounds.center * invScale, faceCenterBounds.size * invScale),
                    config.TranslationHandlePrefabColliderType, CursorContextInfo.CursorAction.Move, config.ColliderPadding, parent, config.DrawTetherWhenManipulating);

                handles.Add(faceCenter.transform);
            }

            VisualUtils.HandleIgnoreCollider(config.HandlesIgnoreCollider, handles);

            objectsChangedEvent.Invoke(this);
        }

        protected override void RecreateVisuals()
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                // get parent of visual
                Transform obsoleteChild = handles[i].Find("visuals");
                if (obsoleteChild)
                {
                    // get old child and remove it
                    obsoleteChild.parent = null;
                    Object.Destroy(obsoleteChild.gameObject);
                }
                else
                {
                    Debug.LogError($"Couldn't find translation visual on recreating visuals, index {i}");
                }

                // create new visual
                Bounds visualBounds = CreateVisual(i, handles[i].gameObject);

                // update handle collider bounds
                UpdateColliderBounds(handles[i], visualBounds.size);
            }

            objectsChangedEvent.Invoke(this);
        }

        protected override void UpdateColliderBounds(Transform handle, Vector3 visualSize)
        {
            var invScale = visualSize.x == 0.0f ? 0.0f : config.HandleSize / visualSize.x;
            GetVisual(handle).transform.localScale = new Vector3(invScale, invScale, invScale);
            Vector3 colliderSizeScaled = visualSize * invScale;
            if (config.TranslationHandlePrefabColliderType == HandlePrefabCollider.Box)
            {
                BoxCollider collider = handle.gameObject.GetComponent<BoxCollider>();
                collider.size = colliderSizeScaled;
                collider.size += BaseConfig.ColliderPadding;
            }
            else
            {
                SphereCollider collider = handle.gameObject.GetComponent<SphereCollider>();
                collider.radius = colliderSizeScaled.x * 0.5f;
                collider.radius += VisualUtils.GetMaxComponent(config.ColliderPadding);
            }
        }

        private Bounds CreateVisual(int handleIndex, GameObject parent)
        {
            GameObject midpointVisual;
            GameObject prefabType = config.HandlePrefab;
            if (prefabType != null)
            {
                midpointVisual = Object.Instantiate(prefabType);
            }
            else
            {
                midpointVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Object.Destroy(midpointVisual.GetComponent<SphereCollider>());
            }

            // Even handle indices point in the positive direction along their axis,
            // and odd handle indices point in the negative direction;
            var directionSign = handleIndex % 2 == 0 ? 1.0f : -1.0f;

            // Align handle with its edge assuming that the prefab is initially aligned with the up direction 
            if (faceAxes[handleIndex] == CardinalAxisType.X)
            {
                Quaternion realignment = Quaternion.FromToRotation(Vector3.forward, directionSign * Vector3.right);
                midpointVisual.transform.localRotation = realignment * midpointVisual.transform.localRotation;
            }
            else if (faceAxes[handleIndex] == CardinalAxisType.Z)
            {
                Quaternion realignment = Quaternion.FromToRotation(Vector3.forward, directionSign * Vector3.forward);
                midpointVisual.transform.localRotation = realignment * midpointVisual.transform.localRotation;
            }
            else if (faceAxes[handleIndex] == CardinalAxisType.Y)
            {
                Quaternion realignment = Quaternion.FromToRotation(Vector3.forward, directionSign * Vector3.up);
                midpointVisual.transform.localRotation = realignment * midpointVisual.transform.localRotation;
            }

            Bounds midpointBounds = VisualUtils.GetMaxBounds(midpointVisual);
            float maxDim = VisualUtils.GetMaxComponent(midpointBounds.size);
            float invScale = maxDim == 0.0f ? 0.0f : config.HandleSize / maxDim;

            midpointVisual.name = "visuals";
            midpointVisual.transform.parent = parent.transform;
            midpointVisual.transform.localScale = new Vector3(invScale, invScale, invScale);
            midpointVisual.transform.localPosition = Vector3.zero;

            if (config.HandleMaterial != null)
            {
                VisualUtils.ApplyMaterialToAllRenderers(midpointVisual, config.HandleMaterial);
            }

            return midpointBounds;
        }

        #region BoundsControlHandlerBase 
        internal override bool IsVisible(Transform handle)
        {
            CardinalAxisType axisType = GetAxisType(handle);
            return IsActive &&
                ((axisType == CardinalAxisType.X && config.ShowTranslationHandleForX) ||
                (axisType == CardinalAxisType.Y && config.ShowTranslationHandleForY) ||
                (axisType == CardinalAxisType.Z && config.ShowTranslationHandleForZ));
        }

        internal override HandleType GetHandleType()
        {
            return HandleType.Translation;
        }

        protected override Transform GetVisual(Transform handle)
        {
            // visual is first child 
            Transform childTransform = handle.GetChild(0);
            if (childTransform != null && childTransform.name == "visuals")
            {
                return childTransform;
            }

            return null;
        }
        #endregion BoundsControlHandlerBase

        #region IProximityScaleObjectProvider 
        public override bool IsActive
        {
            get
            {
                return (config.ShowTranslationHandleForX || config.ShowTranslationHandleForY || config.ShowTranslationHandleForZ) && base.IsActive;
            }
        }

        #endregion IProximityScaleObjectProvider

    }
}
