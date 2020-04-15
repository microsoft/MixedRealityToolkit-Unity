// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if WINDOWS_UWP
using System.Threading.Tasks;
using Windows.Storage;
#endif

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputPlayback")]
    public class UserInputPlayback : MonoBehaviour
    {
        [SerializeField]
        private string customFilename = string.Empty;

        public InputPointerVisualizer _EyeGazeVisualizer;
        public InputPointerVisualizer _HeadGazeVisualizer;

        [SerializeField]
        private DrawOnTexture[] heatmapRefs = null;

        private List<string> loggedLines;

#if WINDOWS_UWP
        private StorageFolder uwpRootFolder = KnownFolders.MusicLibrary;
        private readonly string uwpSubFolderName = $"MRTK_ET_Demo{Path.DirectorySeparatorChar}tester";
        private readonly string uwpFileName = "mrtk_log_mostRecentET.csv";
        private StorageFolder uwpLogSessionFolder;
        private StorageFile uwpLogFile;
#endif

        private void Start()
        {
            IsPlaying = false;
            ResetCurrentStream();
            LoadingStatus_Hide();
        }

        private void ResetCurrentStream()
        {
            loggedLines = new List<string>();
        }

#if WINDOWS_UWP
        public async Task<bool> UWP_Load()
        {
            return await UWP_LoadNewFile(FileName);
        }

        public async Task<bool> UWP_LoadNewFile(string filename)
        {
            ResetCurrentStream();
            bool fileExists = await UWP_FileExists(uwpSubFolderName, uwpFileName);

            if (fileExists)
            {
                txt_LoadingUpdate.text = "File exists: " + uwpFileName;
                await UWP_ReadData(uwpLogFile);
            }
            else
            {
                txt_LoadingUpdate.text = "Error: File does not exist! " + uwpFileName;
                return false;
            }
            
            return true;
        }

        public async Task<bool> UWP_FileExists(string dir, string filename)
        {
            try
            {
                uwpLogSessionFolder = await uwpRootFolder.GetFolderAsync(dir);
                uwpLogFile = await uwpLogSessionFolder.GetFileAsync(filename);
                
                return true;
            }
            catch 
            {
                txt_LoadingUpdate.text = "Error: File could not be found.";
            }

            return false;
        }

        private async Task<bool> UWP_ReadData(StorageFile logfile)
        {
            using (var inputStream = await logfile.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream))
            {
                while (streamReader.Peek() >= 0)
                {
                    loggedLines.Add(streamReader.ReadLine());
                }
                txt_LoadingUpdate.text = "Finished loading log file. Lines: " + loggedLines.Count;
                return true;
            }
        }
#endif

        public void LoadNewFile(string filename)
        {
            ResetCurrentStream();

            try
            {
#if WINDOWS_UWP
                if (!UnityEngine.Windows.File.Exists(filename))
#else
                if (!System.IO.File.Exists(filename))
#endif
                {
                    txt_LoadingUpdate.text += "Error: Playback log file does not exist! ->>   " + filename + "   <<";
                    Log(("Error: Playback log file does not exist! ->" + filename + "<"));
                    Debug.LogError("Playback log file does not exist! " + filename);
                    return;
                }

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
                {
                    string line;
                    // Read and display lines from the file until the end of the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        loggedLines.Add(line);
                    }
                    txt_LoadingUpdate.text = "Finished loading log file. Lines: " + loggedLines.Count;
                    Log(("Finished loading log file. Lines: " + loggedLines.Count));
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Debug.Log("The file could not be read:");
                Debug.Log(e.Message);
            }
        }

        #region Parsers
        private Vector3 TryParseStringToVector3(string x, string y, string z, out bool isValid)
        {
            isValid = true;
            float tx, ty, tz;
            if (!float.TryParse(x, out tx))
            {
                tx = float.NaN;
                isValid = false;
            }

            if (!float.TryParse(y, out ty))
            {
                ty = float.NaN;
                isValid = false;
            }

            if (!float.TryParse(z, out tz))
            {
                tz = float.NaN;
                isValid = false;
            }

            return new Vector3(tx, ty, tz);
        }

        private float ParseStringToFloat(string val)
        {
            float tval;
            if (!float.TryParse(val, out tval))
                tval = float.NaN;

            return tval;
        }
        #endregion

        #region Available player actions
        public void Load()
        {
#if UNITY_EDITOR
            LoadInEditor();
#elif WINDOWS_UWP
            LoadInUWP();
#endif
        }

        private void LoadInEditor()
        {
            txt_LoadingUpdate.text = "Load: " + FileName;
            LoadNewFile(FileName);
        }

#if WINDOWS_UWP
        private async void LoadInUWP()
        {
            txt_LoadingUpdate.text = "[Load.1] " + FileName;
            await UWP_Load();
        }
