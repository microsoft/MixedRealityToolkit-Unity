// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Configuration profile settings for the Mixed Reality Toolkit
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Configuration Profile")]
    public class MixedRealityConfigurationProfile : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Manager Registry properties

        /// <summary>
        /// Serialized list of managers for the Mixed Reality manager
        /// </summary>
        [SerializeField]
        private IMixedRealityManager[] initialManagers;

        /// <summary>
        /// Serialized list of the Interface types for the Mixed Reality manager
        /// </summary>
        [SerializeField]
        private Type[] initialManagertypes;

        /// <summary>
        /// Dictionary list of active managers used by the Mixed Reality Manager at runtime
        /// </summary>
        [NonSerialized]
        public Dictionary<Type, IMixedRealityManager> ActiveManagers = new Dictionary<Type, IMixedRealityManager>();

        #endregion

        #region Mixed Reality Manager configurable properties

        #region Input System

        /// <summary>
        /// Enable and configure the Input System component for the Mixed Reality Toolkit
        /// </summary>
        [Header("Input Settings")]
        [Tooltip("Enable the Input System on Startup")]
        public bool EnableInputSystem;

        #endregion

        #region Boundary

        [Header("Boundary Settings")]
        /// <summary>
        /// Enable and configure the Boundary component on the Mixed Reality Camera
        /// </summary>
        [Tooltip("Enable the Boundary on Startup")]
        public bool EnableBoundary;

        #endregion

        #region Motion Controllers

        /// <summary>
        /// Enable and configure the controller rendering for the Mixed Reality Toolkit
        /// </summary>
        [Header("Motion Controllers")]
        [Tooltip("Enable the Motion Controllers on Startup")]
        public bool EnableControllers;

        #endregion

        #region Focus Control

        [Header("Focus Options")]
        /// <summary>
        /// Enable and configure the Focus component for the Mixed Reality Toolkit
        /// </summary>
        public bool EnableFocus;

        #endregion

        #endregion

        #region ISerializationCallbackReceiver Interface

        /// <summary>
        /// Unity function to prepare data for serialization, unused in the Mixed Reality Toolkit
        /// </summary>
        public void OnBeforeSerialize() { }

        /// <summary>
        /// Unity function to resolve data from serialization when a project is loaded
        /// </summary>
        public void OnAfterDeserialize()
        {
            // From the serialized fields for the MixedRealityConfigurationProfile, populate the Active managers list
            // *NOte This will only take effect once the Mixed Reality Toolkit has a custom editor for the MixedRealityConfigurationProfile
            int managerCount = initialManagers.Length;
            for (int i = 0; i < managerCount; i++)
            {
                ActiveManagers.Add(initialManagertypes[i].GetType(), initialManagers[i]);
            }
        }

        #endregion
    }
}