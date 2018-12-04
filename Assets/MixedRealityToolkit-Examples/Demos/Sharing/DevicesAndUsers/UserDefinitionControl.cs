using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pixie.Demos
{
    public class UserDefinitionControl : MonoBehaviour
    {
        public Image BackgroundImage;

        public Text UserID;

        public Button RoleButton;
        public Button TeamButton;
        public Button FillStateButton;

        public Text RoleButtonText;
        public Text TeamButtonText;
        public Text FillStateText;

        public AssignedDeviceControl[] AssignedDevices;

        public bool ClickedFillStateButton;

        public void ClickFillState()
        {
            ClickedFillStateButton = true;
        }

        internal void Reset()
        {
            ClickedFillStateButton = false;
        }
    }
}
