using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class DebugView : MonoBehaviour {
	[SerializeField] int _logCount = 10;
	TextMesh     _text;
	List<string> _lines = new List<string>();

	// Use this for initialization
	void Awake () {
		_text = GetComponent<TextMesh>();
		Application.logMessageReceived += OnLogMessage;
		_text.text = "";
	}
	private void OnDestroy() {
		Application.logMessageReceived -= OnLogMessage;
	}

	private void OnLogMessage(string message, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception) {
			_lines.Add("<color=red>" + message + "</color>\n" + stackTrace);
		} else if (type == LogType.Warning) {
			_lines.Add("<color=yellow>" + message + "</color>");
		} else {
			_lines.Add(message);
		}

		if (_lines.Count > _logCount)
			_lines.RemoveAt(0);

		string result = "";
		for (int i = 0; i < _lines.Count; i++) {
			result += _lines[i] + "\n";
		}
		_text.text = result;
	}
}
