// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_ANDROID && SPATIALALIGNMENT_ASA
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity.Android;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    /// <summary>
    /// Android implementation of the Azure Spatial Anchors coordinate service.
    /// </summary>
    internal class SpatialAnchorsAndroidCoordinateService : SpatialAnchorsCoordinateService
    {
        private long lastFrameProcessedTimeStamp;

        /// <summary>
        /// Instantiates a new <see cref="SpatialAnchorsAndroidCoordinateService"/>.
        /// </summary>
        /// <param name="spatialAnchorsConfiguration">Azure Spatial Anchors configuration.</param>
        public SpatialAnchorsAndroidCoordinateService(SpatialAnchorsConfiguration spatialAnchorsConfiguration)
            : base(spatialAnchorsConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override Task OnInitializeAsync()
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            UnityAndroidHelper.Instance.DispatchUiThread(unityActivity =>
            {
                try
                {
                    // We should only run the java initialization once
                    using (AndroidJavaClass cloudServices = new AndroidJavaClass("com.microsoft.CloudServices"))
                    {
                        cloudServices.CallStatic("initialize", unityActivity);
                        taskCompletionSource.SetResult(null);
                    }
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            return taskCompletionSource.Task;
        }

        /// <inheritdoc/>
        protected override void OnConfigureSession(CloudSpatialAnchorSession session)
        {
            session.Session = GoogleARCoreInternal.ARCoreAndroidLifecycleManager.Instance.NativeSession.SessionHandle;
        }

        /// <inheritdoc/>
        protected override void OnFrameUpdate()
        {
            if (!IsTracking || session == null)
            {
                return;
            }

            GoogleARCoreInternal.NativeSession nativeSession = GoogleARCoreInternal.ARCoreAndroidLifecycleManager.Instance.NativeSession;

            if (nativeSession.FrameHandle == IntPtr.Zero)
            {
                return;
            }

            long latestFrameTimeStamp = nativeSession.FrameApi.GetTimestamp();

            bool newFrameToProcess = latestFrameTimeStamp > lastFrameProcessedTimeStamp;

            if (newFrameToProcess)
            {
                session.ProcessFrame(nativeSession.FrameHandle);
                lastFrameProcessedTimeStamp = latestFrameTimeStamp;
            }
        }
    }
}
#endif