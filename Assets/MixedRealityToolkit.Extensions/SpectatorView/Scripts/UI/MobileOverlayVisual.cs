// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    /// <summary>
    /// Delegate called to toggle visibility for <see cref="IMobileOverlayVisualChild"/>ren.
    /// </summary>
    /// <param name="visible">If true, show any associated game object/UI. If false, hide said content</param>
    public delegate void OverlayVisibilityRequest(bool visible);

    /// <summary>
    /// Interface implemented by classes that show/hide based on spectator view UI visibility changes
    /// </summary>
    public interface IMobileOverlayVisualChild
    {
        /// <summary>
        /// Show any associated UI/GameObjects
        /// </summary>
        void Show();

        /// <summary>
        /// Hide any associated UI/GameObjects
        /// </summary>
        void Hide();

        /// <summary>
        /// Event for requesting to show/hide spectator view UI
        /// </summary>
        event OverlayVisibilityRequest OverlayVisibilityRequest;
    }

    /// <summary>
    /// Helper class responsible for facilitating visibility changes in mobile UI
    /// </summary>
    public class MobileOverlayVisual : MonoBehaviour
    {
        /// <summary>
        /// Amount of time (in seconds) to press and hold the screen in order to toggle mobile UI visibility.
        /// </summary>
        [Tooltip("Amount of time (in seconds) to press and hold the screen in order to toggle mobile UI visibility")]
        [SerializeField]
        float _holdTimeToDisplay = 1.0f;

        /// <summary>
        /// List of MonoBehaviours implementing <see cref="IMobileOverlayVisualChild"/>. Errors will be thrown if any elements don't implement IMobileOverlayVisualChild.
        /// </summary>
        [Tooltip("List of MonoBehaviours implementing IMobileOverlayVisualChild. Errors will be thrown if any elements don't implement IMobileOverlayVisualChild.")]
        [SerializeField]
        List<MonoBehaviour> _children;

        private List<IMobileOverlayVisualChild> _overlayChildren;
        private float _lastTouchTime = 0;
        private bool _touching = false;
        private bool _uiToggledForTouch = false;
        private bool _showingUI = false;
        private bool _uiNeedsUpdate = false;

        private void OnValidate()
        {
#if UNITY_EDITOR
            foreach (var child in _children)
            {
                FieldHelper.ValidateType<IMobileOverlayVisualChild>(child);
            }
#endif
        }

        private void Awake()
        {
            _overlayChildren = new List<IMobileOverlayVisualChild>();
            foreach(var child in _children)
            {
                var overlayChild = child as IMobileOverlayVisualChild;
                if (overlayChild != null)
                {
                    overlayChild.OverlayVisibilityRequest += OnOverlayVisibilityRequest;
                    _overlayChildren.Add(overlayChild);
                }
            }
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                if (!_touching)
                {
                    _lastTouchTime = Time.time;
                    _touching = true;
                }

                if (!_uiToggledForTouch &&
                    (Time.time - _lastTouchTime) > _holdTimeToDisplay)
                {
                    ToggleUIVisibility();
                    _uiToggledForTouch = true;
                }
            }
            else
            {
                _touching = false;
                _uiToggledForTouch = false;
            }

            if (_uiNeedsUpdate)
            {
                UpdateUI();
            }
        }

        private void ToggleUIVisibility()
        {
            SetUIVisibility(!_showingUI);
        }

        private void OnOverlayVisibilityRequest(bool visibility)
        {
            SetUIVisibility(visibility);
        }

        private void SetUIVisibility(bool visibility)
        {
            if (_showingUI != visibility)
            {
                _showingUI = visibility;
                _uiNeedsUpdate = true;
            }
        }

        private void UpdateUI()
        {
            if (_children != null)
            {
                foreach (var child in _overlayChildren)
                {
                    if (_showingUI)
                    {
                        child.Show();
                    }
                    else
                    {
                        child.Hide();
                    }
                }
            }

            _uiNeedsUpdate = false;
        }
    }
}
