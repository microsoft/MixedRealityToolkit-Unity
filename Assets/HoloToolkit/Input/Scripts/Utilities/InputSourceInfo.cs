// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// InputSourceInfo gives you the input source like hands or motion controller.
    /// It will also report the source id for that source.
    /// </summary>
    public struct InputSourceInfo
    {
        public IInputSource InputSource;
        public uint SourceId;

        public InputSourceInfo(IInputSource inputSource, uint sourceId) :
            this()
        {
            InputSource = inputSource;
            SourceId = sourceId;
        }

        public bool Matches(IInputSource otherInputSource, uint otherSourceId)
        {
            return ((InputSource == otherInputSource) && (SourceId == otherSourceId));
        }
    }
}
