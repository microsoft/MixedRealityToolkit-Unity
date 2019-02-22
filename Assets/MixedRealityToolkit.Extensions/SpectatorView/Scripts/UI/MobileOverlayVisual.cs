// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    public delegate void OverlayVisibilityRequest(bool visible);

    public interface IMobileOverlayVisualChild
    {
        void Show();
        void Hide();
        event OverlayVisibilityRequest OverlayVisibilityRequest;
    }

    public class MobileOverlayVisual : MonoBehaviour
    {
        [SerializeField] float _holdTimeToDisplay = 1.0f;
        [SerializeField] List<MonoBehaviour> _children;
        List<IMobileOverlayVisualChild> _overlayChildren;
        float _lastTouchTime = 0;
        bool _touching = false;
        bool _uiToggledForTouch = false;
        bool _showingUI = false;
        bool _uiNeedsUpdate = false;

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

        void Update()
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

        void ToggleUIVisibility()
        {
            SetUIVisibility(!_showingUI);
        }

        void OnOverlayVisibilityRequest(bool visibility)
        {
            SetUIVisibility(visibility);
        }

        void SetUIVisibility(bool visibility)
        {
            if (_showingUI != visibility)
            {
                _showingUI = visibility;
                _uiNeedsUpdate = true;
            }
        }

        void UpdateUI()
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
