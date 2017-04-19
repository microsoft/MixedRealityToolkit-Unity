using UnityEngine;
using UnityEngine.SceneManagement;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity
{
    public class HeadsetAdjustment : MonoBehaviour, IInputClickHandler, ISpeechHandler
    {
        [Tooltip("The name of the scene to load when the user is ready. If left empty, the next scene is loaded as specified in the 'Scenes in Build')")]
        public string NextSceneName;

        private void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            GotoNextScene();
        }

        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            GotoNextScene();
        }

        private void GotoNextScene()
        {
            InputManager.Instance.RemoveGlobalListener(gameObject);

            if (!string.IsNullOrEmpty(NextSceneName))
            {
                SceneManager.LoadScene(NextSceneName);
            }
            else
            {
                int sceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(sceneIndex + 1);
            }
        }
    }
}
