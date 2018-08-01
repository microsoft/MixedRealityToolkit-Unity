using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.XR.iOS
{

    public class UnityRemoteVideo : MonoBehaviour
    {
#if UNITY_IOS || UNITY_EDITOR
        public ConnectToEditor connectToEditor;

        private UnityARSessionNativeInterface m_Session;
#endif
        private bool bTexturesInitialized;

        private int currentFrameIndex;
#if !UNITY_EDITOR
        private byte[] m_textureYBytes = null;
        private byte[] m_textureUVBytes = null;
        private byte[] m_textureYBytes2 = null;
        private byte[] m_textureUVBytes2 = null;
#endif
        private GCHandle m_pinnedYArray;
        private GCHandle m_pinnedUVArray;

#if !UNITY_EDITOR
        public void Start()
        {
#if UNITY_IOS
            m_Session = UnityARSessionNativeInterface.GetARSessionNativeInterface ();
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateCamera;
            currentFrameIndex = 0;
            bTexturesInitialized = false;
#endif
        }

#if UNITY_IOS
        void UpdateCamera(UnityARCamera camera)
        {
            if (!bTexturesInitialized) {
                InitializeTextures (camera);
            }
            UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateCamera;

        }

        void InitializeTextures(UnityARCamera camera)
        {
            int numYBytes = camera.videoParams.yWidth * camera.videoParams.yHeight;
            int numUVBytes = camera.videoParams.yWidth * camera.videoParams.yHeight / 2; //quarter resolution, but two bytes per pixel
            
            m_textureYBytes = new byte[numYBytes];
            m_textureUVBytes = new byte[numUVBytes];
            m_textureYBytes2 = new byte[numYBytes];
            m_textureUVBytes2 = new byte[numUVBytes];
            m_pinnedYArray = GCHandle.Alloc (m_textureYBytes);
            m_pinnedUVArray = GCHandle.Alloc (m_textureUVBytes);
            bTexturesInitialized = true;
        }
#endif

        IntPtr PinByteArray(ref GCHandle handle, byte[] array)
        {
            handle.Free ();
            handle = GCHandle.Alloc (array, GCHandleType.Pinned);
            return handle.AddrOfPinnedObject ();
        }

        byte [] ByteArrayForFrame(int frame,  byte[] array0,  byte[] array1)
        {
            return frame == 1 ? array1 : array0;
        }

        byte [] YByteArrayForFrame(int frame)
        {
            return ByteArrayForFrame (frame, m_textureYBytes, m_textureYBytes2);
        }

        byte [] UVByteArrayForFrame(int frame)
        {
            return ByteArrayForFrame (frame, m_textureUVBytes, m_textureUVBytes2);
        }

#if UNITY_IOS || UNITY_EDITOR
        void OnDestroy()
        {
            m_Session.SetCapturePixelData (false, IntPtr.Zero, IntPtr.Zero);

            m_pinnedYArray.Free ();
            m_pinnedUVArray.Free ();

        }
#endif

        public void OnPreRender()
        {
#if UNITY_IOS || UNITY_EDITOR
            ARTextureHandles handles = m_Session.GetARVideoTextureHandles();
            if (handles.textureY == System.IntPtr.Zero || handles.textureCbCr == System.IntPtr.Zero)
            {
                return;
            }

            if (!bTexturesInitialized)
                return;
            
            currentFrameIndex = (currentFrameIndex + 1) % 2;

            Resolution currentResolution = Screen.currentResolution;


            m_Session.SetCapturePixelData (true, PinByteArray(ref m_pinnedYArray,YByteArrayForFrame(currentFrameIndex)), PinByteArray(ref m_pinnedUVArray,UVByteArrayForFrame(currentFrameIndex)));

            connectToEditor.SendToEditor (ConnectionMessageIds.screenCaptureYMsgId, YByteArrayForFrame(1-currentFrameIndex));
            connectToEditor.SendToEditor (ConnectionMessageIds.screenCaptureUVMsgId, UVByteArrayForFrame(1-currentFrameIndex));
#endif
        }
#endif
    }
}
