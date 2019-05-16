// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class InputAnimationTarget : UnityEngine.Object
    {
        // TODO
    }

    public static class InputAnimationSerializationUtils
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        public const string Extension = "bin";

        /// <summary>
        /// Serialize an animation curve as binary data.
        /// </summary>
        public static void WriteFloatCurve(BinaryWriter writer, AnimationCurve curve)
        {
            writer.Write((int)curve.preWrapMode);
            writer.Write((int)curve.postWrapMode);

            writer.Write(curve.length);
            for (int i = 0; i < curve.length; ++i)
            {
                var keyframe = curve.keys[i];
                writer.Write(keyframe.time);
                writer.Write(keyframe.value);
                writer.Write(keyframe.inTangent);
                writer.Write(keyframe.outTangent);
                writer.Write(keyframe.inWeight);
                writer.Write(keyframe.outWeight);
                writer.Write((int)keyframe.weightedMode);
            }
        }

        /// <summary>
        /// Serialize an animation curve as binary data.
        /// </summary>
        public static void WriteBoolCurve(BinaryWriter writer, AnimationCurve curve)
        {
            writer.Write((int)curve.preWrapMode);
            writer.Write((int)curve.postWrapMode);

            writer.Write(curve.length);
            for (int i = 0; i < curve.length; ++i)
            {
                var keyframe = curve.keys[i];
                writer.Write(keyframe.time);
                writer.Write(keyframe.value);
            }
        }

        /// <summary>
        /// Deserialize an animation curve from binary data.
        /// </summary>
        public static void ReadFloatCurve(BinaryReader reader, AnimationCurve curve)
        {
            curve.preWrapMode = (WrapMode)reader.ReadInt32();
            curve.postWrapMode = (WrapMode)reader.ReadInt32();

            int keyframeCount = reader.ReadInt32();

            Keyframe[] keys = new Keyframe[keyframeCount];
            for (int i = 0; i < keyframeCount; ++i)
            {
                keys[i].time = reader.ReadSingle();
                keys[i].value = reader.ReadSingle();
                keys[i].inTangent = reader.ReadSingle();
                keys[i].outTangent = reader.ReadSingle();
                keys[i].inWeight = reader.ReadSingle();
                keys[i].outWeight = reader.ReadSingle();
                keys[i].weightedMode = (WeightedMode)reader.ReadInt32();
            }

            curve.keys = keys;
        }

        /// <summary>
        /// Deserialize an animation curve from binary data.
        /// </summary>
        public static void ReadBoolCurve(BinaryReader reader, AnimationCurve curve)
        {
            curve.preWrapMode = (WrapMode)reader.ReadInt32();
            curve.postWrapMode = (WrapMode)reader.ReadInt32();

            int keyframeCount = reader.ReadInt32();

            Keyframe[] keys = new Keyframe[keyframeCount];
            for (int i = 0; i < keyframeCount; ++i)
            {
                keys[i].time = reader.ReadSingle();
                keys[i].value = reader.ReadSingle();
                keys[i].inTangent = 0.0f;
                keys[i].outTangent = 0.0f;
                keys[i].inWeight = 0.0f;
                keys[i].outWeight = 1.0e6f;
                keys[i].weightedMode = WeightedMode.Both;
            }

            curve.keys = keys;
        }

// AnimationClip modification is only supported in the editor
#if UNITY_EDITOR

        public static void AnimationClipToStream(AnimationClip clip, Stream stream)
        {
            var writer = new BinaryWriter(stream);

            WriteBoolCurve(clip, writer, "Hand.Left.IsTracked");
            WriteBoolCurve(clip, writer, "Hand.Right.IsTracked");
            WriteBoolCurve(clip, writer, "Hand.Left.IsPinching");
            WriteBoolCurve(clip, writer, "Hand.Right.IsPinching");
            WriteJointCurves(clip, writer, "Hand.Left.Joints");
            WriteJointCurves(clip, writer, "Hand.Right.Joints");
        }

        public static void AnimationClipFromStream(AnimationClip clip, Stream stream)
        {
            var reader = new BinaryReader(stream);

            ReadBoolCurve(clip, reader, "Hand.Left.IsTracked");
            ReadBoolCurve(clip, reader, "Hand.Right.IsTracked");
            ReadBoolCurve(clip, reader, "Hand.Left.IsPinching");
            ReadBoolCurve(clip, reader, "Hand.Right.IsPinching");
            ReadJointCurves(clip, reader, "Hand.Left.Joints");
            ReadJointCurves(clip, reader, "Hand.Right.Joints");
        }

        private static void WriteJointCurves(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            for (int i = 0; i < jointCount; ++i)
            {
                WritePoseCurves(clip, writer, propertyName + $"[{i}]");
            }
        }

        private static void ReadJointCurves(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            for (int i = 0; i < jointCount; ++i)
            {
                ReadPoseCurves(clip, reader, propertyName + $"[{i}]");
            }
        }

        private static void WritePoseCurves(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            WriteFloatCurve(clip, writer, propertyName + ".Position.x");
            WriteFloatCurve(clip, writer, propertyName + ".Position.y");
            WriteFloatCurve(clip, writer, propertyName + ".Position.z");

            WriteFloatCurve(clip, writer, propertyName + ".Rotation.x");
            WriteFloatCurve(clip, writer, propertyName + ".Rotation.y");
            WriteFloatCurve(clip, writer, propertyName + ".Rotation.z");
            WriteFloatCurve(clip, writer, propertyName + ".Rotation.w");
        }

        private static void ReadPoseCurves(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            ReadFloatCurve(clip, reader, propertyName + ".Position.x");
            ReadFloatCurve(clip, reader, propertyName + ".Position.y");
            ReadFloatCurve(clip, reader, propertyName + ".Position.z");

            ReadFloatCurve(clip, reader, propertyName + ".Rotation.x");
            ReadFloatCurve(clip, reader, propertyName + ".Rotation.y");
            ReadFloatCurve(clip, reader, propertyName + ".Rotation.z");
            ReadFloatCurve(clip, reader, propertyName + ".Rotation.w");
        }

        private static string bindingPath = "";
        private static Type bindingType = typeof(InputAnimationTarget);

        private static void WriteFloatCurve(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            var binding = EditorCurveBinding.FloatCurve(bindingPath, bindingType, propertyName);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            WriteFloatCurve(writer, curve);
        }

        private static void WriteBoolCurve(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            var binding = EditorCurveBinding.DiscreteCurve(bindingPath, bindingType, propertyName);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            WriteBoolCurve(writer, curve);
        }

        private static void ReadFloatCurve(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            var curve = new AnimationCurve();
            ReadFloatCurve(reader, curve);

            var binding = EditorCurveBinding.FloatCurve(bindingPath, bindingType, propertyName);
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        private static void ReadBoolCurve(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            var curve = new AnimationCurve();
            ReadBoolCurve(reader, curve);

            var binding = EditorCurveBinding.DiscreteCurve(bindingPath, bindingType, propertyName);
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

#endif
    }
}