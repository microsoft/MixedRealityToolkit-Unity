// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Utilities
{

    /// <summary>
    /// REST Class for CRUD Transactions.
    /// </summary>
    public static class Rest
    {
        /// <summary>
        /// Use SSL Connections when making rest calls.
        /// </summary>
        public static bool UseSSL { get; set; } = true;

        #region Authentication

        /// <summary>
        /// Gets the Basic auth header.
        /// </summary>
        /// <param name="username">The Username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The Basic authorization header encoded to base 64.</returns>
        public static string GetBasicAuthentication(string username, string password)
        {
            return $"Basic {Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{username}:{password}"))}";
        }

        /// <summary>
        /// Gets the Bearer auth header.
        /// </summary>
        /// <param name="authToken">OAuth Token to be used.</param>
        /// <returns>The Bearer authorization header.</returns>
        public static string GetBearerOAuthToken(string authToken)
        {
            return $"Bearer {authToken}";
        }

        #endregion Authentication

        #region GET

        /// <summary>
        /// Rest GET.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> GetAsync(string query, Dictionary<string, string> headers = null, int timeout = -1)
        {
            using (var webRequest = UnityWebRequest.Get(query))
            {
                return await ProcessRequestAsync(webRequest, timeout, headers);
            }
        }

        #endregion GET

        #region POST

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, Dictionary<string, string> headers = null, int timeout = -1)
        {
            using (var webRequest = UnityWebRequest.Post(query, null as string))
            {
                return await ProcessRequestAsync(webRequest, timeout, headers);
            }
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="formData">Form Data.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, WWWForm formData, Dictionary<string, string> headers = null, int timeout = -1)
        {
            using (var webRequest = UnityWebRequest.Post(query, formData))
            {
                return await ProcessRequestAsync(webRequest, timeout, headers);
            }
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="jsonData">JSON data for the request.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, string jsonData, Dictionary<string, string> headers = null, int timeout = -1)
        {
            using (var webRequest = UnityWebRequest.Post(query, "POST"))
            {
                var data = new UTF8Encoding().GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(data);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Accept", "application/json");
                return await ProcessRequestAsync(webRequest, timeout, headers);
            }
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="bodyData">The raw data to post.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, byte[] bodyData, Dictionary<string, string> headers = null, int timeout = -1)
        {
            using (var webRequest = UnityWebRequest.Post(query, "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(bodyData);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
                return await ProcessRequestAsync(webRequest, timeout, headers);
            }
        }

        #endregion POST

        #region PUT

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="jsonData">Data to be submitted.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PutAsync(string query, string jsonData, Dictionary<string, string> headers = null, int timeout = -1)
        {
            using (var webRequest = UnityWebRequest.Put(query, jsonData))
            {
                webRequest.SetRequestHeader("Content-Type", "application/json");
                return await ProcessRequestAsync(webRequest, timeout, headers);
            }
        }

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="bodyData">Data to be submitted.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PutAsync(string query, byte[] bodyData, Dictionary<string, string> headers = null, int timeout = -1)
        {
            using (var webRequest = UnityWebRequest.Put(query, bodyData))
            {
                webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
                return await ProcessRequestAsync(webRequest, timeout, headers);
            }
        }

        #endregion PUT

        #region DELETE

        /// <summary>
        /// Rest DELETE.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> DeleteAsync(string query, Dictionary<string, string> headers = null, int timeout = -1)
        {
            using (var webRequest = UnityWebRequest.Delete(query))
            {
                return await ProcessRequestAsync(webRequest, timeout, headers);
            }
        }

        #endregion DELETE

        private static async Task<Response> ProcessRequestAsync(UnityWebRequest webRequest, int timeout, Dictionary<string, string> headers = null)
        {
            if (timeout > 0)
            {
                webRequest.timeout = timeout;
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            // HACK: Workaround for extra quotes around boundary.
            if (webRequest.method == UnityWebRequest.kHttpVerbPOST ||
                webRequest.method == UnityWebRequest.kHttpVerbPUT)
            {
                string contentType = webRequest.GetRequestHeader("Content-Type");
                if (contentType != null)
                {
                    contentType = contentType.Replace("\"", "");
                    webRequest.SetRequestHeader("Content-Type", contentType);
                }
            }

            await webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                if (webRequest.responseCode == 401) { return new Response(false, "Invalid Credentials", null, webRequest.responseCode); }

                if (webRequest.GetResponseHeaders() == null)
                {
                    return new Response(false, "Device Unavailable", null, webRequest.responseCode);
                }

                string responseHeaders = webRequest.GetResponseHeaders().Aggregate(string.Empty, (current, header) => $"\n{header.Key}: {header.Value}");
                Debug.LogError($"REST Error: {webRequest.responseCode}\n{webRequest.downloadHandler?.text}{responseHeaders}");
                return new Response(false, $"{responseHeaders}\n{webRequest.downloadHandler?.text}", webRequest.downloadHandler?.data, webRequest.responseCode);
            }

            return new Response(true, webRequest.downloadHandler?.text, webRequest.downloadHandler?.data, webRequest.responseCode);
        }
    }
}
