// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

//#define MAFINC_IN_OBJECT

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Object encapsulating the components of a spatial awareness mesh object.
    /// </summary>
    public class SpatialAwarenessMeshObject : BaseSpatialAwarenessObject
    {
        /// <summary>
        /// When a mesh is created we will need to create a game object with a minimum 
        /// set of components to contain the mesh.  These are the required component types.
        /// </summary>
        private static Type[] requiredMeshComponents =
        {
#if !MAFINC_IN_OBJECT
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(MeshCollider)
#else // MAFINC_IN_OBJECT
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(MeshCollider),
            typeof(PlayspaceAdapter)
#endif // MAFINC_IN_OBJECT
        };

        /// <summary>
        /// The collider for the mesh object.
        /// </summary>
        public MeshCollider Collider { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        private SpatialAwarenessMeshObject() : base() { }

#if MAFINC_IN_OBJECT // mafinc
        private class PlayspaceAdapter : MonoBehaviour
        {
            /// <summary>
            /// Compute concatenation of lhs * rhs such that lhs * (rhs * v) = Concat(lhs, rhs) * v
            /// </summary>
            /// <remarks>
            /// Start defining pose * vector = pose.p + pose.r * vector
            /// lhs * (rhs * v)
            /// = lhs * (rhs.p + rhs.r * v)
            /// = lhs.p + lhs.r * (rhs.p + rhs.r * v) 
            /// = lhs.p + lhs.r * rhs.p + lhs.r * rhs.r * v
            /// = Pose(lhs.p + lhs.r * rhs.p, lhs.r * rhs.r) * v
            /// </remarks>
            /// <param name="lhs">Second transform to apply</param>
            /// <param name="rhs">First transform to apply</param>
            /// <returns></returns>
            private static Pose Concatenate(Pose lhs, Pose rhs)
            {
                return new Pose(lhs.position + lhs.rotation * rhs.position, lhs.rotation * rhs.rotation);
            }

            /// <summary>
            /// Compute the pose that concatenated with the input produces an identity pose (pos=0, rot=none).
            /// </summary>
            /// <remarks>
            /// Concatenate(inv, pose) = Pose.identity === (Vector3.zero, Quatenion.identity)  
            /// (inv.p + inv.r * pose.p, inv.r * pose.r) = (Vector3.zero, Quatenion.identity)  // see def of Concatena5te above.
            /// Therefore:
            /// inv.r * pose.r = Quaternion.identity
            /// inv.r * pose.r * pose.r.inverse() = Quaternion.identity * pose.r.inverse()
            /// inv.r = pose.r.inverse()
            /// And
            /// inv.p + inv.r * pose.p = Vector3.zero
            /// inv.p = -(inv.r * pose.p)
            /// inv.p = -(pose.r.inverse() * pose.p) // using inv.r == pose.r.inverse() from above.
            /// So then:
            /// inv = (-(pose.r.inverse() * pose.p), pose.r.inverse())
            /// </remarks>
            /// <param name="pose">The pose to return the inverse of.</param>
            /// <returns>The inverse of pose</returns>
            private static Pose Invert(Pose pose)
            {
                var inverseRotation = Quaternion.Inverse(pose.rotation);
                return new Pose(-(inverseRotation * pose.position), inverseRotation);
            }

            private Pose ComputeAdaptedPose()
            {
                Pose worldFromPlayspace = new Pose(MixedRealityPlayspace.Position, MixedRealityPlayspace.Rotation);
                Transform parentTransform = transform.parent.transform;
                Pose playspaceFromParent = new Pose(parentTransform.position, parentTransform.rotation);
                Pose parentFromPlayspace = Invert(playspaceFromParent);

                return Concatenate(parentFromPlayspace, Concatenate(worldFromPlayspace, playspaceFromParent));
                //return Concatenate(parentFromPlayspace, playspaceFromParent);
                //return new Pose(Vector3.zero, Quaternion.identity);
            }

            private void Update()
            {
                Pose adaptedPose = ComputeAdaptedPose();
                transform.localPosition = adaptedPose.position;
                transform.localRotation = adaptedPose.rotation;
            }
        }

#endif // marfinc

        /// <summary>
        /// Creates a <see cref="SpatialAwarenessMeshObject"/>.
        /// </summary>
        /// <param name="mesh"></param> todo: add comments
        /// <param name="layer"></param>
        /// <param name="name"></param>
        /// <param name="meshId"></param>
        /// <returns>
        /// SpatialMeshObject containing the fields that describe the mesh.
        /// </returns>
        public static SpatialAwarenessMeshObject Create(Mesh mesh, int layer, string name, int meshId)
        {
            SpatialAwarenessMeshObject newMesh = new SpatialAwarenessMeshObject();

#if !MAFINC_IN_OBJECT // mafinc
            newMesh.Id = meshId;
            newMesh.GameObject = new GameObject(name, requiredMeshComponents);
            newMesh.GameObject.layer = layer;

            newMesh.Filter = newMesh.GameObject.GetComponent<MeshFilter>();
            newMesh.Filter.sharedMesh = mesh;

            newMesh.Renderer = newMesh.GameObject.GetComponent<MeshRenderer>();

            // Reset the surface mesh collider to fit the updated mesh. 
            // Unity tribal knowledge indicates that to change the mesh assigned to a
            // mesh collider, the mesh must first be set to null.  Presumably there
            // is a side effect in the setter when setting the shared mesh to null.
            newMesh.Collider = newMesh.GameObject.GetComponent<MeshCollider>();
            newMesh.Collider.sharedMesh = null;
            newMesh.Collider.sharedMesh = newMesh.Filter.sharedMesh;
#else // mafinc
            newMesh.Id = meshId;
            newMesh.GameObject = new GameObject(name);
            newMesh.GameObject.layer = layer;

            var adapter = new GameObject(name + "transformed", requiredMeshComponents);
            //adapter.AddComponent<PlayspaceAdapter>(); // mafinc - add to required?
            adapter.transform.SetParent(newMesh.GameObject.transform, false);

            newMesh.Filter = adapter.GetComponent<MeshFilter>();
            newMesh.Filter.sharedMesh = mesh;

            newMesh.Renderer = adapter.GetComponent<MeshRenderer>();

            // Reset the surface mesh collider to fit the updated mesh. 
            // Unity tribal knowledge indicates that to change the mesh assigned to a
            // mesh collider, the mesh must first be set to null.  Presumably there
            // is a side effect in the setter when setting the shared mesh to null.
            newMesh.Collider = adapter.GetComponent<MeshCollider>();
            newMesh.Collider.sharedMesh = null;
            newMesh.Collider.sharedMesh = newMesh.Filter.sharedMesh;
#endif // mafinc

            return newMesh;
        }

        /// <summary>
        /// Clean up the resources associated with the surface.
        /// </summary>
        /// <param name="meshObject">The <see cref="SpatialAwarenessMeshObject"/> whose resources will be cleaned up.</param>
        /// <param name="destroyGameObject"></param>
        /// <param name="destroyMeshes"></param>
        public static void Cleanup(SpatialAwarenessMeshObject meshObject, bool destroyGameObject = true, bool destroyMeshes = true)
        {
            if (meshObject.GameObject == null)
            {
                return;
            }

            if (destroyGameObject)
            {
                UnityEngine.Object.Destroy(meshObject.GameObject);
                meshObject.GameObject = null;
                return;
            }

            if (destroyMeshes)
            {
                Mesh filterMesh = meshObject.Filter.sharedMesh;
                Mesh colliderMesh = meshObject.Collider.sharedMesh;

                if (filterMesh != null)
                {
                    UnityEngine.Object.Destroy(filterMesh);
                    meshObject.Filter.sharedMesh = null;
                }

                if ((colliderMesh != null) && (colliderMesh != filterMesh))
                {
                    UnityEngine.Object.Destroy(colliderMesh);
                    meshObject.Collider.sharedMesh = null;
                }
            }
        }

    }
}