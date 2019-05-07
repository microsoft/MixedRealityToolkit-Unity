using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveLoadTesting : MonoBehaviour
{
    MixedRealityToolkit[] toolkits = null;
    int numActive = 0;

    IEnumerator Start()
    {
        // Load all 3 instances
        AsyncOperation loadOp1 = SceneManager.LoadSceneAsync("MRTKInstance1", LoadSceneMode.Additive);
        AsyncOperation loadOp2 = SceneManager.LoadSceneAsync("MRTKInstance2", LoadSceneMode.Additive);
        AsyncOperation loadOp3 = SceneManager.LoadSceneAsync("MRTKInstance3", LoadSceneMode.Additive);

        loadOp1.allowSceneActivation = true;
        loadOp2.allowSceneActivation = true;
        loadOp3.allowSceneActivation = true;

        while (!(loadOp1.isDone && loadOp2.isDone && loadOp3.isDone))
            yield return null;

        Debug.Log("--Testing 3 loaded instances--");

        toolkits = FindAllToolkits();
        numActive = 0;
        foreach (MixedRealityToolkit toolkit in toolkits)
        {
            if (toolkit.IsActiveInstance)
                numActive++;
        }

        // Test results
        Debug.Assert(toolkits.Length == 3, "Num toolkits: " + toolkits.Length);
        Debug.Assert(numActive == 1, "Num active: " + numActive);
        
        yield return new WaitForSeconds(5f);

        // Unload first instance
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("MRTKInstance1");
        while (!unloadOp.isDone)
            yield return null;

        Debug.Log("--Testing 2 loaded instances--");

        // Test to see if another instance was made active
        toolkits = FindAllToolkits();
        numActive = 0;
        foreach (MixedRealityToolkit toolkit in toolkits)
        {
            if (toolkit.IsActiveInstance)
                numActive++;
        }

        // Test results
        Debug.Assert(toolkits.Length == 2, "Num toolkits: " + toolkits.Length);
        Debug.Assert(numActive == 1, "Num active: " + numActive);

        yield return new WaitForSeconds(5f);

        // Unload other instances
        unloadOp = SceneManager.UnloadSceneAsync("MRTKInstance2");
        while (!unloadOp.isDone)
            yield return null;

        unloadOp = SceneManager.UnloadSceneAsync("MRTKInstance3");
        while (!unloadOp.isDone)
            yield return null;

        Debug.Log("--Testing 0 loaded instances--");

        // There should be no toolkits left
        // Test to see if another instance was made active
        toolkits = FindAllToolkits();
        numActive = 0;
        foreach (MixedRealityToolkit toolkit in toolkits)
        {
            if (toolkit.IsActiveInstance)
                numActive++;
        }

        // Test results
        Debug.Assert(toolkits.Length == 0, "Num toolkits: " + toolkits.Length);
        Debug.Assert(numActive == 0, "Num active: " + numActive);

        yield return new WaitForSeconds(5f);

        Debug.Log("--Done--");
    }

    private MixedRealityToolkit[] FindAllToolkits()
    {
        List<MixedRealityToolkit> toolkits = new List<MixedRealityToolkit>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            foreach (GameObject rootGameObject in loadedScene.GetRootGameObjects())
            {
                FindToolkits(rootGameObject.transform, toolkits);
            }
        }
        return toolkits.ToArray();
    }

    private void FindToolkits (Transform parent, List<MixedRealityToolkit> toolkits)
    {
        MixedRealityToolkit mrtk = parent.GetComponent<MixedRealityToolkit>();
        if (mrtk != null)
            toolkits.Add(mrtk);

        foreach (Transform child in parent)
        {
            FindToolkits(child, toolkits);
        }
    }
}
