using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Extensions.Webrtc.Signaling
{
    /// <summary>
    /// A signaler implementation for reference
    /// </summary>
    /// <remarks>
    /// This is based on https://github.com/bengreenier/node-dss and
    /// is not a production ready signaling solution. It is included
    /// here to demonstrate how one might implement such a solution.
    /// </remarks>
    public class NodeDssSignaler : MonoBehaviour, ISignaler
    {
        /// <summary>
        /// The https://github.com/bengreenier/node-dss HTTP service address to connect to
        /// </summary>
        /// <remarks>
        /// This value should end with a forward-slash (/)
        /// </remarks>
        [Tooltip("The node-dss server to connect to (should end with a forward-slash)")]
        public string HttpServerAddress;

        /// <summary>
        /// The interval (in ms) that the server is polled at
        /// </summary>
        [Tooltip("The interval (in ms) that the server is polled at")]
        public float PollTimeMs = 5f;

        /// <summary>
        /// Internal timing helper
        /// </summary>
        private float timeSincePoll = 0f;

        /// <summary>
        /// Internal last poll response status flag
        /// </summary>
        private bool lastGetComplete = true;

        /// <summary>
        /// Event that occurs when the signaler receives a new message
        /// </summary>
        public event Action<SignalerMessage> OnMessage;
        
        /// <summary>
        /// Event that occurs when the signaler experiences some failure
        /// </summary>
        public event Action<Exception> OnFailure;

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="message">message to send</param>
        public void SendMessageAsync(SignalerMessage message)
        {
            StartCoroutine(PostToServer(message));
        }

        /// <summary>
        /// Unity Engine Update() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
        /// </remarks>
        private void Update()
        {
            // if we have not reached our PollTimeMs value...
            if (timeSincePoll <= PollTimeMs)
            {
                // we keep incrementing our local counter until we do.
                timeSincePoll += Time.deltaTime;
                return;
            }

            // if we have a pending request still going, don't queue another yet.
            if (!lastGetComplete)
            {
                return;
            }

            // when we have reached our PollTimeMs value...
            timeSincePoll = 0f;

            // begin the poll and process.
            lastGetComplete = false;
            StartCoroutine(CO_GetAndProcessFromServer());
        }

        /// <summary>
        /// Internal helper for sending http data to the dss server using POST
        /// </summary>
        /// <param name="msg">the message to send</param>
        private IEnumerator PostToServer(SignalerMessage msg)
        {
            var data = Encoding.UTF8.GetBytes(JsonUtility.ToJson(msg));
            var www = new UnityWebRequest(HttpServerAddress + "data/" + msg.TargetId, UnityWebRequest.kHttpVerbPOST);
            www.uploadHandler = new UploadHandlerRaw(data);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                OnFailure?.Invoke(new Exception("Failure sending message: " + www.error));
            }
        }

        /// <summary>
        /// Internal coroutine helper for receiving http data from the dss server using GET
        /// and processing it as needed
        /// </summary>
        /// <returns>the message</returns>
        private IEnumerator CO_GetAndProcessFromServer()
        {
            var www = UnityWebRequest.Get(HttpServerAddress + "data/" + SystemInfo.deviceUniqueIdentifier);
            yield return www.SendWebRequest();

            lastGetComplete = true;

            if (www.isNetworkError || www.isHttpError)
            {
                if (www.responseCode != 404)
                {
                    OnFailure?.Invoke(new Exception("Failure receiving message: " + www.error));
                }
            }
            else
            {
                var json = www.downloadHandler.text;

                var msg = JsonUtility.FromJson<SignalerMessage>(json);

                // if the message is good
                if (msg != null)
                {
                    // send it off
                    OnMessage?.Invoke(msg);
                }
            }
        }
    }
}
