// ----------------------------------------------------------------------------
// <copyright file="AccountService.cs" company="Exit Games GmbH">
//   Photon Cloud Account Service - Copyright (C) 2012 Exit Games GmbH
// </copyright>
// <summary>
//   Provides methods to register a new user-account for the Photon Cloud and
//   get the resulting appId.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#if UNITY_EDITOR

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using ExitGames.Client.Photon;

using Newtonsoft.Json;

namespace Photon.Pun
{
    public class AccountService
    {
        private const string ServiceUrl = "https://service.exitgames.com/AccountExt/AccountServiceExt.aspx";

        private Action<AccountService> registrationCallback; // optional (when using async reg)

        public string Message { get; private set; } // msg from server (in case of success, this is the appid)

        protected internal Exception Exception { get; set; } // exceptions in account-server communication

        public string AppId { get; private set; }

        public string AppId2 { get; private set; }

        public int ReturnCode { get; private set; } // 0 = OK. anything else is a error with Message

        public enum Origin : byte
        {
            ServerWeb = 1,
            CloudWeb = 2,
            Pun = 3,
            Playmaker = 4
        };

        /// <summary>
        /// Creates a instance of the Account Service to register Photon Cloud accounts.
        /// </summary>
        public AccountService()
        {
            WebRequest.DefaultWebProxy = null;
            ServicePointManager.ServerCertificateValidationCallback = Validator;
        }

        public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true; // any certificate is ok in this case
        }

        /// <summary>
        /// Attempts to create a Photon Cloud Account.
        /// Check ReturnCode, Message and AppId to get the result of this attempt.
        /// </summary>
        /// <param name="email">Email of the account.</param>
        /// <param name="origin">Marks which channel created the new account (if it's new).</param>
        /// <param name="serviceType">Defines which type of Photon-service is being requested.</param>
        public void RegisterByEmail(string email, Origin origin, string serviceType = null)
        {
            this.registrationCallback = null;
            this.AppId = string.Empty;
            this.AppId2 = string.Empty;
            this.Message = string.Empty;
            this.ReturnCode = -1;

            string result;
            try
            {
                WebRequest req = HttpWebRequest.Create(this.RegistrationUri(email, (byte) origin, serviceType));
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

                // now read result
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                this.Message = "Failed to connect to Cloud Account Service. Please register via account website.";
                this.Exception = ex;
                return;
            }

            this.ParseResult(result);
        }

        /// <summary>
        /// Attempts to create a Photon Cloud Account asynchronously.
        /// Once your callback is called, check ReturnCode, Message and AppId to get the result of this attempt.
        /// </summary>
        /// <param name="email">Email of the account.</param>
        /// <param name="origin">Marks which channel created the new account (if it's new).</param>
        /// <param name="serviceType">Defines which type of Photon-service is being requested.</param>
        /// <param name="callback">Called when the result is available.</param>
        public void RegisterByEmailAsync(string email, Origin origin, string serviceType, Action<AccountService> callback = null)
        {
            this.registrationCallback = callback;
            this.AppId = string.Empty;
            this.AppId2 = string.Empty;
            this.Message = string.Empty;
            this.ReturnCode = -1;

            try
            {
                HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(this.RegistrationUri(email, (byte) origin, serviceType));
                req.Timeout = 5000;
                req.BeginGetResponse(this.OnRegisterByEmailCompleted, req);
            }
            catch (Exception ex)
            {
                this.Message = "Failed to connect to Cloud Account Service. Please register via account website.";
                this.Exception = ex;
                if (this.registrationCallback != null)
                {
                    this.registrationCallback(this);
                }
            }
        }

        /// <summary>
        /// Internal callback with result of async HttpWebRequest (in RegisterByEmailAsync).
        /// </summary>
        /// <param name="ar"></param>
        private void OnRegisterByEmailCompleted(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest) ar.AsyncState;
                HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse;

                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    // no error. use the result
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string result = reader.ReadToEnd();

                    this.ParseResult(result);
                }
                else
                {
                    // a response but some error on server. show message
                    this.Message = "Failed to connect to Cloud Account Service. Please register via account website.";
                }
            }
            catch (Exception ex)
            {
                // not even a response. show message
                this.Message = "Failed to connect to Cloud Account Service. Please register via account website.";
                this.Exception = ex;
            }

            if (this.registrationCallback != null)
            {
                this.registrationCallback(this);
            }
        }

        /// <summary>
        /// Creates the service-call Uri, escaping the email for security reasons.
        /// </summary>
        /// <param name="email">Email of the account.</param>
        /// <param name="origin">1 = server-web, 2 = cloud-web, 3 = PUN, 4 = playmaker</param>
        /// <param name="serviceType">Defines which type of Photon-service is being requested. Options: "", "voice", "chat"</param>
        /// <returns>Uri to call.</returns>
        private Uri RegistrationUri(string email, byte origin, string serviceType)
        {
            if (serviceType == null)
            {
                serviceType = string.Empty;
            }

            string emailEncoded = Uri.EscapeDataString(email);
            string uriString = string.Format("{0}?email={1}&origin={2}&serviceType={3}", ServiceUrl, emailEncoded, origin, serviceType);

            return new Uri(uriString);
        }

        /// <summary>
        /// Reads the Json response and applies it to local properties.
        /// </summary>
        /// <param name="result"></param>
        private void ParseResult(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                this.Message = "Server's response was empty. Please register through account website during this service interruption.";
                return;
            }

            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            if (values == null)
            {
                this.Message = "Service temporarily unavailable. Please register through account website.";
                return;
            }

            int returnCodeInt = -1;
            string returnCodeString = string.Empty;
            string message;
            string messageDetailed;

            values.TryGetValue("ReturnCode", out returnCodeString);
            values.TryGetValue("Message", out message);
            values.TryGetValue("MessageDetailed", out messageDetailed);

            int.TryParse(returnCodeString, out returnCodeInt);

            this.ReturnCode = returnCodeInt;
            if (returnCodeInt == 0)
            {
                // returnCode == 0 means: all ok. message is new AppId
                this.AppId = message;
                if (PhotonEditorUtils.HasVoice)
                {
                    this.AppId2 = messageDetailed;
                }
            }
            else
            {
                // any error gives returnCode != 0
                this.AppId = string.Empty;
                if (PhotonEditorUtils.HasVoice)
                {
                    this.AppId2 = string.Empty;
                }
                this.Message = message;
            }
        }
    }
}

#endif