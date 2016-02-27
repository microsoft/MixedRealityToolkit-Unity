#if UNITY_METRO && !UNITY_EDITOR
namespace System
{
    public class SystemException : Exception
    {
        public SystemException() {}
        public SystemException(string message) : base(message) {}
        public SystemException(string message, Exception innerException) : base(message, innerException) {}
    }
}
#endif