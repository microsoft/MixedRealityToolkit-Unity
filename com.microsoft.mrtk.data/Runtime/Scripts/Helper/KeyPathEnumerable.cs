// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Efficient enumerable for collections that only creates exactly what is requested.
    /// </summary>
    public class KeyPathEnumerable : IEnumerable<string>, IEnumerator<string>
    {
        protected static readonly string CollectionElementkeyPathPrefixFormat = "{0}[{1:d}]";

        protected int _rangeStart;
        protected int _rangeEnd;
        protected int _currentIndex;
        protected string _keyPathPrefix;

        public KeyPathEnumerable(string keyPathPrefix, int rangeStart, int rangeCount)
        {
            _rangeStart = rangeStart;
            _rangeEnd = rangeStart + rangeCount;
            _keyPathPrefix = keyPathPrefix;
            _currentIndex = _rangeStart;
        }

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator()
        {
            return this;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public string Current
        {
            get
            {
                if (_currentIndex > _rangeStart && _currentIndex <= _rangeEnd)
                {
                    return GenerateKeyPath();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current { get { return Current; } }

        /// <inheritdoc/>
        public void Reset()
        {
            _currentIndex = _rangeStart;
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (_currentIndex <= _rangeEnd)
            {
                _currentIndex++;
            }

            return _currentIndex <= _rangeEnd;
        }

        protected string GenerateKeyPath()
        {
            // minus 1 because a MoveNext is done before fetching first item.
            return string.Format(CollectionElementkeyPathPrefixFormat, _keyPathPrefix, _currentIndex - 1);
        }

        void IDisposable.Dispose() { }

    } // End of class KeyPathEnumerable
} // End of namespace Microsoft.MixedReality.Toolkit.Data
