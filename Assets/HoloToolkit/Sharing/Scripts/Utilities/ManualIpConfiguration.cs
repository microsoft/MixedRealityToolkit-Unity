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

        /// <summary>
        /// Hides the UI when connection is made.
        /// </summary>
        [Tooltip("Hides the UI when connection is made.")]
        public bool HideWhenConnected;

        /// <summary>
        /// How many seconds before server connection times out.
        /// </summary>
        [Tooltip("How many seconds before server connection times out.")]
        public int Timeout = 5;

        [SerializeField]
        private Text ipAddress;

        [SerializeField]
        private Image connectionIndicator;

        private bool timerRunning;

        private float timer;

        private bool isTryingToConnect;

        private void Awake()
        {
            ipAddress.text = PlayerPrefs.GetString("SharingServerIp", "Not Connected");
        }

        private void OnEnable()
        {
            if (SharingStage.Instance != null)
            {
                SharingStage.Instance.SharingManagerConnected += OnConnected;
                SharingStage.Instance.SharingManagerDisconnected += OnDisconnected;
            }

            ConnectToSharingService();
        }

        private void Update()
        {
            if (timerRunning && timer - Time.time < 0)
            {
                isTryingToConnect = false;
                OnDisconnected();

                PlayerPrefs.SetString("SharingServerIp", "Not Connected");
            }
        }

        private void OnDisable()
        {
            if (SharingStage.Instance != null)
            {
                SharingStage.Instance.SharingManagerConnected -= OnConnected;
                SharingStage.Instance.SharingManagerDisconnected -= OnDisconnected;
            }
        }

        private void CheckConnection()
        {
            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.Connection != null &&
                SharingStage.Instance.Connection.IsConnected())
            {
                OnConnected();
            }
            else if (!timerRunning)
            {
                timer = Time.time + Timeout;
                timerRunning = true;
            }
        }

        private void OnConnected(object sender = null, EventArgs e = null)
        {
            timerRunning = false;
            isTryingToConnect = false;
            connectionIndicator.color = Color.green;
            ipAddress.text = SharingStage.Instance.Connection.GetRemoteAddress().ToString();

            PlayerPrefs.SetString("SharingServerIp", ipAddress.text);

            if (HideWhenConnected)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDisconnected(object sender = null, EventArgs e = null)
        {
            timerRunning = false;

            if (!isTryingToConnect)
            {
                connectionIndicator.color = Color.red;
                ipAddress.text = "Not Connected";
            }
        }

        public void InputString(string intput)
        {
            if (ipAddress.text.Contains("Connected") || ipAddress.text.Contains("127.0.0.1"))
            {
                ipAddress.text = string.Empty;
            }

            if (ipAddress.text.Length < 15)
            {
                ipAddress.text += intput;
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
            if (ipAddress.text.Contains("Connected"))
            {
                OnDisconnected();
                return;
            }

            isTryingToConnect = true;
            connectionIndicator.color = Color.yellow;
            SharingStage.Instance.ConnectToServer(ipAddress.text, SharingStage.Instance.ServerPort);
            CheckConnection();
        }
    }
}
