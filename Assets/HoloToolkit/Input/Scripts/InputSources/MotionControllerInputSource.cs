// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using HoloToolkit.Unity.InputModule;

public class MotionControllerInputSource : GamePadInputSource
{
    [Serializable]
    private class MappingEntry
    {
        public MotionControllerMappingTypes Type = MotionControllerMappingTypes.None;
        public string Value = string.Empty;
    }

    private const string MotionControllerRight = "";
    private const string MotionControllerLeft = "";


}
