// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;

#if WINDOWS_UWP
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;
using Windows.Perception.People;
using Windows.UI.Input.Spatial;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Defines the additional data, like hand mesh, that an articulated hand on HoloLens 2 can provide.
    /// </summary>
    /// <remarks>This class is deprecated. Use WindowsMixedRealityHandMeshProvider instead.</remarks>
    [Obsolete("This class is deprecated. Use WindowsMixedRealityHandMeshProvider instead.")]
    public class WindowsMixedRealityArticulatedHandDefinition : ArticulatedHandDefinition
    {
        [Obsolete("This class is deprecated. Use WindowsMixedRealityHandMeshProvider instead.")]
        public WindowsMixedRealityArticulatedHandDefinition(IMixedRealityInputSource source, Handedness handedness) : base(source, handedness)
        { }

#if WINDOWS_UWP
        private HandMeshObserver handMeshObserver = null;

        private ushort[] handMeshTriangleIndices = null;
        private HandMeshVertex[] vertexAndNormals = null;

        private Vector3[] handMeshVerticesUnity = null;
        private Vector3[] handMeshNormalsUnity = null;
        private int[] handMeshTriangleIndicesUnity = null;
        private Vector2[] handMeshUVsUnity = null;

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

            handMeshUVsUnity = new Vector2[neutralPoseVertices.Length];

            for (int ix = 0; ix < neutralPoseVertices.Length; ix++)
            {
                Vector3 p = neutralPoseVertices[ix];

                handMeshUVsUnity[ix] = new Vector2(p.x * scale + 0.5f, (p.y - minY) * scale);
            }
        }

        private static readonly ProfilerMarker UpdateHandMeshPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityArticulatedHandDefinition.UpdateHandMesh");

        /// <summary>
        /// Updates the current hand mesh based on the passed in state of the hand.
        /// </summary>
        /// <param name="sourceState">The current hand state.</param>
        [Obsolete("This class is deprecated. Use WindowsMixedRealityHandMeshProvider.UpdateHandMesh instead.")]
        public void UpdateHandMesh(SpatialInteractionSourceState sourceState)
        {
            using (UpdateHandMeshPerfMarker.Auto())
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
                        CoreServices.InputSystem?.RaiseHandMeshUpdated(InputSource, Handedness, handMeshInfo);
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

                        Array.Copy(handMeshTriangleIndices, handMeshTriangleIndicesUnity, (int)handMeshObserver.TriangleIndexCount);

                        // Compute neutral pose
                        Vector3[] neutralPoseVertices = new Vector3[handMeshObserver.VertexCount];
                        HandPose neutralPose = handMeshObserver.NeutralPose;
                        var neutralVertexAndNormals = new HandMeshVertex[handMeshObserver.VertexCount];
                        HandMeshVertexState handMeshVertexState = handMeshObserver.GetVertexStateForPose(neutralPose);
                        handMeshVertexState.GetVertices(neutralVertexAndNormals);

                        Parallel.For(0, handMeshObserver.VertexCount, i =>
                        {
                            neutralVertexAndNormals[i].Position.ConvertToUnityVector3(ref neutralPoseVertices[i]);
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
                            System.Numerics.Matrix4x4.Decompose(meshTransform.Value,
                                out System.Numerics.Vector3 scale,
                                out System.Numerics.Quaternion rotation,
                                out System.Numerics.Vector3 translation);

                            Parallel.For(0, handMeshObserver.VertexCount, i =>
                            {
                                vertexAndNormals[i].Position.ConvertToUnityVector3(ref handMeshVerticesUnity[i]);
                                vertexAndNormals[i].Normal.ConvertToUnityVector3(ref handMeshNormalsUnity[i]);
                            });

                            /// Hands should follow the Playspace to accommodate teleporting, so fold in the Playspace transform.
                            Vector3 positionUnity = MixedRealityPlayspace.TransformPoint(translation.ToUnityVector3());
                            Quaternion rotationUnity = MixedRealityPlayspace.Rotation * rotation.ToUnityQuaternion();

                            HandMeshInfo handMeshInfo = new HandMeshInfo
                            {
                                vertices = handMeshVerticesUnity,
                                normals = handMeshNormalsUnity,
                                triangles = handMeshTriangleIndicesUnity,
                                uvs = handMeshUVsUnity,
                                position = positionUnity,
                                rotation = rotationUnity
                            };

                            CoreServices.InputSystem?.RaiseHandMeshUpdated(InputSource, Handedness, handMeshInfo);
                        }
                    }
                }
            }
        }
#endif // WINDOWS_UWP
    }
}
