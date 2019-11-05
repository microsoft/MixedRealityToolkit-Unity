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
    public class InteractableToggleCollectionInspector : UnityEditor.Editor
    {
        protected InteractableToggleCollection instance;

        protected virtual void OnEnable()
        {
            instance = (InteractableToggleCollection)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                if (instance != null)
                {
                    if (GUI.changed)
                    {
                        if (instance.CurrentIndex <= instance.ToggleList.Length)
                        {
                            instance.SetSelection(instance.CurrentIndex, false, true);
                        }
                        else
                        {
                            Debug.LogError("Index out of range");
                        }

                    }
                }
            }
        }

    }
}