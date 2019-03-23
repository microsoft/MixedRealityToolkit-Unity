// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Marshalling;

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC
{
    /// <summary>
    /// Play FrameQueue content into a texture over time, like a "video"
    /// </summary>
    /// <remarks>
    /// This component writes to the attached Material, via the attached Renderer
    /// </remarks>
    [RequireComponent(typeof(Renderer))]
    public class WebRtcVideoPlayer : MonoBehaviour
    {
        /// <summary>
        /// A textmesh onto which frame load stat data will be written
        /// </summary>
        /// <remarks>
        /// This is how fast the frames are given from the underlying implementation
        /// </remarks>
        [Tooltip("A textmesh onto which frame load stat data will be written")]
        public TextMesh FrameLoadStatHolder;

        /// <summary>
        /// A textmesh onto which frame present stat data will be written
        /// </summary>
        /// <remarks>
        /// This is how fast we render frames to the display
        /// </remarks>
        [Tooltip("A textmesh onto which frame present stat data will be written")]
        public TextMesh FramePresentStatHolder;

        /// <summary>
        /// A textmesh into which frame skip stat dta will be written
        /// </summary>
        /// <remarks>
        /// This is how often we skip presenting an underlying frame
        /// </remarks>
        [Tooltip("A textmesh onto which frame skip stat data will be written")]
        public TextMesh FrameSkipStatHolder;

        /// <summary>
        /// The frame queue from which frames will be "played"
        /// </summary>
        /// <remarks>
        /// This is allocated and managed by <see cref="WebrtcPeerVideo"/>
        /// </remarks>
        public FrameQueue FrameQueue;

        /// <summary>
        /// Internal reference to the attached texture
        /// </summary>
        private Texture2D tex;

        /// <summary>
        /// Internal timing counter
        /// </summary>
        private float lastUpdateTime;
        
        // Use this for initialization
        private void Start()
        {
            tex = new Texture2D(2, 2);
            tex.SetPixel(0, 0, Color.blue);
            tex.SetPixel(1, 1, Color.blue);
            tex.Apply();
            
            this.GetComponent<Renderer>().material.mainTexture = tex;
        }

        /// <summary>
        /// Unity Engine Start() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
        /// </remarks>
        private void Update()
        {
            if (Time.fixedTime - lastUpdateTime > 1.0 / 31.0)
            {
                lastUpdateTime = Time.fixedTime;
                TryProcessFrame();
            }

            // Share our stats values, if possible.
            if (FrameQueue != null)
            {
                if (FrameLoadStatHolder != null)
                {
                    FrameLoadStatHolder.text = FrameQueue.FrameLoad.Value.ToString();
                }
                if (FramePresentStatHolder != null)
                {
                    FramePresentStatHolder.text = FrameQueue.FramePresent.Value.ToString();
                }
                if (FrameSkipStatHolder != null)
                {
                    FrameSkipStatHolder.text = FrameQueue.FrameSkip.Value.ToString();
                }
            }
        }

        /// <summary>
        /// Internal helper that attempts to process frame data in the frame queue
        /// </summary>
        private void TryProcessFrame()
        {
            if (FrameQueue != null)
            {
                FramePacket packet = FrameQueue.Pop();

                if (packet != null)
                {
                    ProcessFrameBuffer(packet);
                    FrameQueue.Pool(packet);
                }
            }
        }

        /// <summary>
        /// Internal helper that attempts to render frame data to the texture
        /// </summary>
        /// <param name="packet"></param>
        private void ProcessFrameBuffer(FramePacket packet)
        {
            if (packet == null)
            {
                return;
            }

            if (tex == null || (tex.width != packet.width || tex.height != packet.height))
            {
                tex = new Texture2D(packet.width, packet.height, TextureFormat.RGBA32, false);
            }

            // note: this only "looks right" in Unity because we apply the 
            // "YUVFeedShader" to the texture (converting yuv to rgb)
            tex.LoadRawTextureData(packet.Buffer);

            tex.Apply();

            this.GetComponent<Renderer>().material.mainTexture = tex;
        }
    }
}
