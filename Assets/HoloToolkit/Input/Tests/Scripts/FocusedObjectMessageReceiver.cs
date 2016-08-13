using UnityEngine;
using System.Collections;

/// <summary>
/// FocusedObjectMessageReceiver class shows how to handle messages sent by FocusedObjectMessageSender.
/// This particular implementatoin controls object appearance by changing its color when focused.
/// </summary>
public class FocusedObjectMessageReceiver : MonoBehaviour
{
    [Tooltip("Object color changes to this when focused.")]
    public Color FocusedColor = Color.red;

    private Material material;
    private Color originalColor;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        originalColor = material.color;
    }

    public void OnGazeEnter()
    {
        material.color = FocusedColor;
    }

    public void OnGazeLeave()
    {
        material.color = originalColor;
    }
}