// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Button class used by AppBar.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/AppBarButton")]
    public class AppBarButton : MonoBehaviour
    {
        private const float changeSpeed = 5f;

        public AppBar.ButtonTypeEnum ButtonType { get { return buttonType; } }

        public int DisplayOrder { get { return displayOrder; } }

        public bool Visible { get { return gameObject.activeSelf; } }

        [SerializeField]
        private PressableButton button = null;
        [SerializeField]
        private Interactable interactable = null;
        [SerializeField]
        private TextMeshPro[] primaryLabels = null;
        [SerializeField]
        private TextMeshPro seeItSayItLabel = null;
        [SerializeField]
        private MeshRenderer icon = null;
        [SerializeField]
        private AppBar.ButtonTypeEnum buttonType = AppBar.ButtonTypeEnum.Custom;
        [SerializeField]
        private int displayOrder;

        private AppBar parentToolBar;
        private Vector3 targetPosition;
        private Texture buttonIcon;
        private string buttonText;

        #region MonoBehaviour Functions

        private void OnEnable()
        {
            targetPosition = Vector3.zero;
        }

        private void OnDisable()
        {
            targetPosition = Vector3.zero;
        }

        private void Update()
        {
            UpdateButton();
        }

        #endregion

        public void InitializeButtonContent(AppBar parentToolBar)
        {
            this.parentToolBar = parentToolBar;

            switch (buttonType)
            {
                case AppBar.ButtonTypeEnum.Custom:
                    // Do nothing - user will set text, icon and events
                    return;

                default:
                    // Set up our icon and text
                    parentToolBar.GetButtonTextAndIconFromType(buttonType, out buttonText, out buttonIcon, out displayOrder);
                    name = buttonType.ToString();

                    for (int i = 0; i < primaryLabels.Length; i++)
                        primaryLabels[i].text = buttonText;

                    seeItSayItLabel.text = "Say \"" + buttonText + "\"";
                    icon.material.mainTexture = buttonIcon;
                    break;
            }
        }

        private void UpdateButton()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Visible ? targetPosition : Vector3.zero, Time.deltaTime * changeSpeed);
        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
            // Use the interactable theme to make button invisible
            button.enabled = visible;
            interactable.IsEnabled = visible;
        }
    }
}
