// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.InputModule.InputMapping
{
    [Serializable]
    public struct InputSourceMappingValue<T>
    {
        public InputSourceMappingValue(T inputType, string value) : this()
        {
            InputType = inputType;
            Value = value;
        }

        public T InputType { get; private set; }
        public string Value { get; set; }
    }
}
