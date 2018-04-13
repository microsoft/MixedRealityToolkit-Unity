using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal
{
    public interface IEventHandler : IEventSystemHandler
    {
        void OnEventRaised(GenericBaseEventData eventData);
    }
}