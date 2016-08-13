using HoloToolkit.Unity;
using UnityEngine;

/// <summary>
/// FocusedObjectMessageSender class sends Unity message to object currently focused on by GazeManager.
/// Focused object messages can be triggered using voice commands, so keyword responses
/// need to be registered in KeywordManager.
/// </summary>
public class FocusedObjectMessageSender : MonoBehaviour
{
    /// <summary>
    /// Sends message to the object currently focused on by GazeManager.
    /// </summary>
    /// <param name="message">Message to send</param>
    public void SendMessageToFocusedObject(string message)
    {
        if (GazeManager.Instance.FocusedObject != null)
        {
            GazeManager.Instance.FocusedObject.SendMessage(message, SendMessageOptions.DontRequireReceiver);
        }
    }
}