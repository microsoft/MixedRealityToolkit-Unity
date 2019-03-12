// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement to react to focus enter/exit.
    /// </summary>
    public interface IMixedRealityFocusHandler : IEventSystemHandler
    {
        [Obsolete("Use IMixedRealityFocusChangedHandler instead.")]
        void OnBeforeFocusChange(FocusEventData eventData);

        [Obsolete("Use IMixedRealityFocusChangedHandler instead.")]
        void OnFocusChanged(FocusEventData eventData);

        /// <summary>
        /// The Focus Enter event is raised on this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> whenever a <see cref="Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.IMixedRealityPointer"/>'s focus enters this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>'s <see href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see>.
        /// </summary>
        /// <param name="eventData"></param>
        void OnFocusEnter(FocusEventData eventData);

        /// <summary>
        /// The Focus Exit event is raised on this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> whenever a <see cref="Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.IMixedRealityPointer"/>'s focus leaves this <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>'s <see href="https://docs.unity3d.com/ScriptReference/Collider.html">Collider</see>.
        /// </summary>
        /// <param name="eventData"></param>
        void OnFocusExit(FocusEventData eventData);
    }
}
