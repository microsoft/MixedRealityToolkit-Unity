using UnityEngine;
using HoloToolkit.Unity;

public class AdaptiveQualityExample : MonoBehaviour
{
	public TextMesh text;
	public AdaptiveQuality quality;

	void Update ()
	{
		text.text = string.Format("GPUTime:{0:N2}\nQualityLevel:{1}\nViewportScale:{2:N2}", 
			GpuTiming.GetTime("Frame") * 1000.0f,
			quality.QualityLevel,
			UnityEngine.VR.VRSettings.renderViewportScale);
	}
}
