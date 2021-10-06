using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{

    [Serializable]
    // This helps get around issue when mixing generics, concrete classes and interfaces in the inheritance.
    public abstract class DataConsumerThemableBase : DataConsumerGOBase, IDataConsumerThemable
    {
        /// <summary>
        /// The type of data that's expected from the data source
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// Automatically determine the type from analyzing the nature of the provided data
            /// </summary>
            AutoDetect,

            /// <summary>
            /// Direct value of the correct type from data source with no theming
            /// </summary>
            DirectValue,

            /// <summary>
            /// An integral index or string key used to look up the desired value from a local lookup table
            /// </summary>
            DirectLookup,

            /// <summary>
            /// Static themed object of the correct type from the theme data source
            /// </summary>
            StaticThemedValue,

            /// <summary>
            ///  An integral index or string key used to look up the desired theme keypath
            /// </summary>
            ThemeKeypathLookup,

            /// <summary>
            /// A string property name that will be appended to the theme base keypath provided in the Theme Helper.
            /// </summary>
            ThemeKeypathProperty,

            /// <summary>
            /// A resource path for retrieving the value from a Unity resource.
            /// </summary>
            ResourcePath,

            /// <summary>
            /// A file path for retrieving a Unity streaming asset.
            /// </summary>
            FilePath
        }


        [Tooltip("Key path for data-focused data source that will be used to establish the value of the managed data bound element.")]
        [SerializeField]
        protected string dataKeyPath;

        [Tooltip("The type of data expected from the primary data source that will be used either directly, or to fetch the final object from the specified source. The value could be provided locally, or via a theme.")]
        [SerializeField]
        protected DataType expectedDataType;

        [Tooltip("(Optional) Data Consumer Theme Helper is used to manage communication with the theme-focused data source in situations where an element is both dynamic and themed.")]
        [SerializeField]
        protected DataConsumerThemeHelper dataConsumerThemeHelper;

        [Serializable]
        public struct ValueToKeypath
        {
            [Tooltip("Value from the data source to be mapped to an object.")]
            [SerializeField] public string value;

            [Tooltip("Relative or absolute theme keypath that is used to retrieve the desired object.")]
            [SerializeField] public string keypath;
        }

        [Tooltip("(Optional) List of <optionalKey, themeKeypath> pairs, where either an index or a key data source is used to lookup a theme keypath.")]
        [SerializeField]
        protected ValueToKeypath[] valueToThemeKeypathLookup;

        public abstract void ProcessThemeDataChanged(IDataConsumer themeHelper, string resolvedKeyPath, string localKeyPath, object inValue, DataChangeType dataChangeType);
    }


    /// <summary>
    /// This class provides a way to load a resource via many different
    /// potential retrieval means based on the nature of the value received
    /// from the data source:
    /// 
    ///    Object of correct type provided directly
    ///    Numeeric index lookup
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
        protected virtual T GetObjectByThemeLookupIndex(int n)
        {
            if (n < valueToThemeKeypathLookup.Length)
            {
                return dataConsumerThemeHelper?.DataSource?.GetValue(valueToThemeKeypathLookup[n].keypath) as T ?? null;
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
 

        protected virtual T GetObjectByThemeLookupKey(string keyValue)
        {

            foreach (ValueToKeypath valueToKeypath in valueToThemeKeypathLookup)
            {
                if (keyValue == valueToKeypath.value)
                {
                    return dataConsumerThemeHelper?.DataSource?.GetValue(valueToKeypath.keypath) as T ?? null;
                }
            }

            return null;
        }

        public const string ResourcePrefix = "resource://";
        public const string StreamingAssetPrefix = "file://";

        protected List<Component> componentsToManage = new List<Component>();

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



  


        protected override void AddVariableKeyPathsForComponent(Type componentType, Component component)
        {
            componentsToManage.Add(component);
        }

        protected override void AttachDataConsumer()
        {
            AddKeyPathListener(dataKeyPath);

            if (dataConsumerThemeHelper != null)
            {
                dataConsumerThemeHelper.DataConsumerThemable = this as IDataConsumerThemable;
            }
        }


        protected override void DetachDataConsumer()
        {
            componentsToManage.Clear();
            if (dataConsumerThemeHelper != null)
            {
                dataConsumerThemeHelper.DataConsumerThemable = null;
            }
        }

        /// <summary>
        /// Update the provided component using the objectToSet. 
        /// </summary>
        /// <param name="component">Which component to update</param>
        /// <param name="inValue">Original value from IDataSource</param>
        /// <param name="objectToSet">The object used to update component</param>
        protected abstract void SetObject(Component component, object inValue, T objectToSet);

        /// <summary>
        /// Load a streaming asset as specified asset path and return as appropriate type.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
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
        /// <param name="dataSource"></param>
        /// <param name="resolvedKeyPath"></param>
        /// <param name="localKeyPath"></param>
        /// <param name="inValue"></param>
        /// <param name="dataChangeType"></param>
        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object inDataValue, DataChangeType dataChangeType)
        {
            if (localKeyPath == dataKeyPath)
            {
                ProcessThemeOrDataChange(inDataValue);
            }
        }


        /// <summary>
        /// IDataConsumerThemable interface method called by a DataConsumerThemeHelper
        /// </summary>
        /// <param name="themeHelper"></param>
        /// <param name="resolvedKeyPath"></param>
        /// <param name="localKeyPath"></param>
        /// <param name="themeValue"></param>
        /// <param name="dataChangeType"></param>
        public override void ProcessThemeDataChanged(IDataConsumer themeHelper, string resolvedKeyPath, string localKeyPath, object themeValue, DataChangeType dataChangeType)
        {
            object dataValue = null;

            if (DataSource != null)
            {
                dataValue = DataSource.GetValue(DataSource.ResolveKeyPath(ResolvedKeyPathPrefix, dataKeyPath));
            }
            ProcessThemeOrDataChange(dataValue, themeValue);
        }


        protected virtual void ProcessThemeOrDataChange(object dataValue, object themeValue = null)
        {
            T outputValue = default(T);

            switch (expectedDataType)
            {

                case DataType.AutoDetect:
                    outputValue = GetAutoDetectedValue(dataValue, dataConsumerThemeHelper );
                    break;
                case DataType.StaticThemedValue:
                    outputValue = GetAutoDetectedValue(themeValue);
                    break;
                case DataType.DirectLookup:
                    outputValue = GetDirectLookupValue(dataValue);
                    break;
                case DataType.DirectValue:
                    outputValue = dataValue as T;
                    break;
                case DataType.FilePath:
                    outputValue = GetStreamingAsset(dataValue);
                    break;
                case DataType.ResourcePath:
                    outputValue = GetResourceValue(dataValue);
                    break;
                case DataType.ThemeKeypathLookup:
                    outputValue = GetThemeKeypathLookupValue(dataValue);
                    break;
            }


            foreach (Component component in componentsToManage)
            {
                SetObject(component, dataValue, outputValue);
            }
        }




        private T GetThemeKeypathLookupValue( object dataValue)
        {
            int valueAsInt;

            if (IsIntegral(dataValue, out valueAsInt))
            {
                // Use as integer lookup into the value to object lookup
                return GetObjectByThemeLookupIndex(valueAsInt);

            }
            else
            {
                return GetObjectByThemeLookupKey(dataValue.ToString());
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

        private T GetDirectLookupValue(object dataValue)
        {
            int valueAsInt;

            if (IsIntegral(dataValue, out valueAsInt))
            {
                // Use as integer lookup into the value to object lookup
                return GetObjectByIndex(valueAsInt);

            }
            else
            {
                return GetObjectByKey(dataValue.ToString());
            }
        }

        private T GetAutoDetectedValue( object dataValue, DataConsumerThemeHelper themeHelper = null)
        {
            int valueAsInt;

            if (dataValue is T)
            {
                // Direct setting of the object
                return dataValue as T;
            }
            else if (IsIntegral(dataValue, out valueAsInt))
            {
                if (themeHelper == null)
                {
                    // Use as integer lookup into the value to object lookup
                    return GetObjectByIndex(valueAsInt);
                }
                else
                {
                    return GetObjectByThemeLookupIndex(valueAsInt);
                }

            }
            else if (dataValue is string)
            {
                string strValue = (dataValue as string).ToLower();

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
                    if (themeHelper == null)
                    {
                        return GetObjectByKey(dataValue.ToString());
                    }
                    else
                    {
                        return GetObjectByThemeLookupKey(dataValue.ToString());
                    }
                }
            }

            return null;
        }




        public bool IsIntegral(object value, out int outIntValue)
        {
            if ( value is string && Int32.TryParse(value as string, out outIntValue))
            {
                return true;
            } 
            else if (   value is sbyte
                    ||  value is byte
                    ||  value is short
                    ||  value is ushort
                    ||  value is int
                    ||  value is uint
                    ||  value is long
                    ||  value is ulong )
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
