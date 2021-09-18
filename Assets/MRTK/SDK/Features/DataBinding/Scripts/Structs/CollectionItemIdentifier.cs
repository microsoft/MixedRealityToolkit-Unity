// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit.Data
{ 

    public class CollectionItemIdentifier
    {
        public string fullyResolvedKeypath;
        public int indexPosition;

        public CollectionItemIdentifier( string keyPath, int position )
        {
            fullyResolvedKeypath = keyPath;
            indexPosition = position;
        }
    }
}