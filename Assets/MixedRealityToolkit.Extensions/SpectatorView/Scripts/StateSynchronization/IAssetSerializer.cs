// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal interface IAssetSerializer<TAsset>
    {
        ShortID GetID();

        bool CanSerialize(TAsset asset);

        void Serialize(BinaryWriter writer, TAsset asset);

        TAsset Deserialize(BinaryReader reader);
    }
}
