using Microsoft.MixedReality.Toolkit.Core.EventDatum.Teleport;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.TeleportSystem
{
    /// <summary>
    /// Interface to implement for teleport events.
    /// </summary>
    public interface IMixedRealityTeleportHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when a pointer requests a teleport target, but no teleport has begun.
        /// </summary>
        /// <param name="eventData"></param>
        void OnTeleportRequest(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport has started.
        /// </summary>
        /// <param name="eventData"></param>
        void OnTeleportStarted(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport has successfully completed.
        /// </summary>
        /// <param name="eventData"></param>
        void OnTeleportCompleted(TeleportEventData eventData);

        /// <summary>
        /// Raised when a teleport request has been canceled.
        /// </summary>
        /// <param name="eventData"></param>
        void OnTeleportCanceled(TeleportEventData eventData);
    }
}
