using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MixedRealityToolkit.Common.Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace MixedRealityToolkit.Common.Rest
{
    public static class Rest
    {
        public struct Response
        {
            public readonly bool Successful;
            public readonly string ResponseBody;
            public readonly byte[] ResponseData;
            public readonly long ResponseCode;

            public Response(bool successful, string responseBody, byte[] responseData, long responseCode)
            {
                Successful = successful;
                ResponseBody = responseBody;
                ResponseData = responseData;
                ResponseCode = responseCode;
            }
        }

        public static bool UseSSL { get; set; } = true;

        /// <summary>
        /// Rest GET.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> GetAsync(string query, Dictionary<string, string> headers = null)
        {
            using (var webRequest = UnityWebRequest.Get(query))
            {
                return await ProcessRequestAsync(webRequest, headers);
            }
        }

        /// <summary>
        /// Rest GET for Audio Files.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="audioType">Expected Audio Type.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <returns>The response data.</returns>
        public static async Task<AudioClip> GetAudioClipAsync(string query, AudioType audioType, Dictionary<string, string> headers = null)
        {
            using (var webRequest = UnityWebRequestMultimedia.GetAudioClip(query, audioType))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        webRequest.SetRequestHeader(header.Key, header.Value);
                    }
                }

                await webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    string responseHeaders = webRequest.GetResponseHeaders().Aggregate
                        (string.Empty, (current, header) => $"{current}{header.Key}: {header.Value}\n");
                    Debug.LogErrorFormat("REST Error: {0}\n{1}", webRequest.responseCode, responseHeaders);
                }

                return DownloadHandlerAudioClip.GetContent(webRequest);
            }
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, Dictionary<string, string> headers = null)
        {
            using (var webRequest = UnityWebRequest.Post(query, null as string))
            {
                return await ProcessRequestAsync(webRequest, headers);
            }
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="json">Optional JSON data for the request.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, string json, Dictionary<string, string> headers = null)
        {
            using (var webRequest = UnityWebRequest.Post(query, "POST"))
            {
                var data = new UTF8Encoding().GetBytes(json);
                webRequest.uploadHandler = new UploadHandlerRaw(data);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Accept", "application/json");
                return await ProcessRequestAsync(webRequest, headers);
            }
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="bodyData">The raw data to post.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, byte[] bodyData, Dictionary<string, string> headers = null)
        {
            using (var webRequest = UnityWebRequest.Post(query, "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(bodyData);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
                return await ProcessRequestAsync(webRequest, headers);
            }
        }

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="bodyData">Data to be submitted.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PutAsync(string query, byte[] bodyData, Dictionary<string, string> headers = null)
        {
            using (var webRequest = UnityWebRequest.Put(query, bodyData))
            {
                webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
                return await ProcessRequestAsync(webRequest, headers);
            }
        }

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="jsonData">Json to be submitted.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PutAsync(string query, string jsonData, Dictionary<string, string> headers = null)
        {
            using (var webRequest = UnityWebRequest.Put(query, jsonData))
            {
                webRequest.SetRequestHeader("Content-Type", "application/json");
                return await ProcessRequestAsync(webRequest, headers);
            }
        }

        /// <summary>
        /// Rest DELETE.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> DeleteAsync(string query, Dictionary<string, string> headers = null)
        {
            using (var webRequest = UnityWebRequest.Delete(query))
            {
                return await ProcessRequestAsync(webRequest, headers);
            }
        }

        /// <summary>
        /// Gets the Basic auth header.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetBasicAuthentication(string username, string password)
        {
            return $"Basic {Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(string.Format("{0}:{1}", username, password)))}";
        }

        /// <summary>
        /// Gets the Bearer auth header.
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public static string GetBearerOAuthToken(string authToken)
        {
            return $"Bearer {authToken}";
        }

        private static async Task<Response> ProcessRequestAsync(UnityWebRequest webRequest, Dictionary<string, string> headers = null)
        {
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
                string responseHeaders = webRequest.GetResponseHeaders().Aggregate
                    (string.Empty, (current, header) => $"{current}{header.Key}: {header.Value}\n");
                Debug.LogErrorFormat("REST Error: {0} | {1}\n{2}", webRequest.responseCode, webRequest.downloadHandler?.text, responseHeaders);
                return new Response(false, webRequest.downloadHandler?.text, webRequest.downloadHandler?.data, webRequest.responseCode);
            }

            return new Response(true, webRequest.downloadHandler?.text, webRequest.downloadHandler?.data, webRequest.responseCode);
        }
    }
}
