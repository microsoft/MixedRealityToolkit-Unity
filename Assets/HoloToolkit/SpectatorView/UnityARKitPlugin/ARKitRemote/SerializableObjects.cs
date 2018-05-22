#if UNITY_IOS || UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Text;

namespace ARKit.Utils
{
    /// <summary>
    /// Since unity doesn't flag the Vector4 as serializable, we
    /// need to create our own version. This one will automatically convert
    /// between Vector4 and SerializableVector4
    /// </summary>
    [Serializable]
    public class SerializableVector4
    {
        /// <summary>
        /// x component
        /// </summary>
        public float x;

        /// <summary>
        /// y component
        /// </summary>
        public float y;

        /// <summary>
        /// z component
        /// </summary>
        public float z;

        /// <summary>
        /// w component
        /// </summary>
        public float w;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        /// <param name="rZ"></param>
        /// <param name="rW"></param>
        public SerializableVector4(float rX, float rY, float rZ, float rW)
        {
            x = rX;
            y = rY;
            z = rZ;
            w = rW;
        }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
        }

        /// <summary>
        /// Automatic conversion from SerializableVector4 to Vector4
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Vector4(SerializableVector4 rValue)
        {
            return new Vector4(rValue.x, rValue.y, rValue.z, rValue.w);
        }

