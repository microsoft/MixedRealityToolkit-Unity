#if UNITY_IOS || UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;

namespace UnityEngine.XR.iOS {

    /// <summary>
    /// A struct that allows us go from native Matrix4x4 to managed
    /// </summary>
    public struct UnityARMatrix4x4
    {
        public Vector4 column0;
        public Vector4 column1;
        public Vector4 column2;
        public Vector4 column3;

        public UnityARMatrix4x4(Vector4 c0, Vector4 c1, Vector4 c2, Vector4 c3)
        {
            column0 = c0; column1 = c1; column2 = c2; column3 = c3;
        }
    };

    [Serializable]
    public struct UnityVideoParams
    {
        public int yWidth;
        public int yHeight;
        public int screenOrientation;
        public float texCoordScale;
        public IntPtr cvPixelBufferPtr;
    };

    [Serializable]
    public struct UnityARLightEstimate
    {
        public float ambientIntensity;
        public float ambientColorTemperature;
    };

    struct internal_UnityARCamera
    {
        public UnityARMatrix4x4 worldTransform;
        public UnityARMatrix4x4 projectionMatrix;
        public ARTrackingState trackingState;
        public ARTrackingStateReason trackingReason;
        public UnityVideoParams videoParams;
        public UnityARLightEstimate lightEstimation;
        public UnityARMatrix4x4 displayTransform;
        public uint getPointCloudData;
    };

    public struct UnityARCamera
    {
        public UnityARMatrix4x4 worldTransform;
        public UnityARMatrix4x4 projectionMatrix;
        public ARTrackingState trackingState;
        public ARTrackingStateReason trackingReason;
        public UnityVideoParams videoParams;
        public UnityARLightEstimate lightEstimation;
        public UnityARMatrix4x4 displayTransform;
        public Vector3[] pointCloudData;

        public UnityARCamera(UnityARMatrix4x4 wt, UnityARMatrix4x4 pm, ARTrackingState ats, ARTrackingStateReason atsr, UnityVideoParams uvp, UnityARLightEstimate lightEst, UnityARMatrix4x4 dt, Vector3[] pointCloud)
        {
            worldTransform = wt;
            projectionMatrix = pm;
            trackingState = ats;
            trackingReason = atsr;
            videoParams = uvp;
            lightEstimation = lightEst;
            displayTransform = dt;
            pointCloudData = pointCloud;
        }
    };


    public struct UnityARAnchorData
    {
        public IntPtr ptrIdentifier;

        /**
         The transformation matrix that defines the anchor's rotation, translation and scale in world coordinates.
         */
        public UnityARMatrix4x4 transform;

        /**
         The alignment of the plane.
         */

        public ARPlaneAnchorAlignment alignment;

        /**
        The center of the plane in the anchor’s coordinate space.
        */

        public Vector4 center;

        /**
        The extent of the plane in the anchor’s coordinate space.
         */
        public Vector4 extent;

        public string identifierStr 
        { 
            get 
            { 
                #if PLATFORM_IOS 
                return Marshal.PtrToStringAuto(this.ptrIdentifier);
                #else
                return null;
                #endif 
                } 
            }

        public static UnityARAnchorData UnityARAnchorDataFromGameObject(GameObject go) {
            // create an anchor data struct from a game object transform
            Matrix4x4 matrix = Matrix4x4.TRS(go.transform.position, go.transform.rotation, go.transform.localScale);
            UnityARAnchorData ad = new UnityARAnchorData();
            ad.transform.column0 = matrix.GetColumn(0);
            ad.transform.column1 = matrix.GetColumn(1);
            ad.transform.column2 = matrix.GetColumn(2);
            ad.transform.column3 = matrix.GetColumn(3);
            return ad;
        }
    };

    public struct UnityARUserAnchorData 
    {

        public IntPtr ptrIdentifier;

        /**
         The transformation matrix that defines the anchor's rotation, translation and scale in world coordinates.
         */
        public UnityARMatrix4x4 transform;

        public string identifierStr 
        {
            get 
            {
                #if PLATOFORM_IOS
                return Marshal.PtrToStringAuto(this.ptrIdentifier);
                #else
                return null;
                #endif
            } 
        }

