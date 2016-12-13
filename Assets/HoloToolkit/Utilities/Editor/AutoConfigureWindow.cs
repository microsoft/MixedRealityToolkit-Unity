using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
	public class AutoConfigureWindow : UnityEditor.EditorWindow
	{
		// Member Variables
		private string statusMessage = string.Empty;
		private Vector2 scrollPosition = Vector2.zero;

		// Virtual methods
		/// <summary>
		/// Called when the Apply button is clicked.
		/// </summary>
		protected virtual void OnApply() {}

		// Behavior Overrides
		/// <summary>
		/// Called when the window is created.
		/// </summary>
		protected virtual void Awake() { }

		/// <summary>
		/// Renders the GUI
		/// </summary>
		protected virtual void OnGUI()
		{
			// Status box area
			GUILayout.BeginVertical(EditorStyles.helpBox);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.Label(statusMessage);
			GUILayout.EndScrollView();
			GUILayout.EndVertical();

			// Apply button
			GUILayout.BeginVertical(EditorStyles.miniButtonRight);
			bool applyClicked = GUILayout.Button("Apply");
			GUILayout.EndVertical();

			// Clicked?
			if (applyClicked) { OnApply(); }
		}

		// Properties

		/// <summary>
		/// Gets or sets the status message displayed at the bottom of the window.
		/// </summary>
		public string StatusMessage { get { return statusMessage; } set { statusMessage = value; } }
	}
}