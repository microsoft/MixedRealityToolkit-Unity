// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

/// <summary>
/// Adds an FPS counter to the debug panel.
/// </summary>
public class DebugPanelFPSCounter : MonoBehaviour
{

    /// <summary>
    /// Variables for an FPS counter
    /// </summary>
    private int frameCount;
    private int framesPerSecond;
    private int lastWholeTime = 0;

    // Use this for initialization
    void Start ()
    {
        DebugPanel debugPanel = DebugPanel.Instance;
        if (debugPanel != null)
        {
            DebugPanel.Instance.RegisterExternalLogCallback(GetFps);
        }
	}

    string GetFps()
    {
        // calculate the fps first 
        // (Note that we might want to do this in our update loop)
        UpdateFPS();
        return string.Format("FPS: {0}\n", framesPerSecond); ;
    }

    /// <summary>
    /// Keeps track of rough frames per second.
    /// </summary>
    void UpdateFPS()
    {
        frameCount++;
        int currentWholeTime = (int)Time.realtimeSinceStartup;
        if (currentWholeTime != lastWholeTime)
        {
            lastWholeTime = currentWholeTime;
            framesPerSecond = frameCount;
            frameCount = 0;
        }
    }
}
