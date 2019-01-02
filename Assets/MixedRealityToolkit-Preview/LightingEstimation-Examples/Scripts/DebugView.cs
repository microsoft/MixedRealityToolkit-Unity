using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
	[RequireComponent(typeof(TextMesh))]
	public class DebugView : MonoBehaviour
	{
		[SerializeField] int logCount = 10;

		TextMesh     text;
		List<string> lines = new List<string>();

		// Use this for initialization
		void Awake ()
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