using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to control writing the Debug.Log output to a control.
/// </summary>
public class DebugPanel : MonoBehaviour
{
    /// <summary>
    /// The text mesh we will write log messages to
    /// </summary>
    private TextMesh textMesh;

    /// <summary>
    /// Using two queues, one for the current frame and one for the next frame
    /// </summary>
    private Queue<string> logMessages = new Queue<string>();
    private Queue<string> nextLogMessages = new Queue<string>();

    /// <summary>
    /// Track the max # of log messages we will allow in our Queue
    /// </summary>
    private int maxLogMessages = 30;

    /// <summary>
    /// Variables for an FPS counter
    /// </summary>
    private int frameCount;
    private int framesPerSecond;
    private int lastWholeTime = 0;

    void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
    }

    void Update()
    {
        UpdateFPS();

        string logMessageString = CalculateLogMessageString();
        string FPSString = string.Format("FPS: {0}\n", framesPerSecond);

        textMesh.text = string.Format("{0}\n{1}\n", FPSString, logMessageString);
    }

    /// <summary>
    /// Formats the log messages into a string
    /// </summary>
    /// <returns>The formatted string</returns>
    string CalculateLogMessageString()
    {
        string logMessageString = "";

        lock (logMessages)
        {
            while (logMessages.Count > 0)
            {
                string nextMessage = logMessages.Dequeue();
                logMessageString += string.Format("{0}\n", nextMessage);
                // for the next frame...
                nextLogMessages.Enqueue(nextMessage);
            }

            Queue<string> tmp = logMessages;
            logMessages = nextLogMessages;
            nextLogMessages = tmp;
            nextLogMessages.Clear();
        }

        return logMessageString;
    }

    /// <summary>
    /// Keeps track of rough frames per second.
    /// </summary>
    void UpdateFPS()
    {
        frameCount++;
        int currentWholeTime = (int)Time.realtimeSinceStartup;
        if (currentWholeTime != lastWholeTime)
        {
            lastWholeTime = currentWholeTime;
            framesPerSecond = frameCount;
            frameCount = 0;
        }
    }

    /// <summary>
    /// Called when the application calls Debug.Log and friends
    /// </summary>
    /// <param name="condition">The message</param>
    /// <param name="stackTrace">The stack trace</param>
    /// <param name="type">The type of log</param>
    private void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
    {
        LogCallback(condition, stackTrace, type);
    }

    private void LogCallback(string Message, string stack, LogType logType)
    {
        lock (logMessages)
        {
            while (logMessages.Count > maxLogMessages)
            {
                logMessages.Dequeue();
            }

            logMessages.Enqueue(Message);
            if (logType != LogType.Log)
            {
                // Also add the stack. This will push us beyond our max messages, but it is a soft limit, 
                // and the stack information is valuable in error/warning cases.
                logMessages.Enqueue(stack);
            }
        }
    }
}
