// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Search
{
    /// <summary>
    /// Struct for configuring a search.
    /// </summary>
    public struct SearchConfig
    {
        public string SearchFieldString;
        public bool RequireAllKeywords;
        public bool SearchTooltips;
        public bool SearchFieldContent;
        public HashSet<string> Keywords;
    }
}