using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

namespace Microsoft.MixedReality.Toolkit.UX
{
    public sealed class ScrollableSelectionFilter : MonoBehaviour, IXRSelectFilter
    {
        public bool canProcess => true;

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            return !(interactable is Scrollable);
        }
    }
}
