using UnityEngine;
using HoloToolkit.Unity;

public class FocusedObjectMessageSender : MonoBehaviour
{
    /// <summary>
    /// Sends message to the object currently focused on.
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