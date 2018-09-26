// ----------------------------------------------------------------------------
// <copyright file="WebRpc.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   This class wraps responses of a Photon WebRPC call, coming from a
//   third party web service.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_1 || UNITY_6_0
#define UNITY
#endif

namespace ExitGames.Client.Photon.LoadBalancing
{
    using ExitGames.Client.Photon;
    using System.Collections.Generic;

    //#if UNITY_EDITOR || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    //#endif


    public class WebRpcResponse
    {
        public string Name { get; private set; }
        /// <summary> 1 is "OK" for WebRPCs. -1 tells you: No ReturnCode by WebRpc service (check OperationResponse.ReturnCode).</summary>
        public int ReturnCode { get; private set; }
        public string DebugMessage { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }

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

        public string ToStringFull()
        {
            return string.Format("{0}={2}: {1} \"{3}\"", Name, SupportClass.DictionaryToString(Parameters), ReturnCode, DebugMessage);
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
            set { WebhookFlags = (byte)(WebhookFlags | HttpForwardConst); }
        }
        public const byte HttpForwardConst = 0x01;
        /// <summary>
        /// Indicates whether to send AuthCookie of actor in the HTTP request to web service or not.
        /// </summary>
        public bool SendAuthCookie
        {
            get { return (WebhookFlags & SendAuthCookieConst) != 0; }
            set { WebhookFlags |= SendAuthCookieConst; }
        }
        public const byte SendAuthCookieConst = 0x02;
        /// <summary>
        /// Indicates whether to send HTTP request synchronously or asynchronously to web service.
        /// </summary>
        public bool SendSync
        {
            get { return (WebhookFlags & SendSyncConst) != 0; }
            set { WebhookFlags |= SendSyncConst; }
        }
        public const byte SendSyncConst = 0x04;
        /// <summary>
        /// Indicates whether to send serialized game state in HTTP request to web service or not.
        /// </summary>
        public bool SendState
        {
            get { return (WebhookFlags & SendStateConst) != 0; }
            set { WebhookFlags |= SendStateConst; }
        }
        public const byte SendStateConst = 0x08;

        public WebFlags(byte webhookFlags)
        {
            WebhookFlags = webhookFlags;
        }
    }

}
