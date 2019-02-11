// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Networking;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
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

#if UNITY_WSA
        TouchScreenKeyboard keyboard = null;
#endif

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

#if UNITY_WSA
        void Update()
        {
            if (_serverPortInputField == null ||
                _clientPortInputField == null)
            {
                Debug.LogError("Error: Input fields not specified for UDPBroadcastNetworkingServiceVisual");
                return;
            }

            if (_serverPortInputField.isFocused)
            {
                if (keyboard == null)
                {
                    Debug.Log("Attempting to show keyboard for server port intput field");
                    keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
                    keyboard.active = true;
                }

                if (keyboard != null)
                {
                    _serverPortInputField.text = keyboard.text;
                }
                else
                {
                    Debug.LogWarning("TouchScreenKeyboard was null when attempting to set server port input field text, which was not expected");
                }
            }
            else if (_clientPortInputField.isFocused)
            {
                if (keyboard == null)
                {
                    Debug.Log("Attempting to show keyboard for client port intput field");
                    keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
                    keyboard.active = true;
                }

                if (keyboard != null)
                {
                    _clientPortInputField.text = keyboard.text;
                }
                else
                {
                    Debug.LogWarning("TouchScreenKeyboard was null when attempting to set client port input field, which was not expected");
                }
            }
            else if (keyboard != null)
            {
                Debug.Log("Clearing cached TouchScreenKeyboard");
                keyboard.active = false;
                keyboard = null;
            }
        }
#endif

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
