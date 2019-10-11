// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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