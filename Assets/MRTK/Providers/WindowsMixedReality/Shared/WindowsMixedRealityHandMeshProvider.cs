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
    /// Queries the hand mesh data that an articulated hand on HoloLens 2 can provide.
    /// </summary>
    public class WindowsMixedRealityHandMeshProvider
    {
        /// <summary>
        /// The user's left hand.
        /// </summary>
        public static WindowsMixedRealityHandMeshProvider Left { get; } = new WindowsMixedRealityHandMeshProvider(Handedness.Left);

        /// <summary>
        /// The user's right hand.
        /// </summary>
        public static WindowsMixedRealityHandMeshProvider Right { get; } = new WindowsMixedRealityHandMeshProvider(Handedness.Right);

        private WindowsMixedRealityHandMeshProvider(Handedness handedness) => this.handedness = handedness;

        [Obsolete("WindowsMixedRealityHandMeshProvider(IMixedRealityController) is obsolete. Please use either the static Left or Right members and call SetInputSource()")]
        public WindowsMixedRealityHandMeshProvider(IMixedRealityController controller) : this(controller.ControllerHandedness)
        {
            SetInputSource(controller.InputSource);
        }

        private readonly Handedness handedness;
        private IMixedRealityInputSource inputSource = null;

        /// <summary>
        /// Sets the <see cref="IMixedRealityInputSource"/> that represents the current hand for this mesh.
        /// </summary>
        /// <param name="inputSource">Implementation of the hand input source.</param>
        public void SetInputSource(IMixedRealityInputSource inputSource)
        {
            this.inputSource = inputSource;
#if WINDOWS_UWP
            hasRequestedHandMeshObserver = false;
            handMeshObserver = null;
#endif // WINDOWS_UWP
        }

#if WINDOWS_UWP
        private HandMeshObserver handMeshObserver = null;
        private bool hasRequestedHandMeshObserver = false;

        private int handMeshModelId = -1;
        private int neutralPoseVersion = -1;

        private ushort[] handMeshTriangleIndices = null;
        private HandMeshVertex[] vertexAndNormals = null;

        private Vector3[] handMeshVerticesUnity = null;
        private Vector3[] handMeshNormalsUnity = null;
        private int[] handMeshTriangleIndicesUnity = null;
        private Vector2[] handMeshUVsUnity = null;

        private async void SetHandMeshObserver(SpatialInteractionSourceState sourceState)
        {
            handMeshObserver = await sourceState.Source.TryCreateHandMeshObserverAsync();
        }

        private void InitializeUVs(Vector3[] poseVertices)
        {
            if (poseVertices.Length == 0)
            {
                Debug.LogError("Loaded 0 verts for poseVertices");
            }

            float minY = poseVertices[0].y;
            float maxY = minY;

            for (int ix = 1; ix < poseVertices.Length; ix++)
            {
                Vector3 p = poseVertices[ix];

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

            if ((handMeshUVsUnity == null) ||
                (handMeshUVsUnity.Length != poseVertices.Length))
            {
                handMeshUVsUnity = new Vector2[poseVertices.Length];
            }

            for (int ix = 0; ix < poseVertices.Length; ix++)
            {
                Vector3 p = poseVertices[ix];

                handMeshUVsUnity[ix] = new Vector2(p.x * scale + 0.5f, (p.y - minY) * scale);
            }
        }

        private static readonly ProfilerMarker UpdateHandMeshPerfMarker = new ProfilerMarker($"[MRTK] {nameof(WindowsMixedRealityHandMeshProvider)}.UpdateHandMesh");

        private HandMeshInfo handMeshInfo = new HandMeshInfo();
        private Vector3[] neutralPoseVertices = null;
        private HandMeshVertex[] neutralVertexAndNormals = null;

        /// <summary>
        /// Updates the current hand mesh based on the passed in state of the hand.
        /// </summary>
        /// <param name="sourceState">The current hand state.</param>
        public void UpdateHandMesh(SpatialInteractionSourceState sourceState)
        {
            using (UpdateHandMeshPerfMarker.Auto())
            {
                MixedRealityInputSystemProfile inputSystemProfile = CoreServices.InputSystem?.InputSystemProfile;
                MixedRealityHandTrackingProfile handTrackingProfile = inputSystemProfile != null ? inputSystemProfile.HandTrackingProfile : null;

                if (handTrackingProfile == null || !handTrackingProfile.EnableHandMeshVisualization)
                {
                    // If hand mesh visualization is disabled make sure to destroy our hand mesh observer if it has already been created
                    if (handMeshObserver != null)
                    {
                        // Notify that hand mesh has been updated (cleared)
                        handMeshInfo = new HandMeshInfo();
                        CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, handMeshInfo);
                        hasRequestedHandMeshObserver = false;
                        handMeshObserver = null;
                    }
                    return;
                }

                // Accessing the hand mesh data involves copying quite a bit of data, so only do it if application requests it.
                if (handMeshObserver == null && !hasRequestedHandMeshObserver)
                {
                    SetHandMeshObserver(sourceState);
                    hasRequestedHandMeshObserver = true;
                }

                HandPose handPose = sourceState.TryGetHandPose();
                if (handMeshObserver != null && handPose != null)
                {
                    uint triangleIndexCount = handMeshObserver.TriangleIndexCount;
                    if (handMeshTriangleIndices == null ||
                        handMeshTriangleIndices.Length != triangleIndexCount)
                    {
                        handMeshTriangleIndices = new ushort[triangleIndexCount];
                        handMeshTriangleIndicesUnity = new int[triangleIndexCount];
                    }

                    int modelId = handMeshObserver.ModelId;
                    if (handMeshModelId != modelId)
                    {
                        handMeshObserver.GetTriangleIndices(handMeshTriangleIndices);
                        handMeshModelId = modelId;
                        Array.Copy(handMeshTriangleIndices, handMeshTriangleIndicesUnity, triangleIndexCount);
                    }

                    int poseVersion = handMeshObserver.NeutralPoseVersion;
                    if (neutralPoseVersion != poseVersion)
                    {
                        // Compute neutral pose
                        if ((neutralPoseVertices == null) ||
                            (neutralPoseVertices.Length != handMeshObserver.VertexCount))
                        {
                            neutralPoseVertices = new Vector3[handMeshObserver.VertexCount];
                        }
                        HandPose neutralPose = handMeshObserver.NeutralPose;
                        if ((neutralVertexAndNormals == null) ||
                            (neutralVertexAndNormals.Length != handMeshObserver.VertexCount))
                        {
                             neutralVertexAndNormals = new HandMeshVertex[handMeshObserver.VertexCount];
                        }
                        HandMeshVertexState handMeshVertexState = handMeshObserver.GetVertexStateForPose(neutralPose);
                        handMeshVertexState.GetVertices(neutralVertexAndNormals);

                        for (int i = 0; i < handMeshObserver.VertexCount; i++)
                        {
                            neutralVertexAndNormals[i].Position.ConvertToUnityVector3(ref neutralPoseVertices[i]);
                        };

                        neutralPoseVersion = poseVersion;

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
                        HandMeshVertexState handMeshVertexState = null;
                        try
                        {
                            handMeshVertexState = handMeshObserver.GetVertexStateForPose(handPose);
                        }
                        catch (ArgumentException)
                        {
                            Debug.Log($"{nameof(WindowsMixedRealityHandMeshProvider)} failed to update the hand mesh. This might happen if a source was detected and lost rapidly. Otherwise, the mesh might be stale this frame.");
                        }

                        if (handMeshVertexState == null)
                        {
                            return;
                        }

                        handMeshVertexState.GetVertices(vertexAndNormals);

                        var meshTransform = handMeshVertexState.CoordinateSystem.TryGetTransformTo(WindowsMixedRealityUtilities.SpatialCoordinateSystem);
                        if (meshTransform.HasValue)
                        {
                            System.Numerics.Matrix4x4.Decompose(meshTransform.Value,
                                out _,
                                out System.Numerics.Quaternion rotation,
                                out System.Numerics.Vector3 translation);

                            for (int i = 0; i < handMeshObserver.VertexCount; i++)
                            {
                                vertexAndNormals[i].Position.ConvertToUnityVector3(ref handMeshVerticesUnity[i]);
                                vertexAndNormals[i].Normal.ConvertToUnityVector3(ref handMeshNormalsUnity[i]);
                            };

                            // Hands should follow the Playspace to accommodate teleporting, so fold in the Playspace transform.
                            Vector3 positionUnity = MixedRealityPlayspace.TransformPoint(translation.ToUnityVector3());
                            Quaternion rotationUnity = MixedRealityPlayspace.Rotation * rotation.ToUnityQuaternion();

                            handMeshInfo.vertices = handMeshVerticesUnity;
                            handMeshInfo.normals = handMeshNormalsUnity;
                            handMeshInfo.triangles = handMeshTriangleIndicesUnity;
                            handMeshInfo.uvs = handMeshUVsUnity;
                            handMeshInfo.position = positionUnity;
                            handMeshInfo.rotation = rotationUnity;

                            CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, handMeshInfo);
                        }
                    }
                }
            }
        }
#endif // WINDOWS_UWP
    }
}
