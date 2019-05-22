// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor
{
    public class EditorWindowBase<TWindow> : EditorWindow where TWindow : EditorWindowBase<TWindow>
    {
        private static Dictionary<Type, EditorWindow> windows = new Dictionary<Type, EditorWindow>();

        public static TWindow TryGetWindow()
        {
            EditorWindow window;
            if (windows.TryGetValue(typeof(TWindow), out window))
            {
                return window as TWindow;
            }

            return null;
        }

        protected virtual void Update()
        {
            CheckEditorWindowStatus();
        }

        /// <summary>
        /// Ensure the window is still active.  When entering or leaving Play mode, the window can be set to null.
        /// </summary>
        private void CheckEditorWindowStatus()
        {
            TWindow window = TryGetWindow();
            if (window != null)
            {
                window.Repaint();
            }
            else
            {
                // Window has been destroyed, recreate it.
                // This might happen when transitioning to play mode in Unity.
                ShowWindow();
            }
        }

        protected static string GetWindowName()
        {
            return typeof(TWindow).GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault()?.Description ?? typeof(TWindow).Name;
        }

        protected static void ShowWindow()
        {
            TWindow window = (TWindow)GetWindow(typeof(TWindow), false, GetWindowName(), true);
            windows[typeof(TWindow)] = window;

            Vector2 minDimensons = new Vector2(315, 400);
            window.minSize = minDimensons;

            window.Show();
        }

        protected void RenderTitle(string title, Color color)
        {
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            try
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField(title);
                EditorGUILayout.EndVertical();
            }
            finally
            {
                GUI.backgroundColor = oldColor;
            }
        }
    }
}