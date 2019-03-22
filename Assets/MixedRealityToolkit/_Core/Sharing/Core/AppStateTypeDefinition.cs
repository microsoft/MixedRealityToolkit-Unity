using System;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTK.Core;
using UnityEngine;

namespace MRTK.StateControl
{
    [Serializable]
    public struct AppStateTypeDefinition
    {
        public SystemType ItemStateType { get { return itemStateType; } }
        public SystemType StateArrayType { get { return stateArrayType; } }
        public bool SynchronizeType { get { return synchronizeType; } }
        public bool SubscribeToType { get { return subscribeToType; } }

        [Implements(typeof(IItemState), TypeGrouping.ByNamespaceFlat)]
        [SerializeField]
        private SystemType itemStateType;

        [Implements(typeof(IStateArrayBase), TypeGrouping.ByNamespaceFlat)]
        [SerializeField]
        private SystemType stateArrayType;

        [SerializeField]
        private bool synchronizeType;

        [SerializeField]
        private bool subscribeToType;
    }
}