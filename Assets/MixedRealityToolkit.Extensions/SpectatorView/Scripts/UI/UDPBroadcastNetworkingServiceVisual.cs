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
        [SerializeField] int _defaultServerPort = 48888;
        [SerializeField] int _defaultClientPort = 48889;
        
        public event UDPBroadcastConnectHandler OnConnect;

#if UNITY_WSA
        TouchScreenKeyboard _keyboard = null;
        InputField _previousInputField = null;
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
                if (_keyboard == null ||
                    _previousInputField != _serverPortInputField)
                {
                    Debug.Log("Attempting to show _keyboard for server port intput field");
                    _keyboard = TouchScreenKeyboard.Open(_serverPortInputField.text, TouchScreenKeyboardType.NumberPad);
                    _keyboard.active = true;
                }

                _previousInputField = _serverPortInputField;

                if (_keyboard != null)
                {
                    _serverPortInputField.text = _keyboard.text;
                }
                else
                {
                    Debug.LogWarning("TouchScreen_keyboard was null when attempting to set server port input field text, which was not expected");
                }
            }
            else if (_clientPortInputField.isFocused)
            {
                if (_keyboard == null ||
                    _previousInputField != _clientPortInputField)
                {
                    Debug.Log("Attempting to show _keyboard for client port intput field");
                    _keyboard = TouchScreenKeyboard.Open(_clientPortInputField.text, TouchScreenKeyboardType.NumberPad);
                    _keyboard.active = true;
                }

                _previousInputField = _clientPortInputField;

                if (_keyboard != null)
                {
                    _clientPortInputField.text = _keyboard.text;
                }
                else
                {
                    Debug.LogWarning("TouchScreen_keyboard was null when attempting to set client port input field, which was not expected");
                }
            }
            else if (_keyboard != null)
            {
                Debug.Log("Clearing cached TouchScreen_keyboard");
                _keyboard.active = false;
                _keyboard = null;
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
