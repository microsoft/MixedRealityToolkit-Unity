// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// IInputSourceInfoProvider gives you the input source like hands or motion controller.
    /// It will also report the source id for that source.
    /// </summary>
    public interface IInputSourceInfoProvider
    {
        IInputSource InputSource { get; }
        uint SourceId { get; }
    }
}