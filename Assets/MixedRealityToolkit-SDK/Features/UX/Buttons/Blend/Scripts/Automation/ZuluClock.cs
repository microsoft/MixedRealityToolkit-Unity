// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Blend.Automation
{
	/// <summary>
	/// Add a digital stype clock to a UI text object.
	/// </summary>
	public class ZuluClock : MonoBehaviour
	{

		// Customize the start time of the clock.
		public int StartHour = 0;
		public int StartMinute = 0;
		public int StartSecond = 0;
		public bool OutputToText = true;
		public bool UTC = true;
		public int HourOffset = 0;
		public string EmbededString = "#";

		private string mCurrentTime = "00:00:00";
		private Text mText;
		private TextMesh mTextMesh;
		private System.DateTime mDate;

		// Use this for initialization
		void Start()
		{
			mDate = System.DateTime.Now;
			if (UTC)
			{
				mDate = System.DateTime.UtcNow;
			}

			int year = mDate.Year;
			int month = mDate.Month;
			int day = mDate.Day;

			if (StartHour != 0 || StartMinute != 0 || StartSecond != 0)
			{
				mDate = new System.DateTime(year, month, day, StartHour, StartMinute, StartSecond);
			}

			if (HourOffset != 0)
			{
				mDate = mDate.AddHours(HourOffset);
			}

			mText = this.gameObject.GetComponent<Text>();
			mTextMesh = this.gameObject.GetComponent<TextMesh>();
		}

		public string GetShortTime()
		{
			return EmbedString(mDate.ToString("HH:mm"));
		}

		private string EmbedString(string time)
		{
			string newString = time;
			if (EmbededString != "" && EmbededString != "#")
			{
				string[] tempStr = EmbededString.Split('#');
				if (tempStr.Length > 0)
				{
					newString = tempStr[0] + time + tempStr[1];
				}
			}
			return newString;
		}

		// Update is called once per frame
		void Update()
		{
			mDate = mDate.AddSeconds(Time.deltaTime);
			mCurrentTime = mDate.ToString("HH:mm:ss");
			if (mText != null && OutputToText)
			{
				mText.text = EmbedString(mCurrentTime);
			}

			if (mTextMesh != null && OutputToText)
			{
				mTextMesh.text = EmbedString(mCurrentTime);
			}
		}
	}
}
