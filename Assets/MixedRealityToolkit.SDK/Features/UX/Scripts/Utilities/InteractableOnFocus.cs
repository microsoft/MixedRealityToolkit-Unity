// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Adds or removes materials to target renderer for highlighting Focused <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>s.
    /// </summary>
    /// <remarks>Useful with focusable <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see>s</remarks>
    [AddComponentMenu("Scripts/MRTK/SDK/InteractableOnFocus")]
    public class InteractableOnFocus : BaseFocusHandler
    {
        /// <summary>
        /// List of profiles can match themes with gameObjects
        /// </summary>
        [SerializeField]
        [HideInInspector]
        protected List<VisualProfile> Profiles = new List<VisualProfile>();

        protected InteractableStates.InteractableStateEnum State
        {
            get
            {
                return HasFocus ? InteractableStates.InteractableStateEnum.Focus : InteractableStates.InteractableStateEnum.Default;
            }
        }

        private List<InteractableThemeBase> themes = new List<InteractableThemeBase>();

        public void Awake()
        {
            foreach (var profile in Profiles)
            {
                var themeEngines = profile.CreateThemeEngines();

                themes.AddRange(themeEngines);
            }
        }

        public void Update()
        {
            foreach (var theme in themes)
            {
                theme.OnUpdate((int)State, false);
            }
        }
    }
}