        public static UnityARUserAnchorData UnityARUserAnchorDataFromGameObject(GameObject go) {
            // create an anchor data struct from a game object transform
            Matrix4x4 matrix = Matrix4x4.TRS(go.transform.position, go.transform.rotation, go.transform.localScale);
            UnityARUserAnchorData ad = new UnityARUserAnchorData();
            ad.transform.column0 = matrix.GetColumn(0);
            ad.transform.column1 = matrix.GetColumn(1);
            ad.transform.column2 = matrix.GetColumn(2);
            ad.transform.column3 = matrix.GetColumn(3);
            return ad;
        }
    };

    public struct UnityARHitTestResult
    {
        /**
         The type of the hit-test result.
         */
        public ARHitTestResultType type;

        /**
        The distance from the camera to the intersection in meters.
        */
        public double distance;

        /**
        The transformation matrix that defines the intersection's rotation, translation and scale
        relative to the anchor or nearest feature point.
        */
        public Matrix4x4 localTransform;

        /**
        The transformation matrix that defines the intersection's rotation, translation and scale
        relative to the world.
        */
        public Matrix4x4 worldTransform;

        /**
        The anchor that the hit-test intersected.
        */
        public IntPtr anchor;

        /**
        True if the test represents a valid hit test. Data is undefined otherwise.
        */
        public bool isValid;

    };

    public enum UnityARAlignment
    {
        UnityARAlignmentGravity,
        UnityARAlignmentGravityAndHeading,
        UnityARAlignmentCamera
    }

    public enum UnityARPlaneDetection
    {
        None = 0,
        Horizontal = (1 << 0)
    }
   
    public struct ARKitSessionConfiguration
    {
        public UnityARAlignment alignment; 
        public bool getPointCloudData;
        public bool enableLightEstimation;
        public bool IsSupported { get { return IsARKitSessionConfigurationSupported(); } private set {} }

        public ARKitSessionConfiguration(UnityARAlignment alignment = UnityARAlignment.UnityARAlignmentGravity,
            bool getPointCloudData = false, 
            bool enableLightEstimation = false)
        {
            this.getPointCloudData = getPointCloudData;
            this.alignment = alignment;
            this.enableLightEstimation = enableLightEstimation;
        }
        
        [DllImport("__Internal")]
        private static extern bool IsARKitSessionConfigurationSupported();
    }



    public struct ARKitWorldTrackingSessionConfiguration
    {
        public UnityARAlignment alignment; 
        public UnityARPlaneDetection planeDetection;
        public bool getPointCloudData;
        public bool enableLightEstimation;
        public bool IsSupported { get { return IsARKitWorldTrackingSessionConfigurationSupported(); } private set {} }

        public ARKitWorldTrackingSessionConfiguration(UnityARAlignment alignment = UnityARAlignment.UnityARAlignmentGravity,
                UnityARPlaneDetection planeDetection = UnityARPlaneDetection.Horizontal,
            bool getPointCloudData = false, 
            bool enableLightEstimation = false)
        {
            this.getPointCloudData = getPointCloudData;
            this.alignment = alignment;
            this.planeDetection = planeDetection;
            this.enableLightEstimation = enableLightEstimation;

        }
        [DllImport("__Internal")]
        private static extern bool IsARKitWorldTrackingSessionConfigurationSupported();
    }

    public enum UnityARSessionRunOption {
        /** The session will reset tracking. */
        ARSessionRunOptionResetTracking           = (1 << 0),

        /** The session will remove existing anchors. */
        ARSessionRunOptionRemoveExistingAnchors   = (1 << 1)
    }

    public class UnityARSessionNativeInterface {

//        public delegate void ARFrameUpdate(UnityARMatrix4x4 cameraPos, UnityARMatrix4x4 projection);
//        public static event ARFrameUpdate ARFrameUpdatedEvent;

        // Plane Anchors
        public delegate void ARFrameUpdate(UnityARCamera camera);
        public static event ARFrameUpdate ARFrameUpdatedEvent;

