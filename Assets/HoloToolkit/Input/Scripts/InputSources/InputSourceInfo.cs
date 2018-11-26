// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// InputSourceInfo gives you the input source like hands or motion controller.
    /// It will also report the source id for that source.
    /// </summary>
    public struct InputSourceInfo : IInputSourceInfoProvider
    {
        public IInputSource InputSource;
        public uint SourceId;

        public InputSourceInfo(IInputSource inputSource, uint sourceId) : this()
        {
            InputSource = inputSource;
            SourceId = sourceId;
        }

        IInputSource IInputSourceInfoProvider.InputSource
        {
            get { return InputSource; }
        }

        uint IInputSourceInfoProvider.SourceId
        {
            get { return SourceId; }
        }

        public bool Matches(IInputSourceInfoProvider other)
        {
            return ((other != null) && Matches(other.InputSource, other.SourceId));
        }

        public bool Matches(IInputSource otherInputSource, uint otherSourceId)
        {
            return ((InputSource == otherInputSource) && (SourceId == otherSourceId));
        }
    }
}
