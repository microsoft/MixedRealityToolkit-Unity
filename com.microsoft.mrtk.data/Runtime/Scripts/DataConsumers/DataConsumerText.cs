// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Text")]
    public class DataConsumerText : DataConsumerGOBase
    {
        protected class ComponentInformation
        {
            private class TextVariableInformation
            {
                public string ResolvedKeyPath { get; set; }
                public string LocalKeyPath { get; set; }
                public object CurrentValue { get; set; }
            }

            private TemplateString _templateStr;

            private Component _genericTextComponent = null;
            private bool _hasChanged;
            private bool _truncate;
            private int _maxChars;     // if truncating, what are the maximum allowed characters
            private bool _isParentFixedHierarchy = false;

            /* Used to find all Components which are affected by a change in a specific keypath */
            private Dictionary<string, TextVariableInformation> _keyPathToVariableInformation = new Dictionary<string, TextVariableInformation>();
            private Dictionary<string, TextVariableInformation> _localPathToVariableInformation = new Dictionary<string, TextVariableInformation>();

            public bool IsTemplatedString => _templateStr.VariableCount > 0;
            public IEnumerable<string> TemplateVars => _templateStr.VariableNames;

            public ComponentInformation(Component theComponent, bool truncate, int maxChars, bool isParentFixedHierarchy)
            {
                _genericTextComponent = theComponent;
                _isParentFixedHierarchy = isParentFixedHierarchy;
                _truncate = truncate;
                _maxChars = maxChars;
                string textValue = GetValue();
                if (textValue != null)
                {
                    _templateStr = new TemplateString(textValue);
                }
            }

            public string GetTemplate()
            {
                return _templateStr.OriginalTemplate;
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
                    string textToChange = _templateStr.Build(s => _localPathToVariableInformation[s].CurrentValue.ToString());
                    if (_truncate)
                    {
                        textToChange = TruncateString.TruncateStringMiddle(textToChange, _maxChars);
                    }

                    SetValue(textToChange);

                    _hasChanged = false;
                }
            }

            public void Detach()
            {
                // Reset the value on the Text field to either the original Template or string.Empty so if it is reused, it will not have stale data.
                SetValue(_isParentFixedHierarchy ? string.Empty : GetTemplate());

                // clear old keypaths when object is going back to re-use pool.
                _keyPathToVariableInformation.Clear();
                _localPathToVariableInformation.Clear();
            }

            public bool AddKeyPathListener(string resolvedKeyPath, string localKeyPath)
            {
                if (!_keyPathToVariableInformation.ContainsKey(resolvedKeyPath))
                {
                    TextVariableInformation textVariableInfo = new TextVariableInformation
                    {
                        CurrentValue = localKeyPath,
                        ResolvedKeyPath = resolvedKeyPath,
                        LocalKeyPath = localKeyPath
                    };

                    _keyPathToVariableInformation[resolvedKeyPath] = textVariableInfo;
                    _localPathToVariableInformation[localKeyPath] = textVariableInfo;
                    return false;
                }
                else
                {
                    return true;
                }
            }

            /// <summary>
            /// Utility function to get the string contained within a Component of type TMP_Text or Unity text.
            /// </summary>
            public string GetValue()
            {
                string currentValue = null;
                switch (_genericTextComponent)
                {
                    case TMP_Text tmpUGUIComponent:
                        currentValue = tmpUGUIComponent.text;
                        break;
                    case UnityEngine.UI.Text unityUITextComponent:
                        currentValue = unityUITextComponent.text;
                        break;
                }
                return currentValue;
            }

            /// <summary>
            /// Utility function to set the string of a Component of type TMP_Text or Unity text.
            /// </summary>
            public void SetValue(string newValue)
            {
                switch (_genericTextComponent)
                {
                    case TMP_Text tmpUGUIComponent:
                        tmpUGUIComponent.text = newValue;
                        break;
                    case UnityEngine.UI.Text unityUITextComponent:
                        unityUITextComponent.text = newValue;
                        break;
                }
            }

        } /* End of protected class ComponentInformation */

        [Tooltip("Manage text components in child game objects as well as this one.")]
        [SerializeField]
        private bool manageChildren = true;

        [Header("Truncation")]
        [Tooltip("Specifies if the constructed text should be truncated to a maximum number of characters.")]
        [SerializeField]
        private bool truncateString = false;

        [Tooltip("If truncation is set to true, specifies the maximum number of characters to retain in the constructed text.")]
        [SerializeField]
        private int maxTruncatedCharacters = 25;

        /* Used to find all keypaths that influence a specific component to make sure all variable data is updated when any one element changes */
        protected Dictionary<Component, ComponentInformation> _componentInfoLookup = new Dictionary<Component, ComponentInformation>();

        /// </inheritdoc/>
        protected override Type[] GetComponentTypes()
        {
            Type[] types = { typeof(TextMeshProUGUI), typeof(UnityEngine.UI.Text), typeof(TextMeshPro) };
            return types;
        }

        /// </inheritdoc/>
        protected override bool ManageChildren()
        {
            return manageChildren;
        }

        /// </inheritdoc/>
        public override void DataChangeSetEnd(IDataSource dataSource)
        {
            foreach (ComponentInformation componentInfo in _componentInfoLookup.Values)
            {
                componentInfo.ApplyAllChanges();
            }
        }

        /// </inheritdoc/>
        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object value, DataChangeType dataChangeType)
        {
            foreach (ComponentInformation componentInfo in _componentInfoLookup.Values)
            {
                componentInfo.ProcessDataChanged(dataSource, resolvedKeyPath, localKeyPath, value, dataChangeType);
            }
        }

        /// </inheritdoc/>
        protected override void DetachDataConsumer()
        {
            foreach (ComponentInformation ci in _componentInfoLookup.Values)
            {
                ci.Detach();
            }

            if (!IsFixedHierarchyWillUseCachedValues)
            {
                _componentInfoLookup.Clear();
            }
        }

        /// </inheritdoc/>
        protected override void AddVariableKeyPathsForComponent(Component component)
        {
            if (_componentInfoLookup.TryGetValue(component, out ComponentInformation componentInfo))
            {
                IDataSource dataSource = GetBestDataSource();

                foreach (string varName in componentInfo.TemplateVars)
                {
                    string resolvedKeyPath = dataSource.ResolveKeyPath(ResolvedKeyPathPrefix, varName);

                    componentInfo.AddKeyPathListener(resolvedKeyPath, varName);

                    AddKeyPathListener(varName);
                }
            }
        }

        private void Awake()
        {
            foreach (Component managedComponent in FindComponentsToManage())
            {
                ComponentInformation componentInfo = new ComponentInformation(managedComponent, truncateString, maxTruncatedCharacters, IsFixedHierarchyWillUseCachedValues);
                // Make sure this DataConsumerText ONLY manages Text values which have a template, indicated by {{DataValue}}.
                if (componentInfo.IsTemplatedString)
                {
                    _componentInfoLookup[managedComponent] = componentInfo;
                    // Clear out the values on the Text components, as we don't want the template visible during the time Databinding is Attach()ing
                    componentInfo.SetValue(string.Empty);
                }
            }
        }
    }
}
