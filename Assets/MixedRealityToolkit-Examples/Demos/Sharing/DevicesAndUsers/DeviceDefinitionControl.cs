using UnityEngine;
using UnityEngine.UI;

namespace Pixie.Demos
{
    public class DeviceDefinitionControl : MonoBehaviour
    {
        public Image BackgroundImage;

        public Text DeviceID;

        public Button DeviceTypeButton;
        public Button DeviceStatusButton;

        public Text DeviceTypeButtonText;
        public Text DeviceStatusButtonText;

        public RectTransform ConnectorTransform;
        public LineRenderer ConnectorLine;

        public bool ClickedAssignButton;

        public void ClickAssign()
        {
            ClickedAssignButton = true;
        }

        public void Reset()
        {
            ClickedAssignButton = false;
        }
    }
}
