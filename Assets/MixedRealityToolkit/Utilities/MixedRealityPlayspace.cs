using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    public static class MixedRealityPlayspace
    {
        /// <summary>
        /// The name of the playspace <see cref="GameObject"/>.
        /// </summary>
        public const string Name = "MixedRealityPlayspace";

        private static Transform transform;

        /// <summary>
        /// The transform of the playspace <see cref="GameObject"/>.
        /// </summary>
        public static Transform Transform
        {
            get
            {
                if (transform != null)
                {
                    return transform;
                }

                if (CameraCache.Main.transform.parent == null)
                {
                    transform = new GameObject(Name).transform;
                    CameraCache.Main.transform.SetParent(transform);
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

                    transform = CameraCache.Main.transform.parent;
                }

                // It's very important that the Playspace align with the tracked space,
                // otherwise reality-locked things like playspace boundaries won't be aligned properly.
                // For now, we'll just assume that when the playspace is first initialized, the
                // tracked space origin overlaps with the world space origin. If a platform ever does
                // something else (i.e, placing the lower left hand corner of the tracked space at world 
                // space 0,0,0), we should compensate for that here.
                return transform;
            }
        }

        /// <summary>
        /// The position of the playspace.
        /// </summary>
        public static Vector3 Position => Transform.position;

        /// <summary>
        /// Add a <see cref="GameObject"/> as a child of the playspace.
        /// </summary>
        /// <param name="transform">The transform of the <see cref="GameObject"/> to be added.</param>
        public static void AddChild(Transform transform)
        {
            transform.SetParent(Transform);
        }

        // todo: remove child

        // todo: child count

        // todo: get children

        // todo: detatch children
    }
}