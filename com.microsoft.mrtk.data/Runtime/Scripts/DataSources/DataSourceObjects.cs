// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data source that manages structured data similar to what can be
    /// represented in JSON or XML, in particularly collections, maps and primitives.
    ///
    /// It can also manage arbitrary binary data or other complex data types, in the
    /// event that a data consumer exists that can consumer the data type, such as JPEG or WAV.
    ///
    /// This is a standalone data set that can be populated directly via
    /// SetValue() instead of depending on an external data source like a JSON or XML
    /// data stream.
    ///
    /// Note that SetValue() will automatically fabricate any missing structure such
    /// that it can be the sole means of populating even the most complex data set.
    /// Note that it is possible to inadvertently create large lists, such as by only
    /// creating the 1000th element of a collection.
    ///
    /// TODO: Refactor DataSourceJson to use this as its base implementation.
    /// </summary>
    public class DataSourceObjects : DataSourceBase
    {
        protected const string CollectionElementkeyPathPrefixFormat = "{0}[{1:d}]";
        protected const string DictionaryElementkeyPathPrefixFormat = "{0}[{1}]";

        protected const string ArrayTokenPattern = @"^\s*\[\s*([a-zA-Z0-9\-_]*?)\s*\]";
        protected const string KeyTokenPattern = @"^\s*([a-zA-Z0-9\-_]+?)(?:[.\[]|$)";

        protected readonly Regex _arrayTokenRegex = new Regex(ArrayTokenPattern);
        protected readonly Regex _keyTokenRegex = new Regex(KeyTokenPattern);

        protected DataNodeObject _rootNode;
        protected Dictionary<string, IDataNode> _keyPathToNodeLookup = new Dictionary<string, IDataNode>();

        public DataSourceObjects()
        {
            _rootNode = new DataNodeObject(DataNodeType.Map);
        }

        /// <summary>
        /// Constructor that provides the root node
        /// </summary>
        /// <param name="rootNodeType">Type for the root node</param>
        /// <param name="value">The value associated with the root node.</param>
        public DataSourceObjects(DataNodeType rootNodeType, object value = null)
        {
            _rootNode = new DataNodeObject(rootNodeType, value);
        }

        /// <inheritdoc/>
        public override bool IsCollectionAtKeyPath(string resolvedKeyPath)
        {
            IDataNode node = KeyPathToNode(resolvedKeyPath);
            if (node != null)
            {
                return node.IsArray();
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override int GetCollectionCount(string resolvedKeyPath)
        {
            IDataNode node = KeyPathToNode(resolvedKeyPath);
            if (node != null)
            {
                return node.GetCollectionCount();
            }
            else
            {
                return 0;
            }
        }

        /// <inheritdoc/>
        public override string GetNthCollectionKeyPathAt(string resolvedKeyPath, int n)
        {
            IDataNode dataNode = KeyPathToNode(resolvedKeyPath);
            if (dataNode != null && dataNode.IsArray() && n < dataNode.GetCollectionCount())
            {
                return string.Format(CollectionElementkeyPathPrefixFormat, resolvedKeyPath, n);
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public override void SetValueInternal(string resolvedKeyPath, object value)
        {
            // navigate path, creating any missing structure
            IDataNode dataNode = KeyPathToNode(resolvedKeyPath, true);
            if (dataNode != null)
            {
                dataNode.SetValue(value);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetCollectionKeyPathRange(string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            if (IsDataAvailable())
            {
                IDataNode dataNode = KeyPathToNode(resolvedKeyPath);
                if (dataNode != null && dataNode.IsArray())
                {
                    return GetValueAsArrayKeyPaths(resolvedKeyPath, rangeStart, rangeCount);
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public override object GetValueInternal(string resolvedKeyPath)
        {
            if (IsDataAvailable())
            {
                IDataNode dataNode = KeyPathToNode(resolvedKeyPath);
                if (dataNode != null)
                {
                    if (dataNode.IsArray())
                    {
                        return GetValueAsArrayKeyPaths(resolvedKeyPath, 0, dataNode.GetCollectionCount());
                    }
                    else if (dataNode.IsMap())
                    {
                        return GetValueAsDictionaryKeyPaths(dataNode, resolvedKeyPath);
                    }
                    else
                    {
                        return dataNode.GetValue();
                    }
                }
            }

            return null;
        }

        protected IEnumerable<string> GetValueAsArrayKeyPaths(string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            return new KeyPathEnumerable(resolvedKeyPath, rangeStart, rangeCount);
        }

        protected IEnumerable<string> GetValueAsDictionaryKeyPaths(IDataNode mapNode, string resolvedKeyPath)
        {
            List<string> keyPaths = new List<string>();
            foreach (string key in mapNode.GetMapKeys())
            {
                keyPaths.Add(string.Format(DictionaryElementkeyPathPrefixFormat, resolvedKeyPath, key));
            }
            return keyPaths;
        }

        protected IDataNode KeyPathToNode(string resolvedKeyPath, bool createIfMissing = false)
        {
            // walk down a simple path like:
            //      [1].name
            //      address.street
            //      addresses[1].street
            //      contacts[2].email_addresses[2]
            //      matrix[1][2]

            if (_keyPathToNodeLookup.ContainsKey(resolvedKeyPath))
            {
                return _keyPathToNodeLookup[resolvedKeyPath];
            }
            else
            {
                IDataNode currentNode = _rootNode;

                string keyPath = resolvedKeyPath;

                while (currentNode != null && keyPath != null && keyPath != "")
                {
                    int amountToSkip = 0;
                    MatchCollection arrayMatches = _arrayTokenRegex.Matches(keyPath);
                    if (arrayMatches.Count > 0)
                    {
                        string arrayIndexText = arrayMatches[0].Groups[1].Value;
                        int arrayIndex = int.Parse(arrayIndexText);

                        if (createIfMissing)
                        {
                            if (currentNode.IsUnassigned())
                            {
                                currentNode.SetNodeType(DataNodeType.Array);
                            }

                            while (arrayIndex >= currentNode.GetCollectionCount())
                            {
                                currentNode.AddToArray(new DataNodeObject());
                            }
                        }

                        if (currentNode.IsArray())
                        {
                            currentNode = currentNode.GetNodeByIndex(arrayIndex);
                        }
                        else
                        {
                            currentNode = null;
                        }
                        amountToSkip = arrayMatches[0].Value.Length;
                    }
                    else
                    {
                        MatchCollection keyMatches = _keyTokenRegex.Matches(keyPath);
                        if (keyMatches.Count > 0)
                        {
                            string key = keyMatches[0].Groups[1].Value;

                            if (createIfMissing)
                            {
                                if (currentNode.IsUnassigned())
                                {
                                    currentNode.SetNodeType(DataNodeType.Map);
                                }

                                if (currentNode.GetNodeByKey(key) == null)
                                {
                                    currentNode.AddToMap(key, new DataNodeObject());
                                }
                            }

                            if (currentNode.IsMap())
                            {
                                currentNode = currentNode.GetNodeByKey(key);
                            }
                            else
                            {
                                currentNode = null;
                            }

                            amountToSkip = key.Length;
                        }
                    }

                    if (keyPath.Length > amountToSkip && keyPath[amountToSkip] == '.')
                    {
                        amountToSkip++;
                    }
                    if (amountToSkip > 0)
                    {
                        keyPath = keyPath.Substring(amountToSkip);
                    }
                    else
                    {
                        currentNode = null;
                        break;      // was not a valid path piece
                    }
                }

                if (currentNode != null)
                {
                    _keyPathToNodeLookup[resolvedKeyPath] = currentNode;
                }
                return currentNode;
            }
        }

        /// <inheritdoc/>
        public override bool IsDataAvailable()
        {
            return _rootNode != null;
        }
    } // End of class DataSourceObjects
} // End of namespace Microsoft.MixedReality.Toolkit.Data
