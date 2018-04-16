// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Sources;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.InputSystem.EventData
{
    /// <summary>
    /// Describes an source state event that has a source id. 
    /// </summary>
    public class SourceStateEventData : BaseInputEventData
    {
        public SourceStateEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, object[] tags)
        {
            BaseInitialize(inputSource, tags);
        }
    }
}
