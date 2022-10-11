// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Component subclassed from TMP_InputField to account for XR interactions.
    /// </summary>
    [AddComponentMenu("MRTK/UX/MRTK TMP Input Field")]
    public class MRTKTMPInputField : TMP_InputField
    {
        /// <summary>
        /// Activate the input field.
        /// </summary>
        public void ActivateMRTKTMPInputField()
        {
            MRTKInputFieldManager.SetCurrentInputField(this);
            ActivateInputField();
        }

        /// <inheritdoc />
        /// <remarks>
        /// <para>Override OnDeselect such that the base is only called when the call comes from the TMP_InputField/MRTKInputFieldManager scripts or we are not using an HMD.
        /// When using HMD we don't want the input field to be deselected just because someone did a pinch or another gesture that triggers this function.
        /// We also call the base when we are selecting another input field so that we don't have multiple ones being selected at once.</para>
        /// </remarks>
        public override void OnDeselect(BaseEventData eventData)
        {
            if (eventData == null || XRSubsystemHelpers.DisplaySubsystem == null)
            {
                base.OnDeselect(eventData);
                MRTKInputFieldManager.RemoveCurrentInputField(this);
            }
        }
    }
}
