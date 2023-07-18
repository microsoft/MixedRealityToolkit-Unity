// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
using Microsoft.MixedReality.Toolkit.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// When applied to a Unity game object along with a <see cref="PressableButton"/>, this component
    /// will enable a "see-it say-it" label if speech input is enabled within the application.
    /// </summary>
    /// <remarks>
    /// A "see-it say-it" label is used for accessibility and voice input. The label displays the keyword
    /// that can be spoken to active or click the associated <see cref="PressableButton"/>.
    /// 
    /// This class will only enable a "see-it say-it" label if the application has included the MRTK input
    /// package, and has an active <see cref="SpeechInteractor"/> object when this component's 
    /// <see cref="Start"/> method is invoked.
    /// </remarks>
    [RequireComponent(typeof(PressableButton))]
    [AddComponentMenu("MRTK/UX/See It Say It Label")]
    public class SeeItSayItLabelEnabler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The GameObject for the see-it say-it label to be enabled.")]
        private GameObject seeItSayItLabel;

        /// <summary>
        /// The <see cref="GameObject"/> for the see-it say-it label to be enabled.
        /// </summary>
        public GameObject SeeItSayItLabel
        {
            get => seeItSayItLabel;
            set => seeItSayItLabel = value;
        }

        [SerializeField]
        [Tooltip("The Transform that the label will be dynamically positioned off of. Empty by default. If positioning a Canvas label, this must be a RectTransform.")]
        private Transform positionControl;

        /// <summary>
        /// The <see cref="Transform"/> that the label will be dynamically positioned off of. Empty by default. If positioning a Canvas label, this must be a <see cref="UnityEngine.RectTransform"/>.
        /// </summary>
        public Transform PositionControl
        {
            get => positionControl;
            set => positionControl = value;
        }

        private float canvasOffset = -10f;
        private float nonCanvasOffset = -.004f;

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        protected virtual void Start()
        {
            // Check if voice commands are enabled for this button
            PressableButton pressableButton = gameObject.GetComponent<PressableButton>();
            if (pressableButton != null && pressableButton.AllowSelectByVoice)
            {
                // Check if input and speech packages are present
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
                // If we can't find any active speech interactors, then do not enable the labels.
                if (!ComponentCache<SpeechInteractor>.FindFirstActiveInstance())
                {
                    return;
                }

                SeeItSayItLabel.SetActive(true);

                // Children must be disabled so that they are not initially visible 
                foreach (Transform child in SeeItSayItLabel.transform)
                {
                    child.gameObject.SetActive(false);
                }

                // Set the label text to reflect the speech recognition keyword
                string keyword = pressableButton.SpeechRecognitionKeyword;
                if (keyword != null)
                {
                    TMP_Text labelText = SeeItSayItLabel.GetComponentInChildren<TMP_Text>(true);
                    if (labelText != null)
                    {
                        labelText.text = $"Say '{keyword}'";
                    }
                }

                // If a Transform is specified, use it to reposition the object dynamically
                if (positionControl != null)
                {
                    // The control RectTransform used to position the label's height
                    RectTransform controlRectTransform = PositionControl.gameObject.GetComponent<RectTransform>();

                    // If PositionControl is a RectTransform, reposition label relative to Canvas button
                    if (controlRectTransform != null &&  SeeItSayItLabel.transform.childCount > 0)
                    {
                        // The parent RectTransform used to center the label
                        RectTransform canvasTransform = SeeItSayItLabel.GetComponent<RectTransform>();

                        // The child RectTransform used to set the final position of the label 
                        RectTransform labelTransform = SeeItSayItLabel.transform.GetChild(0).gameObject.GetComponent<RectTransform>();

                        if (labelTransform != null && canvasTransform != null)
                        {
                            labelTransform.anchoredPosition3D = new Vector3(canvasTransform.rect.width / 2f, canvasTransform.rect.height / 2f + (controlRectTransform.rect.height /  2f * -1) + canvasOffset, canvasOffset);
                        }
                    }
                    else
                    {
                        SeeItSayItLabel.transform.localPosition = new Vector3(PositionControl.localPosition.x, (PositionControl.lossyScale.y / 2f * -1) + nonCanvasOffset, PositionControl.localPosition.z + nonCanvasOffset);
                    }
                }
#endif
            }
        }
    }
}
