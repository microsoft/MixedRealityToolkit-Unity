// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

using Utils = Microsoft.MixedReality.Toolkit.Input.InputAnimationGltfUtils;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility class for exporting input animation data in glTF format.
    /// </summary>
    /// <remarks>
    /// Input animation curves are converted into animation data. The camera as well as each hand joint included in the
    /// animation is represented as a node in the glTF file.
    /// </remarks>
    public static class InputAnimationGltfImporter
    {
        /// <summary>
        /// Utility class to preprocess a GltfObject and find relevant data to convert into InputAnimation.
        /// </summary>
        internal class InputAnimationGltfContent
        {
            public GltfObject gltfObject;

            /// The animation used for input
            public GltfAnimation gltfAnim;

            /// Combined node index and animation channel index
            public class NodeInfo
            {
                public int Node { get; private set; } = -1;
                public GltfAnimationChannel PositionChannel { get; set; }
                public GltfAnimationChannel RotationChannel { get; set; }

                public bool Valid => Node >= 0;

                public bool TrySetNode(int node, string name)
                {
                    if (Valid)
                    {
                        Debug.LogWarning($"More than one {name} node found in glTF file, only the first node will be used.");
                        return false;
                    }

                    this.Node = node;
                    return true;
                }
            }

            /// Main camera node index
            public NodeInfo camera = new NodeInfo();

            public class HandInfo
            {
                /// Nodes for tracking and pinching state
                public NodeInfo tracking = new NodeInfo();
                public NodeInfo pinching = new NodeInfo();
                /// Nodes used for hand joints
                public Dictionary<TrackedHandJoint, NodeInfo> jointMap = new Dictionary<TrackedHandJoint, NodeInfo>();
            }
            public Dictionary<Handedness, HandInfo> handMap = new Dictionary<Handedness, HandInfo>()
            {
                { Handedness.Left, new HandInfo() },
                { Handedness.Right, new HandInfo() },
            };

            public bool TryParseGltfObject(GltfObject gltfObject)
            {
                this.gltfObject = gltfObject;
                if (!TryFindAnimation(gltfObject))
                {
                    return false;
                }
                if (!TryFindNodes(gltfObject))
                {
                    return false;
                }

                FindAnimationChannels();

                return true;
            }

            private bool TryFindAnimation(GltfObject gltfObject)
            {
                if (gltfObject.animations.Length == 0)
                {
                    Debug.LogError("Cannot import input animation, no animations found in glTF file.");
                    return false;
                }

                if (gltfObject.animations.Length > 1)
                {
                    Debug.LogWarning("More than one animation found in glTF file, only the first animation will be used for input.");
                }

                gltfAnim = gltfObject.animations[0];
                return true;
            }

            private bool TryFindNodes(GltfObject gltfObject)
            {
                if (gltfObject.cameras.Length == 0)
                {
                    Debug.LogError("Cannot import input animation, no cameras found in glTF file.");
                    return false;
                }

                for (int i = 0; i < gltfObject.nodes.Length; ++i)
                {
                    var node = gltfObject.nodes[i];

                    if (node.camera >= 0)
                    {
                        camera.TrySetNode(i, "camera");
                    }

                    Handedness handedness;
                    TrackedHandJoint joint;
                    if (Utils.TryParseTrackingNodeName(node.name, out handedness))
                    {
                        GetOrCreateHandInfo(handedness).tracking.TrySetNode(i, node.name);
                    }
                    if (Utils.TryParsePinchingNodeName(node.name, out handedness))
                    {
                        GetOrCreateHandInfo(handedness).pinching.TrySetNode(i, node.name);
                    }
                    if (Utils.TryParseJointNodeName(node.name, out handedness, out joint))
                    {
                        GetOrCreateJointInfo(handedness, joint).TrySetNode(i, node.name);
                    }
                }

                return camera.Valid;
            }

            private HandInfo GetOrCreateHandInfo(Handedness handedness)
            {
                if (!handMap.TryGetValue(handedness, out HandInfo handInfo))
                {
                    handInfo = new HandInfo();
                    handMap.Add(handedness, handInfo);
                }
                return handInfo;
            }

            private NodeInfo GetOrCreateJointInfo(Handedness handedness, TrackedHandJoint joint)
            {
                HandInfo handInfo = GetOrCreateHandInfo(handedness);

                if (!handInfo.jointMap.TryGetValue(joint, out NodeInfo nodeInfo))
                {
                    nodeInfo = new NodeInfo();
                    handInfo.jointMap.Add(joint, nodeInfo);
                }
                return nodeInfo;
            }

            private void FindAnimationChannels()
            {
                // Temp. dictionary for looking up and setting the joint animation channel from the node index
                var nodeIndexLookup = new Dictionary<int, NodeInfo>();

                nodeIndexLookup[camera.Node] = camera;

                foreach (var handInfo in handMap.Values)
                {
                    nodeIndexLookup[handInfo.tracking.Node] = handInfo.tracking;
                    nodeIndexLookup[handInfo.pinching.Node] = handInfo.pinching;
                    foreach (var jointInfo in handInfo.jointMap.Values)
                    {
                        nodeIndexLookup[jointInfo.Node] = jointInfo;
                    }
                }

                foreach (GltfAnimationChannel channel in gltfAnim.channels)
                {
                    int node = channel.target.node;

                    if (nodeIndexLookup.TryGetValue(node, out NodeInfo nodeInfo))
                    {
                        switch (channel.target.path)
                        {
                            case GltfAnimationChannelPath.translation:
                                nodeInfo.PositionChannel = channel;
                                break;
                            case GltfAnimationChannelPath.rotation:
                                nodeInfo.RotationChannel = channel;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Serialize the given input animation and save it at the given path.
        /// </summary>
        public static async Task<InputAnimation> OnImportInputAnimation(string path)
        {
            GltfObject importedObject = await GltfUtility.ImportGltfObjectFromPathAsync(path, false);
            if (importedObject == null)
            {
                return null;
            }

            var content = new InputAnimationGltfContent();
            if (!content.TryParseGltfObject(importedObject))
            {
                return null;
            }

            InputAnimation anim = new InputAnimation();

            ImportPoseCurves(content, content.camera, anim.CameraCurves);

            foreach (var handItem in content.handMap)
            {
                switch (handItem.Key)
                {
                    case Handedness.Left:
                        ImportBoolCurve(content, handItem.Value.tracking, anim.HandTrackedCurveLeft);
                        ImportBoolCurve(content, handItem.Value.pinching, anim.HandPinchCurveLeft);
                        break;
                    case Handedness.Right:
                        ImportBoolCurve(content, handItem.Value.tracking, anim.HandTrackedCurveRight);
                        ImportBoolCurve(content, handItem.Value.pinching, anim.HandPinchCurveRight);
                        break;
                }

                foreach (var jointItem in handItem.Value.jointMap)
                {
                    var poseCurves = anim.CreateHandJointCurves(handItem.Key, jointItem.Key);
                    ImportPoseCurves(content, jointItem.Value, poseCurves);
                }
            }

            anim.ComputeDuration();

            return anim;
        }

        private static void ImportPoseCurves(InputAnimationGltfContent content, InputAnimationGltfContent.NodeInfo nodeInfo, InputAnimation.PoseCurves poseCurvesOut)
        {
            if (nodeInfo.PositionChannel != null)
            {
                TryImportCurves(content, nodeInfo.PositionChannel,
                    new AnimationCurve[] { poseCurvesOut.PositionX, poseCurvesOut.PositionY, poseCurvesOut.PositionZ });
            }
            if (nodeInfo.RotationChannel != null)
            {
                TryImportCurves(content, nodeInfo.RotationChannel,
                    new AnimationCurve[] { poseCurvesOut.RotationX, poseCurvesOut.RotationY, poseCurvesOut.RotationZ, poseCurvesOut.RotationW });
            }
        }

        private static void ImportBoolCurve(InputAnimationGltfContent content, InputAnimationGltfContent.NodeInfo nodeInfo, AnimationCurve curveOut)
        {
            if (nodeInfo.PositionChannel != null)
            {
                TryImportCurves(content, nodeInfo.PositionChannel, new AnimationCurve[] { curveOut, null, null });
            }
        }

        private static int GetAccessorByteSize(GltfAccessor accessor)
        {
            int stride = GetNumComponents(accessor.type) * GetComponentSize(accessor.componentType);
            return stride * accessor.count;
        }

        private static int GetNumComponents(string type)
        {
            switch (type)
            {
                case "SCALAR": return 1;
                case "VEC2": return 2;
                case "VEC3": return 3;
                case "VEC4": return 4;
                case "MAT2": return 4;
                case "MAT3": return 9;
                case "MAT4": return 16;
            }
            return 0;
        }

        private static int GetComponentSize(GltfComponentType type)
        {
            switch (type)
            {
                case GltfComponentType.Byte: return 1;
                case GltfComponentType.UnsignedByte: return 1;
                case GltfComponentType.Short: return 2;
                case GltfComponentType.UnsignedShort: return 2;
                case GltfComponentType.UnsignedInt: return 4;
                case GltfComponentType.Float : return 4;
            }
            return 0;
        }

        /// Arbitrarily large weight for representing a boolean value in float curves.
        const float boolOutWeight = 1.0e6f;

        private static bool TryImportCurves(InputAnimationGltfContent content, GltfAnimationChannel gltfChannel, AnimationCurve[] curves)
        {
            GltfAnimationSampler sampler = content.gltfAnim.samplers[gltfChannel.sampler];
            GltfAccessor accTime = content.gltfObject.accessors[sampler.input];
            GltfAccessor accValue = content.gltfObject.accessors[sampler.output];
            byte[] bufferData = content.gltfObject.buffers[0].BufferData;

            int numComponents = GetNumComponents(accValue.type);
            if (curves.Length != numComponents)
            {
                Debug.LogWarning($"Expected {curves.Length} components in output accessor for animation sampler {gltfChannel.sampler}, cannot import animation curves.");
                return false;
            }
            if (accTime.componentType != GltfComponentType.Float)
            {
                Debug.LogWarning($"{accValue.componentType} component type in input accessor for animation sampler {gltfChannel.sampler} is not supported, cannot import animation curves.");
                return false;
            }
            if (accValue.componentType != GltfComponentType.Float)
            {
                Debug.LogWarning($"{accValue.componentType} component type in output accessor for animation sampler {gltfChannel.sampler} is not supported, cannot import animation curves.");
                return false;
            }
            if (sampler.interpolation != GltfInterpolationType.STEP && sampler.interpolation != GltfInterpolationType.LINEAR)
            {
                Debug.LogWarning($"{sampler.interpolation} interpolation type in animation sampler {gltfChannel.sampler} is not supported, will use linear interpolation.");
            }

            var timeStream = new MemoryStream(bufferData, accTime.byteOffset, GetAccessorByteSize(accTime), false);
            var valueStream = new MemoryStream(bufferData, accValue.byteOffset, GetAccessorByteSize(accValue), false);
            var timeReader = new BinaryReader(timeStream);
            var valueReader = new BinaryReader(valueStream);

            Keyframe[][] curveKeys = new Keyframe[numComponents][];
            for (int c = 0; c < numComponents; ++c)
            {
                curveKeys[c] = new Keyframe[accTime.count];
            }
            for (int i = 0; i < accTime.count; ++i)
            {
                float time = timeReader.ReadSingle();

                for (int c = 0; c < numComponents; ++c)
                {
                    ref Keyframe key = ref curveKeys[c][i];
                    key.time = time;
                    key.value = valueReader.ReadSingle();

                    switch (sampler.interpolation)
                    {
                        case GltfInterpolationType.STEP:
                            key.weightedMode = WeightedMode.Out;
                            key.inTangent = 0.0f;
                            key.outTangent = 0.0f;
                            key.inWeight = 0.0f;
                            key.outWeight = boolOutWeight;
                            break;
                        case GltfInterpolationType.CUBICSPLINE:
                        case GltfInterpolationType.CATMULLROMSPLINE:
                        case GltfInterpolationType.LINEAR:
                            key.weightedMode = WeightedMode.Both;
                            key.inTangent = 0.0f;
                            key.outTangent = 0.0f;
                            key.inWeight = 0.0f;
                            key.outWeight = 0.0f;
                            break;
                    }
                }
            }

            for (int c = 0; c < numComponents; ++c)
            {
                if (curves[c] != null)
                {
                    curves[c].keys = curveKeys[c];
                    curves[c].postWrapMode = WrapMode.Clamp;
                    curves[c].preWrapMode = WrapMode.Clamp;
                }
            }

            return true;
        }
    }
}
