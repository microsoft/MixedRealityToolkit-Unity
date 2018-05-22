using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.XR.iOS
{

    public class UnityARVideo : MonoBehaviour
    {
        public Material m_ClearMaterial;

        private CommandBuffer m_VideoCommandBuffer;
        private Texture2D _videoTextureY;
        private Texture2D _videoTextureCbCr;
        private Matrix4x4 _displayTransform;

        private bool bCommandBufferInitialized;

        public void Start()
        {
#if UNITY_IOS || UNITY_EDITOR
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateFrame;
            bCommandBufferInitialized = false;
#endif
        }
#if UNITY_IOS || UNITY_EDITOR
        void UpdateFrame(UnityARCamera cam)
        {
            _displayTransform = new Matrix4x4();
            _displayTransform.SetColumn(0, cam.displayTransform.column0);
            _displayTransform.SetColumn(1, cam.displayTransform.column1);
            _displayTransform.SetColumn(2, cam.displayTransform.column2);
            _displayTransform.SetColumn(3, cam.displayTransform.column3);        
        }

        void InitializeCommandBuffer()
        {
            m_VideoCommandBuffer = new CommandBuffer(); 
            m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, m_ClearMaterial);
            GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
            bCommandBufferInitialized = true;

        }

        void OnDestroy()
        {
            GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
            UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateFrame;
            bCommandBufferInitialized = false;
        }
#endif

#if !UNITY_EDITOR && UNITY_IOS

        public void OnPreRender()
        {
            ARTextureHandles handles = UnityARSessionNativeInterface.GetARSessionNativeInterface ().GetARVideoTextureHandles();
            if (handles.textureY == System.IntPtr.Zero || handles.textureCbCr == System.IntPtr.Zero)
            {
                return;
            }

            if (!bCommandBufferInitialized) {
                InitializeCommandBuffer ();
            }

            Resolution currentResolution = Screen.currentResolution;

            // Texture Y
            if (_videoTextureY == null) {
              _videoTextureY = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
                  TextureFormat.R8, false, false, (System.IntPtr)handles.textureY);
              _videoTextureY.filterMode = FilterMode.Bilinear;
              _videoTextureY.wrapMode = TextureWrapMode.Repeat;
              m_ClearMaterial.SetTexture("_textureY", _videoTextureY);
            }

            // Texture CbCr
            if (_videoTextureCbCr == null) {
              _videoTextureCbCr = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
                  TextureFormat.RG16, false, false, (System.IntPtr)handles.textureCbCr);
              _videoTextureCbCr.filterMode = FilterMode.Bilinear;
              _videoTextureCbCr.wrapMode = TextureWrapMode.Repeat;
              m_ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);
            }

            _videoTextureY.UpdateExternalTexture(handles.textureY);
            _videoTextureCbCr.UpdateExternalTexture(handles.textureCbCr);

            m_ClearMaterial.SetMatrix("_DisplayTransform", _displayTransform);
        }
#elif UNITY_EDITOR

        public void SetYTexure(Texture2D YTex)
        {
            _videoTextureY = YTex;
        }

        public void SetUVTexure(Texture2D UVTex)
        {
            _videoTextureCbCr = UVTex;
        }

        public void OnPreRender()
        {

            if (!bCommandBufferInitialized) 
            {
                InitializeCommandBuffer ();
            }

            m_ClearMaterial.SetTexture("_textureY", _videoTextureY);
            m_ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);

            m_ClearMaterial.SetMatrix("_DisplayTransform", _displayTransform);
        }
 
#endif
    }
}
