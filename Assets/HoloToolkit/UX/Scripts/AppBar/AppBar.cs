// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Unity.UX
{
    /// <summary>
    /// Logic for the App Bar. Generates buttons, manages states.
    /// </summary>
    public class AppBar : InteractionReceiver
    {
        private float buttonWidth = 1.50f;

        /// <summary>
        /// How many custom buttons can be added to the toolbar
        /// </summary>
        public const int MaxCustomButtons = 5;

        /// <summary>
        /// Where to display the app bar on the y axis
        /// This can be set to negative values
        /// to force the app bar to appear below the object
        /// </summary>
        public float HoverOffsetYScale = 0.25f;

        /// <summary>
        /// Pushes the app bar away from the object
        /// </summary>
        public float HoverOffsetZ = 0f;

        [SerializeField]
        [Tooltip("Uses an alternate follow style that works better for very oblong objects.")]
        private bool useTightFollow = false;

        /// <summary>
        /// Uses an alternate follow style that works better for very oblong objects
        /// </summary>
        public bool UseTightFollow
        {
            get { return useTightFollow; }
            set { useTightFollow = value; }
        }

        /// <summary>
        /// Class used for building toolbar buttons
        /// (not yet in use)
        /// </summary>
        [Serializable]
        public struct ButtonTemplate
        {
            public ButtonTemplate(ButtonTypeEnum type, string name, string icon, string text, int defaultPosition, int manipulationPosition)
            {
                Type = type;
                Name = name;
                Icon = icon;
                Text = text;
                DefaultPosition = defaultPosition;
                ManipulationPosition = manipulationPosition;
                EventTarget = null;
                OnTappedEvent = null;
            }

            public bool IsEmpty
            {
                get { return string.IsNullOrEmpty(Name); }
            }

            public int DefaultPosition;
            public int ManipulationPosition;
            public ButtonTypeEnum Type;
            public string Name;
            public string Icon;
            public string Text;
            public InteractionReceiver EventTarget;
            public UnityEvent OnTappedEvent;
        }

        [Flags]
        public enum ButtonTypeEnum
        {
            Custom = 0,
            Remove = 1,
            Adjust = 2,
            Hide = 4,
            Show = 8,
            Done = 16
        }

        public enum AppBarDisplayTypeEnum
        {
            Manipulation,
            Standalone
        }

        public enum AppBarStateEnum
        {
            Default,
            Manipulation,
            Hidden
        }

        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        private BoundingBoxRig boundingRig;

        /// <summary>
        /// a reference to the boundingBoxRig that the appbar turns on and off
        /// </summary>
        public BoundingBoxRig BoundingRig
        {
            get { return boundingRig; }
            set { boundingRig = value; }
        }

        public GameObject SquareButtonPrefab;

        public int NumDefaultButtons { get; private set; }

        public int NumManipulationButtons { get; private set; }

        public bool UseRemove = true;
        public bool UseAdjust = true;
        public bool UseHide = true;

        public ButtonTemplate[] Buttons
        {
            get { return buttons; }
            set { buttons = value; }
        }

        public ButtonTemplate[] DefaultButtons { get; private set; }

        public AppBarDisplayTypeEnum DisplayType = AppBarDisplayTypeEnum.Manipulation;

        public AppBarStateEnum State = AppBarStateEnum.Default;

        /// <summary>
        /// Custom icon profile
        /// If null, the profile in the SquareButtonPrefab object will be used
        /// </summary>
        public ButtonIconProfile CustomButtonIconProfile;

        [SerializeField]
        private ButtonTemplate[] buttons = new ButtonTemplate[MaxCustomButtons];

        [SerializeField]
        private Transform buttonParent = null;

        [SerializeField]
        private GameObject baseRenderer = null;

        [SerializeField]
        private GameObject backgroundBar = null;

        [SerializeField]
        private BoundingBox boundingBox;

        private Vector3 targetBarSize = Vector3.one;
        private float lastTimeTapped = 0f;
        private float coolDownTime = 0.5f;
        private int numHiddenButtons;
        private BoundingBoxHelper helper;

        public void Reset()
        {
            State = AppBarStateEnum.Default;
            FollowBoundingBox(false);
            lastTimeTapped = Time.time + coolDownTime;
        }

        public void Start()
        {
            State = AppBarStateEnum.Default;

            if (interactables.Count == 0)
            {
                RefreshTemplates();
                for (int i = 0; i < DefaultButtons.Length; i++)
                {
                    CreateButton(DefaultButtons[i], null);
                }

                for (int i = 0; i < buttons.Length; i++)
                {
                    CreateButton(buttons[i], CustomButtonIconProfile);
                }
            }

            helper = new BoundingBoxHelper();
        }

        protected override void InputClicked(GameObject obj, InputClickedEventData eventData)
        {
            if (Time.time < lastTimeTapped + coolDownTime)
            {
                return;
            }

            lastTimeTapped = Time.time;

            base.InputClicked(obj, eventData);

            switch (obj.name)
            {
                case "Remove":
                    // Destroy the target object, Bounding Box, Bounding Box Rig and App Bar
                    boundingBox.Target.GetComponent<BoundingBoxRig>().Deactivate();
                    Destroy(boundingBox.Target.GetComponent<BoundingBoxRig>());
                    Destroy(boundingBox.Target);
                    Destroy(gameObject);
                    break;

                case "Adjust":
                    // Make the bounding box active so users can manipulate it
                    State = AppBarStateEnum.Manipulation;
                    // Activate BoundingBoxRig
                    boundingBox.Target.GetComponent<BoundingBoxRig>().Activate();
                    break;

                case "Hide":
                    // Make the bounding box inactive and invisible
                    State = AppBarStateEnum.Hidden;
                    break;

                case "Show":
                    State = AppBarStateEnum.Default;
                    // Deactivate BoundingBoxRig
                    boundingBox.Target.GetComponent<BoundingBoxRig>().Deactivate();
                    break;

                case "Done":
                    State = AppBarStateEnum.Default;
                    // Deactivate BoundingBoxRig
                    boundingBox.Target.GetComponent<BoundingBoxRig>().Deactivate();
                    break;

                default:
                    break;
            }
        }

        private void CreateButton(ButtonTemplate template, ButtonIconProfile customIconProfile)
        {
            if (template.IsEmpty)
            {
                return;
            }

            switch (template.Type)
            {
                case ButtonTypeEnum.Custom:
                    NumDefaultButtons++;
                    break;

                case ButtonTypeEnum.Adjust:
                    NumDefaultButtons++;
                    break;

                case ButtonTypeEnum.Done:
                    NumManipulationButtons++;
                    break;

                case ButtonTypeEnum.Remove:
                    NumManipulationButtons++;
                    NumDefaultButtons++;
                    break;

                case ButtonTypeEnum.Hide:
                    NumDefaultButtons++;
                    break;

                case ButtonTypeEnum.Show:
                    numHiddenButtons++;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameObject newButton = Instantiate(SquareButtonPrefab, buttonParent);
            newButton.name = template.Name;
            newButton.transform.localPosition = Vector3.zero;
            newButton.transform.localRotation = Quaternion.identity;
            AppBarButton mtb = newButton.AddComponent<AppBarButton>();
            mtb.Initialize(this, template, customIconProfile);
        }

        private void FollowBoundingBox(bool smooth)
        {
            if (boundingBox == null)
            {
                if (DisplayType == AppBarDisplayTypeEnum.Manipulation)
                {
                    // Hide our buttons
                    baseRenderer.SetActive(false);
                }
                else
                {
                    baseRenderer.SetActive(true);
                }
                return;
            }

            // Show our buttons
            baseRenderer.SetActive(true);

            //calculate best follow position for AppBar
            Vector3 finalPosition = Vector3.zero;
            Vector3 headPosition = Camera.main.transform.position;
            LayerMask ignoreLayers = new LayerMask();
            List<Vector3> boundsPoints = new List<Vector3>();
            if (boundingBox != null)
            {
                helper.UpdateNonAABoundingBoxCornerPositions(boundingBox.Target, boundsPoints, ignoreLayers);
                int followingFaceIndex = helper.GetIndexOfForwardFace(headPosition);
                Vector3 faceNormal = helper.GetFaceNormal(followingFaceIndex);

                //finally we have new position
                finalPosition = helper.GetFaceBottomCentroid(followingFaceIndex) + (faceNormal * HoverOffsetZ);
            }

            // Follow our bounding box
            transform.position = smooth ? Vector3.Lerp(transform.position, finalPosition, 0.5f) : finalPosition;

            // Rotate on the y axis
            Vector3 eulerAngles = Quaternion.LookRotation((boundingBox.transform.position - finalPosition).normalized, Vector3.up).eulerAngles;
            eulerAngles.x = 0f;
            eulerAngles.z = 0f;
            transform.eulerAngles = eulerAngles;
        }

        private void Update()
        {
            FollowBoundingBox(true);

            switch (State)
            {
                case AppBarStateEnum.Default:
                    targetBarSize = new Vector3(NumDefaultButtons * buttonWidth, buttonWidth, 1f);
                    break;

                case AppBarStateEnum.Hidden:
                    targetBarSize = new Vector3(numHiddenButtons * buttonWidth, buttonWidth, 1f);
                    break;

                case AppBarStateEnum.Manipulation:
                    targetBarSize = new Vector3(NumManipulationButtons * buttonWidth, buttonWidth, 1f);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            backgroundBar.transform.localScale = Vector3.Lerp(backgroundBar.transform.localScale, targetBarSize, 0.5f);
        }

        private void RefreshTemplates()
        {
            int numCustomButtons = 0;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (!buttons[i].IsEmpty)
                {
                    numCustomButtons++;
                }
            }

            var defaultButtonsList = new List<ButtonTemplate>();

            // Create our default button templates based on user preferences
            if (UseRemove)
            {
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Remove, numCustomButtons, UseHide, UseAdjust));
            }

            if (UseAdjust)
            {
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Adjust, numCustomButtons, UseHide, UseAdjust));
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Done, numCustomButtons, UseHide, UseAdjust));
            }

            if (UseHide)
            {
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Hide, numCustomButtons, UseHide, UseAdjust));
                defaultButtonsList.Add(GetDefaultButtonTemplateFromType(ButtonTypeEnum.Show, numCustomButtons, UseHide, UseAdjust));
            }
            DefaultButtons = defaultButtonsList.ToArray();
        }

