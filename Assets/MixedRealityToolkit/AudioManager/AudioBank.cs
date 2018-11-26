// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// A collection of AudioEvents and AudioParameters
    /// </summary>
    [CreateAssetMenu]
    public class AudioBank : ScriptableObject
    {
        /// <summary>
        /// The events included in the bank
        /// </summary>
        [SerializeField]
        private List<AudioEvent> audioEvents;
        /// <summary>
        /// The parameters included in the bank
        /// </summary>
        [SerializeField]
        private List<AudioParameter> parameters;

        /// <summary>
        /// The public accessor for the events in the bank
        /// </summary>
        public List<AudioEvent> AudioEvents
        {
            get { return this.audioEvents; }
        }

        private void OnEnable()
        {
            if (this.audioEvents == null)
            {
                this.audioEvents = new List<AudioEvent>();
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// EDITOR: Get or set the array of AudioEvents
        /// </summary>
        public List<AudioEvent> EditorEvents
        {
            get { return this.audioEvents; }
            set { this.audioEvents = value; }
        }

        /// <summary>
        /// EDITOR: Get or set the array of AudioParameters
        /// </summary>
        public List<AudioParameter> EditorParameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// EDITOR: Create a new event and add it to the array of events in the bank
        /// </summary>
        /// <param name="outputPos">The position of the Output node</param>
        /// <returns></returns>
        public AudioEvent AddEvent(Vector2 outputPos)
        {
            AudioEvent newEvent = ScriptableObject.CreateInstance<AudioEvent>();
            newEvent.name = "New Audio Event";
            AssetDatabase.AddObjectToAsset(newEvent, this);
            newEvent.InitializeEvent(outputPos);
            this.audioEvents.Add(newEvent);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            return newEvent;
        }

        /// <summary>
        /// Destroy an event object and remove it from the array of events
        /// </summary>
        /// <param name="eventToDelete"></param>
        public void DeleteEvent(AudioEvent eventToDelete)
        {
            eventToDelete.DeleteNodes();
            this.audioEvents.Remove(eventToDelete);
            ScriptableObject.DestroyImmediate(eventToDelete, true);
        }

        /// <summary>
        /// Create a new AudioParameter and add it to the array of parameters
        /// </summary>
        /// <returns>The AudioParameter instance that was created</returns>
        public AudioParameter AddParameter()
        {
            AudioParameter newParameter = ScriptableObject.CreateInstance<AudioParameter>();
            newParameter.name = "New Audio Parameter";
            newParameter.InitializeParameter();
            AssetDatabase.AddObjectToAsset(newParameter, this);
            this.parameters.Add(newParameter);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            return newParameter;
        }

        /// <summary>
        /// Destroy an AudioParameter and remove it from the array of parameters
        /// </summary>
        /// <param name="parameterToDelete">The AudioParameter you wish to delete</param>
        public void DeleteParameter(AudioParameter parameterToDelete)
        {
            this.parameters.Remove(parameterToDelete);
            ScriptableObject.DestroyImmediate(parameterToDelete, true);
        }

#endif
    }
}