// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Data structure for mapping Voice and Keyboard input to <see cref="MixedRealityInputAction"/>s that can be raised by the Input System.
    /// </summary>
    [Serializable]
    public struct SpeechCommands
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keyword">The Keyword.</param>
        /// <param name="keyCode">The KeyCode.</param>
        /// <param name="action">The Action to perform when Keyword or KeyCode is recognized.</param>
        /// <param name="localizationKey"> An optional key to use to override the keyword with a localized version </param>
        public SpeechCommands(string keyword, KeyCode keyCode, MixedRealityInputAction action, string localizationKey = "")
        {
            this.keyword = keyword;
            this.keyCode = keyCode;
            this.action = action;
            this.localizationKey = localizationKey;
            this.localizedKeyword = null;
        }

        [SerializeField]
        [Tooltip("The key to use to find a localized keyword")]
        private string localizationKey;

        private string localizedKeyword;

        /// <summary>
        /// The localized version of the keyword
        /// </summary>
        public string LocalizedKeyword
        {
            get
            {
#if WINDOWS_UWP
                if (!string.IsNullOrWhiteSpace(localizationKey) && string.IsNullOrWhiteSpace(localizedKeyword))
                {
                    try
                    {
                        var resourceLoader = global::Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
                        localizedKeyword = resourceLoader.GetString(localizationKey);
                    }
                    catch(System.Exception e)
                    {
                        // Ignore the exception and just use the fallback
                        Debug.LogError("GetLocalizedKeywordException: " + e.Message);
                    }
                }
#endif
                return string.IsNullOrWhiteSpace(localizedKeyword) ? keyword : localizedKeyword;
            }
        }

        [SerializeField]
        [Tooltip("The Fallback keyword to listen for.")]
        private string keyword;

        /// <summary>
        /// The Fallback Keyword to listen for, or the localization key if no fallback keyword was set.
        /// </summary>
        public string Keyword
        {
            get
            {
                return string.IsNullOrWhiteSpace(keyword) ? localizationKey : keyword;
            }
        }

        [SerializeField]
        [Tooltip("The corresponding KeyCode that also raises the same action as the Localized Keyword.")]
        private KeyCode keyCode;

        /// <summary>
        /// The corresponding KeyCode that also raises the same action as the Keyword.
        /// </summary>
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("The Action that is raised by either the Localized Keyword or KeyCode.")]
        private MixedRealityInputAction action;

        /// <summary>
        /// The <see cref="MixedRealityInputAction"/> that is raised by either the Keyword or KeyCode.
        /// </summary>
        public MixedRealityInputAction Action => action;
    }
}
