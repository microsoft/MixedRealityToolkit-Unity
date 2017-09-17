// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Base class for auto configuration build windows.
    /// </summary>
    public abstract class AutoConfigureWindow<TSetting> : EditorWindow
    {
        #region Member Fields

        private Dictionary<TSetting, bool> values = new Dictionary<TSetting, bool>();
        private Dictionary<TSetting, string> names = new Dictionary<TSetting, string>();
        private Dictionary<TSetting, string> descriptions = new Dictionary<TSetting, string>();

        private string statusMessage = string.Empty;
        private Vector2 scrollPosition = Vector2.zero;
        private GUIStyle wrapStyle;

        #endregion // Member Fields

        #region Internal Methods

        private void SettingToggle(TSetting setting)
        {
            EditorGUI.BeginChangeCheck();

            // Draw and update setting flag
            values[setting] = GUILayout.Toggle(values[setting], new GUIContent(names[setting]));

            if (EditorGUI.EndChangeCheck())
            {
                OnGuiChanged();
            }

            // If this control is the one under the mouse, update the status message
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                StatusMessage = descriptions[setting];
                Repaint();
            }
        }

        /// <summary>
        /// Gets or sets the status message displayed at the bottom of the window.
        /// </summary>
        private string StatusMessage { get { return statusMessage; } set { statusMessage = value; } }

        #endregion // Internal Methods

        #region Overridables / Event Triggers

        /// <summary>
        /// Called when settings should be applied.
        /// </summary>
        protected abstract void ApplySettings();

        /// <summary>
        /// Called when settings should be loaded.
        /// </summary>
        protected abstract void LoadSettings();

        /// <summary>
        /// Called when string names and descriptions should be loaded.
        /// </summary>
        protected abstract void LoadStrings();

        /// <summary>
        /// Called when a toggle has been flipped and a change has been detected.
        /// </summary>
        protected abstract void OnGuiChanged();

        #endregion // Overridables / Event Triggers

        #region Overrides / Event Handlers

        /// <summary>
        /// Called when the window is created.
        /// </summary>
        protected virtual void Awake()
        {
            wrapStyle = new GUIStyle("label")
            {
                wordWrap = true,
                richText = true
            };
        }

        protected virtual void OnEnable()
        {
            LoadStrings();
            LoadSettings();
        }

        /// <summary>
        /// Renders the GUI
        /// </summary>
        protected virtual void OnGUI()
        {
            // Begin Settings Section
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Individual Settings
            var keys = values.Keys.ToArray();
            for (int iKey = 0; iKey < keys.Length; iKey++)
            {
                SettingToggle(keys[iKey]);
            }

            // End Settings Section
            GUILayout.EndVertical();

            // Status box area
            GUILayout.BeginVertical(EditorStyles.helpBox);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.Label(statusMessage, wrapStyle);
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            // Apply button
            GUILayout.BeginVertical(EditorStyles.miniButtonRight);
            bool applyClicked = GUILayout.Button("Apply");
            GUILayout.EndVertical();

            // Clicked?
            if (applyClicked)
            {
                ApplySettings();
            }
        }

        #endregion // Overrides / Event Handlers

        #region Protected Properties

        /// <summary>
        /// Gets the descriptions of the settings.
        /// </summary>
        protected Dictionary<TSetting, string> Descriptions
        {
            get
            {
                return descriptions;
            }

            set
            {
                descriptions = value;
            }
        }

        /// <summary>
        /// Gets the names of the settings.
        /// </summary>
        protected Dictionary<TSetting, string> Names
        {
            get
            {
                return names;
            }

            set
            {
                names = value;
            }
        }

        /// <summary>
        /// Gets the values of the settings.
        /// </summary>
        protected Dictionary<TSetting, bool> Values
        {
            get
            {
                return values;
            }

            set
            {
                values = value;
            }
        }

        #endregion // Protected Properties
    }
}