// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Perception.People;
using Windows.UI.Input.Spatial;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Defines the additional data, like hand mesh, that an articulated hand on HoloLens 2 can provide.
    /// </summary>
    public class WindowsMixedRealityArticulatedHandDefinition : ArticulatedHandDefinition
    {
        public WindowsMixedRealityArticulatedHandDefinition(IMixedRealityInputSource source, Handedness handedness) : base(source, handedness) { }

#if WINDOWS_UWP
        private Vector2[] handMeshUVs = null;
        private HandMeshObserver handMeshObserver = null;

        private ushort[] handMeshTriangleIndices = null;
        private HandMeshVertex[] vertexAndNormals = null;

        private Vector3[] handMeshVerticesUnity = null;
        private Vector3[] handMeshNormalsUnity = null;
        private int[] handMeshTriangleIndicesUnity = null;

        private bool hasRequestedHandMeshObserver = false;

        private async void SetHandMeshObserver(SpatialInteractionSourceState sourceState)
        {
            handMeshObserver = await sourceState.Source.TryCreateHandMeshObserverAsync();
        }

        private void InitializeUVs(Vector3[] neutralPoseVertices)
        {
            if (neutralPoseVertices.Length == 0)
            {
                Debug.LogError("Loaded 0 verts for neutralPoseVertices");
            }

            float minY = neutralPoseVertices[0].y;
            float maxY = minY;

            for (int ix = 1; ix < neutralPoseVertices.Length; ix++)
            {
                Vector3 p = neutralPoseVertices[ix];

                if (p.y < minY)
                {
                    minY = p.y;
                }
                else if (p.y > maxY)
                {
                    maxY = p.y;
                }
            }

            float scale = 1.0f / (maxY - minY);

            handMeshUVs = new Vector2[neutralPoseVertices.Length];

            for (int ix = 0; ix < neutralPoseVertices.Length; ix++)
            {
                Vector3 p = neutralPoseVertices[ix];

                handMeshUVs[ix] = new Vector2(p.x * scale + 0.5f, (p.y - minY) * scale);
            }
        }

        /// <summary>
        /// Updates the current hand mesh based on the passed in state of the hand.
        /// </summary>
        /// <param name="sourceState">The current hand state.</param>
        public void UpdateHandMesh(SpatialInteractionSourceState sourceState)
        {
            MixedRealityHandTrackingProfile handTrackingProfile = null;
            MixedRealityInputSystemProfile inputSystemProfile = CoreServices.InputSystem?.InputSystemProfile;
            if (inputSystemProfile != null)
            {
                handTrackingProfile = inputSystemProfile.HandTrackingProfile;
            }

            if (handTrackingProfile == null || !handTrackingProfile.EnableHandMeshVisualization)
            {
                // If hand mesh visualization is disabled make sure to destroy our hand mesh observer if it has already been created
                if (handMeshObserver != null)
                {
                    // Notify that hand mesh has been updated (cleared)
                    HandMeshInfo handMeshInfo = new HandMeshInfo();
                    CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, handMeshInfo);
                    hasRequestedHandMeshObserver = false;
                    handMeshObserver = null;
                }
                return;
            }

            HandPose handPose = sourceState.TryGetHandPose();

            // Accessing the hand mesh data involves copying quite a bit of data, so only do it if application requests it.
            if (handMeshObserver == null && !hasRequestedHandMeshObserver)
            {
                SetHandMeshObserver(sourceState);
                hasRequestedHandMeshObserver = true;
            }

            if (handMeshObserver != null && handPose != null)
            {
                if (handMeshTriangleIndices == null)
                {
                    handMeshTriangleIndices = new ushort[handMeshObserver.TriangleIndexCount];
                    handMeshTriangleIndicesUnity = new int[handMeshObserver.TriangleIndexCount];
                    handMeshObserver.GetTriangleIndices(handMeshTriangleIndices);

                    // Compute neutral pose
                    Vector3[] neutralPoseVertices = new Vector3[handMeshObserver.VertexCount];
                    HandPose neutralPose = handMeshObserver.NeutralPose;
                    var neutralVertexAndNormals = new HandMeshVertex[handMeshObserver.VertexCount];
                    HandMeshVertexState handMeshVertexState = handMeshObserver.GetVertexStateForPose(neutralPose);
                    handMeshVertexState.GetVertices(neutralVertexAndNormals);

                    // TODO: Troy - parrelize
                    /*
                    for (int i = 0; i < handMeshObserver.VertexCount; i++)
                    {
                        neutralPoseVertices[i] = neutralVertexAndNormals[i].Position.ToUnityVector3();
                    }
                    */

                    Parallel.For(0, handMeshObserver.VertexCount,
                    i =>
                    {
                        neutralPoseVertices[i] = neutralVertexAndNormals[i].Position.ToUnityVector3();
                    });

                    // Compute UV mapping
                    InitializeUVs(neutralPoseVertices);
                }

                if (vertexAndNormals == null)
                {
                    vertexAndNormals = new HandMeshVertex[handMeshObserver.VertexCount];
                    handMeshVerticesUnity = new Vector3[handMeshObserver.VertexCount];
                    handMeshNormalsUnity = new Vector3[handMeshObserver.VertexCount];
                }

                if (vertexAndNormals != null && handMeshTriangleIndices != null)
                {
                    var handMeshVertexState = handMeshObserver.GetVertexStateForPose(handPose);
                    handMeshVertexState.GetVertices(vertexAndNormals);

                    var meshTransform = handMeshVertexState.CoordinateSystem.TryGetTransformTo(WindowsMixedRealityUtilities.SpatialCoordinateSystem);
                    if (meshTransform.HasValue)
                    {
                        System.Numerics.Vector3 scale;
                        System.Numerics.Quaternion rotation;
                        System.Numerics.Vector3 translation;
                        System.Numerics.Matrix4x4.Decompose(meshTransform.Value, out scale, out rotation, out translation);

                        // TODO: Troy - SIMD
                        Parallel.For(0, handMeshObserver.VertexCount,
                        i =>
                        {
                            handMeshVerticesUnity[i] = vertexAndNormals[i].Position.ToUnityVector3();
                            handMeshNormalsUnity[i] = vertexAndNormals[i].Normal.ToUnityVector3();
                        });

                        // TODO: Compare*
                        /*
                        for (int i = 0; i < handMeshObserver.VertexCount; i++)
                        {
                            handMeshVertices[i] = vertexAndNormals[i].Position.ToUnityVector3();
                            handMeshNormals[i] = vertexAndNormals[i].Normal.ToUnityVector3();
                        }*/

                        HandMeshInfo handMeshInfo = new HandMeshInfo
                        {
                            vertices = handMeshVerticesUnity,
                            normals = handMeshNormalsUnity,
                            triangles = handMeshTriangleIndicesUnity,
                            uvs = handMeshUVs,
                            position = translation.ToUnityVector3(),
                            rotation = rotation.ToUnityQuaternion()
                        };

                        CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, handMeshInfo);
                    }
                }
            }
        }
#endif // WINDOWS_UWP
    }
}
