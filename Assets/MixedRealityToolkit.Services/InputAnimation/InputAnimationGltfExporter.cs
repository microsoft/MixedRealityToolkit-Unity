// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
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
    public static class InputAnimationGltfExporter
    {
        /// <summary>
        /// Serialize the given input animation and save it at the given path.
        /// </summary>
        public static async void OnExportInputAnimation(InputAnimation input, string path, MixedRealityInputRecordingProfile profile)
        {
            GltfObject exportedObject;
            using (var builder = new GltfObjectBuilder(profile?.LicenseString, Utils.GeneratorString))
            {
                int scene = builder.CreateScene(Utils.SceneName, true);

                int camera;
                if (CameraCache.Main)
                {
                    var cameraData = CameraCache.Main;
                    camera = builder.CreateCameraPerspective(Utils.CameraName, cameraData.aspect, cameraData.fieldOfView, cameraData.nearClipPlane, cameraData.farClipPlane);
                }
                else
                {
                    camera = builder.CreateCameraPerspective(Utils.CameraName, 4.0/3.0, 55.0, 0.1, 100.0);
                }

                CreateAnimation(builder, input, camera);

                exportedObject = builder.Build();
            }

            await GltfUtility.ExportGltfObjectToPathAsync(exportedObject, path);
        }

        /// Create an animation from input data and return its index.
        private static int CreateAnimation(GltfObjectBuilder builder, InputAnimation input, int camera)
        {
            using (var animBuilder = new GltfAnimationBuilder(builder, Utils.AnimationName))
            {
                int cameraNode = builder.CreateRootNode(Utils.CameraName, Vector3.zero, Quaternion.identity, Vector3.one, 0, camera);
                CreatePoseAnimation(animBuilder, input.CameraCurves, GltfInterpolationType.LINEAR, cameraNode);

                int leftHandNode = builder.CreateRootNode(Utils.GetHandNodeName(Handedness.Left), Vector3.zero, Quaternion.identity, Vector3.one, 0);
                int rightHandNode = builder.CreateRootNode(Utils.GetHandNodeName(Handedness.Right), Vector3.zero, Quaternion.identity, Vector3.one, 0);

                int leftTrackingNode = builder.CreateChildNode(Utils.GetTrackingNodeName(Handedness.Left), Vector3.zero, Quaternion.identity, Vector3.one, leftHandNode);
                int rightTrackingNode = builder.CreateChildNode(Utils.GetTrackingNodeName(Handedness.Right), Vector3.zero, Quaternion.identity, Vector3.one, rightHandNode);
                CreateBoolAnimation(animBuilder, input.HandTrackedCurveLeft, leftTrackingNode);
                CreateBoolAnimation(animBuilder, input.HandTrackedCurveRight, rightTrackingNode);

                int leftPinchingNode = builder.CreateChildNode(Utils.GetPinchingNodeName(Handedness.Left), Vector3.zero, Quaternion.identity, Vector3.one, leftHandNode);
                int rightPinchingNode = builder.CreateChildNode(Utils.GetPinchingNodeName(Handedness.Right), Vector3.zero, Quaternion.identity, Vector3.one, rightHandNode);
                CreateBoolAnimation(animBuilder, input.HandPinchCurveLeft, leftPinchingNode);
                CreateBoolAnimation(animBuilder, input.HandPinchCurveRight, rightPinchingNode);

                int leftJointsNode = builder.CreateChildNode(Utils.GetHandJointsNodeName(Handedness.Left), Vector3.zero, Quaternion.identity, Vector3.one, leftHandNode);
                int rightJointsNode = builder.CreateChildNode(Utils.GetHandJointsNodeName(Handedness.Right), Vector3.zero, Quaternion.identity, Vector3.one, rightHandNode);
                foreach (var joint in Utils.TrackedHandJointValues)
                {
                    string jointName = Utils.TrackedHandJointNames[(int)joint];
    
                    InputAnimation.PoseCurves jointCurves;
                    if (input.TryGetHandJointCurves(Handedness.Left, joint, out jointCurves))
                    {
                        int leftJointNode = builder.CreateChildNode(Utils.GetJointNodeName(Handedness.Left, joint), Vector3.zero, Quaternion.identity, Vector3.one, leftJointsNode);
                        CreatePoseAnimation(animBuilder, jointCurves, GltfInterpolationType.LINEAR, leftJointNode);
                    }
                    if (input.TryGetHandJointCurves(Handedness.Right, joint, out jointCurves))
                    {
                        int rightJointNode = builder.CreateChildNode(Utils.GetJointNodeName(Handedness.Right, joint), Vector3.zero, Quaternion.identity, Vector3.one, rightJointsNode);
                        CreatePoseAnimation(animBuilder, jointCurves, GltfInterpolationType.LINEAR, rightJointNode);
                    }
                }

                return animBuilder.Index;
            }
        }

        private static void CreatePoseAnimation(GltfAnimationBuilder builder, InputAnimation.PoseCurves poseCurves, GltfInterpolationType interpolation, int node)
        {
            var positionCurves = new AnimationCurve[] { poseCurves.PositionX, poseCurves.PositionY, poseCurves.PositionZ };
            var rotationCurves = new AnimationCurve[] { poseCurves.RotationX, poseCurves.RotationY, poseCurves.RotationZ, poseCurves.RotationW };
            builder.CreateTranslationAnimation(positionCurves, interpolation, node);
            builder.CreateRotationAnimation(rotationCurves, interpolation, node);
        }

        // Store a boolean animation as a translation curve.
        // glTF 2.0 does not support custom animation targets other than translation/rotation/scale/weights.
        private static void CreateBoolAnimation(GltfAnimationBuilder builder, AnimationCurve curve, int node)
        {
            var positionCurves = new AnimationCurve[] { curve, null, null };
            builder.CreateTranslationAnimation(positionCurves, GltfInterpolationType.STEP, node);
        }
    }
}