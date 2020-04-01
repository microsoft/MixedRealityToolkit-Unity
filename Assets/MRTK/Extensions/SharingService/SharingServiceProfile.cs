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
		public bool AutoConnectOnStartup => autoConnectOnStartup;
		public float ConnectAttemptTimeout => connectAttemptTimeout;
		public AppRole DefaultRequestedRole => defaultRequestedRole;
		public string DefaultLobbyName => defaultLobbyName;
		public string DefaultRoomName => defaultRoomName;
		public SubscriptionModeEnum DefaultSubscriptionMode => defaultSubscriptionMode;
		public IEnumerable<short> DefaultSubscriptionTypes => defaultSubscriptionTypes;

		[SerializeField, Tooltip("If true, the service will connect once enabled.")]
		private bool autoConnectOnStartup = false;
		[SerializeField, Tooltip("How long to wait before giving up on a connect request.")]
		private float connectAttemptTimeout = 10f;
		[SerializeField, Tooltip("The app role to be used if none is specified when connecting.")]
		private AppRole defaultRequestedRole = AppRole.None;
		[SerializeField, Tooltip("The lobby name to be used if none is specified when connecting.")]
		private string defaultLobbyName = "MRTKLobby";
		[SerializeField, Tooltip("The room name to be used if none is specified when connecting.")]
		private string defaultRoomName = "MRTKRoom";
		[SerializeField, Tooltip("The subscription mode to be used if none is specified when connecting.")]
		private SubscriptionModeEnum defaultSubscriptionMode = SubscriptionModeEnum.Default;
		[SerializeField, Tooltip("Data types to be used if default subscription mode is set to manual.")]
		private short[] defaultSubscriptionTypes = new short[0];

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