#endif

        private string FileName
        {
            get
            {
#if WINDOWS_UWP
                return "C:\\Data\\Users\\DefaultAccount\\Music\\MRTK_ET_Demo\\tester\\" + customFilename;
#else
                return customFilename;
#endif
            }
        }

        public bool IsPlaying
        {
            private set;
            get;
        }

        public void Play()
        {
            IsPlaying = true;
            lastUpdatedTime = DateTime.UtcNow;
            deltaTimeToUpdateInMs = 0f;
            _EyeGazeVisualizer.gameObject.SetActive(true);
            _EyeGazeVisualizer.UnpauseApp();
            if (_HeadGazeVisualizer != null)
            {
                _HeadGazeVisualizer.gameObject.SetActive(true);
                _HeadGazeVisualizer.UnpauseApp();
            }
        }

        public void Pause()
        {
            IsPlaying = false;
            _EyeGazeVisualizer.PauseApp();

            if (_HeadGazeVisualizer != null)
            {
                _HeadGazeVisualizer.PauseApp();
            }
        }

        public void Clear()
        {
            _EyeGazeVisualizer.ResetVisualizations();
            _HeadGazeVisualizer.ResetVisualizations();
        }

        public void SpeedUp() { }

        public void SlowDown() { }

        public void ShowAllAndFreeze()
        {
            Debug.Log(">> ShowAllAndFreeze");
            ShowAllAndFreeze(_EyeGazeVisualizer, InputSourceType.Eyes);
            ShowAllAndFreeze(_HeadGazeVisualizer, InputSourceType.Head);
            ShowHeatmap();
        }
        private void ShowAllAndFreeze(InputPointerVisualizer visualizer, InputSourceType iType)
        {
            if (visualizer != null)
            {
                visualizer.gameObject.SetActive(true);

#if UNITY_EDITOR
                Load();
#elif WINDOWS_UWP
                txt_LoadingUpdate.text = "[Load.2] " + FileName;
                bool result = AsyncHelpers.RunSync<bool>(() => UWP_Load());
                txt_LoadingUpdate.text = "[Load.2] Done. ";
#endif
                txt_LoadingUpdate.text = "Loading done. Visualize data...";

                // Let's unpause the visualizer to make updates
                visualizer.UnpauseApp();

                // Let's make sure that the visualizer will show all data at once
                visualizer.AmountOfSamples = loggedLines.Count;

                // Now let's populate the visualizer
                for (int i = 0; i < loggedLines.Count; i++)
                {
                    string[] split = loggedLines[i].Split(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToCharArray());
                    if (iType == InputSourceType.Eyes)
                        UpdateEyeGazeSignal(split, visualizer);

                    if (iType == InputSourceType.Head)
                        UpdateHeadGazeSignal(split, visualizer);
                }
                visualizer.PauseApp();
            }
        }

        private void ShowHeatmap()
        {
            if ((heatmapRefs != null) && (heatmapRefs.Length > 0))
            {
                // First, let's load the data
                if (!DataIsLoaded)
                {
                    Load();
                }

                counter = 0;

                StartCoroutine(UpdateStatus(0.2f));
                StartCoroutine(PopulateHeatmap());
            }
        }

        public TextMesh txt_LoadingUpdate;

        private void LoadingStatus_Hide()
        {
        }

        private void LoadingStatus_Show()
        {
            if (txt_LoadingUpdate != null)
            {
                if (!txt_LoadingUpdate.gameObject.activeSelf)
                    txt_LoadingUpdate.gameObject.SetActive(true);
            }
        }

        private void UpdateLoadingStatus(int now, int total)
        {
            if (txt_LoadingUpdate != null)
            {
                LoadingStatus_Show();
                txt_LoadingUpdate.text = String.Format($"Replay status: {((100f * now) / total):0}%");
            }
        }

        private void Log(string msg)
        {
            if (txt_LoadingUpdate != null)
            {
                LoadingStatus_Show();
                txt_LoadingUpdate.text = String.Format($"{msg}");
            }
        }


        private static int counter = 0;
        private IEnumerator PopulateHeatmap()
        {
            float maxTargetingDistInMeters = 10f;
            RaycastHit hit;

            // Now let's populate the visualizer
            for (int i = 0; i < loggedLines.Count; i++)
            {
                Ray? currentPointingRay = GetEyeRay(loggedLines[i].Split(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToCharArray()));
                if (currentPointingRay.HasValue)
                {
                    if (UnityEngine.Physics.Raycast(currentPointingRay.Value, out hit, maxTargetingDistInMeters))
                    {
                        for (int hi = 0; hi < heatmapRefs.Length; hi++)
                        {
                            if (heatmapRefs[hi].gameObject == hit.collider.gameObject)
                            {
                                heatmapRefs[hi].DrawAtThisHitPos(hit.point);
                            }
                        }
                    }
                    counter = i;
                    yield return null;
                }
            }
        }

        private IEnumerator UpdateStatus(float updateFrequency)
        {
            while (counter < loggedLines.Count - 1)
            {
                UpdateLoadingStatus(counter, loggedLines.Count);
                yield return new WaitForSeconds(updateFrequency);
            }
            LoadingStatus_Hide();
        }

        private IEnumerator AddToCounter(float time)
        {
            while (counter < 100)
            {
                yield return new WaitForSeconds(time);
                counter++;
            }
        }

        private Ray? GetEyeRay(string[] split)
        {
            return GetRay(split[9], split[10], split[11], split[12], split[13], split[14]);
        }

        private Ray? GetRay(string ox, string oy, string oz, string dirx, string diry, string dirz)
        {
            bool isValidVec1 = false, isValidVec2 = false;
            Vector3 origin = TryParseStringToVector3(ox, oy, oz, out isValidVec1);
            Vector3 dir = TryParseStringToVector3(dirx, diry, dirz, out isValidVec2);

            if (isValidVec1 && isValidVec2)
            {
                return new Ray(origin, dir);
            }
            return null;
        }
        #endregion

        #region Handle data replay
        private bool DataIsLoaded
        {
            get { return (loggedLines.Count > 0); }
        }

        DateTime lastUpdatedTime;
        float deltaTimeToUpdateInMs;
        float lastTimestampInMs = 0;
        private void UpdateTimestampForNextReplay(string[] split)
        {
            float timestampInMs;

            if (float.TryParse(split[2], out timestampInMs))
            {
                lastUpdatedTime = DateTime.UtcNow;
                deltaTimeToUpdateInMs = timestampInMs - lastTimestampInMs;
                lastTimestampInMs = timestampInMs;
            }
        }


        private void UpdateEyeGazeSignal(string[] split, InputPointerVisualizer vizz)
        {
            Ray? ray = GetEyeRay(split);
            if (ray.HasValue)
            {
                vizz.UpdateDataVis(new Ray(ray.Value.origin, ray.Value.direction));
            }
        }

        private void UpdateHeadGazeSignal(string[] split, InputPointerVisualizer vizz)
        {
        }

        private void UpdateTargetingSignal(string ox, string oy, string oz, string dirx, string diry, string dirz, Ray cursorRay, InputPointerVisualizer vizz)
        {
            bool isValidVec1 = false, isValidVec2 = false;
            Vector3 origin = TryParseStringToVector3(ox, oy, oz, out isValidVec1);
            Vector3 dir = TryParseStringToVector3(dirx, diry, dirz, out isValidVec2);

            if (isValidVec1 && isValidVec2)
            {
                cursorRay = new Ray(origin, dir);
                vizz.UpdateDataVis(cursorRay);
            }
        }


        private int replayIndex = 0;
        private bool replayNotStartedYet = true;
        public int nrOfSamples = 30;

        [Range(0.00f, 10.0f)]
        public float replaySpeed = 1;

        private void Update()
        {
            // First, let's checked if we're paused
            if (IsPlaying)
            {
                // Second, let's check that it's time to add a new data point
                if ((DateTime.UtcNow - lastUpdatedTime).TotalMilliseconds * replaySpeed > deltaTimeToUpdateInMs)
                {
                    PlayNext();
                }
            }
        }

        private void PlayNext()
        {
            // Have we started the replay yet? 
            if (replayNotStartedYet)
            {
                replayNotStartedYet = false;
                replayIndex = 0;

                if (!DataIsLoaded)
                {
                    Load();
                }

                // Let's unpause the visualizer to make updates
                // Show only a certain amount of data at once 
                if (_EyeGazeVisualizer != null)
                {
                    _EyeGazeVisualizer.UnpauseApp();
                    _EyeGazeVisualizer.AmountOfSamples = nrOfSamples;
                }

                if (_HeadGazeVisualizer != null)
                {
                    _HeadGazeVisualizer.UnpauseApp();
                    _HeadGazeVisualizer.AmountOfSamples = nrOfSamples;
                }
            }

            // Now let's populate the visualizer step by step
            if (replayIndex < loggedLines.Count)
            {
                string[] split = loggedLines[replayIndex].Split(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToCharArray());
                UpdateEyeGazeSignal(split, _EyeGazeVisualizer);
                UpdateHeadGazeSignal(split, _HeadGazeVisualizer);
                UpdateTimestampForNextReplay(split);
                replayIndex++;
            }
            else
            {
                txt_LoadingUpdate.text = String.Format($"Replay done!");
                Pause();
                replayNotStartedYet = true;
            }
        }
        #endregion
    }
}