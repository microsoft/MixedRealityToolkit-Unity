// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Functions for importing/exporting input animation data as Unity AnimationClip.
    /// </summary>
    /// <remarks>
    /// AnimationClip modification is only supported in the editor.
    /// </remarks>
    public static class InputAnimationImportExportUtils
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Serialize an AnimationClip as binary input animation data.
        /// </summary>
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

        /// <summary>
        /// Deserialize binary input animation data into an AnimationClip.
        /// </summary>
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

        /// Serialize all animation curves for hand joints.
        /// Handedness is encoded in the propertyName string.
        private static void WriteJointCurves(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            for (int i = 0; i < jointCount; ++i)
            {
                WritePoseCurves(clip, writer, propertyName + jointNames[i]);
            }
        }

        /// Deserialize all animation curves for hand joints.
        /// Handedness is encoded in the propertyName string.
        private static void ReadJointCurves(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            for (int i = 0; i < jointCount; ++i)
            {
                ReadPoseCurves(clip, reader, propertyName + jointNames[i]);
            }
        }

        /// Serialize all animation curves of a MixedRealityPose.
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

        /// Deserialize all animation curves of a MixedRealityPose.
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
        // InputAnimationTarget serves as a binding type for AnimationClip,
        // with concrete serializable fields for each of the input properties.
        private static Type bindingType = typeof(InputAnimationTarget);

        /// Find a float curve in the AnimationClip for the given binding and serialize it.
        private static void WriteFloatCurve(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            var binding = EditorCurveBinding.FloatCurve(bindingPath, bindingType, propertyName);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            // Curve may not exist in the animation clip, use a fallback
            InputAnimationSerializationUtils.WriteFloatCurve(writer, curve ?? new AnimationCurve());
        }

        /// Deserialize a float curve and insert it in the AnimationClip with the given binding.
        private static void ReadFloatCurve(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            var curve = new AnimationCurve();
            InputAnimationSerializationUtils.ReadFloatCurve(reader, curve);

            var binding = EditorCurveBinding.FloatCurve(bindingPath, bindingType, propertyName);
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        /// Find a bool curve in the AnimationClip for the given binding and serialize it.
        private static void WriteBoolCurve(AnimationClip clip, BinaryWriter writer, string propertyName)
        {
            var binding = EditorCurveBinding.DiscreteCurve(bindingPath, bindingType, propertyName);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            // Curve may not exist in the animation clip, use a fallback
            InputAnimationSerializationUtils.WriteBoolCurve(writer, curve ?? new AnimationCurve());
        }

        /// Deserialize a bool curve and insert it in the AnimationClip with the given binding.
        private static void ReadBoolCurve(AnimationClip clip, BinaryReader reader, string propertyName)
        {
            var curve = new AnimationCurve();
            InputAnimationSerializationUtils.ReadBoolCurve(reader, curve);

            var binding = EditorCurveBinding.DiscreteCurve(bindingPath, bindingType, propertyName);
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        /// Serialize animation events as markers.
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

        /// Deserialize markers into animation events.
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

    }
}
