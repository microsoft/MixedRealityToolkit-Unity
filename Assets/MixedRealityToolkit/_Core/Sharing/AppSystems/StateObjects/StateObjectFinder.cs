using Pixie.Core;
using Pixie.Initialization;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.AppSystems.StateObjects
{
    public class StateObjectFinder : MonoBehaviour, IStateObjectFinder
    {
        public IEnumerable<T> FindStateObjectsOfType<S,T>(bool requireActive = true) where S : IItemState, IItemStateComparer<S> where T : Component, IStateObject<S>
        {
            // If we can't find a state view, just use built in Unity method
            if (stateView == null && !ComponentFinder.FindInScenes<IStateView>(out stateView, ComponentFinder.SearchTypeEnum.RootGameObjects))
            {
                foreach (T stateObject in GameObject.FindObjectsOfType<T>())
                {
                    if (requireActive && !stateObject.gameObject.activeSelf)
                        continue;

                    yield return stateObject;
                }
                yield break;
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