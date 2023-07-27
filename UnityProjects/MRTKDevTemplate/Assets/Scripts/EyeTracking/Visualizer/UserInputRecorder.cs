// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Allows the user to record a log file of eye gaze interactions for playback at another time.
    /// </summary>
    /// <remarks>
    /// The log file is a CSV file which is created and written to while this behaviour is enabled.
    /// </remarks>
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputRecorder")]
    public class UserInputRecorder : MonoBehaviour
    {
        [Tooltip("File name segment appended to the log file name")]
        [SerializeField]
        private string filenameToUse = "test";

        [Tooltip("Prepends a timestamp to the log file name if enabled")]
        [SerializeField]
        private bool addTimestampToLogFileName = false;

        [Tooltip("The log structure to gather eye gaze samples from")]
        [SerializeField]
        private LogStructure logStructure = null;

        [Tooltip("User name added to the log structure, and added to UWP file names and folder structure")]
        [SerializeField]
        private string userName = "tester";

        [Tooltip("Session description added to UWP file names")]
        [SerializeField]
        private string sessionDescription = "Session00";

        [Tooltip("Displays status updates for file loading and replay status")]
        [SerializeField]
        private TextMeshPro recordingUpdateStatusText;

        private FileInputLogger fileLogger = null;
        private string dataFormat;
        private DateTime timerStart;

        private void OnEnable()
        {
            fileLogger = new FileInputLogger(Filename);
            timerStart = DateTime.Now;

            try
            {
                fileLogger.AppendLog(GetHeader());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                DisplayMessage($"Failed to write log header ({ex.Message}");
            }
        }

        private void OnDisable()
        {
            fileLogger.Dispose();
            fileLogger = null;
        }

        private void Update()
        {
            object[] data = MergeObjArrays(GetData_Part1(), logStructure.GetData());

            try
            {
                fileLogger.AppendLog(string.Format(dataFormat, data));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                DisplayMessage($"Failed to write to log ({ex.Message}");
            }
        }

        private static string FormattedTimeStamp
        {
            get
            {
                return DateTime.Now.ToString("yMMddHHmmss");
            }
        }

        private string Filename
        {
            get { return addTimestampToLogFileName ? FilenameWithTimestamp : FilenameNoTimestamp; }
        }
        
        private string FilenameWithTimestamp
        {
            get { return $"{FormattedTimeStamp}_{FilenameNoTimestamp}.csv"; }
        }

        private string FilenameNoTimestamp
        {
            get { return GetFileName() + ".csv"; }
        }

        private string GetFileName()
        {
            return !string.IsNullOrEmpty(filenameToUse) ? filenameToUse : $"{sessionDescription}-{userName}";
        }

        private string GetHeader()
        {
            if (logStructure != null)
            {
                string[] headerColumns = logStructure.GetHeaderColumns();
                dataFormat = GetStringFormat(headerColumns);
                return string.Format(dataFormat, headerColumns);
            }

            return "";
        }

        private object[] GetData_Part1()
        {
            object[] data = {
                // UserId
                userName,
                // SessionType
                sessionDescription,
                // Timestamp
                (DateTime.UtcNow - timerStart).TotalMilliseconds
            };

            return data;
        }

        private object[] MergeObjArrays(object[] part1, object[] part2)
        {
            object[] data = new object[part1.Length + part2.Length];
            part1.CopyTo(data, 0);
            part2.CopyTo(data, part1.Length);
            return data;
        }

        private static string GetStringFormat(IReadOnlyCollection<object> data)
        {
            StringBuilder strFormat = new StringBuilder();
            for (int i = 0; i < data.Count - 1; i++)
            {
                strFormat.Append("{" + i + "}" + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ");
            }
            strFormat.Append("{" + (data.Count - 1) + "}" + Environment.NewLine);
            return strFormat.ToString();
        }

        private void DisplayMessage(string message)
        {
            if (recordingUpdateStatusText != null)
            {
                recordingUpdateStatusText.text = message;
            }
        }
    }
}
