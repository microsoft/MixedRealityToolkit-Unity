// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Rendering
{
    [ExecuteInEditMode]
    public class DepthBufferRenderer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If not null, depth buffer rendering output will blit to this RenderTexture. If null, normal operation will blit the depth buffer as color to the screen.")]
        private RenderTexture outputTexture = null;

        /// <summary>
        /// If not null, depth buffer rendering output will blit to this RenderTexture.
        /// If null, normal operation will blit the depth buffer as color to the screen.
        /// </summary>
        public RenderTexture OutputTexture
        {
            get => outputTexture;
            set => outputTexture = value;
        }

        private const string DepthShaderName = "Mixed Reality Toolkit/Depth Buffer Viewer";

#if UNITY_EDITOR
        private RenderTexture originalRT;
        private Material postProcessMaterial;
        private RenderTexture depthTexture;
        private int textureWidth, textureHeight;
        private Camera cam;

        private void Awake()
        {
            originalRT = CameraCache.Main.targetTexture;
            postProcessMaterial = new Material(Shader.Find(DepthShaderName));

            cam = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            SetUp();
        }

        private void SetUp()
        {
            textureWidth = Screen.width;
            textureHeight = Screen.height;

            depthTexture = new RenderTexture(textureWidth, textureHeight, 24, RenderTextureFormat.Depth);
            RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 0);

            postProcessMaterial.SetTexture("_DepthTex", depthTexture);
       
            cam.depthTextureMode = DepthTextureMode.Depth;
            cam.SetTargetBuffers(renderTexture.colorBuffer, depthTexture.depthBuffer);
        }

        private void Update()
        {
            if (textureWidth != Screen.width || textureHeight != Screen.height)
            {
                SetUp();
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var target = OutputTexture != null ? outputTexture : destination;
            Graphics.Blit(source, target, postProcessMaterial);
        }

        private void OnDisable()
        {
            cam.targetTexture = originalRT;
        }
#endif
    }
}