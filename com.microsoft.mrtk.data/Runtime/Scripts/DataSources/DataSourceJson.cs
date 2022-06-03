// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data source that can parse a JSON text stream and notify any data consumers
    /// listening for changes.
    ///
    /// This data source has on Unity specific dependencies.
    ///
    /// TODO: refactor this to use DataSourceObjects.  Should be a fairly straight refactor since
    ///       DataSourceObjects was derived from this source code. See DataNodeJson
    ///
    /// TODO: Add support for SetValue() and ability to report changes to a back-end
    ///       services for persisting those changes. This would then allow for
    ///       data entry slates.
    ///
    /// </summary>
    public class DataSourceJson : DataSourceBase
    {
        private JToken _jsonRootNode = null;
        private const string CollectionElementkeyPathPrefixFormat = "{0}[{1:d}]";
        private const string DictionaryElementkeyPathPrefixFormat = "{0}[{1}]";

        private const string ArrayTokenPattern = @"^\s*\[\s*([a-zA-Z0-9\-_]*?)\s*\]";
        private const string KeyTokenPattern = @"^\s*([a-zA-Z0-9\-_]+?)(?:[.\[]|$)";

        protected readonly Regex _arrayTokenRegex = new Regex(ArrayTokenPattern);
        protected readonly Regex _keyTokenRegex = new Regex(KeyTokenPattern);

        protected Dictionary<string, JToken> _keyPathToJTokenLookup = new Dictionary<string, JToken>();

        /// <inheritdoc/>
        public override bool IsCollectionAtKeyPath(string resolvedKeyPath)
        {
            JToken node = KeyPathToNode(resolvedKeyPath);
            if (node != null)
            {
                return node.Type == JTokenType.Array;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override int GetCollectionCount(string resolvedKeyPath)
        {
            JToken node = KeyPathToNode(resolvedKeyPath);
            TryGetNodeContainerCount(node, out int count);
            return count;
        }

        private bool NodeIsArray(JToken node)
        {
            return node != null && node.Type == JTokenType.Array;
        }

        private bool NodeIsObject(JToken node)
        {
            return node != null && node.Type == JTokenType.Object;
        }

        private bool NodeIsNumber(JToken node)
        {
            return node != null && (node.Type == JTokenType.Integer || node.Type == JTokenType.Float);
        }

        private bool TryGetNodeContainerCount(JToken node, out int outCount)
        {
            if (NodeIsArray(node))
            {
                JContainer nodeContainer = node as JContainer;
                outCount = nodeContainer.Count;
                return true;
            }
            else
            {
                outCount = 0;
                return false;
            }
        }

        /// <inheritdoc/>
        public override string GetNthCollectionKeyPathAt(string resolvedKeyPath, int n)
        {
            JToken node = KeyPathToNode(resolvedKeyPath);

            if (TryGetNodeContainerCount(node, out int count) && n < count)
            {
                return string.Format(CollectionElementkeyPathPrefixFormat, resolvedKeyPath, n);
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetCollectionKeyPathRange(string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            if (IsDataAvailable())
            {
                JToken node = KeyPathToNode(resolvedKeyPath);
                if (node != null)
                {
                    if (NodeIsArray(node))
                    {
                        return GetValueAsArrayKeyPaths(node, resolvedKeyPath, rangeStart, rangeCount);
                    }
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public override object GetValueInternal(string resolvedKeyPath)
        {
            if (IsDataAvailable())
            {
                JToken node = KeyPathToNode(resolvedKeyPath);
                if (node != null)
                {
                    if (NodeIsArray(node))
                    {
                        TryGetNodeContainerCount(node, out int count);

                        return GetValueAsArrayKeyPaths(node, resolvedKeyPath, 0, count);
                    }
                    else if (NodeIsObject(node))
                    {
                        return GetValueAsDictionaryKeyPaths(node, resolvedKeyPath);
                    }
                    else if (NodeIsNumber(node))
                    {
                        return node.Value<float>();
                    }
                    else
                    {
                        return node.Value<string>();
                    }
                }
            }
            return null;
        }

        protected IEnumerable<string> GetValueAsArrayKeyPaths(JToken arrayNode, string resolvedKeyPath, int rangeStart, int rangeCount)
        {
            // TODO: To optimize for large collections, this method should instantiate a class that can provide an IEnumerable<string>
            //       but then only generates key paths as they are requested. This avoids creating large lists of key paths at once
            //       that may never get used and will occupy an unnecessary amount of RAM.

            TryGetNodeContainerCount(arrayNode, out int arraySize);

            int rangeMax = rangeStart + rangeCount;
            if (rangeMax > arraySize)
            {
                rangeMax = arraySize;
            }

            List<string> keyPaths = new List<string>();
            for (int idx = rangeStart; idx < rangeMax; idx++)
            {
                keyPaths.Add(string.Format(CollectionElementkeyPathPrefixFormat, resolvedKeyPath, idx));
            }
            return keyPaths;
        }

        protected IEnumerable<string> GetValueAsDictionaryKeyPaths(JToken dictNode, string resolvedKeyPath)
        {
            // TODO: To optimize for large collections, this method should instantiate a class that can provide an IEnumerable<string>
            //       but then only generates key paths as they are requested. This avoids creating large lists of key paths at once
            //       that may never get used and will occupy an unnecessary amount of RAM.

            List<string> keyPaths = new List<string>();
            Dictionary<string, object> dictObj = dictNode.ToObject<Dictionary<string, object>>();

            foreach (string key in dictObj.Keys)
            {
                keyPaths.Add(string.Format(DictionaryElementkeyPathPrefixFormat, resolvedKeyPath, key));
            }
            return keyPaths;
        }

        protected JToken KeyPathToNode(string resolvedKeyPath)
        {
            // walk down a simple path like:
            //      address.street
            //      addresses[1].street
            //      contacts[2].email_addresses[2]
            //      matrix[1][2]
            //
            // and find the specified node of the data set.

            if (_keyPathToJTokenLookup.ContainsKey(resolvedKeyPath))
            {
                return _keyPathToJTokenLookup[resolvedKeyPath];
            }
            else if (_jsonRootNode == null)
            {
                return null;
            }
            else
            {
                JToken node = _jsonRootNode;
                string keyPath = resolvedKeyPath;

                while (node != null && keyPath != null && keyPath != "")
                {
                    int amountToSkip = 0;
                    MatchCollection arrayMatches = _arrayTokenRegex.Matches(keyPath);
                    if (arrayMatches.Count > 0)
                    {
                        string arrayIndexText = arrayMatches[0].Groups[1].Value;
                        int arrayIndex = int.Parse(arrayIndexText);

                        node = node[arrayIndex];
                        amountToSkip = arrayMatches[0].Value.Length;
                    }
                    else
                    {
                        MatchCollection keyMatches = _keyTokenRegex.Matches(keyPath);
                        if (keyMatches.Count > 0)
                        {
                            string key = keyMatches[0].Groups[1].Value;

                            node = node[key];
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
                        node = null;
                        break;      // was not a valid path piece
                    }
                }

                if (node != null)
                {
                    _keyPathToJTokenLookup[resolvedKeyPath] = node;
                }
                return node;
            }
        }

        public void UpdateFromJson(string jsonString)
        {
            DataChangeSetBegin();
            _keyPathToJTokenLookup.Clear();
            _jsonRootNode = JToken.Parse(jsonString);

            NotifyListeners();
            DataChangeSetEnd();
        }

        protected void NotifyListeners()
        {
            List<string> keys = new List<string>(_keyPathToDataConsumers.Keys);

            foreach (string keyPath in keys)
            {
                NotifyDataChanged(keyPath, GetValue(keyPath), DataChangeType.DatumModified, false);
            }
        }

        /// <inheritdoc/>
        public override bool IsDataAvailable()
        {
            return _jsonRootNode != null;
        }
    }
}
