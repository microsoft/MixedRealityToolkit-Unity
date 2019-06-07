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
    public static class InputAnimationSerializationUtils
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        public const string Extension = "bin";

        /// <summary>
        /// Serialize an animation curve with tangents as binary data.
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
        /// Deserialize an animation curve with tangents from binary data.
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
        /// Serialize an animation curve as binary data, ignoring tangents.
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
        /// Deserialize an animation curve from binary data, ignoring tangents.
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

        public static void WriteFloatCurveArray(BinaryWriter writer, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.WriteFloatCurve(writer, curve);
            }
        }

        public static void ReadFloatCurveArray(BinaryReader reader, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.ReadFloatCurve(reader, curve);
            }
        }

        public static void WriteBoolCurveArray(BinaryWriter writer, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.WriteBoolCurve(writer, curve);
            }
        }

        public static void ReadBoolCurveArray(BinaryReader reader, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.ReadBoolCurve(reader, curve);
            }
        }

        public static void WriteMarkerList(BinaryWriter writer, List<InputAnimationMarker> markers)
        {
            writer.Write(markers.Count);
            foreach (var marker in markers)
            {
                writer.Write(marker.time);
                writer.Write(marker.name);
            }
        }

        public static void ReadMarkerList(BinaryReader reader, List<InputAnimationMarker> markers)
        {
            markers.Clear();
            int count = reader.ReadInt32();
            markers.Capacity = count;
            for (int i = 0; i < count; ++i)
            {
                var marker = new InputAnimationMarker();
                marker.time = reader.ReadSingle();
                marker.name = reader.ReadString();
                markers.Add(marker);
            }
        }

// AnimationClip modification is only supported in the editor
#if UNITY_EDITOR

        public static void AnimationClipToStream(AnimationClip clip, Stream stream)
        {
            var writer = new BinaryWriter(stream);

            WritePoseCurves(clip, writer, "cameraPose");
            WriteBoolCurve(clip, writer, "leftHand.isTracked");
            WriteBoolCurve(clip, writer, "rightHand.isTracked");
            WriteBoolCurve(clip, writer, "leftHand.isPinching");
            WriteBoolCurve(clip, writer, "rightHand.isPinching");
            WriteJointCurves(clip, writer, "leftHand.joint");
            WriteJointCurves(clip, writer, "rightHand.joint");
            WriteMarkerList(clip, writer);
        }

        public static void AnimationClipFromStream(AnimationClip clip, Stream stream)
        {
            var reader = new BinaryReader(stream);

            ReadPoseCurves(clip, reader, "cameraPose");
            ReadBoolCurve(clip, reader, "leftHand.isTracked");
            ReadBoolCurve(clip, reader, "rightHand.isTracked");
            ReadBoolCurve(clip, reader, "leftHand.isPinching");
            ReadBoolCurve(clip, reader, "rightHand.isPinching");
            ReadJointCurves(clip, reader, "leftHand.joint");
            ReadJointCurves(clip, reader, "rightHand.joint");
            ReadMarkerList(clip, reader);
        }

        private static string[] jointNames = Enum.GetNames(typeof(TrackedHandJoint));

        private static void WriteJointCurves(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            for (int i = 0; i < jointCount; ++i)
            {
                WritePoseCurves(clip, writer, propertyName + jointNames[i]);
            }
        }

        private static void ReadJointCurves(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            for (int i = 0; i < jointCount; ++i)
            {
                ReadPoseCurves(clip, reader, propertyName + jointNames[i]);
            }
        }

        private static void WritePoseCurves(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            WriteFloatCurve(clip, writer, propertyName + ".position.x");
            WriteFloatCurve(clip, writer, propertyName + ".position.y");
            WriteFloatCurve(clip, writer, propertyName + ".position.z");

            WriteFloatCurve(clip, writer, propertyName + ".rotation.x");
            WriteFloatCurve(clip, writer, propertyName + ".rotation.y");
            WriteFloatCurve(clip, writer, propertyName + ".rotation.z");
            WriteFloatCurve(clip, writer, propertyName + ".rotation.w");
        }

        private static void ReadPoseCurves(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            ReadFloatCurve(clip, reader, propertyName + ".position.x");
            ReadFloatCurve(clip, reader, propertyName + ".position.y");
            ReadFloatCurve(clip, reader, propertyName + ".position.z");

            ReadFloatCurve(clip, reader, propertyName + ".rotation.x");
            ReadFloatCurve(clip, reader, propertyName + ".rotation.y");
            ReadFloatCurve(clip, reader, propertyName + ".rotation.z");
            ReadFloatCurve(clip, reader, propertyName + ".rotation.w");
        }

        private static string bindingPath = "";
        private static Type bindingType = typeof(InputAnimationTarget);

        private static void WriteFloatCurve(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            var binding = EditorCurveBinding.FloatCurve(bindingPath, bindingType, propertyName);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            WriteFloatCurve(writer, curve ?? new AnimationCurve());
        }

        private static void ReadFloatCurve(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            var curve = new AnimationCurve();
            ReadFloatCurve(reader, curve);

            var binding = EditorCurveBinding.FloatCurve(bindingPath, bindingType, propertyName);
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        private static void WriteBoolCurve(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            var binding = EditorCurveBinding.DiscreteCurve(bindingPath, bindingType, propertyName);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            WriteBoolCurve(writer, curve ?? new AnimationCurve());
        }

        private static void ReadBoolCurve(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            var curve = new AnimationCurve();
            ReadBoolCurve(reader, curve);

            var binding = EditorCurveBinding.DiscreteCurve(bindingPath, bindingType, propertyName);
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        private static void WriteMarkerList(AnimationClip clip, BinaryWriter writer)
        {
            AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);
            writer.Write(events.Length);
            foreach (var evt in events)
            {
                writer.Write(evt.time);
                writer.Write(evt.stringParameter);
            }
        }

        private static void ReadMarkerList(AnimationClip clip, BinaryReader reader)
        {
            int count = reader.ReadInt32();
            AnimationEvent[] events = new AnimationEvent[count];
            for (int i = 0; i < count; ++i)
            {
                events[i].time = reader.ReadSingle();
                events[i].stringParameter = reader.ReadString();
            }
            AnimationUtility.SetAnimationEvents(clip, events);
        }

#endif
    }
}