using HoloToolkit.Unity;
using UnityEngine;

public class SelectedObjectMessageSender : MonoBehaviour
{
    /// <summary>
    /// Currently selected object.
    /// </summary>
    private GameObject selectedObject;

    /// <summary>
    /// Sets selection to currently focused object.
    /// </summary>
    public void OnSelectObject()
    {
        selectedObject = GazeManager.Instance.FocusedObject;
    }

    /// <summary>
    /// Clears currently selected object.
    /// </summary>
    public void OnClearSelection()
    {
        selectedObject = null;
    }

    /// <summary>
    /// Sends message to currently selected object.
    /// </summary>
    /// <param name="message">Message to send</param>
    public void SendMessageToSelectedObject(string message)
    {
        if (selectedObject != null)
        {
            selectedObject.SendMessage(message, SendMessageOptions.DontRequireReceiver);
        }
    }
}