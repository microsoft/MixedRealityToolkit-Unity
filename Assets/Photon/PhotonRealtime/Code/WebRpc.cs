// ----------------------------------------------------------------------------
// <copyright file="WebRpc.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   This class wraps responses of a Photon WebRPC call, coming from a
//   third party web service.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    #if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    #endif


    /// <summary>Reads an operation response of a WebRpc and provides convenient access to most common values.</summary>
    /// <remarks>
    /// See LoadBalancingClient.OpWebRpc.<br/>
    /// Create a WebRpcResponse to access common result values.<br/>
    /// The operationResponse.OperationCode should be: OperationCode.WebRpc.<br/>
    /// </remarks>
    public class WebRpcResponse
    {
        /// <summary>Name of the WebRpc that was called.</summary>
        public string Name { get; private set; }

        /// <summary>ReturnCode of the WebService that answered the WebRpc.</summary>
        /// <remarks>
        ///  1 is: "OK" for WebRPCs.<br/>
        /// -1 is: No ReturnCode by WebRpc service (check OperationResponse.ReturnCode).<br/>
        /// Other ReturnCodes are defined by the individual WebRpc and service.
        /// </remarks>
        public int ReturnCode { get; private set; }

        /// <summary>Might be empty or null.</summary>
        public string DebugMessage { get; private set; }

        /// <summary>Other key/values returned by the webservice that answered the WebRpc.</summary>
        public Dictionary<string, object> Parameters { get; private set; }

        /// <summary>An OperationResponse for a WebRpc is needed to read it's values.</summary>
        public WebRpcResponse(OperationResponse response)
        {
            object value;
            response.Parameters.TryGetValue(ParameterCode.UriPath, out value);
            this.Name = value as string;

            response.Parameters.TryGetValue(ParameterCode.WebRpcReturnCode, out value);
            this.ReturnCode = (value != null) ? (byte)value : -1;

            response.Parameters.TryGetValue(ParameterCode.WebRpcParameters, out value);
            this.Parameters = value as Dictionary<string, object>;

            response.Parameters.TryGetValue(ParameterCode.WebRpcReturnMessage, out value);
            this.DebugMessage = value as string;
        }

        /// <summary>Turns the response into an easier to read string.</summary>
        /// <returns>String resembling the result.</returns>
        public string ToStringFull()
        {
            return string.Format("{0}={2}: {1} \"{3}\"", this.Name, SupportClass.DictionaryToString(this.Parameters), this.ReturnCode, this.DebugMessage);
        }
    }


    /// <summary>
    /// Optional flags to be used in Photon client SDKs with Op RaiseEvent and Op SetProperties.
    /// Introduced mainly for webhooks 1.2 to control behavior of forwarded HTTP requests.
    /// </summary>
    public class WebFlags
    {

        public readonly static WebFlags Default = new WebFlags(0);
        public byte WebhookFlags;
        /// <summary>
        /// Indicates whether to forward HTTP request to web service or not.
        /// </summary>
        public bool HttpForward
        {
            get { return (WebhookFlags & HttpForwardConst) != 0; }
            set {
                if (value)
                {
                    WebhookFlags |= HttpForwardConst;
                }
                else
                {
                    WebhookFlags = (byte) (WebhookFlags & ~(1 << 0));
                }
            }
        }
        public const byte HttpForwardConst = 0x01;
        /// <summary>
        /// Indicates whether to send AuthCookie of actor in the HTTP request to web service or not.
        /// </summary>
        public bool SendAuthCookie
        {
            get { return (WebhookFlags & SendAuthCookieConst) != 0; }
            set {
                if (value)
                {
                    WebhookFlags |= SendAuthCookieConst;
                }
                else
                {
                    WebhookFlags = (byte)(WebhookFlags & ~(1 << 1));
                }
            }
        }
        public const byte SendAuthCookieConst = 0x02;
        /// <summary>
        /// Indicates whether to send HTTP request synchronously or asynchronously to web service.
        /// </summary>
        public bool SendSync
        {
            get { return (WebhookFlags & SendSyncConst) != 0; }
            set {
                if (value)
                {
                    WebhookFlags |= SendSyncConst;
                }
                else
                {
                    WebhookFlags = (byte)(WebhookFlags & ~(1 << 2));
                }
            }
        }
        public const byte SendSyncConst = 0x04;
        /// <summary>
        /// Indicates whether to send serialized game state in HTTP request to web service or not.
        /// </summary>
        public bool SendState
        {
            get { return (WebhookFlags & SendStateConst) != 0; }
            set {
                if (value)
                {
                    WebhookFlags |= SendStateConst;
                }
                else
                {
                    WebhookFlags = (byte)(WebhookFlags & ~(1 << 3));
                }
            }
        }
        public const byte SendStateConst = 0x08;

        public WebFlags(byte webhookFlags)
        {
            WebhookFlags = webhookFlags;
        }
    }

}
