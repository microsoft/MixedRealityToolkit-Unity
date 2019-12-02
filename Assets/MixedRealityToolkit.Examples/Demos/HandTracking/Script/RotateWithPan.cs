// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Rotates a game object in response to panning motion from the specified
    /// panzoom component.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/RotateWithPan")]
    public class RotateWithPan : MonoBehaviour
    {
        private Renderer rd;
        [SerializeField]
        [Tooltip("The pan object to listen to events from. If null, will listen on this object or look for first descendant")]
        private HandInteractionPanZoom panInputSource;
        private void OnEnable()
        {
            rd = GetComponent<Renderer>();
            if (panInputSource == null)
            {
                panInputSource = GetComponentInChildren<HandInteractionPanZoom>();
            }
            if (panInputSource == null)
            {
                Debug.LogError("RotateWithPan did not find a HandInteractionPanZoom to listen to, the component will not work", gameObject);
            }
            else
            {
                panInputSource.PanStarted.AddListener(OnPanStarted);
                panInputSource.PanStopped.AddListener(OnPanEnded);
                panInputSource.PanUpdated.AddListener(OnPanning);
            }
        }

        private void OnDisable()
        {
            if (panInputSource != null)
            {
                panInputSource.PanStarted.RemoveListener(OnPanStarted);
                panInputSource.PanStopped.RemoveListener(OnPanEnded);
                panInputSource.PanUpdated.RemoveListener(OnPanning);
            }
        }

        public void OnPanEnded(HandPanEventData eventData)
        {
            if (rd != null)
            {
                rd.material.color = new Color(1.0f, 1.0f, 1.0f);
            }

        }

        public void OnPanning(HandPanEventData eventData)
        {
            Vector3 eulers = new Vector3(eventData.PanDelta.y * (2.0f * Mathf.PI), eventData.PanDelta.x * (2.0f * Mathf.PI), 0.0f);
            eulers *= Mathf.Rad2Deg;
            eulers *= 0.2f;
            transform.localEulerAngles += eulers;
        }

        public void OnPanStarted(HandPanEventData eventData)
        {
            if (rd != null)
            {
                rd.material.color = new Color(0.0f, 1.0f, 0.0f);
            }
        }
    }
}