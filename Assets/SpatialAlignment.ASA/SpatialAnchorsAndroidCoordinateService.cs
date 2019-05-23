// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_ANDROID
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity.Android;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.AzureSpatialAnchors
{
    internal class SpatialAnchorsAndroidCoordinateService : SpatialAnchorsCoordinateService
    {
        private long lastFrameProcessedTimeStamp;

        public SpatialAnchorsAndroidCoordinateService(SpatialAnchorsConfiguration spatialAnchorsConfiguration)
            : base(spatialAnchorsConfiguration)
        {
        }

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

        protected override void OnConfigureSession(CloudSpatialAnchorSession session)
        {
            session.Session = GoogleARCoreInternal.ARCoreAndroidLifecycleManager.Instance.NativeSession.SessionHandle;
        }

        //TODO this needs pumping
        private void ProcessLatestFrame()
        {
            if (!IsTracking)
            {
                return;
            }

            if (session == null)
            {
                throw new InvalidOperationException("Cloud spatial anchor session is not available.");
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