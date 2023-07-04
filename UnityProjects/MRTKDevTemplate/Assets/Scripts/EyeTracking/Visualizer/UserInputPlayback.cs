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
    using UnityEngine.Events;

    /// <summary>
    /// Allows the user to playback a recorded log file of eye gaze interactions with a heat map.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputPlayback")]
    public class UserInputPlayback : MonoBehaviour
    {
        [Tooltip("Filename of the log file to be replayed")]
        [SerializeField]
        private string playbackLogFilename = string.Empty;

        [Tooltip("References to the heatmaps to show playback of eye gaze interactions")]
        [SerializeField]
        private DrawOnTexture[] heatmapReferences = null;

        [Tooltip("Displays status updates for file loading and replay status")]
        [SerializeField]
        private TextMeshPro loadingUpdateStatusText;

        [Tooltip("Event that is fired when playback of a log file has completed")]
        [SerializeField]
        private UnityEvent onPlaybackCompleted;

        private IReadOnlyList<string> loggedLines;
        private Coroutine showHeatmapCoroutine;

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
            LoadingStatus_Hide();
        }

        private void Update()
        {
            if (IsPlaying && IsDataLoaded && counter < loggedLines.Count - 1)
            {
                UpdateLoadingStatus(counter, loggedLines.Count);
            }
        }

        private void ResetCurrentStream()
        {
            loggedLines = null;
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
                loadingUpdateStatusText.text = "File exists: " + uwpFileName;
                await UWP_ReadData(uwpLogFile);
            }
            else
            {
                loadingUpdateStatusText.text = "Error: File does not exist! " + uwpFileName;
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
                loadingUpdateStatusText.text = "Error: File could not be found.";
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
            loadingUpdateStatusText.text = "Finished loading log file. Lines: " + loggedLines.Count;
            return true;
        }
#endif

        private bool IsDataLoaded
        {
            get { return loggedLines != null && loggedLines.Count > 0; }
        }

        private void LoadNewFile(string filename)
        {
            ResetCurrentStream();

            try
            {
                if (!File.Exists(filename))
                {
                    loadingUpdateStatusText.text += "Error: Playback log file does not exist! ->>   " + filename + "   <<";
                    Log(("Error: Playback log file does not exist! ->" + filename + "<"));
                    Debug.LogError("Playback log file does not exist! " + filename);
                    return;
                }

                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                loggedLines = File.ReadAllLines(filename);
                loadingUpdateStatusText.text = "Finished loading log file. Lines: " + loggedLines.Count;
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
        private void Load()
        {
#if WINDOWS_UWP
            LoadInUWP();
#else
            LoadInEditor();
#endif
        }
        
        private void LoadInEditor()
        {
            loadingUpdateStatusText.text = "Load: " + FileName;
            LoadNewFile(FileName);
        }

#if WINDOWS_UWP
        private async void LoadInUWP()
        {
            loadingUpdateStatusText.text = "[Load.1] " + FileName;
            await UWP_Load();
        }
#endif
        /// <summary>
        /// True while the GameObject is playing back eye gaze data from the log file.
        /// </summary>
        public bool IsPlaying
        {
            private set;
            get;
        }

        /// <summary>
        /// Begins playback of the eye gaze data.
        /// </summary>
        public void StartPlayback()
        {
            ShowHeatmap();
        }

        private string FileName
        {
            get
            {
#if WINDOWS_UWP
                return "C:\\Data\\Users\\DefaultAccount\\Music\\MRTK_ET_Demo\\tester\\" + playbackLogFilename;
#else
                return Application.persistentDataPath + "/" + playbackLogFilename;
#endif
            }
        }
        
        private void ShowHeatmap()
        {
            if (heatmapReferences != null && heatmapReferences.Length > 0)
            {
                // First, let's load the data
                if (!IsDataLoaded)
                {
                    Load();
                }

                if (showHeatmapCoroutine != null)
                    StopCoroutine(showHeatmapCoroutine);

                IsPlaying = true;
                counter = 0;
                showHeatmapCoroutine = StartCoroutine(PopulateHeatmap());
            }
        }

        /// <summary>
        /// Pauses playback of the eye gaze data.
        /// </summary>
        public void PauseHeatmapPlayback()
        {
            IsPlaying = false;
        }

        /// <summary>
        /// Resumes playback of the eye gaze data.
        /// </summary>
        public void ResumeHeatmapPlayback()
        {
            IsPlaying = true;
        }
        
        private int counter = 0;
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
                    if (IsPlaying)
                    {
                        yield return null;
                    }
                    else
                    {
                        yield return new WaitUntil(() => IsPlaying);
                    }
                }
            }

            PlaybackCompleted();
        }

        private void PlaybackCompleted()
        {
            IsPlaying = false;
            ResetCurrentStream();
            onPlaybackCompleted.Invoke();
        }
        
        private void LoadingStatus_Hide()
        {
            if (loadingUpdateStatusText != null)
            {
                loadingUpdateStatusText.gameObject.SetActive(true);
            }
        }

        private void LoadingStatus_Show()
        {
            if (loadingUpdateStatusText != null)
            {
                if (!loadingUpdateStatusText.gameObject.activeSelf)
                    loadingUpdateStatusText.gameObject.SetActive(true);
            }
        }

        private void Log(string msg)
        {
            if (loadingUpdateStatusText != null)
            {
                LoadingStatus_Show();
                loadingUpdateStatusText.text = string.Format($"{msg}");
            }
        }

        private void UpdateLoadingStatus(int now, int total)
        {
            if (loadingUpdateStatusText != null)
            {
                LoadingStatus_Show();
                loadingUpdateStatusText.text = $"Replay status: {100f * now / total:0}%";
            }
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
    }
}
