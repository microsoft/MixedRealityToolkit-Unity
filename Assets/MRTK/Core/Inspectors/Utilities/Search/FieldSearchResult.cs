// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Search
{
    /// <summary>
    /// Struct for storing search results
    /// </summary>
    public struct FieldSearchResult
    {
        public SerializedProperty Property;
        public int MatchStrength;
    }
}