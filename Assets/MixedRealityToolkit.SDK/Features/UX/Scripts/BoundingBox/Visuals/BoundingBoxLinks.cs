
using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    internal class BoundingBoxLinks
    {
        private List<Transform> links;
        private List<Renderer> linkRenderers;


        public void Init(bool showWireframe)
        {
            if (showWireframe)
            {
                links = new List<Transform>();
                linkRenderers = new List<Renderer>();
            }
        }
        public void Clear()
        {
            if (links != null)
            {
                foreach (Transform transform in links)
                {
                    Object.Destroy(transform.gameObject);
                }
                links.Clear();
                links = null;
            }
        }


        public void UpdateVisibilityInInspector(HideFlags flags)
        {
            if (links != null)
            {
                foreach (var link in links)
                {
                    link.hideFlags = flags;
                }
            }
        }

        public void ResetVisibility(bool isVisible)
        {
            if (links != null)
            {
                for (int i = 0; i < linkRenderers.Count; ++i)
                {
                    if (linkRenderers[i] != null)
                    {
                        linkRenderers[i].enabled = isVisible;
                    }
                }
            }
        }

        public void Update(BoundingBoxRotationHandles rotationHandles, float wireframeEdgeRadius, Vector3 linkDimensions)
        {
            for (int i = 0; i < BoundingBoxRotationHandles.NumEdges; ++i)
            {

                if (links != null)
                {
                    links[i].position = rotationHandles.GetEdgeCenter(i);

                    if (rotationHandles.GetAxisType(i) == CardinalAxisType.X)
                    {
                        links[i].localScale = new Vector3(wireframeEdgeRadius, linkDimensions.x, wireframeEdgeRadius);
                    }
                    else if (rotationHandles.GetAxisType(i) == CardinalAxisType.Y)
                    {
                        links[i].localScale = new Vector3(wireframeEdgeRadius, linkDimensions.y, wireframeEdgeRadius);
                    }
                    else//Z
                    {
                        links[i].localScale = new Vector3(wireframeEdgeRadius, linkDimensions.z, wireframeEdgeRadius);
                    }
                }
            }
        }

        public void Flatten(BoundingBoxRotationHandles rotationHandles)
        {
            if (flattenedHandles != null && linkRenderers != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    linkRenderers[flattenedHandles[i]].enabled = false;
                }
            }
        }

        public void CreateLinks(BoundingBoxRotationHandles rotationHandles, float wireframeEdgeRadius, Vector3 linkDimensions,
            Transform parent, Material wireframeMaterial, WireframeType wireframeShape)
        {
            if (links != null)
            {
                GameObject link;
                for (int i = 0; i < BoundingBoxRotationHandles.NumEdges; ++i)
                {
                    if (wireframeShape == WireframeType.Cubic)
                    {
                        link = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Object.Destroy(link.GetComponent<BoxCollider>());
                    }
                    else
                    {
                        link = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        Object.Destroy(link.GetComponent<CapsuleCollider>());
                    }
                    link.name = "link_" + i.ToString();

                    if (rotationHandles.GetAxisType(i) == CardinalAxisType.Y)
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.y, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
                    }
                    else if (rotationHandles.GetAxisType(i) == CardinalAxisType.Z)
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.z, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                    }
                    else//X
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.x, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
                    }

                    link.transform.position = rotationHandles.GetEdgeCenter(i);
                    link.transform.parent = parent;
                    Renderer linkRenderer = link.GetComponent<Renderer>();
                    linkRenderers.Add(linkRenderer);

                    if (wireframeMaterial != null)
                    {
                        linkRenderer.material = wireframeMaterial;
                    }

                    links.Add(link.transform);
                }
            }
        }
    }
}
