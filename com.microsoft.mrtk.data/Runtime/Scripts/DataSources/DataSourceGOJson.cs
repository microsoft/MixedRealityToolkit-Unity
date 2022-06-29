// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data source MonoBehaviour that manages a structured data set that was provided
    /// by a JSON text stream.
    ///
    /// The JSON provided to this call will typically be the result of a RESTful
    /// call to a back-end service.  That JSON can be provided directly to this
    /// data source and will trigger changes to any data consumers listening for
    /// data changes to this source.
    ///
    /// TODO: Compare deltas between JSON fetches to reduce the number of
    ///       data consumer notifications to only this keypaths that have actually
    ///       changed since the last data fetch.
    /// </summary>
    [AddComponentMenu("MRTK/Data Binding/Sources/Data Source JSON")]
    public class DataSourceGOJson : DataSourceGOBase
    {
        public delegate void RequestSuccessDelegate(string jsonText, object requestRef);
        public delegate void RequestFailureDelegate(string errorString, object requestRef);

        public DataSourceJson DataSourceJson { get { return DataSource as DataSourceJson; } }

        protected Regex _callbackRegex = new Regex(@"^([a-zA-Z0-9_]+)\(");

        /// <summary>
        /// Set the text that will be parsed and used to build the memory based DOM.
        /// </summary>
        /// <param name="jsonText">THe json string to parse.</param>
        public void SetJson(string jsonText)
        {
            DataSourceJson.UpdateFromJson(jsonText);
        }

        /// <inheritdoc/>
        public override IDataSource AllocateDataSource()
        {
            return new DataSourceJson();
        }

        public IEnumerator StartJsonRequest(string uri, RequestSuccessDelegate successDelegate = null, RequestFailureDelegate failureDelegate = null, object requestRef = null)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
                if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
#else
                if (webRequest.isHttpError || webRequest.isNetworkError)
#endif
                {
                    if (failureDelegate != null)
                    {
                        failureDelegate.Invoke(webRequest.error, requestRef);
                    }
                }
                else
                {
                    string jsonText = RemoveCallbackWrapper(webRequest.downloadHandler.text);

                    DataSourceJson.UpdateFromJson(jsonText);
                    if (successDelegate != null)
                    {
                        successDelegate.Invoke(jsonText, requestRef);
                    }
                }
            }
        }

        private string RemoveCallbackWrapper(string jsonText)
        {
            // Quick check to find a trailing parenthesis to validate that it is
            // wrapped in a callback and then also used for trimming that parens off.
            int lastCharToIncludeIdx;

            for (lastCharToIncludeIdx = jsonText.Length - 1; lastCharToIncludeIdx >= 0; lastCharToIncludeIdx--)
            {
                char lastChar = jsonText[lastCharToIncludeIdx];
                if (!char.IsWhiteSpace(lastChar))
                {
                    if (lastChar == ')')
                    {
                        MatchCollection matches = _callbackRegex.Matches(jsonText);

                        if (matches.Count == 1)
                        {
                            string callbackName = matches[0].Groups[1].Value;
                            int remainingCharCount = lastCharToIncludeIdx - callbackName.Length - 1; // 1 for open parens
                            return jsonText.Substring(callbackName.Length + 1, remainingCharCount); // Also remove parens
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return jsonText;
        }
    }
}
