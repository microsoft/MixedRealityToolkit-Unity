// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
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
		public string LobbyName => lobbyName;
		public RoomConfig DefaultRoomConfig => defaultRoomConfig;
		public SubscriptionMode DefaultSubscriptionMode => defaultSubscriptionMode;
		public IEnumerable<short> DefaultSubscriptionTypes => defaultSubscriptionTypes;

		[SerializeField, Tooltip("If true, the service will connect to the default room and lobby once enabled.")]
		private bool fastConnectOnStartup = false;
		[SerializeField, Tooltip("How long to wait before giving up on a connect request.")]
		private float connectAttemptTimeout = 10f;
		[SerializeField, Tooltip("The app role to be used if none is specified when connecting.")]
		private AppRole defaultRequestedRole = AppRole.None;
		[SerializeField, Tooltip("The subscription mode to be used if none is specified when connecting.")]
		private SubscriptionMode defaultSubscriptionMode = SubscriptionMode.Default;
		[SerializeField, Tooltip("Data types to be used if default subscription mode is set to manual.")]
		private short[] defaultSubscriptionTypes = new short[0];
		[SerializeField, Tooltip("The lobby name to be used when connecting.")]
		private string lobbyName = "MRTKLobby";
		[SerializeField, Tooltip("The room properties to be used if none is specified when connecting.")]
		private RoomConfig defaultRoomConfig = new RoomConfig();

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