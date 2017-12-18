// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class GamePadEventData : InputEventData
    {
        public string GamePadName { get; private set; }

        public GamePadEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource source, uint sourceId, string eventOrigin, string gamePadName, object[] tag = null)
        {
            BaseInitialize(source, sourceId, eventOrigin, tag);
            GamePadName = gamePadName;
        }
    }
}
