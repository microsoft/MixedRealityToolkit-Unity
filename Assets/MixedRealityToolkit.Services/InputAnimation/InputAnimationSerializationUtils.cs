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

        const long Magic = 0x6a8faf6e0f9e42c6;

        public const int VersionMajor = 1;
        public const int VersionMinor = 0;

        /// <summary>
        /// Generate a file name for export.
        /// </summary>
        public static string GetOutputFilename(string baseName="InputAnimation", bool appendTimestamp=true)
        {
            string filename;
            if (appendTimestamp)
            {
                filename = String.Format("{0}-{1}.{2}", baseName, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"), InputAnimationSerializationUtils.Extension);
            }
            else
            {
                filename = baseName;
            }
            return filename;
        }

        /// <summary>
        /// Write a header for the input animation file format into the stream.
        /// </summary>
        public static void WriteHeader(BinaryWriter writer)
        {
            writer.Write(Magic);
            writer.Write(VersionMajor);
            writer.Write(VersionMinor);
        }

        /// <summary>
        /// Write a header for the input animation file format into the stream.
        /// </summary>
        public static void ReadHeader(BinaryReader reader, out int fileVersionMajor, out int fileVersionMinor)
        {
            long fileMagic = reader.ReadInt64();
            if (fileMagic != Magic)
            {
                throw new Exception("File is not an input animation file");
            }

            fileVersionMajor = reader.ReadInt32();
            fileVersionMinor = reader.ReadInt32();
        }

        /// <summary>
        /// Serialize an animation curve with tangents as binary data.
        /// </summary>
        public static void WriteFloatCurve(BinaryWriter writer, AnimationCurve curve, float startTime)
        {
            writer.Write((int)curve.preWrapMode);
            writer.Write((int)curve.postWrapMode);

            writer.Write(curve.length);
            for (int i = 0; i < curve.length; ++i)
            {
                var keyframe = curve.keys[i];
                writer.Write(keyframe.time - startTime);
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
        public static void WriteBoolCurve(BinaryWriter writer, AnimationCurve curve, float startTime)
        {
            writer.Write((int)curve.preWrapMode);
            writer.Write((int)curve.postWrapMode);

            writer.Write(curve.length);
            for (int i = 0; i < curve.length; ++i)
            {
                var keyframe = curve.keys[i];
                writer.Write(keyframe.time - startTime);
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
        public static void WriteFloatCurveArray(BinaryWriter writer, AnimationCurve[] curves, float startTime)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.WriteFloatCurve(writer, curve, startTime);
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
        public static void WriteBoolCurveArray(BinaryWriter writer, AnimationCurve[] curves, float startTime)
        {
            foreach (AnimationCurve curve in curves)
            {
                InputAnimationSerializationUtils.WriteBoolCurve(writer, curve, startTime);
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
        public static void WriteMarkerList(BinaryWriter writer, List<InputAnimationMarker> markers, float startTime)
        {
            writer.Write(markers.Count);
            foreach (var marker in markers)
            {
                writer.Write(marker.time - startTime);
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