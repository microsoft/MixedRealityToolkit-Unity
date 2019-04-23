// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// 
    /// </summary>
    public static class MixedRealityPlayspace
    {
        private const string Name = "MixedRealityPlayspace";

        private static Transform mixedRealityPlayspace;

        /// <summary>
        /// 
        /// </summary>
        public static Transform Transform
        {
            get
            {
                if (mixedRealityPlayspace)
                {
                    return mixedRealityPlayspace;
                }

                if (CameraCache.Main.transform.parent == null)
                {
                    mixedRealityPlayspace = new GameObject(Name).transform;
                    CameraCache.Main.transform.SetParent(mixedRealityPlayspace);
                }
                else
                {
                    if (CameraCache.Main.transform.parent.name != Name)
                    {
                        // Since the scene is set up with a different camera parent, its likely
                        // that there's an expectation that that parent is going to be used for
                        // something else. We print a warning to call out the fact that we're 
                        // co-opting this object for use with teleporting and such, since that
                        // might cause conflicts with the parent's intended purpose.
                        Debug.LogWarning($"The Mixed Reality Toolkit expected the camera\'s parent to be named {Name}. The existing parent will be renamed and used instead.");
                        // If we rename it, we make it clearer that why it's being teleported around at runtime.
                        CameraCache.Main.transform.parent.name = Name;
                    }

                    mixedRealityPlayspace = CameraCache.Main.transform.parent;
                }

                // It's very important that the Playspace align with the tracked space,
                // otherwise reality-locked things like playspace boundaries won't be aligned properly.
                // For now, we'll just assume that when the playspace is first initialized, the
                // tracked space origin overlaps with the world space origin. If a platform ever does
                // something else (i.e, placing the lower left hand corner of the tracked space at world 
                // space 0,0,0), we should compensate for that here.
                return mixedRealityPlayspace;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector3 Position => Transform.position;

        /// <summary>
        /// 
        /// </summary>
        public static Quaternion Rotation => Transform.rotation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        public static void AddChild(Transform transform)
        {
            transform.SetParent(Transform);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void DetachChildren()
        {
            Transform.DetachChildren();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localPosition"></param>
        /// <returns></returns>
        public static Vector3 TransformPoint(Vector3 localPosition)
        {
            return Transform.TransformPoint(localPosition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3 InverseTransformPoint(Vector3 worldPosition)
        {
            return Transform.InverseTransformPoint(worldPosition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localDirection"></param>
        /// <returns></returns>
        public static Vector3 TransformDirection(Vector3 localDirection)
        {
            return Transform.TransformDirection(localDirection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transformation"></param>
        public static void PerformTransformation(Action<Transform> transformation)
        {
            transformation?.Invoke(Transform);
        }
    }
}