// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    public interface IStateEventConfig
    {
        string StateName { get; set; }

        BaseEventReceiver EventReceiver { get; set; }
    }
}