#if UNITY_EDITOR
        public void EditorRefreshTemplates()
        {
            RefreshTemplates();
        }
#endif

        /// <summary>
        /// Generates a template for a default button based on type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="numCustomButtons"></param>
        /// <param name="useHide"></param>
        /// <param name="useAdjust"></param>
        /// <returns></returns>
        private static ButtonTemplate GetDefaultButtonTemplateFromType(ButtonTypeEnum type, int numCustomButtons, bool useHide, bool useAdjust)
        {
            // Button position is based on custom buttons
            // In the app bar, Hide/Show
            switch (type)
            {
                case ButtonTypeEnum.Custom:
                    return new ButtonTemplate(
                        ButtonTypeEnum.Custom,
                        "Custom",
                        "",
                        "Custom",
                        0,
                        0);

                case ButtonTypeEnum.Adjust:
                    int adjustPosition = numCustomButtons + 1;

                    if (!useHide)
                    {
                        adjustPosition--;
                    }

                    return new ButtonTemplate(
                        ButtonTypeEnum.Adjust,
                        "Adjust",
                        "AppBarAdjust",
                        "Adjust",
                        adjustPosition, // Always next-to-last to appear
                        0);

                case ButtonTypeEnum.Done:
                    return new ButtonTemplate(
                        ButtonTypeEnum.Done,
                        "Done",
                        "AppBarDone",
                        "Done",
                        0,
                        0);

                case ButtonTypeEnum.Hide:
                    return new ButtonTemplate(
                        ButtonTypeEnum.Hide,
                        "Hide",
                        "AppBarHide",
                        "Hide Menu",
                        0, // Always the first to appear
                        0);

                case ButtonTypeEnum.Remove:
                    int removePosition = numCustomButtons + 1;
                    if (useAdjust)
                    {
                        removePosition++;
                    }

                    if (!useHide)
                    {
                        removePosition--;
                    }

                    return new ButtonTemplate(
                        ButtonTypeEnum.Remove,
                        "Remove",
                        "KeyboardKeyGlyphs_Close",
                        "Remove",
                        removePosition, // Always the last to appear
                        1);

                case ButtonTypeEnum.Show:
                    return new ButtonTemplate(
                        ButtonTypeEnum.Show,
                        "Show",
                        "AppBarShow",
                        "Show Menu",
                        0,
                        0);

                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}
