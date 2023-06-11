using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text;

public class ModelSpawner : MonoBehaviour
{
    //[Serializable]
    //public struct SpawnTest
    //{
    //    public string TestName;
    //    public GameObject ModelToSpawn;
    //    public float ModelSize;
    //    public GameObject Parent;
    //    public int StepCount;
    //    public bool Instantiate;
    //}

    [SerializeField]
    private GameObject model;

    [SerializeField]
    private TextMeshProUGUI label;

    [SerializeField]
    private TextMeshProUGUI framerateText;

    [SerializeField]
    private TextMeshProUGUI resultsText;

    [SerializeField]
    private float secondsBetweenFramerateUpdates = 0.25f;

    [SerializeField]
    private GameObject canvasParent;

    [SerializeField]
    private float offset = 0;

    [SerializeField]
    private int columns = 20;

    [SerializeField]
    private int rows = 10;

    [SerializeField]
    private int startCount = 0;

    //[SerializeField]
    private int targetLowFramerate = 60;

    //[SerializeField]
    //public SpawnTest[] Tests;

    private float secondsSinceLastFramerateUpdate = 0.0f;
    private int currentCount = 0;
    private List<GameObject> models = new List<GameObject>();
    private bool testComplete = true;
    StreamWriter writer;
    //StringBuilder resultsStringBuilder = new StringBuilder(8192);
    string filePath;
    private int frameCount = 0;
    private readonly System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private float samplePeriod = 0.1f;
    public float FrameRate = 0f;
    //private int currentTestIdx = -1;
    //private SpawnTest currentTest;
    private int yRank = 0;
    private float yOffset = 0.0f;
    private int zRank = 0;
    private float zOffset = 0.0f;

    private void Start()
    {
        if (columns == 0)
        {
            columns = 20;
        }

    }

    private void OnDestroy()
    {
        stopwatch.Stop();
    }

    //private void WriteResults()
    //{
    //    Debug.Log($"Results sb size: {resultsStringBuilder.Length}");
    //    filePath = $"{Application.persistentDataPath}\\{currentTest.TestName}.csv";
    //    using (StreamWriter sw = new StreamWriter(filePath))
    //    {
    //        sw.Write(resultsStringBuilder.ToString());
    //        sw.Flush();
    //    }

    //    resultsStringBuilder.Clear();
    //}

    public void StartNextTest()
    {
        resultsText.text = string.Empty;
        lowFramerateFramecount = 0;
        SetModelCount(0);
        testComplete = false;

        stopwatch.Start();
    }

    private void LateUpdate()
    {
        secondsSinceLastFramerateUpdate += Time.deltaTime;
        FrameRate = (int)(1.0f / Time.smoothDeltaTime);
        if (secondsSinceLastFramerateUpdate >= secondsBetweenFramerateUpdates)
        {
            framerateText.text = FrameRate.ToString();
            secondsSinceLastFramerateUpdate = 0;
        }
    }

    public void AddModel()
    {
        SetModelCount(currentCount + 1);
    }

    public void RemoveModel()
    {
        SetModelCount(currentCount - 1);
    }

    private int frameWait = 10;
    private int lowFramerateFramecount = 0;

    private void Update()
    {
        if (testComplete)
        {
            return;
        }

        if (FrameRate < targetLowFramerate)
        {
            lowFramerateFramecount++;
        }

        if (currentCount < 800 && lowFramerateFramecount < 20)
        {
            //resultsStringBuilder.Append(currentCount);
            //resultsStringBuilder.Append(",");
            //resultsStringBuilder.AppendLine(FrameRate.ToString());

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
        }
    }

    public void SetModelCount(int count)
    {
        if (count < currentCount)
        {
            // delete models
            while (count < currentCount && models.Count > 0)
            {
                Destroy(models[models.Count - 1]);
                models.RemoveAt(models.Count - 1);
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
                models.Add(m);
                currentCount++;
            }
        }

        label.text = $"Count: {currentCount}";
    }
}
