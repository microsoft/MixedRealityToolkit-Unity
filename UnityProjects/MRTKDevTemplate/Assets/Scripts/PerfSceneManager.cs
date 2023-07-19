// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Manager for the performance evaluation scene.
    /// </summary>
    /// <remarks>
    /// This will instantiate game objects until the frame rate drops below a set limit.
    /// </remarks>
    public class PerfSceneManager : MonoBehaviour
    {
        // Reference to the description panel.  The description is disabled when the test is run.
        [SerializeField]
        private GameObject descriptionPanel;

        // Reference to the GameObject to spawn.
        [SerializeField]
        private GameObject model;

        // TextMeshPro text field for the number of objects instantiated.
        [SerializeField]
        private TextMeshProUGUI countText;

        // TextMeshPro text field for displaying the current framerate
        [SerializeField]
        private TextMeshProUGUI framerateText;

        // TextMeshPro text field for displaying the results of the test.
        [SerializeField]
        private TextMeshProUGUI resultsText;

        // How many seconds between updates of the framerate text.
        [SerializeField]
        private float secondsBetweenFramerateUpdates = 0.25f;

        // Parent for the instantiated objects.
        [SerializeField]
        private GameObject canvasParent;

        // Distance between the instantiated objects.
        [SerializeField]
        private float offset = 0;

        // How many columns to use to display the instantiated objects.
        [SerializeField]
        private int columns = 20;

        // How many rows to use to display the instantiated objects.
        [SerializeField]
        private int rows = 10;

        // The test runs until it drops below this framerate.
        [SerializeField]
        private int targetLowFramerate = 50;

        // tracking field for the framerate text update.
        private float secondsSinceLastFramerateUpdate = 0.0f;

        // Current object count.
        private int currentCount = 0;

        // List for tracking the instantiated objects.
        private List<GameObject> testObjects = new List<GameObject>();

        // Boolean for tracking the end of the test.
        private bool testComplete = true;

        // The current framerate.
        private float frameRate = 0f;

        // Which column is being filled.
        private int yRank = 0;

        // y-Axis local distance for the current instantiated object.
        private float yOffset = 0.0f;

        // Which rank in the z axis is being filled.
        private int zRank = 0;

        // x-Axis local distance for the current instantiated object.
        private float zOffset = 0.0f;

        // How many frames before instantiating the next object.
        private int frameWait = 10;

        // How many frames have had the framerate below the target.
        private int lowFramerateFrameCount = 0;

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            // prevent divide by zero
            if (columns == 0)
            {
                columns = 20;
            }
        }

        public void StartTest()
        {
            // Trigger the test
            descriptionPanel.SetActive(false);
            resultsText.text = string.Empty;
            lowFramerateFrameCount = 0;
            SetModelCount(0);
            testComplete = false;
        }

        /// <summary>
        /// A Unity event function that is called every frame after normal update functions, if this object is enabled.
        /// </summary>
        private void LateUpdate()
        {
            // Framerate calculations.
            secondsSinceLastFramerateUpdate += Time.deltaTime;
            frameRate = (int)(1.0f / Time.smoothDeltaTime);
            if (secondsSinceLastFramerateUpdate >= secondsBetweenFramerateUpdates)
            {
                framerateText.text = frameRate.ToString();
                secondsSinceLastFramerateUpdate = 0;
            }
        }

        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        private void Update()
        {
            if (testComplete)
            {
                return;
            }

            if (frameRate < targetLowFramerate)
            {
                lowFramerateFrameCount++;
            }

            if (currentCount < 2000 && lowFramerateFrameCount < 60)
            {
                if (frameWait == 0)
                {
                    int cachedCount = currentCount;
                    cachedCount++;
                    SetModelCount(cachedCount);
                    frameWait = 10;
                }
                else
                    frameWait--;
            }
            else
            {
                testComplete = true;
                resultsText.text = $"Test dropped below target framerate after {currentCount} objects.  Test complete.";
                Debug.Log(resultsText.text);
                descriptionPanel.SetActive(true);
            }
        }

        public void SetModelCount(int count)
        {
            if (count < currentCount)
            {
                // delete models
                while (count < currentCount && testObjects.Count > 0)
                {
                    Destroy(testObjects[testObjects.Count - 1]);
                    testObjects.RemoveAt(testObjects.Count - 1);
                    currentCount--;
                }
            }
            else if (count > currentCount)
            {
                // spawn object
                while (count > currentCount)
                {
                    var m = Instantiate(model);

                    m.transform.parent = canvasParent.transform;
                    m.transform.localScale = Vector3.one;

                    zRank = currentCount / (rows * columns);
                    zOffset = zRank * offset;
                    yRank = (int)(currentCount / columns);
                    yOffset = (yRank % rows) * offset;
                    m.transform.localPosition = new Vector3((currentCount % columns) * offset, yOffset, zOffset);
                    testObjects.Add(m);
                    currentCount++;
                }
            }

            countText.text = currentCount.ToString();
        }
    }
}
#pragma warning restore CS1591
