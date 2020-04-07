// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Editor;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Extensions.Sharing.Photon;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing.Editor
{	
	[MixedRealityServiceInspector(typeof(ISharingService))]
	public class SharingServiceInspector : BaseMixedRealityServiceInspector
	{
#if PHOTON_UNITY_NETWORKING
		private struct ButtonPress
		{
			public GUIContent Content;
			public ConnectStatus Condition;
			public Action Action;
		}

		private ButtonPress[] buttons;
		private float pingDisplayTime = 3;
		private Color pingDisplayColor = Color.green;
#endif

		public override void DrawInspectorGUI(object target)
		{
#if !PHOTON_UNITY_NETWORKING
			EditorGUILayout.HelpBox(SharingServiceProfile.PhotonPackageWarningMessage, MessageType.Warning);
			if (GUILayout.Button("Download the required Photon package here"))
			{
				Application.OpenURL("https://assetstore.unity.com/packages/tools/network/pun-2-free-119922");
			}
#else

			PhotonSharingService service = (PhotonSharingService)target;
			SharingServiceProfile profile = service.ConfigurationProfile as SharingServiceProfile;

			if (buttons == null)
			{
				buttons = new ButtonPress[]
				{
					new ButtonPress() { 
						Content = new GUIContent("Fast Connect"),
						Condition = ConnectStatus.NotConnected,
						Action = ()=> { service.FastConnect(); } },
					new ButtonPress() { 
						Content = new GUIContent("Join Lobby"), 
						Condition = ConnectStatus.NotConnected | ConnectStatus.ConnectedToServer,
						Action = ()=> { service.JoinLobby(); } },
					new ButtonPress() {
						Content = new GUIContent("Join Default Room"),
						Condition = ConnectStatus.ConnectedToLobby,
						Action = ()=> { service.JoinRoom(new ConnectConfig () { RoomConfig = profile.DefaultRoomConfig }); } },
					new ButtonPress() { 
						Content = new GUIContent("Leave Current Room"),
						Condition = ConnectStatus.FullyConnected,
						Action = ()=> service.LeaveRoom() },
					new ButtonPress() {
						Content = new GUIContent("Disconnect"),
						Condition = ConnectStatus.ConnectedToLobby | ConnectStatus.ConnectedToServer | ConnectStatus.FullyConnected,
						Action = ()=> service.Disconnect() },
				};
			}

			EditorGUILayout.EnumPopup("Connect Status", service.Status);
			EditorGUILayout.EnumPopup("App Role", service.AppRole);
			EditorGUILayout.EnumPopup("Subscription Mode", service.LocalSubscriptionMode);
			EditorGUILayout.LabelField("Lobby Name: " + (string.IsNullOrEmpty (service.LobbyName) ? "(None)" : service.LobbyName));
			EditorGUILayout.LabelField("Room Name: " + (string.IsNullOrEmpty(service.CurrentRoom.Name) ? "(None)" : service.CurrentRoom.Name));

			if (!Application.isPlaying)
				return;

			foreach (ButtonPress button in buttons)
			{
				using (new EditorGUI.DisabledScope((button.Condition & service.Status) == 0))
				{
					if (GUILayout.Button(button.Content))
					{
						button.Action();
					}
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Available Rooms", EditorStyles.boldLabel);
			using (new EditorGUI.DisabledScope((ConnectStatus.ConnectedToLobby & service.Status) == 0))
			{
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
				{
					if (service.NumAvailableRooms == 0)
					{
						EditorGUILayout.LabelField("(None)");
					}
					else
					{
						foreach (RoomInfo room in service.AvailableRooms)
						{
							if (GUILayout.Button("Join " + room.Name))
							{
								service.JoinRoom(new ConnectConfig(room));
							}
						}
					}
				}
			}

			GUI.color = GUI.backgroundColor;
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Connected Devices", EditorStyles.boldLabel);
			using (new EditorGUI.DisabledScope((ConnectStatus.FullyConnected & service.Status) == 0))
			{
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
				{
					if (service.NumAvailableDevices == 0)
					{
						EditorGUILayout.LabelField("(None)");
					}
					else
					{
						foreach (DeviceInfo device in service.AvailableDevices)
						{
							using (new EditorGUI.DisabledScope(device.ConnectionState == DeviceConnectionState.NotConnected))
							{
								using (new EditorGUILayout.HorizontalScope())
								{
									EditorGUILayout.IntField(device.IsLocalDevice ? (device.Name + " (Local)") : device.Name, device.ID);
									EditorGUILayout.EnumPopup(device.ConnectionState);
									if (GUILayout.Button("Ping"))
									{
										service.PingDevice(device.ID);
									}
								}
							}
						}
					}
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Pings", EditorStyles.boldLabel);
			EditorGUILayout.IntField("Num Times Pinged", service.NumTimesPinged);
			float timeSincePinged = Time.realtimeSinceStartup - service.TimeLastPinged;
			float normalizedPing = Mathf.Clamp01(timeSincePinged / pingDisplayTime);
			GUI.color = Color.Lerp(pingDisplayColor, GUI.backgroundColor, normalizedPing);
			EditorGUILayout.FloatField("Time Last Pinged", service.TimeLastPinged);
			GUI.color = GUI.backgroundColor;
#endif
		}

	}
}

#endif