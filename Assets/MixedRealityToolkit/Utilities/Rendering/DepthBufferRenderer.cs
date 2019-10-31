// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Rendering
{
    [ExecuteInEditMode]
    public class DepthBufferRenderer : MonoBehaviour
    {
        public RenderTexture outputTexture;

        private RenderTexture originalRT;

        private Material postProcessMaterial;
        private RenderTexture depthTexture;
        private const string DepthShaderName = "Mixed Reality Toolkit/Depth Buffer Viewer";
        private int textureWidth, textureHeight;
        private Camera cam;

#if UNITY_EDITOR
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
       
            //var cam = CameraCache.Main;
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
            var target = outputTexture != null ? outputTexture : destination;
            Graphics.Blit(source, target, postProcessMaterial);
        }

        private void OnDisable()
        {
            cam.targetTexture = originalRT;
        }
#endif
    }
}