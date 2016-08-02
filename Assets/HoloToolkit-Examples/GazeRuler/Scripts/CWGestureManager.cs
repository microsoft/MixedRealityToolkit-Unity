// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

/// <summary>
/// Base on GestureManager, but add some custom action
/// 
/// </summary>
[RequireComponent(typeof(GazeManager))]
public class CWGestureManager : Singleton<CWGestureManager>
{
    /// <summary>
    /// Key to press in the editor to select the currently gazed hologram
    /// </summary>
    public KeyCode EditorSelectKey = KeyCode.Space;

    /// <summary>
    /// To select even when a hologram is not being gazed at,
    /// set the override focused object.
    /// If its null, then the gazed at object will be selected.
    /// </summary>
    public GameObject OverrideFocusedObject
    {
        get; set;
    }

    /// <summary>
    /// Gets the currently focused object, or null if none.
    /// </summary>
    public GameObject FocusedObject
    {
        get { return focusedObject; }
    }

    private GestureRecognizer gestureRecognizer;
    private GameObject focusedObject;

    void Start()
    {
        // Create a new GestureRecognizer. Sign up for tapped events.
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold);

        gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

        gestureRecognizer.HoldStartedEvent += GestureRecognizer_HoldStartedEvent;
        // Start looking for gestures.
        gestureRecognizer.StartCapturingGestures();
    }

    private void GestureRecognizer_HoldStartedEvent(InteractionSourceKind source, Ray headRay)
    {
        OnHold();
    }

   /// <summary>
   /// if hold gesture happen, try to finish geometry
   /// </summary>
    private void OnHold()
    {
        MeasureManager.Instance.OnPloygonClose();
    }

    private void OnTap()
    {
        //if user tap tip text, relative line and geometry will hide
        if (focusedObject != null && focusedObject.tag.Equals("Tip"))
        {
            focusedObject.SendMessage("OnSelect");
        }
        else
        {
            MeasureManager.Instance.OnSelect();
        }
    }

    private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        OnTap();
    }

    void LateUpdate()
    {
        GameObject oldFocusedObject = focusedObject;

        if (GazeManager.Instance.Hit &&
            OverrideFocusedObject == null &&
            GazeManager.Instance.HitInfo.collider != null)
        {
            // If gaze hits a hologram, set the focused object to that game object.
            // Also if the caller has not decided to override the focused object.
            focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
        }
        else
        {
            // If our gaze doesn't hit a hologram, set the focused object to null or override focused object.
            focusedObject = OverrideFocusedObject;
        }

        if (focusedObject != oldFocusedObject)
        {
            // If the currently focused object doesn't match the old focused object, cancel the current gesture.
            // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
            gestureRecognizer.CancelGestures();
            gestureRecognizer.StartCapturingGestures();
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(EditorSelectKey))
        {
            OnTap();
        }
#endif
    }

    void OnDestroy()
    {
        gestureRecognizer.StopCapturingGestures();
        gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
        gestureRecognizer.HoldStartedEvent -= GestureRecognizer_HoldStartedEvent;
    }
}
