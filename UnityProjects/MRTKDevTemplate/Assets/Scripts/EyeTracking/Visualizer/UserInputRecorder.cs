// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputRecorder")]
    public class UserInputRecorder : MonoBehaviour
    {
        [SerializeField]
        private string filenameToUse = "test/folder";

        [SerializeField]
        private bool addTimestampToLogfileName = false;

        private FileInputLogger fileLogger = null;

        [SerializeField]
        private LogStructure logStructure = null;

        [SerializeField]
        private string userName = "tester";

        [SerializeField]
        private string sessionDescription = "Session00";

        private string dataFormat;
        private DateTime timerStart;

        private void OnEnable()
        {
            fileLogger = new FileInputLogger(userName, Filename);
            timerStart = DateTime.Now;
            fileLogger.AppendLog(GetHeader());
        }

        private void OnDisable()
        {
            fileLogger.Dispose();
            fileLogger = null;
        }

        private void Update()
        {
            object[] data = MergeObjArrays(GetData_Part1(), logStructure.GetData());
            fileLogger.AppendLog(string.Format(dataFormat, data));
        }

        private static string FormattedTimeStamp
        {
            get
            {
                return DateTime.Now.ToString("yMMddHHmmss");
            }
        }

        private string GetFileName()
        {
            return !string.IsNullOrEmpty(filenameToUse) ? filenameToUse : $"{sessionDescription}-{userName}";
        }

        private string Filename
        {
            get { return addTimestampToLogfileName ? FilenameWithTimestamp : FilenameNoTimestamp; }
        }

        private string FilenameWithTimestamp
        {
            get { return $"{FormattedTimeStamp}_{FilenameNoTimestamp}.csv"; }
        }

        private string FilenameNoTimestamp
        {
            get { return GetFileName() + ".csv"; }
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

        protected object[] GetData_Part1()
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

        public object[] MergeObjArrays(object[] part1, object[] part2)
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
    }
}
