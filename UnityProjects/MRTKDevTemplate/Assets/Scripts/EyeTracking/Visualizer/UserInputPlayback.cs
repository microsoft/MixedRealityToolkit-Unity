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

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputPlayback")]
    public class UserInputPlayback : MonoBehaviour
    {
        [SerializeField]
        private string _customFilename = string.Empty;

        public InputPointerVisualizer _EyeGazeVisualizer;
        public InputPointerVisualizer _HeadGazeVisualizer;

        [SerializeField]
        private DrawOnTexture[] heatmapRefs = null;

        private List<string> _loggedLines;

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
            _loggedLines = new List<string>();
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
                _LoadingUpdateStatusText.text = "File exists: " + uwpFileName;
                await UWP_ReadData(uwpLogFile);
            }
            else
            {
                _LoadingUpdateStatusText.text = "Error: File does not exist! " + uwpFileName;
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
                _LoadingUpdateStatusText.text = "Error: File could not be found.";
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
                _loggedLines.Add(streamReader.ReadLine());
            }
            _LoadingUpdateStatusText.text = "Finished loading log file. Lines: " + _loggedLines.Count;
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
                    _LoadingUpdateStatusText.text += "Error: Playback log file does not exist! ->>   " + filename + "   <<";
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
                    _loggedLines.Add(line);
                }
                _LoadingUpdateStatusText.text = "Finished loading log file. Lines: " + _loggedLines.Count;
                Log(("Finished loading log file. Lines: " + _loggedLines.Count));
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
            _LoadingUpdateStatusText.text = "Load: " + FileName;
            LoadNewFile(FileName);
        }

#if WINDOWS_UWP
        private async void LoadInUWP()
        {
            _LoadingUpdateStatusText.text = "[Load.1] " + FileName;
            await UWP_Load();
        }