        public delegate void ARAnchorAdded(ARPlaneAnchor anchorData);
        public static event ARAnchorAdded ARAnchorAddedEvent;

        public delegate void ARAnchorUpdated(ARPlaneAnchor anchorData);
        public static event ARAnchorUpdated ARAnchorUpdatedEvent;

        public delegate void ARAnchorRemoved(ARPlaneAnchor anchorData);
        public static event ARAnchorRemoved ARAnchorRemovedEvent;

        // User Anchors
        public delegate void ARUserAnchorAdded(ARUserAnchor anchorData);
        public static event ARUserAnchorAdded ARUserAnchorAddedEvent;

        public delegate void ARUserAnchorUpdated(ARUserAnchor anchorData);
        public static event ARUserAnchorUpdated ARUserAnchorUpdatedEvent;

        public delegate void ARUserAnchorRemoved(ARUserAnchor anchorData);
        public static event ARUserAnchorRemoved ARUserAnchorRemovedEvent;

        public delegate void ARSessionFailed(string error);
        public static event ARSessionFailed ARSessionFailedEvent;

        public delegate void ARSessionCallback();
        public static event ARSessionCallback ARSessionInterruptedEvent;
        public static event ARSessionCallback ARSessioninterruptionEndedEvent;
        public delegate void ARSessionTrackingChanged(UnityARCamera camera);
        public static event ARSessionTrackingChanged ARSessionTrackingChangedEvent;

        delegate void internal_ARFrameUpdate(internal_UnityARCamera camera);
        public delegate void internal_ARAnchorAdded(UnityARAnchorData anchorData);
        public delegate void internal_ARAnchorUpdated(UnityARAnchorData anchorData);
        public delegate void internal_ARAnchorRemoved(UnityARAnchorData anchorData);
        public delegate void internal_ARUserAnchorAdded(UnityARUserAnchorData anchorData);
        public delegate void internal_ARUserAnchorUpdated(UnityARUserAnchorData anchorData);
        public delegate void internal_ARUserAnchorRemoved(UnityARUserAnchorData anchorData);
        delegate void internal_ARSessionTrackingChanged(internal_UnityARCamera camera);

#if !UNITY_EDITOR
        private IntPtr m_NativeARSession;
#endif

        private static UnityARCamera s_Camera;
        
        [DllImport("__Internal")]
        private static extern IntPtr unity_CreateNativeARSession();

        [DllImport("__Internal")]
        private static extern void session_SetSessionCallbacks(IntPtr nativeSession, internal_ARFrameUpdate frameCallback,
                                            ARSessionFailed sessionFailed,
                                            ARSessionCallback sessionInterrupted,
                                            ARSessionCallback sessionInterruptionEnded,
                                            internal_ARSessionTrackingChanged trackingChanged);

        [DllImport("__Internal")]
        private static extern void session_SetPlaneAnchorCallbacks(IntPtr nativeSession, internal_ARAnchorAdded anchorAddedCallback, 
                                            internal_ARAnchorUpdated anchorUpdatedCallback, 
                                            internal_ARAnchorRemoved anchorRemovedCallback);

        [DllImport("__Internal")]
        private static extern void session_SetUserAnchorCallbacks(IntPtr nativeSession, internal_ARUserAnchorAdded userAnchorAddedCallback, 
                                            internal_ARUserAnchorUpdated userAnchorUpdatedCallback, 
                                            internal_ARUserAnchorRemoved userAnchorRemovedCallback);
        [DllImport("__Internal")]
        private static extern void StartWorldTrackingSession(IntPtr nativeSession, ARKitWorldTrackingSessionConfiguration configuration);

        [DllImport("__Internal")]
        private static extern void StartWorldTrackingSessionWithOptions(IntPtr nativeSession, ARKitWorldTrackingSessionConfiguration configuration, UnityARSessionRunOption runOptions);

        [DllImport("__Internal")]
        private static extern void StartSession(IntPtr nativeSession, ARKitSessionConfiguration configuration);

        [DllImport("__Internal")]
        private static extern void StartSessionWithOptions(IntPtr nativeSession, ARKitSessionConfiguration configuration, UnityARSessionRunOption runOptions);

