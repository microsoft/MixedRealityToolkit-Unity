// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
#if UNITY_EDITOR
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Editor;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    [MixedRealityServiceProfile(typeof(ISharingService))]
	[CreateAssetMenu(fileName = "SharingServiceProfile", menuName = "MixedRealityToolkit/SharingService Configuration Profile")]
	public class SharingServiceProfile : BaseMixedRealityProfile
	{
		public static string PhotonPackageWarningMessage = "The default implementation of the sharing service uses Photon's PUN 2. Without this package installed, the service will not function. See that package's documentation for details on how to set up and configure your Photon account.";

		public bool FastConnectOnStartup => fastConnectOnStartup;
		public float ConnectAttemptTimeout => connectAttemptTimeout;
		public AppRole DefaultRequestedRole => defaultRequestedRole;
		public DeviceTypeEnum DefaultRequestedDeviceType => defaultRequestedDeviceType;
		public SubscriptionMode DefaultSubscriptionMode => defaultSubscriptionMode;
		public IEnumerable<short> DefaultSubscriptionTypes => defaultSubscriptionTypes;
		public string DefaultRequestedName => defaultRequestedName;
		public string LobbyName => lobbyName;
		public RoomConfig DefaultRoomConfig => defaultRoomConfig;
		public SystemType DeviceTypeFinder => deviceTypeFinder;

#if UNITY_EDITOR
		public DeviceTypeEnum EditorRequestedDeviceType => editorRequestedDeviceType;
#endif

		[SerializeField, Tooltip("If true, the service will connect to the default room and lobby once enabled.")]
		private bool fastConnectOnStartup = false;
		[SerializeField, Tooltip("How long to wait before giving up on a connect request.")]
		private float connectAttemptTimeout = 10f;

		[SerializeField, Tooltip("The app role to be used if none is specified when connecting."), Header("Defaults for Fast Connect")]
		private AppRole defaultRequestedRole = AppRole.None;
		[SerializeField, Tooltip("The device type to use if none is specified when connecting. By default the service will attempt to determine your device type automatically.")]
		private DeviceTypeEnum defaultRequestedDeviceType = DeviceTypeEnum.None;
		[SerializeField, Tooltip("The subscription mode to be used if none is specified when connecting.")]
		private SubscriptionMode defaultSubscriptionMode = SubscriptionMode.Default;
		[SerializeField, Tooltip("Data types to be used if default subscription mode is set to manual.")]
		private short[] defaultSubscriptionTypes = new short[0];
		[SerializeField, Tooltip("Name to be used if none is specified when connecting.")]
		private string defaultRequestedName = "Device";

		[SerializeField, Tooltip("The lobby name to be used when connecting."), Header("Room & Lobby Settings")]
		private string lobbyName = "MRTKLobby";
		[SerializeField, Tooltip("The room properties to be used if none is specified when connecting.")]
		private RoomConfig defaultRoomConfig = RoomConfig.Default;

		[SerializeField, Tooltip("Class used to determine what kind of device you're connecting with."), Implements(typeof(IDeviceTypeFinder), TypeGrouping.ByNamespaceFlat), Header("Classes")]
		private SystemType deviceTypeFinder = new SystemType(typeof(DefaultDeviceTypeFinder));

		[SerializeField, Header("Editor Settings"), Tooltip("The device type to use when testing the app in-editor. This value will override the type returned by the finder unless it's set to None.")]
		private DeviceTypeEnum editorRequestedDeviceType = DeviceTypeEnum.HoloLens;

#if UNITY_EDITOR
		[CustomEditor(typeof(SharingServiceProfile))]
		public class SharingServiceProfileEditor : BaseMixedRealityToolkitConfigurationProfileInspector
		{
			private const string profileTitle = "Sharing Service Profile";
			private const string profileDescription = "This profile helps configure the network sharing service.";

			public override void OnInspectorGUI()
			{
				if (!RenderProfileHeader(profileTitle, profileDescription, target))
				{
					return;
				}

#if !PHOTON_UNITY_NETWORKING
				EditorGUILayout.HelpBox(SharingServiceProfile.PhotonPackageWarningMessage, MessageType.Warning);
				if (GUILayout.Button("Download the required Photon package here"))
				{
					Application.OpenURL("https://assetstore.unity.com/packages/tools/network/pun-2-free-119922");
				}
#endif

				base.OnInspectorGUI();
			}

			protected override bool IsProfileInActiveInstance()
			{
				var profile = target as BaseMixedRealityProfile;
				if (MixedRealityToolkit.IsInitialized &&
					MixedRealityToolkit.Instance.HasActiveProfile &&
					MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile != null)
				{
					foreach (var config in MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile.Configurations)
					{
						if (config.Profile == profile)
							return true;
					}
				}
				return false;
			}
		}
#endif
	}
}