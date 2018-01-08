// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Renders the UI and handles update logic for HoloToolkit/Configure/Apply Mixed Reality Capability Settings.
    /// </summary>
    public class CapabilitySettingsWindow : AutoConfigureWindow<PlayerSettings.WSACapability>
    {
        #region Internal Methods

        private void ApplySetting(PlayerSettings.WSACapability setting)
        {
            switch (setting)
            {
                case PlayerSettings.WSACapability.InternetClient:
                    if (Values[setting])
                    {
                        PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, false);
                    }
                    break;
                case PlayerSettings.WSACapability.InternetClientServer:
                    if (Values[setting])
                    {
                        PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, false);
                    }
                    break;
                case PlayerSettings.WSACapability.PrivateNetworkClientServer:
                    if (Values[setting])
                    {
                        PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, false);
                        PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);
                    }
                    break;
                case PlayerSettings.WSACapability.Chat:
                case PlayerSettings.WSACapability.WebCam:
                case PlayerSettings.WSACapability.AllJoyn:
                case PlayerSettings.WSACapability.Location:
                case PlayerSettings.WSACapability.VoipCall:
                case PlayerSettings.WSACapability.Objects3D:
                case PlayerSettings.WSACapability.PhoneCall:
                case PlayerSettings.WSACapability.Bluetooth:
                case PlayerSettings.WSACapability.Proximity:
                case PlayerSettings.WSACapability.Microphone:
                case PlayerSettings.WSACapability.MusicLibrary:
                case PlayerSettings.WSACapability.VideosLibrary:
                case PlayerSettings.WSACapability.CodeGeneration:
                case PlayerSettings.WSACapability.PicturesLibrary:
                case PlayerSettings.WSACapability.RemovableStorage:
                case PlayerSettings.WSACapability.SpatialPerception:
                case PlayerSettings.WSACapability.BlockedChatMessages:
                case PlayerSettings.WSACapability.HumanInterfaceDevice:
                case PlayerSettings.WSACapability.SharedUserCertificates:
                case PlayerSettings.WSACapability.UserAccountInformation:
                case PlayerSettings.WSACapability.InputInjectionBrokered:
                case PlayerSettings.WSACapability.EnterpriseAuthentication:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("setting", setting, null);
            }

            PlayerSettings.WSA.SetCapability(setting, Values[setting]);
        }

        private void LoadSetting(PlayerSettings.WSACapability setting)
        {
            Values[setting] = PlayerSettings.WSA.GetCapability(setting);
        }

        #endregion // Internal Methods

        #region Overrides / Event Handlers

        protected override void ApplySettings()
        {
            Close();
        }

        protected override void LoadSettings()
        {
            LoadSetting(PlayerSettings.WSACapability.Microphone);
            LoadSetting(PlayerSettings.WSACapability.WebCam);
            LoadSetting(PlayerSettings.WSACapability.SpatialPerception);
            LoadSetting(PlayerSettings.WSACapability.InternetClient);
            LoadSetting(PlayerSettings.WSACapability.InternetClientServer);
            LoadSetting(PlayerSettings.WSACapability.PrivateNetworkClientServer);
        }

        protected override void OnGuiChanged()
        {
            ApplySetting(PlayerSettings.WSACapability.Microphone);
            ApplySetting(PlayerSettings.WSACapability.WebCam);
            ApplySetting(PlayerSettings.WSACapability.SpatialPerception);
            ApplySetting(PlayerSettings.WSACapability.InternetClient);
            ApplySetting(PlayerSettings.WSACapability.InternetClientServer);
            ApplySetting(PlayerSettings.WSACapability.PrivateNetworkClientServer);

            LoadSettings();
        }

        protected override void LoadStrings()
        {
            Names[PlayerSettings.WSACapability.Microphone] = "Microphone";
            Descriptions[PlayerSettings.WSACapability.Microphone] = "Required for access to the Microphone. This includes behaviors like DictationRecognizer, " +
                                                                    "GrammarRecognizer, and KeywordRecognizer. This capability is NOT required for the 'Select' keyword.\n\n" +
                                                                    "Recommendation: Only enable if your application needs access to the microphone beyond the 'Select' keyword. " +
                                                                    "The microphone is considered a privacy sensitive resource.";

            Names[PlayerSettings.WSACapability.WebCam] = "Webcam";
            Descriptions[PlayerSettings.WSACapability.WebCam] = "Required for access to the RGB camera (also known as the locatable camera). This includes " +
                                                                "APIs like PhotoCapture and VideoCapture. This capability is NOT required for mixed reality streaming " +
                                                                "or for capturing photos or videos using the start menu.\n\n" +
                                                                "Recommendation: Only enable if your application needs to programmatically capture photos or videos " +
                                                                "from the RGB camera. The RGB camera is considered a privacy sensitive resource.\n\nNote: The webcam capability " +
                                                                "only grants access to the video stream. In order to grant access to the audio stream as well, the microphone " +
                                                                "capability must be added.";

            Names[PlayerSettings.WSACapability.SpatialPerception] = "Spatial Perception";
            Descriptions[PlayerSettings.WSACapability.SpatialPerception] = "Required for access to the HoloLens world mapping capabilities. These include behaviors like " +
                                                                           "SurfaceObserver, SpatialMappingManager and SpatialAnchor.\n\n" +
                                                                           "Recommendation: Enabled, unless your application doesn't use spatial mapping or spatial " +
                                                                           "collisions in any way.";

            Names[PlayerSettings.WSACapability.InternetClient] = "Internet Client";
            Descriptions[PlayerSettings.WSACapability.InternetClient] = "The Internet Client capability indicates that apps can receive incoming data from the Internet. " +
                                                                        "Cannot act as a server. No local network access.\n\n" +
                                                                        "Recommendation: Leave unchecked unless your application uses online services.";

            Names[PlayerSettings.WSACapability.InternetClientServer] = "Internet Client Server";
            Descriptions[PlayerSettings.WSACapability.InternetClientServer] = "The Internet Client Server capability indicates that apps can receive incoming data from the " +
                                                                              "Internet. Can act as a server. No local network access.\n\nNote: Apps that enable peer-to-peer " +
                                                                              "(P2P) scenarios where the app needs to listen for incoming network connections should use " +
                                                                              "Internet Client Server. The Internet Client Server capability includes the access that the " +
                                                                              "Internet Client capability provides, so you don't need to specify Internet Client when you specify " +
                                                                              "Internet Client Server.";

            Names[PlayerSettings.WSACapability.PrivateNetworkClientServer] = "Private Network Client Server";
            Descriptions[PlayerSettings.WSACapability.PrivateNetworkClientServer] = "The Private Network Client Server capability provides inbound and outbound access to home and " +
                                                                                    "work networks through the firewall. This capability is typically used for games that " +
                                                                                    "communicate across the local area network (LAN), and for apps that share data across a variety " +
                                                                                    "of local devices.\n\nRequired when connecting the Unity Profiler to your app on the HoloLens" +
                                                                                    "\n\nNote: On Windows, this capability does not provide access to the Internet.";
        }

        protected override void OnEnable()
        {
            // Pass to base first
            base.OnEnable();

            // Set size
            minSize = new Vector2(350, 350);
            maxSize = minSize;
        }

        #endregion // Overrides / Event Handlers
    }
}
