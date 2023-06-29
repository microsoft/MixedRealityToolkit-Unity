// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

#if WINDOWS_UWP
using System.Threading.Tasks;
using Windows.Storage;
#endif

namespace Microsoft.MixedReality.Toolkit.Examples
{
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputPlayback")]
    public class UserInputPlayback : MonoBehaviour
    {
        [SerializeField]
        private string customFilename = string.Empty;

        public InputPointerVisualizer EyeGazeVisualizer;
        public InputPointerVisualizer HeadGazeVisualizer;

        [SerializeField]
        private DrawOnTexture[] heatmapReferences = null;

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
                LoadingUpdateStatusText.text = "File exists: " + uwpFileName;
                await UWP_ReadData(uwpLogFile);
            }
            else
            {
                LoadingUpdateStatusText.text = "Error: File does not exist! " + uwpFileName;
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
                LoadingUpdateStatusText.text = "Error: File could not be found.";
            }

            return false;
        }

        private async Task<bool> UWP_ReadData(StorageFile logfile)
        {
            using var inputStream = await logfile.OpenReadAsync();
            using var classicStream = inputStream.AsStreamForRead();
            using var streamReader = new StreamReader(classicStream);
            while (streamReader.Peek() >= 0)
            {
                loggedLines.Add(streamReader.ReadLine());
            }
            LoadingUpdateStatusText.text = "Finished loading log file. Lines: " + loggedLines.Count;
            return true;
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
                if (!File.Exists(filename))
#endif
                {
                    LoadingUpdateStatusText.text += "Error: Playback log file does not exist! ->>   " + filename + "   <<";
                    Log(("Error: Playback log file does not exist! ->" + filename + "<"));
                    Debug.LogError("Playback log file does not exist! " + filename);
                    return;
                }

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open));
                // Read and display lines from the file until the end of the file is reached.
                while (sr.ReadLine() is { } line)
                {
                    loggedLines.Add(line);
                }
                LoadingUpdateStatusText.text = "Finished loading log file. Lines: " + loggedLines.Count;
                Log("Finished loading log file. Lines: " + loggedLines.Count);
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Debug.Log("The file could not be read:\n" + e.Message);
            }
        }

        #region Parsers
        private static bool TryParseStringToVector3(string x, string y, string z, out Vector3 vector)
        {
            if (!float.TryParse(x, out float tX))
            {
                vector = Vector3.zero;
                return false;
            }

            if (!float.TryParse(y, out float tY))
            {
                vector = Vector3.zero;
                return false;
            }

            if (!float.TryParse(z, out float tZ))
            {
                vector = Vector3.zero;
                return false;
            }

            vector = new Vector3(tX, tY, tZ);
            return true;
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
            LoadingUpdateStatusText.text = "Load: " + FileName;
            LoadNewFile(FileName);
        }

