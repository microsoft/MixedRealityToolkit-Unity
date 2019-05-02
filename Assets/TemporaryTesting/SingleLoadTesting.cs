using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleLoadTesting : MonoBehaviour
{
    MixedRealityToolkit[] toolkits = null;
    int numActive = 0;

    IEnumerator Start()
    {
        // Make sure we don't get destroyed by single load
        DontDestroyOnLoad(transform);

        // Load all 3 instances one after the other
        AsyncOperation loadOp1 = SceneManager.LoadSceneAsync("MRTKInstance1", LoadSceneMode.Single);
        while (!loadOp1.isDone)
            yield return null;

        Debug.Log("--Testing 1 loaded instances--");

        toolkits = FindAllToolkits();
        numActive = 0;
        foreach (MixedRealityToolkit toolkit in toolkits)
        {
            if (toolkit.IsActiveInstance)
                numActive++;
        }

        // Test results
        Debug.Assert(toolkits.Length == 1, "Num toolkits: " + toolkits.Length);
        Debug.Assert(numActive == 1, "Num active: " + numActive);

        yield return new WaitForSeconds(5f);
        
        AsyncOperation loadOp2 = SceneManager.LoadSceneAsync("MRTKInstance2", LoadSceneMode.Single);
        while (!loadOp2.isDone)
            yield return null;

        Debug.Log("--Testing 1 loaded instances--");

        toolkits = FindAllToolkits();
        numActive = 0;
        foreach (MixedRealityToolkit toolkit in toolkits)
        {
            if (toolkit.IsActiveInstance)
                numActive++;
        }

        // Test results
        Debug.Assert(toolkits.Length == 1, "Num toolkits: " + toolkits.Length);
        Debug.Assert(numActive == 1, "Num active: " + numActive);

        yield return new WaitForSeconds(5f);

        AsyncOperation loadOp3 = SceneManager.LoadSceneAsync("MRTKInstance3", LoadSceneMode.Single);
        while (!loadOp2.isDone)
            yield return null;

        Debug.Log("--Testing 1 loaded instances--");

        toolkits = FindAllToolkits();
        numActive = 0;
        foreach (MixedRealityToolkit toolkit in toolkits)
        {
            if (toolkit.IsActiveInstance)
                numActive++;
        }

        // Test results
        Debug.Assert(toolkits.Length == 1, "Num toolkits: " + toolkits.Length);
        Debug.Assert(numActive == 1, "Num active: " + numActive);

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

    private void FindToolkits(Transform parent, List<MixedRealityToolkit> toolkits)
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
