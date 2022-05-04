// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

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

        /// <summary>
        /// Response body from the resource.
        /// </summary>
        public async Task<string> GetResponseBody()
        {
            if (responseBody != null)
            {
                return responseBody;
            }
            return await responseBodyTask;
        }

        private string responseBody;
        private Func<string> responseBodyAction;
        private Task<string> responseBodyTask;

        /// <summary>
        /// Response data from the resource.
        /// </summary>
        public byte[] ResponseData => responseData ?? (responseData = responseDataAction?.Invoke());
        private byte[] responseData;
        private Func<byte[]> responseDataAction;

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
            responseBodyTask = null;
            this.responseBody = responseBody;
            responseDataAction = null;
            this.responseData = responseData;
            ResponseCode = responseCode;
        }

        public Response(bool successful, Func<string> responseBodyAction, Func<byte[]> responseDataAction, long responseCode)
        {
            Successful = successful;
            this.responseBodyAction = responseBodyAction;
            responseBodyTask = ResponseUtils.BytesToString(responseDataAction.Invoke());
            responseBody = null;
            this.responseDataAction = responseDataAction;
            responseData = null;
            ResponseCode = responseCode;
        }

        public Response(bool successful, Task<string> responseBodyTask, Func<byte[]> responseDataAction, long responseCode)
        {
            Successful = successful;
            responseBodyAction = () => System.Text.Encoding.Default.GetString(responseDataAction.Invoke());
            this.responseBodyTask = responseBodyTask;
            responseBody = null;
            this.responseDataAction = responseDataAction;
            responseData = null;
            ResponseCode = responseCode;
        }

        public Response(bool successful, Func<byte[]> responseDataAction, long responseCode)
        {
            Successful = successful;
            responseBodyAction = () => System.Text.Encoding.Default.GetString(responseDataAction.Invoke());
            responseBodyTask = ResponseUtils.BytesToString(responseDataAction.Invoke());
            responseBody = null;
            this.responseDataAction = responseDataAction;
            responseData = null;
            ResponseCode = responseCode;
        }
    }
}