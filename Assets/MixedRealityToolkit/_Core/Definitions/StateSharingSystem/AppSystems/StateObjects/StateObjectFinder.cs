using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    public class StateObjectFinder : MonoBehaviour, IStateObjectFinder
    {
        public IEnumerable<T> FindStateObjectsOfType<S,T>(bool requireActive = true) where S : IItemState<S> where T : Component, IStateObject<S>
        {
            // If we can't find a state view, just use built in Unity method
            if (stateView == null && !SceneScraper.FindInScenes<IStateView>(out stateView, false))
            {
                foreach (T stateObject in GameObject.FindObjectsOfType<T>())
                {
                    if (requireActive && !stateObject.gameObject.activeSelf)
                        continue;

                    yield return stateObject;
                }
            }

            // Otherwise go straight to the source
            foreach (T stateObject in stateView.GetStateObjects<S, T>())
            {
                if (requireActive && !stateObject.gameObject.activeSelf)
                    continue;

                yield return stateObject;
            }

            yield break;
        }

        private IStateView stateView;
    }
}