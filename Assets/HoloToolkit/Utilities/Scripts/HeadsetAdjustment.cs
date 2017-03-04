using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadsetAdjustment : MonoBehaviour, IInputClickHandler, ISpeechHandler {

    [Tooltip("The name of the scene to load when the user is ready. If left empty, the next scene is loaded as specified in the 'Scenes in Build')")]
    public string NextSceneName;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        GotoNextScene();
    }

    private void GotoNextScene()
    {
        InputManager.Instance.RemoveGlobalListener(this.gameObject);
        if (!string.IsNullOrEmpty(NextSceneName))
        {
            SceneManager.LoadScene(NextSceneName);
        }
        else
        {
            var sceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneIndex + 1);
        }
    }

    public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
    {
        GotoNextScene();
    }

    // Use this for initialization
    private void Start ()
    {
        InputManager.Instance.AddGlobalListener(this.gameObject);
	}	
}
