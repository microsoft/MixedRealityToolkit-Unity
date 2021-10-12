// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;


namespace Microsoft.MixedReality.Toolkit.Data
{

    /// <summary>
    /// A data consumer that can alter the style of a text
    /// component based on a style lookup.
    /// 
    /// Currently supported are:
    ///     TextMeshPro (via TextMeshProUGUI)
    ///     TextMesh (via UnityEngine.UI.Text)
    /// 
    /// One of these data consumer components can manage any number
    /// of text components so long as they are being populated by
    /// the same data source.
    /// 
    /// </summary>
    /// 
    public class DataConsumerTextStyle : DataConsumerGOBase
    {

        [Tooltip("Key path for style sheet name.")]
        [SerializeField]
        private string styleSheetKeyPath = "stylesheet";

        [Tooltip("Resource name prefix for constructing the correct resource name.")]
        [SerializeField]
        private string resourcePrefix = "Stylesheets/";

        [Tooltip("Manage stylesheets for TMPro Components in child game objects as well as this one.")]
        [SerializeField]
        private bool manageChildren = true;

        protected List<Component> _textComponents = new List<Component>();



        protected override Type[] GetComponentTypes()
        {

            Type[] types = { typeof(TextMeshProUGUI), typeof(TextMeshPro) };
            return types;
        }

        protected override bool ManageChildren()
        {
            return manageChildren;
        }

        protected override void AttachDataConsumer()
        {
            AddKeyPathListener(styleSheetKeyPath);
        }

        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object value, DataChangeType dataChangeType )
        {

            if (value is string)
            {

                string stylesheetPath = resourcePrefix + value.ToString();

                TMP_StyleSheet tmpStyleSheet = Resources.Load<TMP_StyleSheet>(stylesheetPath);

                if (tmpStyleSheet == null)
                {
                    UnityEngine.Debug.LogError("Stylesheet not found at resource path " + stylesheetPath);
                }
                else
                {
                    foreach (Component textMeshComponent in _textComponents)
                    {
#if UNITY_2019_1_OR_NEWER
                        if (textMeshComponent is TextMeshPro)
                        {
                            TextMeshPro tmp = textMeshComponent as TextMeshPro;

                            tmp.styleSheet = tmpStyleSheet;
                        }
                        else if (textMeshComponent is TextMeshProUGUI)
                        {
                            TextMeshProUGUI tmpUGUI = textMeshComponent as TextMeshProUGUI;

                            tmpUGUI.styleSheet = tmpStyleSheet;
                        }
#else
                        DebugUtilities.LogWarning("TextMeshPro stylesheets only work in Unity 2019 or later.");
#endif
                    }
                }
            }
        }



        protected override void AddVariableKeyPathsForComponent(Type componentType, Component component)
        {   
            // We only asked for TextMeshProGUI components, so we can confidently cast here.

            _textComponents.Add(component);
        }

    }
}
