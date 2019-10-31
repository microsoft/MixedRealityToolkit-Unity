using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

[ExecuteInEditMode]
public class DepthBufferViewer : MonoBehaviour
{
    public RenderTexture outputTexture;

    private Material postProcessMaterial;

    private void Start()
    {
        var cam = CameraCache.Main;

        // TODO: set 24 or 16 based on depth texture format in build settings WMR?
        RenderTexture depthTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24, RenderTextureFormat.Depth);
        RenderTexture renderTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0);

        postProcessMaterial = new Material(Shader.Find("Custom/DepthShader"));
        postProcessMaterial.SetTexture("_DepthTex", depthTexture);

        cam.depthTextureMode = DepthTextureMode.Depth;

        cam.SetTargetBuffers(renderTexture.colorBuffer, depthTexture.depthBuffer);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
        Graphics.Blit(source, outputTexture, postProcessMaterial);
    }
}
