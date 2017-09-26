// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an source state event that has a source id. 
    /// </summary>
    public class SourceStateEventData : BaseInputEventData
    {
        public SourceStateEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag)
        {
            BaseInitialize(inputSource, sourceId, tag);
        }
    }
}