using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Sharing.Utilities
{
    /// <summary>
    /// Utility for connecting to Sharing Service by Ip Address from inside application at runtime.
    /// </summary>
    public class ManualIpConfiguration : MonoBehaviour
    {
        /// <summary>
        /// The maximum length of characters in a IPv4 address.
        /// <remarks>000.000.000.000</remarks>
        /// </summary>
        private const int MaximumCharacterLength = 15;

        public string IpAddress { get { return ipAddress.text; } }

        /// <summary>
        /// Hides the UI when connection is made.
        /// </summary>
        [Tooltip("Hides the UI when connection is made.")]
        public bool HideWhenConnected;

        /// <summary>
        /// Hides the UI after this many seconds.
        /// </summary>
        [Range(0.1f, 5f)]
        [Tooltip("Hides the UI after this many seconds.")]
        public float HideAfterSeconds = 1f;

        /// <summary>
        /// How many seconds before server connection times out.
        /// </summary>
        [Range(1, 30)]
        [Tooltip("How many seconds before server connection times out.")]
        public int Timeout = 5;

        [SerializeField]
        private Text ipAddress;

        [SerializeField]
        private Image connectionIndicator;

        private bool timerRunning;

        private float timer;

        private bool isTryingToConnect;

        private bool firstRun;

        private void Awake()
        {
            ipAddress.text = PlayerPrefs.GetString("SharingServerIp", "Not Connected");
            firstRun = true;
        }

        private void Start()
        {
            if (SharingStage.Instance != null)
            {
                SharingStage.Instance.SharingManagerConnected += OnConnected;
                SharingStage.Instance.SharingManagerDisconnected += OnDisconnected;
            }
            else
            {
                Debug.LogError("Unable to subscribe to Sharing Stage!");
            }
        }

        private void OnEnable()
        {
            if (firstRun)
            {
                firstRun = false;
                isTryingToConnect = true;
                ConnectToSharingService();
            }
            else
            {
                isTryingToConnect = false;
            }
        }

        private void Update()
        {
            if (timerRunning && timer - Time.time < 0)
            {
                if (isTryingToConnect)
                {
                    isTryingToConnect = false;
                    OnDisconnected();

                    PlayerPrefs.SetString("SharingServerIp", "Not Connected");
                }
            }
        }

        private void OnDestroy()
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
            if (SharingStage.Instance.IsConnected)
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
            connectionIndicator.color = Color.green;
            ipAddress.text = SharingStage.Instance.Connection.GetRemoteAddress().ToString();

            PlayerPrefs.SetString("SharingServerIp", ipAddress.text);

            if (HideWhenConnected && isTryingToConnect)
            {
                StartCoroutine(Hide());
            }

            isTryingToConnect = false;
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

        public void InputString(string input)
        {
            timerRunning = false;
            isTryingToConnect = false;

            if (ipAddress.text.Contains("Connected") || ipAddress.text.Contains("127.0.0.1"))
            {
                ipAddress.text = string.Empty;
            }

            if (ipAddress.text.Length < MaximumCharacterLength)
            {
                ipAddress.text += input;
            }
        }

        public void DeleteLastCharacter()
        {
            timerRunning = false;
            isTryingToConnect = false;

            if (!string.IsNullOrEmpty(ipAddress.text))
            {
                ipAddress.text = ipAddress.text.Substring(0, ipAddress.text.Length - 1);
            }
        }

        public void ClearIpAddressString()
        {
            timerRunning = false;
            isTryingToConnect = false;

            ipAddress.text = "";
        }

        public void ConnectToSharingService()
        {
            timerRunning = false;
            isTryingToConnect = false;

            if (SharingStage.Instance == null || ipAddress.text.Contains("Connected"))
            {
                OnDisconnected();
                return;
            }

            isTryingToConnect = true;
            connectionIndicator.color = Color.yellow;
            SharingStage.Instance.ConnectToServer(ipAddress.text, SharingStage.Instance.ServerPort);
            CheckConnection();
        }

        private IEnumerator Hide()
        {
            yield return new WaitForSeconds(HideAfterSeconds);

            gameObject.SetActive(false);
        }
    }
}
