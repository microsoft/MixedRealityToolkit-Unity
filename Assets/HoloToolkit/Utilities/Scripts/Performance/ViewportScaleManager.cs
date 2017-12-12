using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Windows.Speech;

using UnityEngine.XR;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// This allows the user to dynamically adjust the percentage of 
    /// the render target that they display to. It scales it in both 
    /// spatial directions, so a viewport scale of .8 results in 36% 
    /// reduction in pixels drawn.  Unlike the changing the render 
    /// scale, this can be done after the holographic space is created 
    /// and does not actually reduce the buffer size.
    /// </summary>
    public class ViewportScaleManager : Singleton<ViewportScaleManager>
    {
        public const int MainstreamVerticalResolution = 1280;

        private float currentViewport = 1.0f;
        public float CurrentViewport { get { return currentViewport; } }
        private const float voiceControlledViewportDefault = -1;
        private volatile float voiceControlledViewport = voiceControlledViewportDefault;

        [HideInInspector]
        public int MaxVirticalResolution = 0;
        [HideInInspector]
        public int MaxHorizontalResolution = 0;

        public int CurrentVirticalResolution
        {
            get
            {
                return Mathf.RoundToInt(this.CurrentViewport * this.MaxVirticalResolution);
            }
        }
        public int CurrentHorizontalResoution
        {
            get
            {
                return Mathf.RoundToInt(this.CurrentViewport * this.MaxHorizontalResolution);
            }
        }

        [SerializeField]
        private float minViewportScaleFactor = 0.05f;
        [SerializeField]
        private float maxViewportScaleFactor = 1.0f;

        KeywordRecognizer keywordRecognizer = null;
        Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

        protected override void InitializeInternal()
        {
            var setViewport = XRSettings.renderViewportScale;
            if (setViewport != 0)
            {
                currentViewport = setViewport;
            }
            else
            {
                currentViewport = 1.0f;
            }

            FindScreenResolution();

            //Usefull for HoloLens, make sure to turn on mic capabilities in order to use it
            //SetupKeywords();
        }

        public void SetupKeywords()
        {
            // Viewport kewords
            for (int i = 1; i < 10; ++i)
            {
                float v = (float)i / 10.0f;
                keywords.Add("Set viewport to point " + i, () =>
                {
                    voiceControlledViewport = v;
                });

                keywords.Add("Set viewport to point " + i + " five", () =>
                {
                    voiceControlledViewport = v + 0.05f;
                });
            }

            keywords.Add("Set viewport to one", () =>
            {
                voiceControlledViewport = 1.0f;
            });

            keywords.Add("Set viewport to point zero five", () =>
            {
                voiceControlledViewport = 1.0f;
            });

            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

            // Register a callback for the KeywordRecognizer and start recognizing!
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Start();
        }

        void Update()
        {
            float newViewport;
            if (voiceControlledViewport != voiceControlledViewportDefault)
            {
                newViewport = voiceControlledViewport;
                voiceControlledViewport = voiceControlledViewportDefault;
            }
            else
            {
                var viewportInt = System.Convert.ToInt32(currentViewport * 100);
                viewportInt -= viewportInt % 5;
                newViewport = System.Convert.ToSingle(viewportInt) / 100;

                if (AdaptivePerformance.Instance.EnableDebugKeys)
                {
                    if (Input.GetKeyDown(KeyCode.LeftBracket))
                    {
                        newViewport -= 0.05f;
                    }

                    if (Input.GetKeyDown(KeyCode.RightBracket))
                    {
                        newViewport += 0.05f;
                    }

                    if (Input.GetKeyDown(KeyCode.M))
                    {
                        SetViewportToImitateMainstream();
                    }
                }
            }

#if UNITY_EDITOR
            FindScreenResolution();
#endif

            SetViewport(newViewport);
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            System.Action keywordAction;
            if (keywords.TryGetValue(args.text, out keywordAction))
            {
                keywordAction.Invoke();
            }
        }

        public void SetViewport(float val)
        {
            var newViewport = Mathf.Clamp(val, minViewportScaleFactor, maxViewportScaleFactor);
            if (newViewport != currentViewport)
            {
                Debug.Log("Setting viewport to " + newViewport);
                XRSettings.renderViewportScale = newViewport;
                currentViewport = newViewport;
            }
        }

        public void FindScreenResolution()
        {
            var screenPoint = Camera.main.ViewportToScreenPoint(new Vector3(1.0f, 1.0f, Camera.main.nearClipPlane));
            this.MaxVirticalResolution = Mathf.RoundToInt(screenPoint.y);
            this.MaxHorizontalResolution = Mathf.RoundToInt(screenPoint.x);
        }

        public void SetVirticalResolution(int val)
        {
            if (val <= 0)
            {
                Debug.LogError("Attempting to set virtical resoution to " + val + "! :O");
                return;
            }

            if (val > this.MaxVirticalResolution)
            {
                Debug.LogError("Attempting to set virtical resoution to " + val + " when max is " + this.MaxVirticalResolution
                    + "\r\nSetting to Max");
                val = this.MaxVirticalResolution;
            }

            this.SetViewport(val / (float)this.MaxVirticalResolution);
        }

        /// <summary>
        /// When your target FPS is 60 (you are running on a WindowMR badged computer),
        /// your app will load with a smaller render scale, this simulates that effect.
        /// </summary>
        public void SetViewportToImitateMainstream()
        {
            this.SetVirticalResolution(MainstreamVerticalResolution);
        }
    }
}
