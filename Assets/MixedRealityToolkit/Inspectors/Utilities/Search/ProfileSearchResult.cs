// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Search
{
    /// <summary>
    /// Struct for pairing profiles with a set of search results
    /// </summary>
    public class ProfileSearchResult
    {
        public int ProfileMatchStrength;
        public bool IsCustomProfile;
        public UnityEngine.Object Profile;
        public List<FieldSearchResult> Fields = new List<FieldSearchResult>();
    }
}