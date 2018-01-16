// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.SpatialMapping
{
    public static class DataEventArgs
    {
        public static DataEventArgs<TData> Create<TData>(TData data)
        {
            return new DataEventArgs<TData>(data);
        }
    }

    [Serializable]
    public class DataEventArgs<TData> : EventArgs
    {
        public TData Data { get; private set; }

        public DataEventArgs(TData data)
        {
            Data = data;
        }
    }
}
