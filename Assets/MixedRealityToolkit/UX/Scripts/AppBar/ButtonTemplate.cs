// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using MixedRealityToolkit.UX.Receivers;
using UnityEngine.Events;

namespace MixedRealityToolkit.UX.AppBarControl
{
    /// <summary>
    /// Class used for building toolbar buttons
    /// (not yet in use)
    /// </summary>
    [Serializable]
    public struct ButtonTemplate
    {
        public ButtonTemplate(ButtonTypeEnum type, string name, string icon, string text, int defaultPosition, int manipulationPosition)
        {
            Type = type;
            Name = name;
            Icon = icon;
            Text = text;
            DefaultPosition = defaultPosition;
            ManipulationPosition = manipulationPosition;
            EventTarget = null;
            OnTappedEvent = null;
        }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Name);
            }
        }

        public int DefaultPosition;
        public int ManipulationPosition;
        public ButtonTypeEnum Type;
        public string Name;
        public string Icon;
        public string Text;
        public InteractionReceiver EventTarget;
        public UnityEvent OnTappedEvent;
    }
}