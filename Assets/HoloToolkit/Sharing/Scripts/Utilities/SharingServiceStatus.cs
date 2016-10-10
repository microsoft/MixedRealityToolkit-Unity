using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace HoloToolkit.Sharing
{
    public class SharingServiceStatus : MonoBehaviour
    {
        enum Status
        {
            Disconnected,
            Connecting,
            Connected,
        }


        [Tooltip("Reference to Text UI control where the Status info should be displayed.")]
        public Text Text;

        [Tooltip("Reference to Image UI control where the Status background color should be applied.")]
        public Image Background;

        void Start()
        {
            SetStatus(Status.Disconnected);

            SharingStage.Instance.SharingManagerConnected += SharingManagerConnected;

            if (SharingStage.Instance.AutoDiscoverServer)
                SetStatus(Status.Connecting);
        }

        private void SharingManagerConnected(object sender, System.EventArgs e)
        {
            SetStatus(Status.Connected, SharingStage.Instance.ServerAddress);
        }

        private void SetStatus(Status state, string detail = "")
        {
            Background.color = StatusColor(state);
            Text.text = state.ToString();
            if (detail != "")
                Text.text += ": " + detail;
        }

        private Color StatusColor(Status s)
        {
            switch (s)
            {
                case Status.Disconnected:
                    return Color.red;
                case Status.Connecting:
                    return new Color(255, 175, 0);
                case Status.Connected:
                    return Color.green;
                default:
                    return Color.white;
            }
        }
    }
}