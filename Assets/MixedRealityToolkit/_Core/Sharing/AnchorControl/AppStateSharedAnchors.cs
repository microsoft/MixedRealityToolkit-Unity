using Pixie.Core;
using Pixie.StateControl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.AnchorControl
{
    public class AppStateSharedAnchors : MonoBehaviour, IAppStateSource
    {
        public IEnumerable<Type> StateTypes { get { return new Type[] {
            typeof(AlignmentState),
            typeof(SharedAnchorState),
            typeof(UserAnchorState) }; } }

        [SerializeField]
        private bool synchronized;

        public void GenerateRequiredStates(IAppStateReadWrite appState) { }

        static StateArray<AlignmentState> AlignmentStateArray;
        static StateArray<SharedAnchorState> SharedAnchorStateArray;
        static StateArray<UserAnchorState> UserAnchorStateArray;
    }
}