        [DllImport("__Internal")]
        private static extern void PauseSession(IntPtr nativeSession);

        [DllImport("__Internal")]
        private static extern int HitTest(IntPtr nativeSession, ARPoint point, ARHitTestResultType types);

        [DllImport("__Internal")]
        private static extern UnityARHitTestResult GetLastHitTestResult(int index);

        [DllImport("__Internal")]
        private static extern ARTextureHandles GetVideoTextureHandles();

        [DllImport("__Internal")]
        private static extern float GetAmbientIntensity();

        [DllImport("__Internal")]
        private static extern int GetTrackingQuality();

        [DllImport("__Internal")]
        private static extern bool GetARPointCloud (ref IntPtr verts, ref uint vertLength);

        [DllImport("__Internal")]
        private static extern void SetCameraNearFar (float nearZ, float farZ);

        [DllImport("__Internal")]
        private static extern void CapturePixelData (int enable, IntPtr  pYPixelBytes, IntPtr pUVPixelBytes);

        [DllImport("__Internal")]
        private static extern UnityARUserAnchorData SessionAddUserAnchor (IntPtr nativeSession, UnityARUserAnchorData anchorData);

        [DllImport("__Internal")]
        private static extern void SessionRemoveUserAnchor (IntPtr nativeSession, [MarshalAs(UnmanagedType.LPStr)] string anchorIdentifier);

        public UnityARSessionNativeInterface()
        {
#if !UNITY_EDITOR
            m_NativeARSession = unity_CreateNativeARSession();
            session_SetSessionCallbacks(m_NativeARSession, _frame_update, _ar_session_failed, _ar_session_interrupted, _ar_session_interruption_ended, _ar_tracking_changed);
            session_SetPlaneAnchorCallbacks(m_NativeARSession, _anchor_added, _anchor_updated, _anchor_removed);
            session_SetUserAnchorCallbacks(m_NativeARSession, _user_anchor_added, _user_anchor_updated, _user_anchor_removed);
#endif
        }
        
        static UnityARSessionNativeInterface s_UnityARSessionNativeInterface = null;

        public static UnityARSessionNativeInterface GetARSessionNativeInterface()
        {
                if (s_UnityARSessionNativeInterface == null) {
                    s_UnityARSessionNativeInterface = new UnityARSessionNativeInterface ();
                }    
                return s_UnityARSessionNativeInterface;
        }

#if UNITY_EDITOR
        public static void SetStaticCamera(UnityARCamera scamera)
        {
            s_Camera = scamera;
        }

        public static void RunFrameUpdateCallbacks()
        {
            if (ARFrameUpdatedEvent != null)
            {
                ARFrameUpdatedEvent(s_Camera);
            }
        }

        public static void RunAddAnchorCallbacks(ARPlaneAnchor arPlaneAnchor)
        {
            if (ARAnchorAddedEvent != null)
            {
                ARAnchorAddedEvent(arPlaneAnchor);
            }
        }

        public static void RunUpdateAnchorCallbacks(ARPlaneAnchor arPlaneAnchor)
        {
            if (ARAnchorUpdatedEvent != null)
            {
                ARAnchorUpdatedEvent(arPlaneAnchor); 
            }
        }

        public static void RunRemoveAnchorCallbacks(ARPlaneAnchor arPlaneAnchor)
        {
            if (ARAnchorRemovedEvent != null)
            {
                ARAnchorRemovedEvent(arPlaneAnchor);
            }
        }


#endif

