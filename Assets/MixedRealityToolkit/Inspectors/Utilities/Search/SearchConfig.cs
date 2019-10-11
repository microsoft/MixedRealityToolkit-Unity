// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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