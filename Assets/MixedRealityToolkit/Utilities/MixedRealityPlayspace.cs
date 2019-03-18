using UnityEngine;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities
{
    public static class MixedRealityPlayspace
    {

        private const string PlayspaceName = "MixedRealityPlayspace";

        private static Transform mixedRealityPlayspace;

        private static Queue<Action<Transform>> performTransformation = new Queue<Action<Transform>>(1);

        private static Transform Transform
        {
            get
            {
                if (mixedRealityPlayspace)
                {
                    return mixedRealityPlayspace;
                }

                if (CameraCache.Main.transform.parent == null)
                {
                    mixedRealityPlayspace = new GameObject(PlayspaceName).transform;
                    CameraCache.Main.transform.SetParent(mixedRealityPlayspace);
                }
                else
                {
                    if (CameraCache.Main.transform.parent.name != PlayspaceName)
                    {
                        // Since the scene is set up with a different camera parent, its likely
                        // that there's an expectation that that parent is going to be used for
                        // something else. We print a warning to call out the fact that we're 
                        // co-opting this object for use with teleporting and such, since that
                        // might cause conflicts with the parent's intended purpose.
                        Debug.LogWarning($"The Mixed Reality Toolkit expected the camera\'s parent to be named {PlayspaceName}. The existing parent will be renamed and used instead.");
                        // If we rename it, we make it clearer that why it's being teleported around at runtime.
                        CameraCache.Main.transform.parent.name = PlayspaceName;
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

        public static Vector3 Position => Transform.position;

        public static Vector3 TransformPoint(Vector3 localPosition)
        {
            return Transform.TransformPoint(localPosition);
        }

        public static Vector3 InverseTransformPoint(Vector3 worldPosition)
        {
            return Transform.InverseTransformPoint(worldPosition);
        }

        public static Vector3 TransformDirection(Vector3 localDirection)
        {
            return Transform.TransformDirection(localDirection);
        }

        public static void UpdateTransform()
        {
            while (performTransformation.Count > 0)
            {
                performTransformation.Dequeue()(Transform);
            }
        }

        public static void SetChild(Transform transform)
        {
            transform.SetParent(Transform);
        }

        public static void PerformTransformation(Action<Transform> transformation)
        {
            performTransformation.Enqueue(transformation);
        }
    }
}