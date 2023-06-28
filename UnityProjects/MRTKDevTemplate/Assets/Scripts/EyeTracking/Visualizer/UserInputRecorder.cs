// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    [AddComponentMenu("Scripts/MRTK/Examples/UserInputRecorder")]
    public class UserInputRecorder : CustomInputLogger
    {
        public string FilenameToUse = $"test{Path.AltDirectorySeparatorChar}folder";

        [SerializeField]
        private LogStructure logStructure = null;

        private bool automaticLogging = true;

        #region Singleton
        private static UserInputRecorder instance;
        public static UserInputRecorder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UserInputRecorder>();
                }
                return instance;
            }
        }
        #endregion

        public override string GetHeader()
        {
            if (logStructure != null)
            {
                string[] headerColumns = logStructure.GetHeaderColumns();
                string headerFormat = GetStringFormat(headerColumns);
                return string.Format(headerFormat, headerColumns);
            }
            else
                return "";
        }

        // Todo: Put into BasicLogger?
        protected object[] GetData_Part1()
        {
            object[] data = new object[]
            {
                // UserId
                UserName,
                // SessionType
                sessionDescription,
                // Timestamp
                (DateTime.UtcNow - TimerStart).TotalMilliseconds
            };

            return data;
        }

        // Todo: Put into generic utils class?
        public object[] MergeObjArrays(object[] part1, object[] part2)
        {
            object[] data = new object[part1.Length + part2.Length];
            part1.CopyTo(data, 0);
            part2.CopyTo(data, part1.Length);
            return data;
        }

        protected override string GetFileName()
        {
            return !string.IsNullOrEmpty(FilenameToUse) ? FilenameToUse : $"{sessionDescription}-{UserName}";
        }

        public static string GetStringFormat(object[] data)
        {
            StringBuilder strFormat = new StringBuilder();
            for (int i = 0; i < data.Length - 1; i++)
            {
                strFormat.Append("{" + i + "}" + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ");
            }
            strFormat.Append("{" + (data.Length - 1) + "}");
            return strFormat.ToString();
        }

        public void UpdateLog(string inputType, string inputStatus, EyeTrackingTarget intendedTarget)
        {
            if (Instance != null && IsLogging)
            {
                if (logStructure != null)
                {
                    object[] data = MergeObjArrays(GetData_Part1(), logStructure.GetData(inputType, inputStatus, intendedTarget));
                    string data_format = GetStringFormat(data);
                    Instance.CustomAppend(String.Format(data_format, data));
                }
            }
        }

        public void UpdateLog()
        {
            UpdateLog("", "", null);
        }

        private void Update()
        {
            if (automaticLogging)
            {
                UpdateLog();
            }
        }

        public override void OnDestroy()
        {
            // Disable listening to user input
            if (Instance != null)
            {
                Instance.StopLoggingAndSave();
            }

            base.OnDestroy();
        }
    }
}
