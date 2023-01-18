// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    [Serializable]
    // This helps get around issue when mixing generics, concrete classes and interfaces in the inheritance.
    public abstract class DataConsumerThemableBase : DataConsumerGOBase, IDataBindable
    {
        [Tooltip("One or more data binding profiles to map data and theme keypaths to the materials they should manage.")]
        [SerializeField]
        private DataBindingProfile[] dataBindingProfiles;
        public DataBindingProfile[] DataBindingProfiles
        {
            get
            {
                return dataBindingProfiles;
            }
            set
            {
                dataBindingProfiles = value;
            }
        }

        /// <summary>
        /// All additional information related to a single DataBindingProfile.
        /// </summary>
        protected class BindingInfo
        {
            public DataBindingProfile BindingProfile;
            public IDataSource DataSource;
            public IDataSource ThemeSource;
            public string DataResolvedKeypath;
            public string ThemeResolvedKeypath;
            public HashSet<Component> Components;
        }

        protected BindingInfo[] _bindingInfoList;
        protected Dictionary<string, BindingInfo> _dataKeypathToBindingLookup = new Dictionary<string, BindingInfo>();
        protected Dictionary<string, BindingInfo> _themeKeypathToBindingLookup = new Dictionary<string, BindingInfo>();
        protected Dictionary<string, Regex> _patternToRegexLookup = new Dictionary<string, Regex>();

        /// <inheritdoc/>
        public abstract bool ConfigureFromBindingProfile(string[] dataSourceTypes, DataBindingProfile[] bindingProfiles);
    }

    /// <summary>
    /// This class provides a way to load a resource via many different
    /// potential retrieval means based on the nature of the value received
    /// from the data source:
    ///
    ///    Object of correct type provided directly
    ///    Numeric index lookup
    ///    <key,value> pair lookup
    ///    Resource path to load a Unity resource ("resource://pathToUnityResource")
    ///    Streaming asset path to a file that can be loaded using any appropriate means ("file://pathToUnityStreamingAsset")
    ///
    /// Generic type T is the type of object expected from the data source
    /// Generic type U is the Component type in the scene hierarchy where that object of type T will be modified.
    ///
    /// </summary>

    /// <typeparam name="T">The type of the object expected such as Image, Material, Texture2D, Sprite, Mesh</typeparam>
    /// <typeparam name="U">The type of Component to be managed, typically a Renderer, Material, or Texture2D</typeparam>
    [Serializable]
    public abstract class DataConsumerThemableBase<T> : DataConsumerThemableBase where T : class
    {
        public const string ResourcePrefix = "resource://";
        public const string StreamingAssetPrefix = "file://";

        protected bool manageComponentsInChildren = true;

        public override bool ConfigureFromBindingProfile(string[] dataSourceTypes, DataBindingProfile[] bindingProfiles)
        {
            DataSourceTypes = dataSourceTypes;
            DataBindingProfiles = bindingProfiles;

            // Sparse list only populated for matches and stays null if no qualifying matches are found.
            _bindingInfoList = new BindingInfo[bindingProfiles.Length];
            return bindingProfiles.Length > 0;
        }

        /// <summary>
        /// Update the provided component using the objectToSet.
        /// </summary>
        /// <param name="component">Which component to update</param>
        /// <param name="inValue">Original value from IDataSource</param>
        /// <param name="objectToSet">The object used to update component</param>
        protected abstract void SetObject(Component component, object inValue, T objectToSet);

        /// <summary>
        /// Given an int N, get the nth entry in lookup as a theme keypath to retrieve theme value
        /// </summary>
        /// <remarks>
        /// Override this method to support data sources that provide integral values
        /// that can be then used as an index lookup of the appropriate theme
        /// relative or absolute keypath. If not already present in the returned
        /// value, the keypath will be appended to the base keypath
        /// provided in the DataConsumerThemeHelper.
        ///
        /// Example:
        ///
        /// Given the stored value is an integer status where 0=new, 1=in progress and 2=done
        /// and it is desired to load a sprite for the correct status in the look and feel of
        /// the current theme.
        ///
        ///  The local lookup in the derived class can be structured as an absolute keypath as follows:
        ///     0 : Status.Sprites.New
        ///     1 : Status.Sprites.InProgress
        ///     2 : Status.Sprites.Done
        ///
        /// This keypath returned from this method will be used to retrieve its value via the
        /// DataConsuemerThemeHelper.  Whatever value is stored in the matching field of the
        /// theme data source will then be used to autodetect the method of retrieving the
        /// final sprite and the sprite will be retrieved and returned.
        /// </remarks>
        /// <param name="n">Index for looking up object of type T</param>
        /// <returns>Found object of type T or null if not found</returns>
        protected virtual T GetObjectByThemeLookupIndex(BindingInfo binding, int n)
        {
            if (n < binding.BindingProfile.ValueToThemeKeypathLookup.Length)
            {
                return binding.ThemeSource.GetValue(binding.BindingProfile.ValueToThemeKeypathLookup[n].Keypath) as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Given a string key, lookup desired theme keypath to retrieve the theme value.
        /// </summary>
        /// <remarks>
        /// Override this method to support data sources that provide string key values
        /// that can be then used as a dictionary lookup of the appropriate theme
        /// relative or absolute keypath. If not already present in the returned
        /// value, the keypath will be appended to the base keypath
        /// provided in the DataConsumerThemeHelper.
        /// </remarks>
        /// <param name="keyValue">Key value provided by data source</param>
        /// <returns>Found object of type T or null if not found</returns>
        protected virtual T GetObjectByThemeLookupKey(BindingInfo binding, string keyValue)
        {
            // Lookup lists are usually small, but it would make sense to create a dictionary
            // lookup if this is ever used and size is > some N.
            foreach (DataBindingProfile.ValueToKeypath valueToKeypath in binding.BindingProfile.ValueToThemeKeypathLookup)
            {
                if (keyValue == valueToKeypath.Value)
                {
                    return binding.ThemeSource.GetValue(valueToKeypath.Keypath) as T;
                }
            }

            return null;
        }

        /// </inheritdoc/>
        protected override bool ManageChildren()
        {
            return manageComponentsInChildren;
        }

        /// <summary>
        /// Report whether this is a Unity Component centric data consumer
        /// </summary>
        /// <remarks>
        /// For data consumers that manage Components of specific types like
        /// Renderer, the base class will do some of the discovery and
        /// management of those components.  Some data consumers are not
        /// component centric and therefore can return false to indicate
        /// that no component management is needed.</remarks>
        /// <returns>True means this is a component centric data consumer.</returns>
        protected virtual bool DoesManageSpecificComponents()
        {
            return false;
        }

        /// <summary>
        /// Given an int N, get the nth object of type T
        /// </summary>
        /// <remarks>
        /// Override to support data sources that provide integral lookups</remarks>
        /// <param name="n">Index for looking up object of type T</param>
        /// <returns>Found object of type T or null if not found</returns>
        protected virtual T GetObjectByIndex(int n)
        {
            return null;
        }

        /// <summary>
        /// Given a string key, find and return object of type T
        /// </summary>
        /// <remarks>
        /// Override to support data sources that provide string keys to lookup objects</remarks>
        /// <param name="keyValue">Key value provided by data source</param>
        /// <returns>Found object of type T or null if not found</returns>
        protected virtual T GetObjectByKey(string keyValue)
        {
            return null;
        }

        /// <inheritdoc/>
        protected override bool ComponentMeetsAllQualifications(Component component)
        {
            // Add this component to any binding that meets the qualifications. Note that typically,
            // there will be a 1:1 mapping between a component and a binding, but that's not a requirement.
            bool atLeastOneMatch = false;
            for (int whichSlot = 0; whichSlot < DataBindingProfiles.Length; whichSlot++)
            {
                bool matchFound = false;
                DataBindingProfile bindingProfile = DataBindingProfiles[whichSlot];
                string pattern = bindingProfile.GameObjectNameRegex;

                if (!String.IsNullOrEmpty(pattern))
                {
                    if (bindingProfile.UseRegex)
                    {
                        if (!_patternToRegexLookup.ContainsKey(pattern))
                        {
                            _patternToRegexLookup[pattern] = new Regex(pattern);
                        }

                        Regex regex = _patternToRegexLookup[pattern];
                        matchFound = (regex.Match(component.gameObject.name) != Match.Empty);
                    }
                    else
                    {
                        matchFound = (pattern == component.gameObject.name);
                    }
                }
                else
                {
                    matchFound = true;
                }

                if (matchFound)
                {
                    AddComponentToBindingInfoList(whichSlot, bindingProfile, component);
                    atLeastOneMatch = true;
                }
            }
            return atLeastOneMatch;
        }

        private void AddComponentToBindingInfoList(int whichSlot, DataBindingProfile bindingProfile, Component component)
        {
            BindingInfo binding;

            if (_bindingInfoList[whichSlot] == null)
            {
                binding = new BindingInfo();
                binding.BindingProfile = bindingProfile;
                _dataKeypathToBindingLookup[bindingProfile.DataKeyPath] = binding;
                _dataKeypathToBindingLookup[bindingProfile.ThemeKeyPath] = binding;
                _bindingInfoList[whichSlot] = binding;
            }

            binding = _bindingInfoList[whichSlot];

            binding.Components ??= new HashSet<Component>();
            // Add these here so we don't incur the expense of a 2nd regex in AddKeyPaths
            binding.Components.Add(component);
        }

        protected override void AddKeyPaths(HashSet<Component> componentsToManage)
        {
            // We don't really need componentsToManage since we added them
            // already in ComponentsMeetsAllQualifications to avoid a 2nd round of regex matches.

            for (int index = 0; index < _bindingInfoList.Length; index++)
            {
                BindingInfo binding = _bindingInfoList[index];
                if (binding != null && binding.Components != null && binding.Components.Count > 0)
                {
                    if (!String.IsNullOrEmpty(binding.BindingProfile.DataKeyPath))
                    {
                        binding.DataResolvedKeypath = AddKeyPathListener(binding.BindingProfile.DataKeyPath);
                    }
                    if (!String.IsNullOrEmpty(binding.BindingProfile.ThemeKeyPath))
                    {
                        binding.ThemeResolvedKeypath = AddKeyPathListener(binding.BindingProfile.ThemeKeyPath);
                    }
                }
            }
        }

        /// </inheritdoc/>
        protected override void DetachDataConsumer()
        {
            // TODO: free up any resources like lookups?
        }

        /// <summary>
        /// Load a streaming asset as specified asset path and return as appropriate type.
        /// </summary>
        /// <param name="assetPath">The path for the streaming asset.</param>
        /// <returns>T an asset of the expected type T.</returns>
        protected virtual T LoadStreamingAsset(string assetPath)
        {
            // override. Default provided in case streaming assets are not expected by subclass.
            return null;
        }

        /// <summary>
        /// Receive changed object, determine its, type and process appropriately
        /// </summary>
        /// <remarks>
        /// The object can be any of a number of types and loaded accordingly:
        ///
        /// int                     Use as index to select Nth entry in ValueToObjectInfo
        /// T                       Directly use the value to replace the managed variable of that type
        /// "resource://<<path>>"   Use path to load a Unity Resource
        /// "file://<<path>>"       Use path to load a streaming asset
        /// other string            Use string value to find entry by value in ValueToObjectInfo
        ///
        /// </remarks>
        /// <param name="dataSource">Which data source called this method.</param>
        /// <param name="resolvedKeyPath">Fully resolved keypath for datum that changed.</param>
        /// <param name="localKeyPath">Local keypath for the datum that changed.</param>
        /// <param name="inDataValue">The current value of the datum</param>
        /// <param name="dataChangeType">The type of change that has occurred.</param>
        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object inDataOrThemeValue, DataChangeType dataChangeType)
        {
            if (_themeKeypathToBindingLookup.TryGetValue(localKeyPath, out BindingInfo binding))
            {
                // In this case, inDataOrThemeValue is a changed theme value
                object dataValue = binding.DataSource.GetValue(binding.DataResolvedKeypath);
                ProcessThemeOrDataChange(binding, dataValue, inDataOrThemeValue);
            }
            else if (_dataKeypathToBindingLookup.TryGetValue(localKeyPath, out BindingInfo binding2))
            {
                // In this case, inDataOrThemeValue is a changed data value
                ProcessThemeOrDataChange(binding2, inDataOrThemeValue);
            }
        }

        /// <summary>
        /// Given a theme value and a datum value, determine the final output value
        /// </summary>
        /// <param name="dataValue">The data value that may be themed.</param>
        /// <param name="themeValue">The theme value used to theme the data value.</param>
        protected virtual void ProcessThemeOrDataChange(BindingInfo binding, object dataValue, object themeValue = null)
        {
            T outputValue = default(T);

            switch (binding.BindingProfile.RetrievalMethod)
            {

                case DataRetrievalMethod.AutoDetect:
                    outputValue = GetAutoDetectedValue(binding, dataValue);
                    break;
                case DataRetrievalMethod.StaticThemedValue:
                    outputValue = GetAutoDetectedValue(binding, themeValue);
                    break;
                case DataRetrievalMethod.DirectLookup:
                    outputValue = GetDirectLookupValue(binding, dataValue);
                    break;
                case DataRetrievalMethod.DirectValue:
                    outputValue = dataValue as T;
                    break;
                case DataRetrievalMethod.FilePath:
                    outputValue = GetStreamingAsset(dataValue);
                    break;
                case DataRetrievalMethod.ResourcePath:
                    outputValue = GetResourceValue(dataValue);
                    break;
                case DataRetrievalMethod.ThemeKeypathLookup:
                    outputValue = GetThemeKeypathLookupValue(binding, dataValue);
                    break;
            }

            if (binding.Components != null)
            {
                foreach (Component component in binding.Components)
                {
                    if (outputValue == null)
                    {
                        Debug.LogWarning("Setting a UX element to null for GameObject " + component.gameObject.name);
                    }
                    SetObject(component, dataValue, outputValue);
                }

            }
        }


        private T GetThemeKeypathLookupValue(BindingInfo binding, object dataValue)
        {
            if (IsInteger(dataValue, out int valueAsInt))
            {
                // Use as integer lookup into the value to object lookup
                return GetObjectByThemeLookupIndex(binding, valueAsInt);
            }
            else
            {
                return GetObjectByThemeLookupKey(binding, dataValue.ToString());
            }
        }

        private T GetStreamingAsset(object dataValue)
        {
            string strValue = dataValue.ToString();

            if (strValue.StartsWith(StreamingAssetPrefix))
            {
                strValue = strValue.Substring(StreamingAssetPrefix.Length);
            }

            return LoadStreamingAsset(strValue);
        }

        private T GetResourceValue(object dataValue)
        {
            string strValue = dataValue.ToString();

            if (strValue.StartsWith(ResourcePrefix))
            {
                strValue = strValue.Substring(ResourcePrefix.Length);
            }

            return Resources.Load(strValue) as T;
        }

        private T GetDirectLookupValue(BindingInfo binding, object dataValue)
        {
            if (IsInteger(dataValue, out int valueAsInt))
            {
                // Use as integer lookup into the value to object lookup
                return GetObjectByIndex(valueAsInt);

            }
            else
            {
                return GetObjectByKey(dataValue.ToString());
            }
        }

        private bool HasValueToThemeLookup(BindingInfo binding)
        {
            return binding.BindingProfile.ValueToThemeKeypathLookup != null && binding.BindingProfile.ValueToThemeKeypathLookup.Length > 0;
        }

        private T GetAutoDetectedValue(BindingInfo binding, object dataValue)
        {
            if (dataValue is T dataValueT)
            {
                // Direct setting of the object
                return dataValueT;
            }
            else if (IsInteger(dataValue, out int valueAsInt))
            {
                if (HasValueToThemeLookup(binding))
                {
                    return GetObjectByThemeLookupIndex(binding, valueAsInt);
                }
                else
                {
                    // Use as integer lookup into the value to object lookup
                    return GetObjectByIndex(valueAsInt);
                }
            }
            else if (dataValue is string dataValueString)
            {
                string strValue = dataValueString.ToLower();

                if (strValue.StartsWith(ResourcePrefix))
                {
                    strValue = strValue.Substring(ResourcePrefix.Length);
                    return Resources.Load(strValue) as T;
                }
                else if (strValue.StartsWith(StreamingAssetPrefix))
                {
                    strValue = strValue.Substring(StreamingAssetPrefix.Length);
                    return LoadStreamingAsset(strValue);
                }
                else
                {
                    if (HasValueToThemeLookup(binding))
                    {
                        return GetObjectByThemeLookupKey(binding, dataValue.ToString());
                    }
                    else
                    {
                        return GetObjectByKey(dataValue.ToString());
                    }
                }
            }

            return null;
        }


        public static bool IsInteger(object value, out int outIntValue)
        {
            if (value is string valueString && int.TryParse(valueString, out outIntValue))
            {
                return true;
            }
            else if (value is sbyte
                  || value is byte
                  || value is short
                  || value is ushort
                  || value is int
                  || value is uint
                  || value is long
                  || value is ulong)
            {
                outIntValue = (int)value;
                return true;
            }
            else
            {
                outIntValue = 0;
                return false;
            }
        }
    }
}
