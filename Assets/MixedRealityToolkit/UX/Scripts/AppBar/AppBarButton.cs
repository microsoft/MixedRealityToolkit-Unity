// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Buttons;
using MixedRealityToolkit.UX.Buttons.Profiles;
using MixedRealityToolkit.UX.Buttons.Utilities;
using UnityEngine;

namespace MixedRealityToolkit.UX.AppBarControl
{
    public class AppBarButton : MonoBehaviour
    {
        private ButtonIconProfile customIconProfile;
        private ButtonTemplate template;
        private Vector3 targetPosition;
        private Vector3 defaultOffset;
        private Vector3 hiddenOffset;
        private Vector3 manipulationOffset;
        private CompoundButton cButton;
        private Renderer highlightMeshRenderer;
        private CompoundButtonText text;
        private CompoundButtonIcon icon;
        private AppBar parentToolBar;
        private bool initialized = false;

        public const float ButtonWidth = 0.12f;
        public const float ButtonDepth = 0.0001f;

        public void Initialize(AppBar newParentToolBar, ButtonTemplate newTemplate, ButtonIconProfile newCustomProfile)
        {
            template = newTemplate;
            customIconProfile = newCustomProfile;
            parentToolBar = newParentToolBar;

            cButton = GetComponent<CompoundButton>();
            cButton.MainRenderer.enabled = false;
            text = GetComponent<CompoundButtonText>();
            text.Text = template.Text;
            icon = GetComponent<CompoundButtonIcon>();
            highlightMeshRenderer = cButton.GetComponent<CompoundButtonMesh>().Renderer;

            if (customIconProfile != null)
            {
                icon.Profile = customIconProfile;
                icon.IconName = string.Empty;
            }

            icon.IconName = template.Icon;
            initialized = true;
            Hide();

            if (newTemplate.EventTarget != null)
            {
                // Register the button with its target interactable
                newTemplate.EventTarget.Registerinteractable(gameObject);
            }
            else
            {
                // Register the button with the parent app bar
                newParentToolBar.Registerinteractable(gameObject);
            }
        }

        protected void OnEnable()
        {
            Hide();
        }

        protected void Update()
        {
            if (!initialized) { return; }

            RefreshOffsets();

            switch (parentToolBar.State)
            {
                case AppBarStateEnum.Default:
                    // Show hide, adjust, remove buttons
                    // The rest are hidden
                    targetPosition = defaultOffset;
                    switch (template.Type)
                    {
                        case ButtonTypeEnum.Hide:
                        case ButtonTypeEnum.Remove:
                        case ButtonTypeEnum.Adjust:
                        case ButtonTypeEnum.Custom:
                            Show();
                            break;

                        default:
                            Hide();
                            break;
                    }
                    break;

                case AppBarStateEnum.Hidden:
                    // Show show button
                    // The rest are hidden
                    targetPosition = hiddenOffset;
                    switch (template.Type)
                    {
                        case ButtonTypeEnum.Show:
                            Show();
                            break;

                        default:
                            Hide();
                            break;
                    }
                    break;

                case AppBarStateEnum.Manipulation:
                    // Show done / remove buttons
                    // The rest are hidden
                    targetPosition = manipulationOffset;
                    switch (template.Type)
                    {
                        case ButtonTypeEnum.Done:
                        case ButtonTypeEnum.Remove:
                            Show();
                            break;

                        default:
                            Hide();
                            break;
                    }
                    break;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 0.5f);
        }

        private void Hide()
        {
            if (!initialized) { return; }

            icon.Alpha = 0f;
            text.DisableText = true;
            cButton.enabled = false;
            highlightMeshRenderer.enabled = false;
            cButton.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        private void Show()
        {
            if (!initialized) { return; }

            icon.Alpha = 1f;
            text.DisableText = false;
            cButton.enabled = true;
            highlightMeshRenderer.enabled = true;
            cButton.gameObject.layer = LayerMask.NameToLayer("UI");
        }

        private void RefreshOffsets()
        {
            // Apply offset based on total number of buttons
            float xDefaultOffset = (parentToolBar.NumDefaultButtons * 0.5f) * ButtonWidth;
            float xManipulationOffset = (parentToolBar.NumManipulationButtons * 0.5f) * ButtonWidth;

            // For odd numbers of buttons, add an additional 1/2 button offset
            if (parentToolBar.NumDefaultButtons > 1 && parentToolBar.NumDefaultButtons % 2 == 0)
            {
                xDefaultOffset -= (ButtonWidth * 0.5f);
            }
            if (parentToolBar.NumManipulationButtons > 1 && parentToolBar.NumManipulationButtons % 2 == 0)
            {
                xManipulationOffset -= (ButtonWidth * 0.5f);
            }

            defaultOffset = new Vector3(
                template.DefaultPosition * ButtonWidth - xDefaultOffset,
                0f,
                template.DefaultPosition * ButtonDepth);
            manipulationOffset = new Vector3(
                template.ManipulationPosition * ButtonWidth - xManipulationOffset,
                0f,
                template.ManipulationPosition * ButtonDepth);
            hiddenOffset = Vector3.zero;
        }
    }
}