        public Matrix4x4 GetCameraPose()
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0, s_Camera.worldTransform.column0);
            matrix.SetColumn(1, s_Camera.worldTransform.column1);
            matrix.SetColumn(2, s_Camera.worldTransform.column2);
            matrix.SetColumn(3, s_Camera.worldTransform.column3);
            return matrix; 
        }

        public Matrix4x4 GetCameraProjection()
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0, s_Camera.projectionMatrix.column0);
            matrix.SetColumn(1, s_Camera.projectionMatrix.column1);
            matrix.SetColumn(2, s_Camera.projectionMatrix.column2);
            matrix.SetColumn(3, s_Camera.projectionMatrix.column3);
            return matrix;
        }

        public void SetCameraClipPlanes(float nearZ, float farZ)
        {
#if !UNITY_EDITOR
            SetCameraNearFar (nearZ, farZ);
#endif
        }

        public void SetCapturePixelData(bool enable, IntPtr pYByteArray, IntPtr pUVByteArray)
        {
#if !UNITY_EDITOR
            int iEnable = enable ? 1 : 0;
            CapturePixelData (iEnable,pYByteArray, pUVByteArray);
#endif
        }

        [MonoPInvokeCallback(typeof(internal_ARFrameUpdate))]
        static void _frame_update(internal_UnityARCamera camera)
        {
            UnityARCamera pubCamera = new UnityARCamera();
            pubCamera.projectionMatrix = camera.projectionMatrix;
            pubCamera.worldTransform = camera.worldTransform;
            pubCamera.trackingState = camera.trackingState;
            pubCamera.trackingReason = camera.trackingReason;
            pubCamera.videoParams = camera.videoParams;
            pubCamera.lightEstimation = camera.lightEstimation;
            pubCamera.displayTransform = camera.displayTransform;
            s_Camera = pubCamera;

            if (camera.getPointCloudData == 1)
            {
                UpdatePointCloudData (ref s_Camera);
            }      

            if (ARFrameUpdatedEvent != null)
            {
                ARFrameUpdatedEvent(s_Camera);
            }
        }

        [MonoPInvokeCallback(typeof(internal_ARSessionTrackingChanged))]
        static void _ar_tracking_changed(internal_UnityARCamera camera)
        {
            // we only update the current camera's tracking state since that's all 
            // this cllback is for
            s_Camera.trackingReason = camera.trackingReason;
            if (ARSessionTrackingChangedEvent != null)
            {
                ARSessionTrackingChangedEvent(s_Camera);
            }
        }

        static void UpdatePointCloudData(ref UnityARCamera camera)
        {
            #if PLATFORM_IOS
            IntPtr ptrResultVerts = IntPtr.Zero;
            uint resultVertLength = 0;
            bool success = GetARPointCloud (ref ptrResultVerts, ref resultVertLength);
            float[] resultVertices = null;
            if (success) {
                // Load the results into a managed array.
                resultVertices = new float[resultVertLength];
                Marshal.Copy (ptrResultVerts, resultVertices, 0, (int)resultVertLength); 

                Vector3[] verts = new Vector3[(resultVertLength / 4)];

                for (int count = 0; count < resultVertLength; count++)
                {
                    verts [count / 4].x = resultVertices[count++];
                    verts [count / 4].y = resultVertices[count++];
                    verts [count / 4].z = -resultVertices[count++];
                }
                camera.pointCloudData = verts;
            }
            #endif

        }

        static ARPlaneAnchor GetPlaneAnchorFromAnchorData(UnityARAnchorData anchor)
        {
            #if PLATFORM_IOS
            //get the identifier for this anchor from the pointer
            ARPlaneAnchor arPlaneAnchor = new ARPlaneAnchor ();
            arPlaneAnchor.identifier = Marshal.PtrToStringAuto(anchor.ptrIdentifier);

            Matrix4x4 matrix = new Matrix4x4 ();
            matrix.SetColumn(0, anchor.transform.column0);
            matrix.SetColumn(1, anchor.transform.column1);
            matrix.SetColumn(2, anchor.transform.column2);
            matrix.SetColumn(3, anchor.transform.column3);

            arPlaneAnchor.transform =  matrix;
            arPlaneAnchor.alignment = anchor.alignment;
            arPlaneAnchor.center = new Vector3(anchor.center.x, anchor.center.y, anchor.center.z);
            arPlaneAnchor.extent = new Vector3(anchor.extent.x, anchor.extent.y, anchor.extent.z);
            return arPlaneAnchor;
            #else
            Debug.Log("Not available on non iOS platforms");
            return new ARPlaneAnchor();
            #endif
        }

        static ARUserAnchor GetUserAnchorFromAnchorData(UnityARUserAnchorData anchor)
        {
            #if PLATOFORM_IOS
            //get the identifier for this anchor from the pointer
            ARUserAnchor arUserAnchor = new ARUserAnchor ();
            arUserAnchor.identifier = Marshal.PtrToStringAuto(anchor.ptrIdentifier);

            Matrix4x4 matrix = new Matrix4x4 ();
            matrix.SetColumn(0, anchor.transform.column0);
            matrix.SetColumn(1, anchor.transform.column1);
            matrix.SetColumn(2, anchor.transform.column2);
            matrix.SetColumn(3, anchor.transform.column3);

            arUserAnchor.transform =  matrix;
            return arUserAnchor;
            #else
            Debug.LogError("Not available on non iOS platform");
            return new ARUserAnchor();
            #endif
        }

        static ARHitTestResult GetHitTestResultFromResultData(UnityARHitTestResult resultData)
        {
            #if PLATOFORM_IOS
            ARHitTestResult arHitTestResult = new ARHitTestResult ();
            arHitTestResult.type = resultData.type;
            arHitTestResult.distance = resultData.distance;
            arHitTestResult.localTransform = resultData.localTransform;
            arHitTestResult.worldTransform = resultData.worldTransform;
            arHitTestResult.isValid = resultData.isValid;
            if (resultData.anchor != IntPtr.Zero) {
                arHitTestResult.anchorIdentifier = Marshal.PtrToStringAuto (resultData.anchor);
            }
            return arHitTestResult;
            #else
            Debug.LogError("Not available on non iOS platform");
            return new ARHitTestResult();
            #endif
        }

