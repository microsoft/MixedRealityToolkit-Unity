// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Adds or removes materials to target renderer for highlighting Focused <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>s.
    /// </summary>
    /// <remarks>Useful with focusable <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>s</remarks>
    public class InteractableOnFocus : BaseFocusHandler
    {
        /// <summary>
        /// List of profiles can match themes with gameObjects
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected List<VisualProfile> Profiles = new List<VisualProfile>();

        private List<InteractableThemeBase> themes = new List<InteractableThemeBase>();

        public void Awake()
        {
            foreach (var profile in this.Profiles)
            {
                var themeEngines = profile.CreateThemeEngines();

                themes.AddRange(themeEngines);
            }
        }

        private void UpdateThemes(bool isFocused)
        {
            var state = isFocused ? InteractableStates.InteractableStateEnum.Focus : InteractableStates.InteractableStateEnum.Default;

            foreach(var theme in this.themes)
            { 
                theme.OnUpdate((int)state, null, true);
            }
        }

        #region IMixedRealityFocusHandler Implementation

        /// <inheritdoc />
        public override void OnFocusEnter(FocusEventData eventData)
        {
            base.OnFocusEnter(eventData);

            UpdateThemes(HasFocus);
        }

        /// <inheritdoc />
        public override void OnFocusExit(FocusEventData eventData)
        {
            base.OnFocusExit(eventData);

            UpdateThemes(HasFocus);
        }

        #endregion IMixedRealityFocusHandler Implementation
    }
}
