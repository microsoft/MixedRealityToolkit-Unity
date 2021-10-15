﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data consumer that can embed data into text components.
    ///
    /// Currently supported are:
    ///     TextMeshPro (via TextMeshProUGUI)
    ///     TextMesh (via UnityEngine.UI.Text)
    ///
    /// One of these data consumer components can manage any number
    /// of text components so long as they are being populated by
    /// the same data source.
    ///
    /// Note that a single text message can support any number of variable phrases. To make this
    /// more efficient, all data changes within a single data set are cached and then applied
    /// at once at the DataSetEnd() method call.
    /// </summary>
    ///
    public class DataConsumerText : DataConsumerGOBase
    {
        protected class ComponentInformation
        {
            protected class TextVariableInformation
            {
                public string DataBindVariable { get; set; }
                public string ResolvedKeyPath { get; set; }
                public string LocalKeyPath { get; set; }
                public object CurrentValue { get; set; }
            }

            private TextMeshProUGUI _textMeshProUGUIComponent = null;
            private UnityEngine.UI.Text _textComponent = null;
            private TextMeshPro _textMeshProComponent = null;
            private string _originalTemplateValue;
            private bool _hasChanged;

            /* Used to find all Components which are affected by a change in a specific keypath */
            private Dictionary<string, TextVariableInformation> _keyPathToVariableInformation = new Dictionary<string, TextVariableInformation>();

            public ComponentInformation(Component theComponent)
            {
                if (typeof(TextMeshProUGUI).IsAssignableFrom(theComponent.GetType()))
                {
                    _textMeshProUGUIComponent = theComponent as TextMeshProUGUI;
                }
                else if (typeof(UnityEngine.UI.Text).IsAssignableFrom(theComponent.GetType()))
                {
                    _textComponent = theComponent as UnityEngine.UI.Text;
                }
                else if (typeof(TextMeshPro).IsAssignableFrom(theComponent.GetType()))
                {
                    _textMeshProComponent = theComponent as TextMeshPro;
                }

                _originalTemplateValue = GetValue();
            }



            public string GetTemplate()
            {
                return _originalTemplateValue;
            }


            public string GetValue()
            {
                if (_textMeshProUGUIComponent != null)
                {
                    return _textMeshProUGUIComponent.text;
                }
                else if (_textComponent != null)
                {
                    return _textComponent.text;
                }
                else if (_textMeshProComponent != null)
                {
                    return _textMeshProComponent.text;
                }
                else
                {
                    return null;
                }
            }


            public void SetValue(string newValue)
            {
                if (_textMeshProUGUIComponent != null)
                {
                    _textMeshProUGUIComponent.text = newValue;
                }
                else if (_textComponent != null)
                {
                    _textComponent.text = newValue;
                }
                else if (_textMeshProComponent != null)
                {
                    _textMeshProComponent.text = newValue;
                }
            }


            public void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object value, DataChangeType dataChangeType)
            {
                if (_keyPathToVariableInformation.ContainsKey(resolvedKeyPath))
                {
                    _keyPathToVariableInformation[resolvedKeyPath].CurrentValue = value;
                    _hasChanged = true;
                }
            }

            public void ApplyAllChanges()
            {
                if (_hasChanged)
                {
                    string textToChange = _originalTemplateValue;

                    foreach (TextVariableInformation tvi in _keyPathToVariableInformation.Values)
                    {
                        textToChange = textToChange.Replace(tvi.DataBindVariable, tvi.CurrentValue.ToString());
                    }

                    SetValue(textToChange);

                    _hasChanged = false;
                }
            }


            public void Detach()
            {
                SetValue(_originalTemplateValue);

                // clear old keypaths when object is going back to re-use pool.
                _keyPathToVariableInformation.Clear();
            }


            public bool AddKeyPathListener(string resolvedKeyPath, string localKeyPath, string entireVariable)
            {
                if (!_keyPathToVariableInformation.ContainsKey(resolvedKeyPath))
                {
                    TextVariableInformation textVariableInfo = new TextVariableInformation();

                    textVariableInfo.DataBindVariable = entireVariable;
                    textVariableInfo.CurrentValue = localKeyPath;
                    textVariableInfo.ResolvedKeyPath = resolvedKeyPath;
                    textVariableInfo.LocalKeyPath = localKeyPath;

                    _keyPathToVariableInformation[resolvedKeyPath] = textVariableInfo;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        } /* End of protected class ComponentInformation */



        [Tooltip("Manage sprites in child game objects as well as this one.")]
        [SerializeField]
        private bool manageChildren = true;

        /* Used to find all keypaths that influence a specific component to make sure all variable data is updated when any one element changes */
        protected Dictionary<Component, ComponentInformation> _componentInfoLookup = new Dictionary<Component, ComponentInformation>();
        private HashSet<string> _localKeypaths = new HashSet<string>();


        protected override Type[] GetComponentTypes()
        {
            Type[] types = { typeof(TextMeshProUGUI), typeof(UnityEngine.UI.Text), typeof(TextMeshPro) };
            return types;
        }

        protected override bool ManageChildren()
        {
            return manageChildren;
        }


        public override void DataChangeSetEnd(IDataSource dataSource)
        {
            foreach (ComponentInformation componentInfo in _componentInfoLookup.Values)
            {
                componentInfo.ApplyAllChanges();
            }
        }


        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object value, DataChangeType dataChangeType )
        {
            foreach (ComponentInformation componentInfo in _componentInfoLookup.Values)
            {
                componentInfo.ProcessDataChanged(dataSource, resolvedKeyPath, localKeyPath, value, dataChangeType);
            }
        }


        protected override void DetachDataConsumer()
        {
            foreach( ComponentInformation ci in _componentInfoLookup.Values )
            {
                ci.Detach();
            }
            _localKeypaths.Clear();
        }

        protected override void AddVariableKeyPathsForComponent(Type componentType, Component component)
        {
            ComponentInformation componentInfo = null;

            if (!_componentInfoLookup.ContainsKey(component))
            {
                componentInfo = new ComponentInformation(component);

                MatchCollection matches = GetVariableMatchingRegex().Matches(componentInfo.GetTemplate());
                if (matches.Count > 0)
                {
                    _componentInfoLookup[component] = componentInfo;

                    foreach (Match match in matches)
                    {
                        string localKeyPath = match.Groups[1].Value;

                        string resolvedKeyPath = DataSource.ResolveKeyPath(ResolvedKeyPathPrefix, localKeyPath);

                        componentInfo.AddKeyPathListener(resolvedKeyPath, localKeyPath, match.Value);

                        if (_localKeypaths.Add(localKeyPath))
                        {
                            // if first occurance, then add keypath listener on data source
                            AddKeyPathListener(localKeyPath);
                        }
                    }
                }
            }
        }
    }
}
