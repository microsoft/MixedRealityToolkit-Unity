// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
using Microsoft.MixedReality.OpenXR;
using System.Collections.Generic;
using UnityEngine;
#endif // MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)

using Microsoft.MixedReality.Toolkit.Input;
using Unity.Profiling;

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    /// <summary>
    /// Provides the ability to load a hand mesh from OpenXR for the corresponding hand.
    /// </summary>
    internal class OpenXRHandMeshProvider
    {
        /// <summary>
        /// The user's left hand.
        /// </summary>
        public static OpenXRHandMeshProvider Left { get; } =
#if MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
            new OpenXRHandMeshProvider(HandMeshTracker.Left, Utilities.Handedness.Left);
#else
            null;
#endif // MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)

        /// <summary>
        /// The user's right hand.
        /// </summary>
        public static OpenXRHandMeshProvider Right { get; } =
#if MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
            new OpenXRHandMeshProvider(HandMeshTracker.Right, Utilities.Handedness.Right);
#else
            null;
#endif // MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)

#if MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
        private OpenXRHandMeshProvider(HandMeshTracker handMeshTracker, Utilities.Handedness handedness)
        {
            this.handMeshTracker = handMeshTracker;
            this.handedness = handedness;

            mesh = new Mesh();
            neutralPoseMesh = new Mesh();
        }

        private readonly HandMeshTracker handMeshTracker;
        private readonly Utilities.Handedness handedness;
        private readonly Mesh mesh;
        private readonly Mesh neutralPoseMesh;

        private readonly List<Vector3> vertices = new List<Vector3>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<int> triangles = new List<int>();

        private Vector2[] handMeshUVs = null;
#endif // MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)

        private IMixedRealityInputSource inputSource = null;

        /// <summary>
        /// Sets the <see cref="IMixedRealityInputSource"/> that represents the current hand for this mesh.
        /// </summary>
        /// <param name="inputSource">Implementation of the hand input source.</param>
        public void SetInputSource(IMixedRealityInputSource inputSource) => this.inputSource = inputSource;

        private static readonly ProfilerMarker UpdateHandMeshPerfMarker = new ProfilerMarker($"[MRTK] {nameof(OpenXRHandMeshProvider)}.UpdateHandMesh");

        /// <summary>
        /// Updates the hand mesh based on the current state of the hand.
        /// </summary>
        public void UpdateHandMesh()
        {
#if MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
            using (UpdateHandMeshPerfMarker.Auto())
            {
                MixedRealityInputSystemProfile inputSystemProfile = CoreServices.InputSystem?.InputSystemProfile;
                MixedRealityHandTrackingProfile handTrackingProfile = inputSystemProfile != null ? inputSystemProfile.HandTrackingProfile : null;

                if (handTrackingProfile == null || !handTrackingProfile.EnableHandMeshVisualization)
                {
                    // If hand mesh visualization is disabled make sure to clean up if we've already initialized
                    if (handMeshUVs != null)
                    {
                        // Notify that hand mesh has been updated (cleared)
                        CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, new HandMeshInfo());
                        handMeshUVs = null;
                    }
                    return;
                }

                if (handMeshUVs == null && handMeshTracker.TryGetHandMesh(FrameTime.OnUpdate, neutralPoseMesh, HandPoseType.ReferenceOpenPalm))
                {
                    handMeshUVs = InitializeUVs(neutralPoseMesh.vertices);
                }

                if (handMeshTracker.TryGetHandMesh(FrameTime.OnUpdate, mesh) && handMeshTracker.TryLocateHandMesh(FrameTime.OnUpdate, out Pose pose))
                {
                    mesh.GetVertices(vertices);
                    mesh.GetNormals(normals);
                    mesh.GetTriangles(triangles, 0);

                    HandMeshInfo handMeshInfo = new HandMeshInfo
                    {
                        vertices = vertices.ToArray(),
                        normals = normals.ToArray(),
                        triangles = triangles.ToArray(),
                        uvs = handMeshUVs,
                        position = MixedRealityPlayspace.TransformPoint(pose.position),
                        rotation = MixedRealityPlayspace.Rotation * pose.rotation
                    };

                    CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, handMeshInfo);
                }
            }
        }

        private Vector2[] InitializeUVs(Vector3[] neutralPoseVertices)
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

            Vector2[] uvs = new Vector2[neutralPoseVertices.Length];

            for (int ix = 0; ix < neutralPoseVertices.Length; ix++)
            {
                Vector3 p = neutralPoseVertices[ix];

                uvs[ix] = new Vector2(p.x * scale + 0.5f, (p.y - minY) * scale);
            }

            return uvs;
#endif // MSFT_OPENXR_0_2_0_OR_NEWER && (UNITY_STANDALONE_WIN || UNITY_WSA)
        }
    }
}
