using Microsoft.MixedReality.Toolkit.UI;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class ProgressIndicatorDemo : MonoBehaviour
    {
        [SerializeField]
        private GameObject progressIndicatorLoadingBarGo = null;
        [SerializeField]
        private GameObject progressIndicatorRotatingObjectGo = null;
        [SerializeField]
        private GameObject progressIndicatorRotatingOrbsGo = null;

        [SerializeField]
        private string[] loadingMessages = new string[] { 
            "First Loading Message",
            "Loading Message 1", 
            "Loading Message 2", 
            "Loading Message 3", 
            "Final Loading Message" };

        [SerializeField, Range(1f, 10f)]
        private float loadingTime = 5f;

        private IProgressIndicator progressIndicatorLoadingBar;
        private IProgressIndicator progressIndicatorRotatingObject;
        private IProgressIndicator progressIndicatorRotatingOrbs;

        public void OnClickBar()
        {
            HandleButtonClick(progressIndicatorLoadingBar);
        }

        public void OnClickRotating()
        {
            HandleButtonClick(progressIndicatorRotatingObject);
        }

        public void OnClickOrbs()
        {
            HandleButtonClick(progressIndicatorRotatingOrbs);
        }

        private async void HandleButtonClick(IProgressIndicator indicator)
        {
            await indicator.AwaitTransition();

            switch (indicator.State)
            {
                case ProgressIndicatorState.Closed:
                    OpenProgressIndicator(indicator);
                    break;

                case ProgressIndicatorState.Open:
                    await indicator.CloseAsync();
                    break;
            }
        }

        private void OnEnable()
        {
            progressIndicatorLoadingBar = progressIndicatorLoadingBarGo.GetComponent<IProgressIndicator>();
            progressIndicatorRotatingObject = progressIndicatorRotatingObjectGo.GetComponent<IProgressIndicator>();
            progressIndicatorRotatingOrbs = progressIndicatorRotatingOrbsGo.GetComponent<IProgressIndicator>();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                HandleButtonClick(progressIndicatorLoadingBar);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                HandleButtonClick(progressIndicatorRotatingObject);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                HandleButtonClick(progressIndicatorRotatingOrbs);
            }
        }

        private async void OpenProgressIndicator(IProgressIndicator indicator)
        {
            await indicator.OpenAsync();

            float timeStarted = Time.time;
            while (Time.time < timeStarted + loadingTime)
            {
                float normalizedProgress = Mathf.Clamp01((Time.time - timeStarted) / loadingTime);
                indicator.Progress = normalizedProgress;
                indicator.Message = loadingMessages[Mathf.FloorToInt(normalizedProgress * loadingMessages.Length)];

                await Task.Yield();

                switch (indicator.State)
                {
                    case ProgressIndicatorState.Open:
                        break;

                    default:
                        // The indicator was closed
                        return;
                }
            }

            await indicator.CloseAsync();
        }
    }
}