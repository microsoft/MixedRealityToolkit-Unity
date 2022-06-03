// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A generic DOM style data node that implements the IDataNode interface.
    ///
    /// Supports primitives, arrays and dictionaries.
    ///
    /// It's used by DataSourceObjects to implement a structured data set.
    /// </summary>
    public class DataNodeObject : IDataNode
    {
        protected object _value;
        protected DataNodeType _type;

        public DataNodeObject()
        {
            _type = DataNodeType.Unassigned;
        }

        public DataNodeObject(DataNodeType type, object value = null)
        {
            SetNodeType(type, value);
            _type = type;
        }

        public void SetNodeType(DataNodeType newNodeType, object value = null)
        {
            _type = newNodeType;

            switch (newNodeType)
            {
                case DataNodeType.Array:
                    _value = new List<IDataNode>();
                    break;
                case DataNodeType.Map:
                    _value = new Dictionary<string, IDataNode>();
                    break;
                default:
                    _value = value;
                    break;
            }
        }

        /// <inheritdoc/>
        public bool IsArray()
        {
            return _type == DataNodeType.Array;
        }

        /// <inheritdoc/>
        public bool IsMap()
        {
            return _type == DataNodeType.Map;
        }

        /// <inheritdoc/>
        public bool IsUnassigned()
        {
            return _type == DataNodeType.Unassigned;
        }

        /// <inheritdoc/>
        public virtual IDataNode GetNodeByIndex(int n)
        {
            List<IDataNode> nodeArray = GetArray();

            if (nodeArray != null)
            {
                return nodeArray[n];
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual IDataNode GetNodeByKey(string key)
        {
            if (IsMap() && GetMap().ContainsKey(key))
            {
                return GetMap()[key];
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public virtual int GetCollectionCount()
        {
            if (IsMap())
            {
                return GetMap().Count;
            }
            else if (IsArray())
            {
                return GetArray().Count;
            }
            else
            {
                return 0;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetMapKeys()
        {
            Dictionary<string, IDataNode> map;

            map = GetMap();
            if (map != null)
            {
                return map.Keys;
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public object GetValue()
        {
            return _value;
        }

        /// <inheritdoc/>
        public void SetValue(object newValue)
        {
            _value = newValue;
        }

        /// <inheritdoc/>
        public DataNodeType GetNodeType()
        {
            return _type;
        }

        /// <inheritdoc/>
        public void AddToArray(IDataNode dataNode)
        {
            if (IsArray())
            {
                GetArray().Add(dataNode);
            }
        }

        /// <inheritdoc/>
        public void AddToMap(string key, IDataNode dataNode)
        {
            if (IsMap())
            {
                GetMap()[key] = dataNode;
            }
        }

        protected Dictionary<string, IDataNode> GetMap()
        {
            return _value as Dictionary<string, IDataNode>;
        }

        protected List<IDataNode> GetArray()
        {
            return _value as List<IDataNode>;
        }
    }
}
