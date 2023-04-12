// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [RequireComponent(typeof(PressableButton))]
    [AddComponentMenu("MRTK/UX/See It Say It Label")]
    public class SeeItSayItLabelCreator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The prefab for the see-it say-it label to be generated.")]
        private GameObject seeItSayItPrefab;

        /// <summary>
        /// The prefab for the see-it say-it label to be generated.
        /// </summary>
        public GameObject SeeItSayItPrefab
        {
            get => seeItSayItPrefab;
            set => seeItSayItPrefab = value;
        }

        [SerializeField]
        [Tooltip("The Transform that the label will be positioned off of. If this is a Canvas button, this should be a RectTransform.")]
        private Transform positionControl;

        /// <summary>
        /// The Transform that the label will be positioned off of. If this is a Canvas button, this should be a RectTransform.
        /// </summary>
        public Transform PositionControl
        {
            get => positionControl;
            set => positionControl = value;
        }

        [SerializeField]
        [Tooltip("Is this a Canvas button?")]
        private bool isCanvas;

        /// <summary>
        /// Is this a Canvas button?
        /// </summary>
        public bool IsCanvas
        {
            get => isCanvas;
            set => isCanvas = value;
        }

        [SerializeField]
        [Tooltip("The offset to bring the label forward by.")]
        private float forwardOffset;

        /// <summary>
        /// The offset to bring the button forward by.
        /// </summary>
        public float ForwardOffset
        {
            get => forwardOffset;
            set => forwardOffset = value;
        }

        [SerializeField]
        [Tooltip("The offset at the bottom of the button.")]
        private float bottomOffset;

        /// <summary>
        /// The offset at the bottom of the button.
        /// </summary>
        public float BottomOffset
        {
            get => bottomOffset;
            set => bottomOffset = value;
        }

        private void Start()
        {
            //check if voice commands are enabled for this button
            PressableButton pressablebutton = gameObject.GetComponent<PressableButton>();
            if (pressablebutton != null && pressablebutton.AllowSelectByVoice)
            {
                //check if input and speech packages are present
#if MRTK_INPUT_PRESENT && MRTK_SPEECH_PRESENT
                GameObject label = Instantiate(SeeItSayItPrefab, transform, false);

                if (IsCanvas && label.transform.childCount > 0)
                {
                    //the control RectTransform used to position the label's height
                    RectTransform controlTransform = PositionControl.gameObject.GetComponent<RectTransform>();

                    //the parent RectTransform used to center the label
                    RectTransform canvasTransform = label.GetComponent<RectTransform>();

                    //the child RectTransform, sets the final position of the label 
                    RectTransform labelTransform = label.transform.GetChild(0).gameObject.GetComponent<RectTransform>();

                    if (labelTransform != null && canvasTransform != null && controlTransform != null)
                    {
                        labelTransform.anchoredPosition3D = new Vector3(canvasTransform.rect.width / 2f, canvasTransform.rect.height / 2f + (controlTransform.rect.height /  2f * -1) + bottomOffset, forwardOffset);
                    }
                }
                else
                {
                    label.transform.localPosition = new Vector3(PositionControl.localPosition.x, (PositionControl.lossyScale.y / 2f * -1) + bottomOffset, PositionControl.localPosition.z + forwardOffset);
                }

                //children must be disabled so that they are not initially visible 
                foreach (Transform child in label.transform)
                {
                    child.gameObject.SetActive(false);
                }

                //set the label text to reflect the speech recognition keyword
                string keyword = pressablebutton.SpeechRecognitionKeyword;
                if (keyword != null)
                {
                    TMP_Text labelText = label.GetComponentInChildren<TMP_Text>(true);
                    if (labelText != null)
                    {
                        labelText.text = $"Say '{keyword}'";
                    }
                }
#endif
            }
        }
    }
}
