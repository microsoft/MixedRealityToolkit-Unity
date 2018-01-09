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
        public IPointingSource PointingSource { get; private set; }

        public SourceStateEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            PointingSource = null;
        }

        public void Initialize(IPointingSource inputSource, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            PointingSource = inputSource;
        }
    }
}