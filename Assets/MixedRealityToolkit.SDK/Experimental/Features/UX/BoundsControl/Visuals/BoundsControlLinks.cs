
using Microsoft.MixedReality.Toolkit.UI.Experimental.BoundsControlTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Experimental
{
    /// <summary>
    /// Links that are rendered in between the corners of <see cref="BoundsControl"/>
    /// </summary>
    public class BoundsControlLinks
    {
        /// <summary>
        /// defines a bounds control link - the line between 2 corners of a box
        /// it keeps track of the transform and the axis the link is representing
        /// </summary>
        private class Link
        {
            public Transform transform;
            public CardinalAxisType axisType;
            public Link(Transform linkTransform, CardinalAxisType linkAxis)
            {
                transform = linkTransform;
                axisType = linkAxis;
            }
        }

        private List<Link> links = new List<Link>();
        private List<Renderer> linkRenderers = new List<Renderer>();

        private BoundsControlLinksConfiguration config;

        internal BoundsControlLinks(BoundsControlLinksConfiguration configuration)
        {
            Debug.Assert(configuration != null, "Can't create BoundsControlLinks without valid configuration");
            config = configuration;
        }

        internal void Clear()
        {
            if (links != null)
            {
                foreach (Link link in links)
                {
                    Object.Destroy(link.transform.gameObject);
                }
                links.Clear();
            }
        }

        internal void UpdateVisibilityInInspector(HideFlags flags)
        {
            if (links != null)
            {
                foreach (var link in links)
                {
                    link.transform.hideFlags = flags;
                }
            }
        }

        internal void ResetVisibility(bool isVisible)
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

        private Vector3 GetLinkDimensions(Vector3 currentBoundsExtents)
        {
            float wireframeEdgeRadius = config.WireframeEdgeRadius;
            float linkLengthAdjustor = config.WireframeShape == WireframeType.Cubic ? 2.0f : 1.0f - (6.0f * wireframeEdgeRadius);
            return (currentBoundsExtents * linkLengthAdjustor) + new Vector3(wireframeEdgeRadius, wireframeEdgeRadius, wireframeEdgeRadius);
        }

        internal void UpdateLinkPositions(ref Vector3[] boundsCorners)
        {
            if (boundsCorners != null)
            {
                for (int i = 0; i < links.Count; ++i)
                {
                    links[i].transform.position = BoundsControlVisualUtils.GetLinkPosition(i, ref boundsCorners);
                }
            }
        }

        internal void UpdateLinkScales(Vector3 currentBoundsExtents)
        {
            if (links != null)
            {
                for (int i = 0; i < links.Count; ++i)
                {
                    Transform parent = links[i].transform.parent;
                    Vector3 rootScale = parent.lossyScale;
                    Vector3 invRootScale = new Vector3(1.0f / rootScale[0], 1.0f / rootScale[1], 1.0f / rootScale[2]);
                    // Compute the local scale that produces the desired world space dimensions
                    Vector3 linkDimensions = Vector3.Scale(GetLinkDimensions(currentBoundsExtents), invRootScale);

                    float wireframeEdgeRadius = config.WireframeEdgeRadius;
                    if (links[i].axisType == CardinalAxisType.X)
                    {
                        links[i].transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.x, wireframeEdgeRadius);
                    }
                    else if (links[i].axisType == CardinalAxisType.Y)
                    {
                        links[i].transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.y, wireframeEdgeRadius);
                    }
                    else//Z
                    {
                        links[i].transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.z, wireframeEdgeRadius);
                    }
                }
            }
        }

        internal void Flatten(ref int[] flattenedHandles)
        {
            if (flattenedHandles != null && linkRenderers != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    linkRenderers[flattenedHandles[i]].enabled = false;
                }
            }
        }

        internal void CreateLinks(BoundsControlRotationHandles rotationHandles, Transform parent, Vector3 currentBoundsExtents)
        {
            // create links
            if (links != null)
            {
                GameObject link;
                Vector3 linkDimensions = GetLinkDimensions(currentBoundsExtents);
                for (int i = 0; i < BoundsControlRotationHandles.NumEdges; ++i)
                {
                    if (config.WireframeShape == WireframeType.Cubic)
                    {
                        link = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        GameObject.Destroy(link.GetComponent<BoxCollider>());
                    }
                    else
                    {
                        link = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        GameObject.Destroy(link.GetComponent<CapsuleCollider>());
                    }
                    link.name = "link_" + i.ToString();

                    CardinalAxisType axisType = rotationHandles.GetAxisType(i);
                    float wireframeEdgeRadius = config.WireframeEdgeRadius;
                    if (axisType == CardinalAxisType.Y)
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.y, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
                    }
                    else if (axisType == CardinalAxisType.Z)
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

                    if (config.WireframeMaterial != null)
                    {
                        linkRenderer.material = config.WireframeMaterial;
                    }

                    links.Add(new Link(link.transform, axisType));
                }
            }
        }
    }
}
