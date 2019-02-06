using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Networking
{
    public class UDPBroadcastNetworkingServiceVisual : MonoBehaviour,
        IUDPBroadcastNetworkingServiceVisual
    {
        [SerializeField] InputField _serverPortInputField;
        [SerializeField] InputField _clientPortInputField;
        [SerializeField] Text _errorText;
        [SerializeField] int _defaultServerPort = 49998;
        [SerializeField] int _defaultClientPort = 49999;
        
        public event UDPBroadcastConnectHandler OnConnect;

        public void ShowVisual()
        {
            gameObject.SetActive(true);
        }

        public void HideVisual()
        {
            if (_errorText != null)
            {
                _errorText.gameObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }

        public void OnConnectButtonPress()
        {
            if (_errorText != null)
            {
                _errorText.gameObject.SetActive(false);
            }

            if (_serverPortInputField == null ||
                _clientPortInputField == null)
            {
                Debug.LogError("Error: Input fields weren't specified for UDPBroadcastNetworkingServiceVisual.");
                return;
            }

            try
            {
                var serverPort = Convert.ToInt32(_serverPortInputField.text.Trim());
                var clientPort = Convert.ToInt32(_clientPortInputField.text.Trim());
                if (clientPort != serverPort &&
                    serverPort >= 0 &&
                    clientPort >= 0)
                {
                    OnConnect?.Invoke(serverPort, clientPort);
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Exception thrown obtaining port numbers: " + e.ToString());
            }

            _serverPortInputField.text = _defaultServerPort.ToString();
            _clientPortInputField.text = _defaultClientPort.ToString();

            if (_errorText != null)
            {
                _errorText.gameObject.SetActive(true);
            }
        }
    }
}
