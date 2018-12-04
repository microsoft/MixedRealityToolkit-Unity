using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pixie.Demos
{
    public class AssignedDeviceControl : MonoBehaviour
    {
        public Image BackgroundImage;
        public RectTransform Connector;

        public Button DeviceButton;
        public Button TypeButton;
        public Button RevokeButton;

        public Text DeviceButtonText;
        public Text TypeButtonText;
        public Text RevokeButtonText;

        public bool ClickedAssignButton;
        public bool ClickedRevokeButton;

        public void ClickRevoke()
        {
            ClickedRevokeButton = true;
        }

        public void ClickAssign()
        {
            ClickedAssignButton = true;
        }

        public void Reset()
        {
            ClickedAssignButton = false;
            ClickedRevokeButton = false;
        }
    }
}
