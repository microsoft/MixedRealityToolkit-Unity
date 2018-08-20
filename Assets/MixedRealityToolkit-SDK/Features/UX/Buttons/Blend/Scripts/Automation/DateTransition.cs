// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blend.Automation
{
    [System.Serializable]
    public struct DateValues
    {
        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
        public int Second;
    }

    public class DateTransition : BlendDateTime
    {
        [Tooltip("the starting date: leave zeros for starting from System.Date.Now")]
        public DateValues StartDate;

        [Tooltip("The target date: leave zeros if the target is now")]
        public DateValues TargetDate;

        [Tooltip("The date formate string: see MSDN for string ideas")]
        //https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
        public string DateFormat = "MMMM dd, yyyy";

        private TextMesh textMesh;
        private Text text;

        protected override void Awake()
        {
            textMesh = GetComponent<TextMesh>();
            text = GetComponent<Text>();

            if (Zeros(StartDate))
            {
                
            }
            else
            {
                startValue = GetNewDate(StartDate);
            }

            startValue = Zeros(StartDate) ? System.DateTime.Now : GetNewDate(StartDate);
            Value = startValue;

            TargetValue = Zeros(TargetDate) ? System.DateTime.Now : GetNewDate(TargetDate);
            

            base.Awake();
        }

        /// <summary>
        /// is the date value all zeros?
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private bool Zeros(DateValues values)
        {
            return (values.Year + values.Month + values.Day + values.Hour + values.Minute + values.Second) == 0;
        }

        /// <summary>
        /// create a new date object based on the dateValue object
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private DateTime GetNewDate(DateValues values)
        {
            DateTime date = new DateTime(values.Year, values.Month, values.Day, values.Hour, values.Minute, values.Second);
            return date;
        }

        public override void SetValue(DateTime value)
        {
            base.SetValue(value);

            if (textMesh != null)
            {
                textMesh.text = value.ToString(DateFormat);
            }
            else if(text != null)
            {
                text.text = value.ToString(DateFormat);
            }
        }
    }
}
