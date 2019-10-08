// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Search
{
    /// <summary>
    /// Struct for pairing profiles with a set of search results
    /// </summary>
    public struct ProfileSearchResult
    {
        public static bool IsEmpty(ProfileSearchResult result)
        {
            return result.Profile == null || result.Fields == null;
        }

        public int ProfileMatchStrength;
        public bool IsCustomProfile;
        public int MaxFieldMatchStrength;
        public UnityEngine.Object Profile;
        public List<FieldSearchResult> Fields;
    }
}