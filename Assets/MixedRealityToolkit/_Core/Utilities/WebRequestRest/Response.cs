// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.WebRequestRest
{
    /// <summary>
    /// Response to a REST Call.
    /// </summary>
    public struct Response
    {
        /// <summary>
        /// Was the REST call successful?
        /// </summary>
        public bool Successful { get; }

        /// <summary>
        /// Response body from the resource.
        /// </summary>
        public string ResponseBody { get; }

        /// <summary>
        /// Response data from the resource.
        /// </summary>
        public byte[] ResponseData { get; }

        /// <summary>
        /// Response code from the resource.
        /// </summary>
        public long ResponseCode { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="successful"></param>
        /// <param name="responseBody"></param>
        /// <param name="responseData"></param>
        /// <param name="responseCode"></param>
        public Response(bool successful, string responseBody, byte[] responseData, long responseCode)
        {
            Successful = successful;
            ResponseBody = responseBody;
            ResponseData = responseData;
            ResponseCode = responseCode;
        }
    }
}