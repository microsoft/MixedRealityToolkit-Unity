// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Interaction
{
    public interface IStateEventConfig
    {
        string StateName { get; set; }

        BaseEventReceiver EventReceiver { get; set; }
    }
}
