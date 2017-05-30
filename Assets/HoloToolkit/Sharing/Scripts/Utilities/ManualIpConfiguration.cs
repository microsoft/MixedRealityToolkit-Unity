using System;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Sharing.Utilities
{
    /// <summary>
    /// Utility for connecting to Sharing Service by Ip Address from inside application at runtime.
    /// </summary>
    public class ManualIpConfiguration : MonoBehaviour
    {
        public string IpAddress { get { return ipAddress.text; } }

        [SerializeField]
        private Text ipAddress;

        public Image ConnectionIndicator;

        private void OnEnable()
        {
            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance != null && SharingStage.Instance.Connection != null &&
                SharingStage.Instance.Connection.IsConnected())
            {
                OnConnected();
            }
            else
            {
                ipAddress.text = "Not Connected";
            }
        }

        private void Start()
        {
            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.Connection != null && SharingStage.Instance.Connection.IsConnected())
            {
                OnConnected();
            }
            else
            {
                SharingStage.Instance.SharingManagerConnected += OnConnected;
            }
        }

        private void OnConnected(object sender = null, EventArgs e = null)
        {
            ConnectionIndicator.color = Color.green;
            ipAddress.text = SharingStage.Instance.Connection.GetRemoteAddress().ToString();
        }

        public void InputString(string s)
        {
            if (ipAddress.text.Contains("Connected") || ipAddress.text.Contains("127.0.0.1"))
            {
                ipAddress.text = string.Empty;
            }

            if (ipAddress.text.Length < 15)
            {
                ipAddress.text += s;
            }
        }

        public void DeleteLastCharacter()
        {
            if (!string.IsNullOrEmpty(ipAddress.text))
            {
                ipAddress.text = ipAddress.text.Substring(0, ipAddress.text.Length - 1);
            }
        }

        public void ClearIpAddressString()
        {
            ipAddress.text = "";
        }

        public void ConnectToSharingService()
        {
            ConnectionIndicator.color = Color.yellow;
            SharingStage.Instance.ConnectToServer(ipAddress.text, SharingStage.Instance.ServerPort);
        }
    }
}
