using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Teleport;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.TeleportSystem
{
    public interface IMixedRealityTeleportHandler : IEventSystemHandler
    {
        void OnTeleportRequest(TeleportEventData eventData);
        void OnTeleportStarted(TeleportEventData eventData);
        void OnTeleportCompleted(TeleportEventData eventData);
        void OnTeleportCanceled(TeleportEventData eventData);
    }
}
