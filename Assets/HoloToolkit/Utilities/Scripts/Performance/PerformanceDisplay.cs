using UnityEngine;

namespace HoloToolkit.Unity
{
    public class PerformanceDisplay : MonoBehaviour
    {
        public float DisplayInterval = 0.5f;
        [SerializeField]
        private KeyCode fpsToggleKey = KeyCode.F;

        UnityEngine.UI.Text fpsGUIText;
        private bool show = true;
        int frames = 0;
        private float lastDisplayUpdate = 0;
        private bool resetNextFrame = false;


        void Awake()
        {
            fpsGUIText = GetComponent<UnityEngine.UI.Text>();

            // ensure singletons are initialized
            object a = AdaptivePerformance.Instance;
            a = ViewportScaleManager.Instance;
            a = QualityManager.Instance;
            a = PerformanceCounters.Instance;
            a = null;
        }

        void Update()
        {
            if (AdaptivePerformance.Instance.EnableDebugKeys)
            {
                if (Input.GetKeyDown(fpsToggleKey))
                {
                    show = !show;
                }
            }

            var currentTime = Time.realtimeSinceStartup;
            var elapsedTime = currentTime - lastDisplayUpdate;
            frames++;

            // Skip a frame after updating text so we don't include the text mesh update time in our FPS calculation.
            if (this.resetNextFrame)
            {
                //Reset
                lastDisplayUpdate = currentTime;
                frames = 0;
                this.resetNextFrame = false;
                return;
            }

            // only update once every display interval
            if (elapsedTime < DisplayInterval)
            {
                return;
            }

            float fps = frames / elapsedTime;

            if (fpsGUIText != null)
            {
                if (show)
                {
                    fpsGUIText.text = string.Format("({0:0.}/{1:0.} fps) v:{2:0.00} q:{3} w:{9} h:{10} {6} {7} - {8}",
                        fps, PerformanceCounters.Instance.TargetFrameRate,
                        ViewportScaleManager.Instance.CurrentViewport, QualityManager.Instance.QualityLevelName,
                        (AdaptivePerformance.Instance.AdaptivePerformanceEnabled ? "on" : "off"),
                        AdaptivePerformance.Instance.CurrentBucketId,
                        GpuWhitelist.GPUVendorName,
                        GpuWhitelist.GPUTypeName,
                        GpuWhitelist.PerformanceLevelString,
                        ViewportScaleManager.Instance.CurrentHorizontalResoution,
                        ViewportScaleManager.Instance.CurrentVirticalResolution);
                    if (PerformanceCounters.Instance.GpuTimeEnabled)
                    {
                        fpsGUIText.text = string.Format("GPU Time:{0:0.00} ms {1}",
                        PerformanceCounters.Instance.LastFrameGpuTime * 1000, fpsGUIText.text);
                    }
                }
                else
                {
                    fpsGUIText.text = "";
                }
            }

            // Skip a frame after updating text so we don't include the text mesh update time in our FPS calculation.
            this.resetNextFrame = true;
        }
    }
}
