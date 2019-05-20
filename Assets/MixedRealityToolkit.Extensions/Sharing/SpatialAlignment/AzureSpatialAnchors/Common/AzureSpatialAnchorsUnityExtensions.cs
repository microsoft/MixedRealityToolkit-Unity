// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System;
using UnityEngine;
#if UNITY_ANDROID
using Microsoft.Azure.SpatialAnchors.Unity.Android.ARCore;
#elif WINDOWS_UWP || UNITY_WSA
using UnityEngine.XR.WSA;
#elif UNITY_IOS
using Microsoft.Azure.SpatialAnchors.Unity.IOS.ARKit;
using UnityEngine.XR.iOS;
#endif

namespace Microsoft.Azure.SpatialAnchors.Unity
{
    public static class AzureSpatialAnchorsUnityExtensions
    {
        /// <summary>
        /// Adds the appropriate platform specific augmented reality anchor.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="System.NullReferenceException"></exception>
        public static void AddARAnchor(this GameObject source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.RemoveARAnchor();

#if UNITY_IOS
            source.AddComponent<UnityARUserAnchorComponent>();
#elif UNITY_ANDROID
            source.AddComponent<UnityARCoreWorldAnchorComponent>();
#elif WINDOWS_UWP
            source.AddComponent<WorldAnchor>();
#else
            throw new PlatformNotSupportedException("Unable to add an anchor. The platform is not supported.");
#endif
        }

        /// <summary>
        /// Gets the anchor pose.
        /// </summary>
        /// <param name="cloudSpatialAnchor">The cloud spatial anchor.</param>
        /// <returns><see cref="Pose"/>.</returns>
        /// <exception cref="System.ArgumentNullException">cloudSpatialAnchor</exception>
        public static Pose GetAnchorPose(this CloudSpatialAnchor cloudSpatialAnchor)
        {
            if (cloudSpatialAnchor == null)
            {
                throw new ArgumentNullException(nameof(cloudSpatialAnchor));
            }

            Pose anchorPose = Pose.identity;

#if UNITY_IOS
            Matrix4x4 matrix4X4 = ARKitNativeHelpers.GetAnchorTransform(cloudSpatialAnchor.LocalAnchor).ToMatrix4x4();
            anchorPose = new Pose(UnityARMatrixOps.GetPosition(matrix4X4), UnityARMatrixOps.GetRotation(matrix4X4));
#elif UNITY_ANDROID
            anchorPose = GoogleARCoreInternal.LifecycleManager.Instance.NativeSession.AnchorApi.GetPose(cloudSpatialAnchor.LocalAnchor);
#else
            throw new NotSupportedException($"Platform is not supported.");
#endif

            return anchorPose;
        }

        /// <summary>
        /// Gets the native anchor pointer.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The native anchor <see cref="IntPtr" />.</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        /// <exception cref="System.NullReferenceException"></exception>
        public static IntPtr GetNativeAnchorPointer(this GameObject source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            IntPtr retval = IntPtr.Zero;

#if UNITY_IOS
            UnityARUserAnchorComponent unityARUserAnchorComponent = source.GetComponent<UnityARUserAnchorComponent>();
            if (unityARUserAnchorComponent != null)
            {
                Debug.LogFormat("User anchor {0}", unityARUserAnchorComponent.AnchorId);
                retval = unityARUserAnchorComponent.GetAnchorPointer();
            }

#elif UNITY_ANDROID
            UnityARCoreWorldAnchorComponent anchorComponent = source.GetComponent<UnityARCoreWorldAnchorComponent>();
            if (anchorComponent != null)
            {
                Debug.LogFormat("User anchor {0}", anchorComponent.WorldAnchor.m_NativeHandle);
                retval = anchorComponent.WorldAnchor.m_NativeHandle;
            }
#elif WINDOWS_UWP
            WorldAnchor worldAnchor = source.GetComponent<WorldAnchor>();
            if (worldAnchor != null)
            {
                retval = worldAnchor.GetNativeSpatialAnchorPtr();
            }
#else
            throw new PlatformNotSupportedException("Unable to retrieve the native anchor pointer. The platform is not supported.");
#endif

            if (retval == IntPtr.Zero)
            {
                Debug.LogError("Didn't find AR anchor on gameobject");
            }

            return retval;
        }

        /// <summary>
        /// Removes the augmented reality anchor, if any.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public static void RemoveARAnchor(this GameObject source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

#if UNITY_IOS
            UnityARUserAnchorComponent unityARUserAnchorComponent = source.GetComponent<UnityARUserAnchorComponent>();
            if (unityARUserAnchorComponent != null)
            {
                GameObject.DestroyImmediate(unityARUserAnchorComponent);
            }
#elif UNITY_ANDROID
            UnityARCoreWorldAnchorComponent anchorComponent = source.GetComponent<UnityARCoreWorldAnchorComponent>();
            if (anchorComponent != null)
            {
                GameObject.DestroyImmediate(anchorComponent);
            }

#elif WINDOWS_UWP
            WorldAnchor worldAnchor = source.GetComponent<WorldAnchor>();
            if (worldAnchor != null)
            {
                GameObject.DestroyImmediate(worldAnchor);
            }
#else
            throw new PlatformNotSupportedException("Unable to remove the anchor. The platform is not supported.");
#endif
        }
    }
}
