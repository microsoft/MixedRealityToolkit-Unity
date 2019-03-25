using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Extensions.StateControl.Core;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.StateControl
{
    [MixedRealityServiceProfile(typeof(IAppState))]
    [CreateAssetMenu(fileName = "AppStateProfile", menuName = "Mixed Reality Toolkit/AppStateProfile")]
    public class AppStateProfile : BaseMixedRealityProfile
    {
        public AppRoleEnum AppRole { get { return appRole; } }

        /// <summary>
        /// The types that we want all connected users to generate arrays for
        /// </summary>
        public IEnumerable<Type> SynchronizedTypes
        {
            get
            {
                if (stateTypeDefinitions == null)
                    yield break;

                for (int i = 0; i < stateTypeDefinitions.Length; i++)
                {
                    if (stateTypeDefinitions[i].SynchronizeType)
                        yield return stateTypeDefinitions[i].ItemStateType;
                }
            }
        }

        /// <summary>
        /// The types for which we want the local user to listen for changes
        /// </summary>
        public IEnumerable<Type> SubscribedTypes
        {
            get
            {
                if (stateTypeDefinitions == null)
                    yield break;

                for (int i = 0; i < stateTypeDefinitions.Length; i++)
                {
                    if (stateTypeDefinitions[i].SynchronizeType && stateTypeDefinitions[i].SubscribeToType)
                        yield return stateTypeDefinitions[i].ItemStateType;
                }
            }
        }

        public Type DataSourceType { get { return dataSourceType.Type; } }

        public SubscriptionModeEnum SubscriptionMode { get { return subscriptionMode; } }

        public string ExperienceName { get { return experienceName; } }

        [SerializeField]
        private AppRoleEnum appRole = AppRoleEnum.Client;

        [SerializeField]
        private AppStateTypeDefinition[] stateTypeDefinitions;

        [SerializeField]
        [Implements(typeof(IAppStateData), TypeGrouping.ByNamespaceFlat)]
        private SystemType dataSourceType;

        [SerializeField]
        private SubscriptionModeEnum subscriptionMode = SubscriptionModeEnum.All;

        [SerializeField]
        private string experienceName = "SharingExperience";
    }
}