#region Plane Anchors
        [MonoPInvokeCallback(typeof(internal_ARAnchorAdded))]
        static void _anchor_added(UnityARAnchorData anchor)
        {
            if (ARAnchorAddedEvent != null)
            {
                ARPlaneAnchor arPlaneAnchor = GetPlaneAnchorFromAnchorData(anchor);
                ARAnchorAddedEvent(arPlaneAnchor);
            }
        }

        [MonoPInvokeCallback(typeof(internal_ARAnchorUpdated))]
        static void _anchor_updated(UnityARAnchorData anchor)
        {
            if (ARAnchorUpdatedEvent != null)
            {
                ARPlaneAnchor arPlaneAnchor = GetPlaneAnchorFromAnchorData(anchor);
                ARAnchorUpdatedEvent(arPlaneAnchor); }
        }

        [MonoPInvokeCallback(typeof(internal_ARAnchorRemoved))]
        static void _anchor_removed(UnityARAnchorData anchor)
        {
            if (ARAnchorRemovedEvent != null)
            {
                ARPlaneAnchor arPlaneAnchor = GetPlaneAnchorFromAnchorData(anchor);
                ARAnchorRemovedEvent(arPlaneAnchor);
            }
        }
#endregion

#region User Anchors
        [MonoPInvokeCallback(typeof(internal_ARUserAnchorAdded))]
        static void _user_anchor_added(UnityARUserAnchorData anchor)
        {
            if (ARUserAnchorAddedEvent != null)
            {
                ARUserAnchor arUserAnchor = GetUserAnchorFromAnchorData(anchor);
                ARUserAnchorAddedEvent(arUserAnchor);
            }
        }

        [MonoPInvokeCallback(typeof(internal_ARUserAnchorUpdated))]
        static void _user_anchor_updated(UnityARUserAnchorData anchor)
        {
            if (ARUserAnchorUpdatedEvent != null)
            {
                ARUserAnchor arUserAnchor = GetUserAnchorFromAnchorData(anchor);
                ARUserAnchorUpdatedEvent(arUserAnchor); }
        }

        [MonoPInvokeCallback(typeof(internal_ARUserAnchorRemoved))]
        static void _user_anchor_removed(UnityARUserAnchorData anchor)
        {
            if (ARUserAnchorRemovedEvent != null)
            {
                ARUserAnchor arUserAnchor = GetUserAnchorFromAnchorData(anchor);
                ARUserAnchorRemovedEvent(arUserAnchor);
            }
        }
