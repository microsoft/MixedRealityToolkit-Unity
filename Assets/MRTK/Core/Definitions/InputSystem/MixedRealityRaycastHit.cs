// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The resulting hit information from an IMixedRealityRaycastProvider.
    /// </summary>
    public struct MixedRealityRaycastHit
    {
        public Vector3 point;
        public Vector3 normal;
        public Vector3 barycentricCoordinate;
        public float distance;
        public int triangleIndex;
        public Vector2 textureCoord;
        public Vector2 textureCoord2;
        public Transform transform;
        public Vector2 lightmapCoord;
        public bool raycastValid;
        public Collider collider;

        public MixedRealityRaycastHit(bool raycastValid, RaycastHit hitInfo)
        {
            this.raycastValid = raycastValid;
            if (raycastValid)
            {
                point = hitInfo.point;
                normal = hitInfo.normal;
                barycentricCoordinate = hitInfo.barycentricCoordinate;
                distance = hitInfo.distance;
                triangleIndex = hitInfo.triangleIndex;

                MeshCollider meshCollider = hitInfo.collider as MeshCollider;
                if (meshCollider == null)
                {
                    textureCoord = hitInfo.textureCoord;
                    textureCoord2 = hitInfo.textureCoord2;
                    lightmapCoord = hitInfo.lightmapCoord;
                }
                else
                {
                    Mesh sharedMesh = meshCollider.sharedMesh;
                    if (sharedMesh != null && sharedMesh.isReadable)
                    {
#if UNITY_2019_4_OR_NEWER
                        if (sharedMesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.TexCoord0))
                        {
                            textureCoord = hitInfo.textureCoord;
                        }
                        else
                        {
                            textureCoord = Vector2.zero;
                        }

                        // This checks for TexCoord1, since textureCoord2 and lightmapCoord both query that index
                        // via CalculateRaycastTexCoord(collider, m_UV, m_Point, m_FaceID, 1); (the last parameter is the index)
                        if (sharedMesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.TexCoord1))
                        {
                            textureCoord2 = hitInfo.textureCoord2;
                            lightmapCoord = hitInfo.lightmapCoord;
                        }
                        else
                        {
                            textureCoord2 = Vector2.zero;
                            lightmapCoord = Vector2.zero;
                        }
#else
                        textureCoord = hitInfo.textureCoord;
                        textureCoord2 = hitInfo.textureCoord2;
                        lightmapCoord = hitInfo.lightmapCoord;
#endif
                    }
                    else
                    {
                        textureCoord = Vector2.zero;
                        textureCoord2 = Vector2.zero;
                        lightmapCoord = Vector2.zero;
                    }
                }

                transform = hitInfo.transform;
                collider = hitInfo.collider;
            }
            else
            {
                point = Vector3.zero;
                normal = Vector3.zero;
                barycentricCoordinate = Vector3.zero;
                distance = 0;
                triangleIndex = 0;
                textureCoord = Vector2.zero;
                textureCoord2 = Vector2.zero;
                transform = null;
                lightmapCoord = Vector2.zero;
                collider = null;
            }
        }
    }
}
