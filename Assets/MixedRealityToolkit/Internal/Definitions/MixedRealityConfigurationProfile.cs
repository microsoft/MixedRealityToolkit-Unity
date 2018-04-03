// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Internal.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Internal.Definitions
{
    /// <summary>
    /// Configuration profile settings for the Mixed Reality Toolkit
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Configuration Profile")]
    public class MixedRealityConfigurationProfile : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        private IManager[] initialManagers;

        [NonSerialized]
        public Dictionary<Type, IManager> ActiveManagers = new Dictionary<Type, IManager>();

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


        #region ISerializationCallbackReceiver Interface

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            int managerCount = initialManagers.Length;
            for (int i = 0; i < managerCount; i++)
            {
                ActiveManagers.Add(initialManagers[i].GetType(), initialManagers[i]);
            }
        }

        #endregion
    }
}