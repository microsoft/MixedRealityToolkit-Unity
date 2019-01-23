// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Preview.Examples.LightEstimation
{
	[RequireComponent(typeof(TextMesh))]
	public class DebugView : MonoBehaviour
	{
		[SerializeField] private int logCount = 10;

		private TextMesh     text;
		private List<string> lines = new List<string>();
		
		private void Awake ()
		{
			text = GetComponent<TextMesh>();
			Application.logMessageReceived += OnLogMessage;
			text.text = "";
		}
		private void OnDestroy()
		{
			Application.logMessageReceived -= OnLogMessage;
		}

		private void OnLogMessage(string message, string stackTrace, LogType type)
		{
			if (type == LogType.Error || type == LogType.Exception)
			{
				lines.Add("<color=red>" + message + "</color>\n" + stackTrace);
			}
			else if (type == LogType.Warning)
			{
				lines.Add("<color=yellow>" + message + "</color>");
			}
			else
			{
				lines.Add(message);
			}

			if (lines.Count > logCount)
				lines.RemoveAt(0);

			string result = "";
			for (int i = 0; i < lines.Count; i++)
			{
				result += lines[i] + "\n";
			}
			text.text = result;
		}
	}
}