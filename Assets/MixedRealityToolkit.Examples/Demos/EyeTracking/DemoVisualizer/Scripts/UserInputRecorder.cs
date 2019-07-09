// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    public class UserInputRecorder : CustomInputLogger
    {
        public string FilenameToUse = "\\test\folder\\";

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

        EyeTrackingTarget prevTarget = null;

        public override string GetHeader()
        {
            if (logStructure != null)
            {
                string[] header_columns = logStructure.GetHeaderColumns();
                string header_format = GetStringFormat(header_columns);
                return String.Format(header_format, header_columns);
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
                sessionDescr,
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
            string name = "";
            if (FilenameToUse != "")
            {
                name = FilenameToUse;
                return name;
            }
            
            name = String.Format("{0}-{1}", sessionDescr, UserName);
            return name;
        }

        private string LimitStringLength(string str, int maxLength)
        {
            if (str.Length < maxLength)
                return str;
            else
            {
                return str.Substring(0, maxLength);
            }
        }
        
        public static string GetStringFormat(object[] data)
        {
            string strFormat = "";
            for (int i = 0; i < data.Length - 1; i++)
            {
                strFormat += ("{" + i + "}" + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ");
            }
            strFormat += ("{" + (data.Length - 1) + "}");
            return strFormat;
        }
        
        public void UpdateLog(string inputType, string inputStatus, EyeTrackingTarget intendedTarget)
        {            
            if ((Instance != null) && (isLogging))
            {
                if (logStructure != null)
                {
                    object[] data = MergeObjArrays(GetData_Part1(), logStructure.GetData(inputType, inputStatus, intendedTarget));
                    string data_format = GetStringFormat(data);
                    Instance.CustomAppend(String.Format(data_format, data));
                    prevTarget = intendedTarget;
                }
            }
        }

        #region Remains the same across different loggers
        protected override void CustomAppend(string msg)
        {
            base.CustomAppend(msg);
        }
        #endregion

        public void UpdateLog()
        {
            UpdateLog("", "", null);
        }

        void Update()
        {
            if (automaticLogging)
            {
                UpdateLog();
            }
        }

        public override void OnDestroy()
        {
            // Disable listening to user input
            if (UserInputRecorder.Instance != null)
            {
                UserInputRecorder.Instance.StopLoggingAndSave();
            }

            base.OnDestroy();
        }
    }
}