#if WINDOWS_UWP
        private async void LoadInUWP()
        {
            LoadingUpdateStatusText.text = "[Load.1] " + FileName;
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
                return Application.persistentDataPath + "/" + customFilename;
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
            EyeGazeVisualizer.gameObject.SetActive(true);
            EyeGazeVisualizer.UnpauseApp();

            if (HeadGazeVisualizer != null)
            {
                HeadGazeVisualizer.gameObject.SetActive(true);
                HeadGazeVisualizer.UnpauseApp();
            }
        }

        public void Pause()
        {
            IsPlaying = false;
            EyeGazeVisualizer.PauseApp();

            if (HeadGazeVisualizer != null)
            {
                HeadGazeVisualizer.PauseApp();
            }
        }

        public void Clear()
        {
            EyeGazeVisualizer.ResetVisualizations();
            HeadGazeVisualizer.ResetVisualizations();
        }

        public void ShowAllAndFreeze()
        {
            Debug.Log(">> ShowAllAndFreeze");
            ShowAllAndFreeze(EyeGazeVisualizer);
            //ShowAllAndFreeze(HeadGazeVisualizer, InputSourceType.Head);
            ShowHeatmap();
        }

        private void ShowAllAndFreeze(InputPointerVisualizer visualizer)
        {
            if (visualizer != null)
            {
                visualizer.gameObject.SetActive(true);

#if UNITY_EDITOR
                Load();
#elif WINDOWS_UWP
                LoadingUpdateStatusText.text = "[Load.2] " + FileName;
                bool result = AsyncHelpers.RunSync<bool>(() => UWP_Load());
                LoadingUpdateStatusText.text = "[Load.2] Done. ";
#endif
                LoadingUpdateStatusText.text = "Loading done. Visualize data...";

                // Let's unpause the visualizer to make updates
                visualizer.UnpauseApp();

                // Let's make sure that the visualizer will show all data at once
                visualizer.AmountOfSamples = loggedLines.Count;

                // Now let's populate the visualizer
                for (int i = 0; i < loggedLines.Count; i++)
                {
                    string[] split = loggedLines[i].Split(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToCharArray());
                    UpdateEyeGazeSignal(split, visualizer);
                }
                visualizer.PauseApp();
            }
        }

        private void ShowHeatmap()
        {
            if (heatmapReferences != null && heatmapReferences.Length > 0)
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

        public TextMeshPro LoadingUpdateStatusText;

        private void LoadingStatus_Hide()
        {
            if (LoadingUpdateStatusText != null)
            {
                LoadingUpdateStatusText.gameObject.SetActive(true);
            }
        }

        private void LoadingStatus_Show()
        {
            if (LoadingUpdateStatusText != null)
            {
                if (!LoadingUpdateStatusText.gameObject.activeSelf)
                    LoadingUpdateStatusText.gameObject.SetActive(true);
            }
        }

        private void UpdateLoadingStatus(int now, int total)
        {
            if (LoadingUpdateStatusText != null)
            {
                LoadingStatus_Show();
                LoadingUpdateStatusText.text = string.Format($"Replay status: {100f * now / total:0}%");
            }
        }

        private void Log(string msg)
        {
            if (LoadingUpdateStatusText != null)
            {
                LoadingStatus_Show();
                LoadingUpdateStatusText.text = string.Format($"{msg}");
            }
        }


        private static int counter = 0;
        private IEnumerator PopulateHeatmap()
        {
            const float maxTargetingDistInMeters = 10f;

            // Now let's populate the visualizer
            for (int i = 0; i < loggedLines.Count; i++)
            {
                Ray? currentPointingRay = GetEyeRay(loggedLines[i].Split(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToCharArray()));
                if (currentPointingRay.HasValue)
                {
                    if (Physics.Raycast(currentPointingRay.Value, out RaycastHit hit, maxTargetingDistInMeters))
                    {
                        for (int hi = 0; hi < heatmapReferences.Length; hi++)
                        {
                            if (heatmapReferences[hi].gameObject == hit.collider.gameObject)
                            {
                                heatmapReferences[hi].DrawAtThisHitPos(hit.point);
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

        private static Ray? GetEyeRay(IReadOnlyList<string> split)
        {
            return GetRay(split[9], split[10], split[11], split[12], split[13], split[14]);
        }

        private static Ray? GetRay(string originX, string originY, string originZ, string dirX, string dirY, string dirZ)
        {
            bool isValidVec1 = TryParseStringToVector3(originX, originY, originZ, out Vector3 origin);
            bool isValidVec2 = TryParseStringToVector3(dirX, dirY, dirZ, out Vector3 dir);

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
            get { return loggedLines.Count > 0; }
        }

        private DateTime lastUpdatedTime;
        private float deltaTimeToUpdateInMs;
        private float lastTimestampInMs;

        private void UpdateTimestampForNextReplay(IReadOnlyList<string> split)
        {
            if (float.TryParse(split[2], out float timestampInMs))
            {
                lastUpdatedTime = DateTime.UtcNow;
                deltaTimeToUpdateInMs = timestampInMs - lastTimestampInMs;
                lastTimestampInMs = timestampInMs;
            }
        }

        private static void UpdateEyeGazeSignal(IReadOnlyList<string>split, InputPointerVisualizer visualizer)
        {
            Ray? ray = GetEyeRay(split);
            if (ray.HasValue)
            {
                visualizer.UpdateDataVis(new Ray(ray.Value.origin, ray.Value.direction));
            }
        }

        private int replayIndex = 0;
        private bool replayNotStartedYet = true;
        public int NumSamples = 30;

        [SerializeField, Range(0f, 10.0f)]
        private float replaySpeed = 1f;

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
                if (EyeGazeVisualizer != null)
                {
                    EyeGazeVisualizer.UnpauseApp();
                    EyeGazeVisualizer.AmountOfSamples = NumSamples;
                }

                if (HeadGazeVisualizer != null)
                {
                    HeadGazeVisualizer.UnpauseApp();
                    HeadGazeVisualizer.AmountOfSamples = NumSamples;
                }
            }

            // Now let's populate the visualizer step by step
            if (replayIndex < loggedLines.Count)
            {
                string[] split = loggedLines[replayIndex].Split(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToCharArray());
                UpdateEyeGazeSignal(split, EyeGazeVisualizer);
                UpdateTimestampForNextReplay(split);
                replayIndex++;
            }
            else
            {
                LoadingUpdateStatusText.text = "Replay done!";
                Pause();
                replayNotStartedYet = true;
            }
        }
        #endregion
    }
}
