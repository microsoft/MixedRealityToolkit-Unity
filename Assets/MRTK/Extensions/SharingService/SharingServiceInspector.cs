#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Editor;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Extensions.Sharing.Photon;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing.Editor
{	
	[MixedRealityServiceInspector(typeof(ISharingService))]
	public class SharingServiceInspector : BaseMixedRealityServiceInspector
	{
		public override void DrawInspectorGUI(object target)
		{
			PhotonSharingService service = (PhotonSharingService)target;

			EditorGUILayout.EnumPopup("Connect Status", service.Status);
			EditorGUILayout.EnumPopup("App Role", service.AppRole);
			EditorGUILayout.EnumPopup("Subscription Mode", service.LocalSubscriptionMode);

			if (!Application.isPlaying)
				return;

			switch (service.Status)
			{
				case ConnectStatus.NotConnected:
					{
						if (GUILayout.Button("Connect"))
						{
							service.Connect();
						}
					}
					break;

				default:
					{
						if (GUILayout.Button("Disconnect"))
						{
							service.Disconnect();
						}
						EditorGUILayout.LabelField("Lobby Name: " + service.LobbyName);
						EditorGUILayout.LabelField("Room Name: " + service.RoomName);
					}
					break;
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Connected Devices", EditorStyles.boldLabel);
			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
			{
				foreach (short deviceID in service.ConnectedDevices)
				{
					GUI.color = (deviceID == service.LocalDeviceID) ? Color.green : Color.white;
					EditorGUILayout.IntField("Device", deviceID);
				}
			}

			GUI.color = Color.white;
		}
	}
}

#endif