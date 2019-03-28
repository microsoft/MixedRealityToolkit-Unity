// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Sharing;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.UI
{
    public class UDPBroadcastNetworkingServiceVisual : MonoBehaviour,
        IUDPBroadcastNetworkingServiceVisual
    {
        /// <summary>
        /// Input Field UI for setting server port
        /// </summary>
        [Tooltip("Input Field containing server port for UDPBroadcastNetworkingService")]
        [SerializeField]
        protected InputField _serverPortInputField;

        /// <summary>
        /// Input Field UI for setting client port
        /// </summary>
        [Tooltip("Input Field containing client port for UDPBroadcastNetworkingService")]
        [SerializeField]
        protected InputField _clientPortInputField;

        /// <summary>
        /// Text UI for displaying error messages
        /// </summary>
        [Tooltip("Text ui for displaying error messages")]
        [SerializeField]
        protected Text _errorText;

        /// <summary>
        /// Default server port value
        /// </summary>
        [Tooltip("Default server port value")]
        [SerializeField]
        protected int _defaultServerPort = 48888;

        /// <summary>
        /// Default client port value
        /// </summary>
        [Tooltip("Default client port value")]
        [SerializeField]
        protected int _defaultClientPort = 48889;

        /// <inheritdoc/>
        public event UDPBroadcastConnectHandler OnConnect;

#if UNITY_WSA
        protected TouchScreenKeyboard _keyboard = null;
        protected InputField _previousInputField = null;
#endif

        /// <inheritdoc/>
        public void ShowVisual()
        {
            gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        public void HideVisual()
        {
            if (_errorText != null)
            {
                _errorText.gameObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }

#if UNITY_WSA
        protected void Update()
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
        /// <summary>
        /// Call to assess user provided server and client ports.
        /// If ports are valid, an attempt will be made to setup a connection for the associated <see cref="Sharing.UDPBroadcastNetworkingService"/>
        /// </summary>
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
