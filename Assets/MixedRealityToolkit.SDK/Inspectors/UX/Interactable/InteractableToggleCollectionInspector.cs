// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
    [CustomEditor(typeof(InteractableToggleCollection))]
    /// <summary>
    /// Custom inspector for InteractableToggleCollection
    /// </summary>
    internal class InteractableToggleCollectionInspector : UnityEditor.Editor
    {
        protected InteractableToggleCollection instance;

        protected virtual void OnEnable()
        {
            instance = (InteractableToggleCollection)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying && instance != null && GUI.changed)
            {
                instance.CurrentIndex = Mathf.Clamp(instance.CurrentIndex, 0, instance.ToggleList.Length - 1);

                if (instance.CurrentIndex >= instance.ToggleList.Length || instance.CurrentIndex < 0)
                {
                    Debug.Log("Index out of range: " + instance.CurrentIndex);
                }
                else
                {
                    instance.SetSelection(instance.CurrentIndex, true, true);
                }  
            }
        }
    }
}
