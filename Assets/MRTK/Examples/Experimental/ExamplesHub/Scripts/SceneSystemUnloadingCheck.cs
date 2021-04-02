using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.SceneUnderstanding;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSystemUnloadingCheck : MonoBehaviour
{
    public DemoSceneUnderstandingController SUController;
    private bool isCleared = false;

    // Start is called before the first frame update
    void Start()
    {
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        sceneSystem.OnWillUnloadContent += HandleSceneOperation;
    }

    private void HandleSceneOperation(IEnumerable<string> obj)
    {
        if(isCleared == false)
        {
            SUController.ClearScene();
            isCleared = true;
        }
    }
}
