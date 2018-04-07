// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Common.RestUtility
{
    public struct Response
    {
        public bool Successful { get; }
        public string ResponseBody { get; }
        public byte[] ResponseData { get; }
        public long ResponseCode { get; }

        public Response(bool successful, string responseBody, byte[] responseData, long responseCode)
        {
            Successful = successful;
            ResponseBody = responseBody;
            ResponseData = responseData;
            ResponseCode = responseCode;
        }
    }
}