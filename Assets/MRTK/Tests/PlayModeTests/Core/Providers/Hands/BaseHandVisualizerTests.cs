// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class BaseHandVisualizerTests : BasePlayModeTests
    {
        /// <summary>
        /// A mock IMixedRealityInputSource, used to test BaseHandVisualizer::OnHandMeshUpdated.
        /// </summary>
        private class MockInputSource : IMixedRealityInputSource
        {
            public IMixedRealityPointer[] Pointers => throw new System.NotImplementedException();
            public InputSourceType SourceType => throw new System.NotImplementedException();
            public uint SourceId => 0;

            public string SourceName => throw new System.NotImplementedException();

            public new bool Equals(object x, object y)
            {
                throw new System.NotImplementedException();
            }

            public int GetHashCode(object obj)
            {
                throw new System.NotImplementedException();
            }
        }

        private class MockController : IMixedRealityController
        {
            public bool Enabled { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

            public TrackingState TrackingState => throw new System.NotImplementedException();

            public Handedness ControllerHandedness => Handedness.None;

            public IMixedRealityInputSource InputSource => throw new System.NotImplementedException();

            public IMixedRealityControllerVisualizer Visualizer => throw new System.NotImplementedException();

            public bool IsPositionAvailable => throw new System.NotImplementedException();

            public bool IsPositionApproximate => throw new System.NotImplementedException();

            public bool IsRotationAvailable => throw new System.NotImplementedException();

            public MixedRealityInteractionMapping[] Interactions => throw new System.NotImplementedException();

            public Vector3 AngularVelocity => throw new System.NotImplementedException();

            public Vector3 Velocity => throw new System.NotImplementedException();

            public bool IsInPointingPose => throw new System.NotImplementedException();
        }

        /// <summary>
        /// Validates that OnHandMeshUpdated can be called with hand mesh vertices of different
        /// lengths and not crash.
        /// </summary>
        [UnityTest]
        public IEnumerator TestOnHandMeshUpdated()
        {
            // First invoke OnHandMeshUpdated with a hand mesh corresponding
            // to a quad, and then invoke it again with a hand mesh
            // corresponding to a triangle. The intent to is to verify that
            // the hand visualizer can be called with different sized
            // input meshes and not crash (which is required on some platforms)

            GameObject baseHandVisualizerGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var baseHandVisualizer = baseHandVisualizerGameObject.AddComponent<BaseHandVisualizer>();
            baseHandVisualizer.Controller = new MockController();

            baseHandVisualizer.OnHandMeshUpdated(CreateQuadInputEventData());
            baseHandVisualizer.OnHandMeshUpdated(CreateTriangleInputEventData());
            yield return null;

            Object.Destroy(baseHandVisualizer);
            Object.Destroy(baseHandVisualizerGameObject);
        }

        private static InputEventData<HandMeshInfo> CreateTriangleInputEventData()
        {
            HandMeshInfo handMeshInfo = new HandMeshInfo
            {
                vertices = new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0),
                },
                normals = new Vector3[]
                {
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                },
                triangles = new int[]
                {
                    0, 2, 1
                }
            };
            var inputEventData = new InputEventData<HandMeshInfo>(EventSystem.current);
            inputEventData.Initialize(new MockInputSource(), Handedness.None, MixedRealityInputAction.None, handMeshInfo);
            return inputEventData;
        }

        private static InputEventData<HandMeshInfo> CreateQuadInputEventData()
        {
            HandMeshInfo handMeshInfo = new HandMeshInfo
            {
                vertices = new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(1, 1, 0),
                 },
                normals = new Vector3[]
                {
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                },
                triangles = new int[]
                {
                    0, 2, 1,
                    2, 3, 1
                }
            };
            var inputEventData = new InputEventData<HandMeshInfo>(EventSystem.current);
            inputEventData.Initialize(new MockInputSource(), Handedness.None, MixedRealityInputAction.None, handMeshInfo);
            return inputEventData;
        }
    }
}

#endif