#endregion

        [MonoPInvokeCallback(typeof(ARSessionFailed))]
        static void _ar_session_failed(string error)
        {
            if (ARSessionFailedEvent != null)
            {
                ARSessionFailedEvent(error);
            }
        }        

        [MonoPInvokeCallback(typeof(ARSessionCallback))]
        static void _ar_session_interrupted()
        {
            Debug.Log("ar_session_interrupted");
            if (ARSessionInterruptedEvent != null)
            {
                ARSessionInterruptedEvent();
            }

        }

        [MonoPInvokeCallback(typeof(ARSessionCallback))]
        static void _ar_session_interruption_ended()
        {
            Debug.Log("ar_session_interruption_ended");
            if (ARSessioninterruptionEndedEvent != null)
            {
                ARSessioninterruptionEndedEvent();
            }
        }

        public void RunWithConfigAndOptions(ARKitWorldTrackingSessionConfiguration config, UnityARSessionRunOption runOptions)
        {
#if !UNITY_EDITOR
            StartWorldTrackingSessionWithOptions (m_NativeARSession, config, runOptions);
#endif
        }

        public void RunWithConfig(ARKitWorldTrackingSessionConfiguration config)
        {
#if !UNITY_EDITOR
            StartWorldTrackingSession(m_NativeARSession, config);
#endif
        }

        public void Run()
        {
            RunWithConfig(new ARKitWorldTrackingSessionConfiguration(UnityARAlignment.UnityARAlignmentGravity, UnityARPlaneDetection.Horizontal));
        }

        public void RunWithConfigAndOptions(ARKitSessionConfiguration config, UnityARSessionRunOption runOptions)
        {
            #if !UNITY_EDITOR
            StartSessionWithOptions (m_NativeARSession, config, runOptions);
            #endif
        }

        public void RunWithConfig(ARKitSessionConfiguration config)
        {
            #if !UNITY_EDITOR
            StartSession(m_NativeARSession, config);
            #endif
        }
            
        public void Pause()
        {
#if !UNITY_EDITOR
            PauseSession(m_NativeARSession);
#endif
        }

        public List<ARHitTestResult> HitTest(ARPoint point, ARHitTestResultType types)
        {
#if !UNITY_EDITOR
            int numResults = HitTest(m_NativeARSession, point, types);
            List<ARHitTestResult> results = new List<ARHitTestResult>();
        
            for (int i = 0; i < numResults; ++i)
            {
                var result = GetLastHitTestResult(i);
                results.Add(GetHitTestResultFromResultData(result));
            }

            return results;
#else
            return new List<ARHitTestResult>();
#endif
        }
        
        public ARTextureHandles GetARVideoTextureHandles()
        {
            return GetVideoTextureHandles ();
        }

        [Obsolete("Hook ARFrameUpdatedEvent instead and get UnityARCamera.ambientIntensity")]
        public float GetARAmbientIntensity()
        {
            return GetAmbientIntensity ();
        }

        [Obsolete("Hook ARFrameUpdatedEvent instead and get UnityARCamera.trackingState")]
        public int GetARTrackingQuality()
        {
            return GetTrackingQuality();
        }
        
        public UnityARUserAnchorData AddUserAnchor(UnityARUserAnchorData anchorData)
        {
#if !UNITY_EDITOR
            return SessionAddUserAnchor(m_NativeARSession, anchorData);
#else 
            return new UnityARUserAnchorData();
#endif
        }

        public UnityARUserAnchorData AddUserAnchorFromGameObject(GameObject go) {
#if !UNITY_EDITOR
            UnityARUserAnchorData data = AddUserAnchor(UnityARUserAnchorData.UnityARUserAnchorDataFromGameObject(go)); 
            return data;  
#else 
            return new UnityARUserAnchorData();
#endif
        }

        public void RemoveUserAnchor(string anchorIdentifier)
        {
#if !UNITY_EDITOR

            SessionRemoveUserAnchor(m_NativeARSession, anchorIdentifier);
#endif
        }
    }
}
#endif