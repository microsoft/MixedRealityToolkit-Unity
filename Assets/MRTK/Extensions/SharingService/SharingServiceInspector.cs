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
		private struct ButtonPress
		{
			public GUIContent Content;
			public ConnectStatus Condition;
			public Action Action;
		}

		private ButtonPress[] buttons;

		public override void DrawInspectorGUI(object target)
		{
			PhotonSharingService service = (PhotonSharingService)target;

			if (buttons == null)
			{
				buttons = new ButtonPress[]
				{
					new ButtonPress() { Content = new GUIContent("Fast Connect"), Condition = ConnectStatus.NotConnected, Action = ()=> { service.FastConnect(); } },
					new ButtonPress() { Content = new GUIContent("Join Lobby"), Condition = ConnectStatus.NotConnected | ConnectStatus.ConnectedToServer, Action = ()=> { service.JoinLobby(); } },
					new ButtonPress() { Content = new GUIContent("Join Default Room"), Condition = ConnectStatus.ConnectedToLobby, Action = ()=> service.JoinRoom() },
					new ButtonPress() { Content = new GUIContent("Leave Current Room"), Condition = ConnectStatus.FullyConnected, Action = ()=> service.LeaveRoom() },
					new ButtonPress() { Content = new GUIContent("Disconnect"), Condition = ConnectStatus.ConnectedToLobby | ConnectStatus.ConnectedToServer | ConnectStatus.FullyConnected, Action = ()=> service.Disconnect() },
				};
			}

			EditorGUILayout.EnumPopup("Connect Status", service.Status);
			EditorGUILayout.EnumPopup("App Role", service.AppRole);
			EditorGUILayout.EnumPopup("Subscription Mode", service.LocalSubscriptionMode);
			EditorGUILayout.LabelField("Lobby Name: " + service.LobbyName);
			EditorGUILayout.LabelField("Room Name: " + service.RoomName);

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
								service.JoinRoom(room.Name);
							}
						}
					}
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Connected Devices", EditorStyles.boldLabel);
			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
			{
				if (service.NumConnectedDevices == 0)
				{
					EditorGUILayout.LabelField("(None)");
				}
				else
				{
					foreach (short deviceID in service.ConnectedDevices)
					{
						bool isLocalDevice = (deviceID == service.LocalDeviceID);
						GUI.color = isLocalDevice ? Color.Lerp(Color.green, Color.white, 0.5f) : Color.white;
						EditorGUILayout.IntField(isLocalDevice ? "Device (Local)" : "Device", deviceID);
					}
				}
			}

			GUI.color = Color.white;
		}

	}
}

#endif