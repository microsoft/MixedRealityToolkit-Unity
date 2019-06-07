// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Functions for serializing input animation data to and from binary files.
    /// </summary>
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

        /// <summary>
        /// Serialize an array of animation curves with tangents as binary data.
        /// </summary>
        public static void WriteFloatCurveArray(BinaryWriter writer, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.WriteFloatCurve(writer, curve);
            }
        }

        /// <summary>
        /// Deserialize an array of animation curves with tangents from binary data.
        /// </summary>
        public static void ReadFloatCurveArray(BinaryReader reader, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.ReadFloatCurve(reader, curve);
            }
        }

        /// <summary>
        /// Serialize an array of animation curves as binary data, ignoring tangents.
        /// </summary>
        public static void WriteBoolCurveArray(BinaryWriter writer, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.WriteBoolCurve(writer, curve);
            }
        }

        /// <summary>
        /// Deserialize an array of animation curves from binary data, ignoring tangents.
        /// </summary>
        public static void ReadBoolCurveArray(BinaryReader reader, AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.ReadBoolCurve(reader, curve);
            }
        }

        /// <summary>
        /// Serialize a list of markers.
        /// </summary>
        public static void WriteMarkerList(BinaryWriter writer, List<InputAnimationMarker> markers)
        {
            writer.Write(markers.Count);
            foreach (var marker in markers)
            {
                writer.Write(marker.time);
                writer.Write(marker.name);
            }
        }

        /// <summary>
        /// Deserialize a list of markers.
        /// </summary>
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
    }
}