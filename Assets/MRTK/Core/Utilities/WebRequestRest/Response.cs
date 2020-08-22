// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
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
        public string ResponseBody => responseBody ?? (responseBody = responseBodyAction?.Invoke());
        private string responseBody;
        private System.Func<string> responseBodyAction;

        /// <summary>
        /// Response data from the resource.
        /// </summary>
        public byte[] ResponseData => responseData ?? (responseData = responseDataAction?.Invoke());
        private byte[] responseData;
        private System.Func<byte[]> responseDataAction;

        /// <summary>
        /// Response code from the resource.
        /// </summary>
        public long ResponseCode { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Response(bool successful, string responseBody, byte[] responseData, long responseCode)
        {
            Successful = successful;
            responseBodyAction = null;
            this.responseBody = responseBody;
            responseDataAction = null;
            this.responseData = responseData;
            ResponseCode = responseCode;
        }

        public Response(bool successful, System.Func<string> responseBodyAction, System.Func<byte[]> responseDataAction, long responseCode)
        {
            Successful = successful;
            this.responseBodyAction = responseBodyAction;
            responseBody = null;
            this.responseDataAction = responseDataAction;
            responseData = null;
            ResponseCode = responseCode;
        }
    }
}