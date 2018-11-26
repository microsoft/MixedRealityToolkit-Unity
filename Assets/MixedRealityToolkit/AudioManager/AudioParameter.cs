// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// A runtime value that affects audio properties on an AudioEvent
    /// </summary>
    public class AudioParameter : ScriptableObject
    {
        /// <summary>
        /// The value the parameter will be set to until explicitly set
        /// </summary>
        [SerializeField]
        private float defaultValue = 0;
        /// <summary>
        /// Whether the value will be updated with the angle between the camera and emitter
        /// </summary>
        [SerializeField]
        private bool useGaze = false;

        /// <summary>
        /// Public accessor for whether the value is updated with the angle between the camera and emitter
        /// </summary>
        public bool UseGaze
        {
            get { return this.useGaze; }
        }

        /// <summary>
        /// The current value of the parameter, before being evaluated on the response curve
        /// </summary>
        public float CurrentValue { get; private set; }

        /// <summary>
        /// Set the initial value of the parameter
        /// </summary>
        public void InitializeParameter()
        {
            this.CurrentValue = this.defaultValue;
        }

        /// <summary>
        /// Set the value back to default
        /// </summary>
        public void ResetParameter()
        {
            this.CurrentValue = this.defaultValue;
        }

        /// <summary>
        /// Set a new value for the parameter to be evaluated on the response curve
        /// </summary>
        /// <param name="newValue">The value to be set as CurrentValue</param>
        public void SetValue(float newValue)
        {
            if (this.useGaze || newValue == this.CurrentValue)
            {
                return;
            }

            this.CurrentValue = newValue;
        }

#if UNITY_EDITOR

        /// <summary>
        /// EDITOR: Draw the properties for the parameter in the graph
        /// </summary>
        public void DrawParameterEditor()
        {
            this.name = EditorGUILayout.TextField("Name", this.name);
            this.defaultValue = EditorGUILayout.FloatField("Default Value", this.defaultValue);
            this.useGaze = EditorGUILayout.Toggle("Use Gaze", this.useGaze);
        }

#endif
    }
}