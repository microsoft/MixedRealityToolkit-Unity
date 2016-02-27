using UnityEngine;

namespace HoloToolkit.Sharing
{
    public class ConsoleLogWriter : LogWriter
    {
        public override void WriteLogEntry(LogSeverity severity, string message)
        {
            if (severity == LogSeverity.Info)
            {
                Debug.Log(message);
            }
            else if (severity == LogSeverity.Warning)
            {
                Debug.LogWarning(message);
            }
            else
            {
                Debug.LogError(message);
            }
        }
    }
}
