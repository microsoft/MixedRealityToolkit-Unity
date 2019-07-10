// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public static class InputAnimationGltfUtils
    {
        public const string Extension = "glb";

        public const string GeneratorString = "Microsoft.MixedReality.Toolkit.InputAnimation-1.0";

        public const string SceneName = "Scene";
        public const string CameraName = "Camera";
        public const string AnimationName = "InputAction";

        public static readonly string[] TrackedHandJointNames = Enum.GetNames(typeof(TrackedHandJoint));
        public static readonly TrackedHandJoint[] TrackedHandJointValues = (TrackedHandJoint[])Enum.GetValues(typeof(TrackedHandJoint));
        private static readonly Dictionary<string, TrackedHandJoint> TrackedHandJointMap = TrackedHandJointValues.ToDictionary(j => Enum.GetName(typeof(TrackedHandJoint), j));

        public static readonly Handedness[] HandednessValues = (Handedness[])Enum.GetValues(typeof(Handedness));
        private static readonly Dictionary<string, Handedness> HandednessMap = HandednessValues.ToDictionary(h => Enum.GetName(typeof(Handedness), h));

        /// <summary>
        /// Generate a file name for export.
        /// </summary>
        public static string GetOutputFilename(string baseName="InputAnimation", bool appendTimestamp=true)
        {
            string filename;
            if (appendTimestamp)
            {
                filename = String.Format("{0}-{1}.{2}", baseName, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"), Extension);
            }
            else
            {
                filename = baseName;
            }
            return filename;
        }

        public static string GetHandNodeName(Handedness handedness)
        {
            return $"Hand.{handedness}";
        }

        public static string GetTrackingNodeName(Handedness handedness)
        {
            return $"Hand.{handedness}.Tracking";
        }

        public static bool TryParseTrackingNodeName(string name, out Handedness handedness)
        {
            string[] parts = name.Split('.');
            if (parts.Length == 3 && parts[0] == "Hand" && parts[2] == "Tracking")
            {
                if (HandednessMap.TryGetValue(parts[1], out handedness))
                {
                    return true;
                }
            }
            handedness = Handedness.None;
            return false;
        }

        public static string GetPinchingNodeName(Handedness handedness)
        {
            return $"Hand.{handedness}.Pinching";
        }

        public static bool TryParsePinchingNodeName(string name, out Handedness handedness)
        {
            string[] parts = name.Split('.');
            if (parts.Length == 3 && parts[0] == "Hand" && parts[2] == "Pinching")
            {
                if (HandednessMap.TryGetValue(parts[1], out handedness))
                {
                    return true;
                }
            }
            handedness = Handedness.None;
            return false;
        }

        public static string GetHandJointsNodeName(Handedness handedness)
        {
            return $"Hand.{handedness}.Joints";
        }

        public static string GetJointNodeName(Handedness handedness, TrackedHandJoint joint)
        {
            return $"Hand.{handedness}.Joint.{joint}";
        }

        public static bool TryParseJointNodeName(string name, out Handedness handedness, out TrackedHandJoint joint)
        {
            string[] parts = name.Split('.');
            if (parts.Length == 4 && parts[0] == "Hand" && parts[2] == "Joint")
            {
                if (HandednessMap.TryGetValue(parts[1], out handedness) && TrackedHandJointMap.TryGetValue(parts[3], out joint))
                {
                    return true;
                }
            }
            handedness = Handedness.None;
            joint = TrackedHandJoint.None;
            return false;
        }
    }
}