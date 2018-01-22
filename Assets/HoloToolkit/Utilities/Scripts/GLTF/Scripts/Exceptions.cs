using System;
using UnityEngine.Networking;

namespace UnityGLTF
{
    [Serializable()]
    public class WebRequestException : Exception
    {
        public WebRequestException() : base() { }
        public WebRequestException(string message) : base(message) { }
        public WebRequestException(string message, Exception inner) : base(message, inner) { }
        public WebRequestException(UnityWebRequest www) : base(string.Format("Error: {0} when requesting: {1}", www.responseCode, www.url)) { }
#if !WINDOWS_UWP
        protected WebRequestException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
#endif
    }
}
