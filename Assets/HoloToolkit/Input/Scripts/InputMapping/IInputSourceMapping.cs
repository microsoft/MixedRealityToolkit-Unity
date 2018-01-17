// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputModule
{
    public interface IInputSourceMapping<T>
    {
        InputSourceMappingValue<T>[] MappingValues { get; set; }
        string GetMapping(T type);
        void SetMapping(T type, string value);
        float GetAxis(T type);
        bool GetButtonUp(T type);
        bool GetButtonDown(T type);
        bool GetButtonPressed(T type);
    }
}