#endif

        private string FileName
        {
            get
            {
#if WINDOWS_UWP
                return "C:\\Data\\Users\\DefaultAccount\\Music\\MRTK_ET_Demo\\tester\\" + _customFilename;
#else
                return Application.persistentDataPath + "/" + _customFilename;
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
            _lastUpdatedTime = DateTime.UtcNow;
            _deltaTimeToUpdateInMs = 0f;
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

        public void ShowAllAndFreeze()
        {
            Debug.Log(">> ShowAllAndFreeze");
            ShowAllAndFreeze(_EyeGazeVisualizer);
            //ShowAllAndFreeze(_HeadGazeVisualizer, InputSourceType.Head);
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
                _LoadingUpdateStatusText.text = "[Load.2] " + FileName;
                bool result = AsyncHelpers.RunSync<bool>(() => UWP_Load());
                _LoadingUpdateStatusText.text = "[Load.2] Done. ";
#endif
                _LoadingUpdateStatusText.text = "Loading done. Visualize data...";

                // Let's unpause the visualizer to make updates
                visualizer.UnpauseApp();

                // Let's make sure that the visualizer will show all data at once
                visualizer.AmountOfSamples = _loggedLines.Count;

                // Now let's populate the visualizer
                for (int i = 0; i < _loggedLines.Count; i++)
                {
                    string[] split = _loggedLines[i].Split(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToCharArray());
                    UpdateEyeGazeSignal(split, visualizer);
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

                _counter = 0;

                StartCoroutine(UpdateStatus(0.2f));
                StartCoroutine(PopulateHeatmap());
            }
        }

        public TextMeshPro _LoadingUpdateStatusText;

        private void LoadingStatus_Hide()
        {
        }

        private void LoadingStatus_Show()
        {
            if (_LoadingUpdateStatusText != null)
            {
                if (!_LoadingUpdateStatusText.gameObject.activeSelf)
                    _LoadingUpdateStatusText.gameObject.SetActive(true);
            }
        }

        private void UpdateLoadingStatus(int now, int total)
        {
            if (_LoadingUpdateStatusText != null)
            {
                LoadingStatus_Show();
                _LoadingUpdateStatusText.text = String.Format($"Replay status: {((100f * now) / total):0}%");
            }
        }

        private void Log(string msg)
        {
            if (_LoadingUpdateStatusText != null)
            {
                LoadingStatus_Show();
                _LoadingUpdateStatusText.text = String.Format($"{msg}");
            }
        }


        private static int _counter = 0;
        private IEnumerator PopulateHeatmap()
        {
            const float maxTargetingDistInMeters = 10f;

            // Now let's populate the visualizer
            for (int i = 0; i < _loggedLines.Count; i++)
            {
                Ray? currentPointingRay = GetEyeRay(_loggedLines[i].Split(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToCharArray()));
                if (currentPointingRay.HasValue)
                {
                    if (Physics.Raycast(currentPointingRay.Value, out RaycastHit hit, maxTargetingDistInMeters))
                    {
                        for (int hi = 0; hi < heatmapRefs.Length; hi++)
                        {
                            if (heatmapRefs[hi].gameObject == hit.collider.gameObject)
                            {
                                heatmapRefs[hi].DrawAtThisHitPos(hit.point);
                            }
                        }
                    }
                    _counter = i;
                    yield return null;
                }
            }
        }

        private IEnumerator UpdateStatus(float updateFrequency)
        {
            while (_counter < _loggedLines.Count - 1)
            {
                UpdateLoadingStatus(_counter, _loggedLines.Count);
                yield return new WaitForSeconds(updateFrequency);
            }
            LoadingStatus_Hide();
        }

        private Ray? GetEyeRay(IReadOnlyList<string> split)
        {
            return GetRay(split[9], split[10], split[11], split[12], split[13], split[14]);
        }

        private Ray? GetRay(string originX, string originY, string originZ, string dirX, string dirY, string dirZ)
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
            get { return (_loggedLines.Count > 0); }
        }

        DateTime _lastUpdatedTime;
        float _deltaTimeToUpdateInMs;
        float _lastTimestampInMs;

        private void UpdateTimestampForNextReplay(IReadOnlyList<string> split)
        {
            if (float.TryParse(split[2], out float timestampInMs))
            {
                _lastUpdatedTime = DateTime.UtcNow;
                _deltaTimeToUpdateInMs = timestampInMs - _lastTimestampInMs;
                _lastTimestampInMs = timestampInMs;
            }
        }

        private void UpdateEyeGazeSignal(IReadOnlyList<string>split, InputPointerVisualizer vizz)
        {
            Ray? ray = GetEyeRay(split);
            if (ray.HasValue)
            {
                vizz.UpdateDataVis(new Ray(ray.Value.origin, ray.Value.direction));
            }
        }

        /*
        private void UpdateHeadGazeSignal(string[] split, InputPointerVisualizer vizz)
        {
        }

        private void UpdateTargetingSignal(string ox, string oy, string oz, string dirx, string diry, string dirz, Ray cursorRay, InputPointerVisualizer vizz)
        {
            bool isValidVec1 = TryParseStringToVector3(ox, oy, oz, out Vector3 origin);
            bool isValidVec2 = TryParseStringToVector3(dirx, diry, dirz, out Vector3 dir);

            if (isValidVec1 && isValidVec2)
            {
                cursorRay = new Ray(origin, dir);
                vizz.UpdateDataVis(cursorRay);
            }
        }
        */
        
        private int _replayIndex = 0;
        private bool _replayNotStartedYet = true;
        public int NumSamples = 30;

        [SerializeField, Range(0f, 10.0f)]
        private float _replaySpeed = 1f;

        private void Update()
        {
            // First, let's checked if we're paused
            if (IsPlaying)
            {
                // Second, let's check that it's time to add a new data point
                if ((DateTime.UtcNow - _lastUpdatedTime).TotalMilliseconds * _replaySpeed > _deltaTimeToUpdateInMs)
                {
                    PlayNext();
                }
            }
        }

        private void PlayNext()
        {
            // Have we started the replay yet? 
            if (_replayNotStartedYet)
            {
                _replayNotStartedYet = false;
                _replayIndex = 0;

                if (!DataIsLoaded)
                {
                    Load();
                }

                // Let's unpause the visualizer to make updates
                // Show only a certain amount of data at once 
                if (_EyeGazeVisualizer != null)
                {
                    _EyeGazeVisualizer.UnpauseApp();
                    _EyeGazeVisualizer.AmountOfSamples = NumSamples;
                }

                if (_HeadGazeVisualizer != null)
                {
                    _HeadGazeVisualizer.UnpauseApp();
                    _HeadGazeVisualizer.AmountOfSamples = NumSamples;
                }
            }

            // Now let's populate the visualizer step by step
            if (_replayIndex < _loggedLines.Count)
            {
                string[] split = _loggedLines[_replayIndex].Split(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToCharArray());
                UpdateEyeGazeSignal(split, _EyeGazeVisualizer);
                UpdateTimestampForNextReplay(split);
                _replayIndex++;
            }
            else
            {
                _LoadingUpdateStatusText.text = String.Format($"Replay done!");
                Pause();
                _replayNotStartedYet = true;
            }
        }
        #endregion
    }
}
