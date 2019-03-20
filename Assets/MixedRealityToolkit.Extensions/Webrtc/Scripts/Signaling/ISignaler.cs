using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Webrtc.Signaling
{
    public interface ISignaler
    {
        /// <summary>
        /// Event that occurs when the signaler receives a new message
        /// </summary>
        event Action<SignalerMessage> OnMessage;

        /// <summary>
        /// Event that occurs when the signaler experiences some failure
        /// </summary>
        event Action<Exception> OnFailure;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="message">message to send</param>
        void SendMessageAsync(SignalerMessage message);
    }
}
