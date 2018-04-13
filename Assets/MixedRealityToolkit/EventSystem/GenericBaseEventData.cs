using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal
{
    public class GenericBaseEventData : BaseEventData
    {
        public IEventSource EventSource { get; private set; }

        public GenericBaseEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IEventSource eventSource)
        {
            Reset();
            EventSource = eventSource;
        }
    }
}