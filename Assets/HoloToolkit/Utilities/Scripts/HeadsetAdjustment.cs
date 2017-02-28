using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadsetAdjustment : MonoBehaviour, IInputClickHandler, ISpeechHandler {

    public string NextSceneName;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Clicked");
        GotoNextScene();
    }

    private void GotoNextScene()
    {
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
        Debug.Log("Speech command recognized");
        GotoNextScene();
    }

    // Use this for initialization
    void Start () {
        InputManager.Instance.AddGlobalListener(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
