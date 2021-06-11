// Copyright (c) Microsoft Corporation.
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
        internal class ComponentInformation
        {

            internal class TextVariableInformation
            {
                public int Position { get; set; }
                public string DataBindVariable { get; set; }
                public string ResolvedKeyPath { get; set; }
                public string LocalKeyPath { get; set; }
                public object CurrentValue { get; set; }
            }

            private TextMeshProUGUI _textMeshProComponent = null;
            private UnityEngine.UI.Text _textComponent = null;
            private string _originalTemplateValue;
            private bool _hasChanged;

            /* Used to find all Components which are affected by a change in a specific keypath */
            private Dictionary<string, TextVariableInformation> _keyPathToVariableInformation = new Dictionary<string, TextVariableInformation>();

            public ComponentInformation(Component theComponent)
            {
                if (typeof(TextMeshProUGUI).IsAssignableFrom(theComponent.GetType()))
                {
                    _textMeshProComponent = theComponent as TextMeshProUGUI;
                }
                else if (typeof(UnityEngine.UI.Text).IsAssignableFrom(theComponent.GetType()))
                {
                    _textComponent = theComponent as UnityEngine.UI.Text;
                }

                _originalTemplateValue = GetValue();

            }



            public string GetTemplate()
            {
                return _originalTemplateValue;
            }


            public string GetValue()
            {
                if (_textMeshProComponent != null)
                {
                    return _textMeshProComponent.text;
                }
                else if (_textComponent != null)
                {
                    return _textComponent.text;
                } else
                {
                    return null;
                }
            }


            public void SetValue(string newValue)
            {

                if (_textMeshProComponent != null)
                {
                    _textMeshProComponent.text = newValue;
                }
                else if (_textComponent != null)
                {
                    _textComponent.text = newValue;
                }
            }


            internal void ProcessDataChanged( IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object newValue)
            {
                if (_keyPathToVariableInformation.ContainsKey(resolvedKeyPath))
                {
                    _keyPathToVariableInformation[resolvedKeyPath].CurrentValue = newValue;
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


            public void ClearKeyPaths()
            {
                // clear old keypaths when object is going back to re-use pool.
                _keyPathToVariableInformation.Clear();
            }


            public void AddKeyPath(string resolvedKeyPath, string localKeyPath, int position, string entireVariable)
            {

                TextVariableInformation textVariableInfo = new TextVariableInformation();

                textVariableInfo.Position = position;
                textVariableInfo.DataBindVariable = entireVariable;
                textVariableInfo.CurrentValue = localKeyPath;
                textVariableInfo.ResolvedKeyPath = resolvedKeyPath;
                textVariableInfo.LocalKeyPath = localKeyPath;

                _keyPathToVariableInformation[resolvedKeyPath] = textVariableInfo;
            }


        } /* End of internal class ComponentInformation */



        [Tooltip("Manage sprites in child game objects as well as this one.")]
        [SerializeField]
        private bool manageChildren = true;


        /* Used to find all keypaths that influence a specific component to make sure all variable data is updated when any one element changes */
        internal Dictionary<Component, ComponentInformation> _componentInfoLookup = new Dictionary<Component, ComponentInformation>();

 
        internal override void InitializeDataConsumer()
        {
        }


        internal override Type[] GetComponentTypes()
        {

            Type[] types = { typeof(TextMeshProUGUI), typeof(UnityEngine.UI.Text) };
            return types;
        }

        internal override bool ManageChildren()
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


        internal override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object newValue)
        {
            foreach (ComponentInformation componentInfo in _componentInfoLookup.Values)
            {
                componentInfo.ProcessDataChanged(dataSource, resolvedKeyPath, localKeyPath, newValue);
            }
        }


        public override void Detach()
        {
            base.Detach();
            foreach( ComponentInformation ci in _componentInfoLookup.Values )
            {
                ci.ClearKeyPaths();
            }
        }

        internal override void AddVariableKeyPathsForComponent(Type componentType, Component component)
        {
            ComponentInformation componentInfo;

            if (!_componentInfoLookup.ContainsKey(component))
            {
                componentInfo = new ComponentInformation(component);
                _componentInfoLookup[component] = componentInfo;
            } else
            {
                componentInfo = _componentInfoLookup[component];
            }

            MatchCollection matches = GetVariableMatchingRegex().Matches(componentInfo.GetTemplate());

            foreach (Match match in matches)
            {
                string localKeyPath = match.Groups[1].Value;

                string resolvedKeyPath = DataSource.ResolveKeyPath(this.ResolvedKeyPathPrefix, localKeyPath);

                componentInfo.AddKeyPath(resolvedKeyPath, localKeyPath, match.Index, match.Value);

                AddKeyPath(localKeyPath);

            }
        }

    }
}