        /// <summary>
        /// Automatic conversion from Vector4 to SerializableVector4
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator SerializableVector4(Vector4 rValue)
        {
            return new SerializableVector4(rValue.x, rValue.y, rValue.z, rValue.w);
        }
    }

    [Serializable]  
    public class serializableUnityARMatrix4x4
    {
        public SerializableVector4 column0;
        public SerializableVector4 column1;
        public SerializableVector4 column2;
        public SerializableVector4 column3;

        public serializableUnityARMatrix4x4(SerializableVector4 v0, SerializableVector4 v1, SerializableVector4 v2, SerializableVector4 v3)
        {
            column0 = v0;
            column1 = v1;
            column2 = v2;
            column3 = v3;
        }

        /// <summary>
        /// Automatic conversion from UnityARMatrix4x4 to serializableUnityARMatrix4x4
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator serializableUnityARMatrix4x4(UnityARMatrix4x4 rValue)
        {
            return new serializableUnityARMatrix4x4(rValue.column0, rValue.column1, rValue.column2, rValue.column3);
        }

        /// <summary>
        /// Automatic conversion from serializableUnityARMatrix4x4 to UnityARMatrix4x4
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator UnityARMatrix4x4(serializableUnityARMatrix4x4 rValue)
        {
            return new UnityARMatrix4x4(rValue.column0, rValue.column1, rValue.column2, rValue.column3);
        }


        public static implicit operator serializableUnityARMatrix4x4(Matrix4x4 rValue)
        {
            return new serializableUnityARMatrix4x4(rValue.GetColumn(0), rValue.GetColumn(1), rValue.GetColumn(2), rValue.GetColumn(3));
        }

        public static implicit operator Matrix4x4(serializableUnityARMatrix4x4 rValue)
        {
            #if UNITY_2017_1_OR_NEWER        
            return new Matrix4x4(rValue.column0, rValue.column1, rValue.column2, rValue.column3);
            #else
            Matrix4x4 mRet = new Matrix4x4 ();
            mRet.SetColumn (0, rValue.column0);
            mRet.SetColumn (1, rValue.column1);
            mRet.SetColumn (2, rValue.column2);
            mRet.SetColumn (3, rValue.column3);
            return mRet;
            #endif
        }

    };

    [Serializable]  
    public class serializableUnityARCamera
    {
        public serializableUnityARMatrix4x4 worldTransform;
        public serializableUnityARMatrix4x4 projectionMatrix;
        public ARTrackingState trackingState;
        public ARTrackingStateReason trackingReason;
        public UnityVideoParams videoParams;
        public UnityARLightEstimate lightEstimation;
        public serializablePointCloud pointCloud;
        public serializableUnityARMatrix4x4 displayTransform;


        public serializableUnityARCamera( serializableUnityARMatrix4x4 wt, serializableUnityARMatrix4x4 pm, ARTrackingState ats, ARTrackingStateReason atsr, UnityVideoParams uvp, UnityARLightEstimate lightEst, serializableUnityARMatrix4x4 dt, serializablePointCloud spc)
        {
            worldTransform = wt;
            projectionMatrix = pm;
            trackingState = ats;
            trackingReason = atsr;
            videoParams = uvp;
            lightEstimation = lightEst;
            displayTransform = dt;
            pointCloud = spc;
        }

        public static implicit operator serializableUnityARCamera(UnityARCamera rValue)
        {
            return new serializableUnityARCamera(rValue.worldTransform, rValue.projectionMatrix, rValue.trackingState, rValue.trackingReason, rValue.videoParams, rValue.lightEstimation, rValue.displayTransform, rValue.pointCloudData);
        }

        public static implicit operator UnityARCamera(serializableUnityARCamera rValue)
        {
            return new UnityARCamera (rValue.worldTransform, rValue.projectionMatrix, rValue.trackingState, rValue.trackingReason, rValue.videoParams, rValue.lightEstimation, rValue.displayTransform, rValue.pointCloud);
        }


    };

    [Serializable]  
    public class serializableUnityARPlaneAnchor
    {
        public serializableUnityARMatrix4x4 worldTransform;
        public SerializableVector4 center;
        public SerializableVector4 extent;
        public ARPlaneAnchorAlignment planeAlignment;
        public byte[] identifierStr;

        public serializableUnityARPlaneAnchor( serializableUnityARMatrix4x4 wt, SerializableVector4 ctr, SerializableVector4 ext, ARPlaneAnchorAlignment apaa,
            byte [] idstr)
        {
            worldTransform = wt;
            center = ctr;
            extent = ext;
            planeAlignment = apaa;
            identifierStr = idstr;
        }

        public static implicit operator serializableUnityARPlaneAnchor(ARPlaneAnchor rValue)
        {
            serializableUnityARMatrix4x4 wt = rValue.transform;
            SerializableVector4 ctr = new SerializableVector4 (rValue.center.x, rValue.center.y, rValue.center.z, 1.0f);
            SerializableVector4 ext = new SerializableVector4 (rValue.extent.x, rValue.extent.y, rValue.extent.z, 1.0f);
            byte[] idstr = Encoding.UTF8.GetBytes (rValue.identifier);
            return new serializableUnityARPlaneAnchor(wt, ctr, ext, rValue.alignment, idstr);
        }

        public static implicit operator ARPlaneAnchor(serializableUnityARPlaneAnchor rValue)
        {
            ARPlaneAnchor retValue;

            retValue.identifier = Encoding.UTF8.GetString (rValue.identifierStr);
            retValue.center = new Vector3 (rValue.center.x, rValue.center.y, rValue.center.z);
            retValue.extent = new Vector3 (rValue.extent.x, rValue.extent.y, rValue.extent.z);
            retValue.alignment = rValue.planeAlignment;
            retValue.transform = rValue.worldTransform;
            
            return retValue;
        }

    };

    [Serializable]
    public class serializablePointCloud
    {
        public byte [] pointCloudData;

        public serializablePointCloud(byte [] inputPoints)
        {
            pointCloudData = inputPoints;
        }

        public static implicit operator serializablePointCloud(Vector3 [] vecPointCloud)
        {
            if (vecPointCloud != null)
            {
                byte [] createBuf = new byte[vecPointCloud.Length * sizeof(float) * 3];
                for(int i = 0; i < vecPointCloud.Length; i++)
                {
                    int bufferStart = i * 3;
                    Buffer.BlockCopy( BitConverter.GetBytes( vecPointCloud[i].x ), 0, createBuf, (bufferStart)*sizeof(float), sizeof(float) );
                    Buffer.BlockCopy( BitConverter.GetBytes( vecPointCloud[i].y ), 0, createBuf, (bufferStart+1)*sizeof(float), sizeof(float) );
                    Buffer.BlockCopy( BitConverter.GetBytes( vecPointCloud[i].z ), 0, createBuf, (bufferStart+2)*sizeof(float), sizeof(float) );

                }
                return new serializablePointCloud (createBuf);
            }
            else 
            {
                return new serializablePointCloud(null);
            }
        }

        public static implicit operator Vector3 [] (serializablePointCloud spc)
        {
            if (spc.pointCloudData != null) 
            {
                int numVectors = spc.pointCloudData.Length / (3 * sizeof(float));
                Vector3 [] pointCloudVec = new Vector3[numVectors];
                for (int i = 0; i < numVectors; i++) 
                {
                    int bufferStart = i * 3;
                    pointCloudVec [i].x = BitConverter.ToSingle (spc.pointCloudData, (bufferStart) * sizeof(float));
                    pointCloudVec [i].y = BitConverter.ToSingle (spc.pointCloudData, (bufferStart+1) * sizeof(float));
                    pointCloudVec [i].z = BitConverter.ToSingle (spc.pointCloudData, (bufferStart+2) * sizeof(float));
                    
                }
                return pointCloudVec;
            } 
            else 
            {
                return null;
            }
        }
    };

    [Serializable]
    public class serializableARSessionConfiguration
    {
        public UnityARAlignment alignment; 
        public UnityARPlaneDetection planeDetection;
        public bool getPointCloudData;
        public bool enableLightEstimation;

        public serializableARSessionConfiguration(UnityARAlignment align, UnityARPlaneDetection planeDet, bool getPtCloud, bool enableLightEst)
        {
            alignment = align;
            planeDetection = planeDet;
            getPointCloudData = getPtCloud;
            enableLightEstimation = enableLightEst;
        }

        public static implicit operator serializableARSessionConfiguration(ARKitWorldTrackingSessionConfiguration awtsc)
        {
            return new serializableARSessionConfiguration (awtsc.alignment, awtsc.planeDetection, awtsc.getPointCloudData, awtsc.enableLightEstimation);
        }

        public static implicit operator ARKitWorldTrackingSessionConfiguration (serializableARSessionConfiguration sasc)
        {
            return new ARKitWorldTrackingSessionConfiguration (sasc.alignment, sasc.planeDetection, sasc.getPointCloudData, sasc.enableLightEstimation);
        }
    };

    [Serializable]
    public class serializableARKitInit
    {
        public serializableARSessionConfiguration config;
        public UnityARSessionRunOption runOption;

        public serializableARKitInit(serializableARSessionConfiguration cfg, UnityARSessionRunOption option)
        {
            config = cfg;
            runOption = option;
        }
    };

    [Serializable]
    public class serializableFromEditorMessage
    {
        public Guid subMessageId;
        public serializableARKitInit arkitConfigMsg;

    };
}
#endif
