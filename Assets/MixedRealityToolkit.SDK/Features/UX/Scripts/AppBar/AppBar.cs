// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Logic for the App Bar. Generates buttons, manages states.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/AppBar")]
    public class AppBar : MonoBehaviour
    {
        private const float backgroundBarMoveSpeed = 5;

        #region Enum Definitions

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

        #endregion

        #region Private Serialized Fields with Public Properties

        [Header("Target Bounding Box")]
        /// <summary>
        /// The bounding box this AppBar will use for its placement and control for manipulation
        /// </summary>
        [Tooltip("The bounding box this AppBar will use for its placement and control for manipulation")]
        [SerializeField]
        private BoundingBox boundingBox = null;

        public BoundingBox TargetBoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        /// <summary>
        /// The parent game object for the renderable objects in the AppBar
        /// </summary>
        [Tooltip("The parent game object for the renderable objects in the app bar")]
        [SerializeField]
        private GameObject baseRenderer = null;
        public GameObject BaseRenderer
        {
            get => baseRenderer;
            set => baseRenderer = value;
        }

        /// <summary>
        /// The parent transform for the button collection
        /// </summary>
        [Tooltip("The parent transform for the button collection")]
        [SerializeField]
        private Transform buttonParent = null;
        public Transform ButtonParent
        {
            get => buttonParent;
            set => buttonParent = value;
        }

        /// <summary>
        /// The background gameobject, scales to fill area behind buttons
        /// </summary>
        [Tooltip("The background gameobject, scales to fill area behind buttons")]
        [SerializeField]
        private GameObject backgroundBar = null;
        public GameObject BackgroundBar
        {
            get => backgroundBar;
            set => backgroundBar = value;
        }
        
        [Header("States")]
        /// <summary>
        /// The AppBar's display type, default is Manipulation
        /// </summary>
        [Tooltip("The AppBar's display type, default is Manipulation")]
        [SerializeField]
        private AppBarDisplayTypeEnum displayType = AppBarDisplayTypeEnum.Manipulation;
        public AppBarDisplayTypeEnum DisplayType
        {
            get { return displayType; }
            set { displayType = value; }
        }

        /// <summary>
        /// The AppBar's current state
        /// </summary>
        [Tooltip("The AppBar's current state")]
        [SerializeField]
        private AppBarStateEnum state = AppBarStateEnum.Default;
        public AppBarStateEnum State
        {
            get { return state; }
            set { state = value; }
        }


        [Header("Default Button Options")]
        /// <summary>
        /// Should the AppBar have a Remove button
        /// </summary>
        [Tooltip("Should the AppBar have a Remove button")]
        [SerializeField]
        private bool useRemove = true;
        public bool UseRemove
        {
            get { return useRemove; }
            set { useRemove = value; }
        }

        /// <summary>
        /// Should the AppBar have an Adjust button
        /// </summary>
        [Tooltip("Should the AppBar have an Adjust button")]
        [SerializeField]
        private bool useAdjust = true;
        public bool UseAdjust
        {
            get { return useAdjust; }
            set { useAdjust = value; }
        }

        /// <summary>
        /// Should the AppBar have a Hide button
        /// </summary>
        [Tooltip("Should the AppBar have a Hide button")]
        [SerializeField]
        private bool useHide = true;
        public bool UseHide
        {
            get { return useHide; }
            set { useHide = value; }
        }

        [Header("Default Button Icons")]
        /// <summary>
        /// The Adjust button texture
        /// </summary>
        [Tooltip("The Adjust button texture")]
        [SerializeField]
        private Texture adjustIcon = null;
        public Texture AdjustIcon
        {
            get => adjustIcon;
            set => adjustIcon = value;
        }

        /// <summary>
        /// The Done button texture
        /// </summary>
        [Tooltip("The Done button texture")]
        [SerializeField]
        private Texture doneIcon = null;
        public Texture DoneIcon
        {
            get => doneIcon;
            set => doneIcon = value;
        }

        /// <summary>
        /// The Hide button texture
        /// </summary>
        [Tooltip("The Hide button texture")]
        [SerializeField]
        private Texture hideIcon = null;
        public Texture HideIcon
        {
            get => hideIcon;
            set => hideIcon = value;
        }

        /// <summary>
        /// The Remove button texture
        /// </summary>
        [Tooltip("The Remove button texture")]
        [SerializeField]
        private Texture removeIcon = null;
        public Texture RemoveIcon
        {
            get => removeIcon;
            set => removeIcon = value;
        }

        /// <summary>
        /// The Show button texture
        /// </summary>
        [Tooltip("The Show button texture")]
        [SerializeField]
        private Texture showIcon = null;
        public Texture ShowIcon
        {
            get => showIcon;
            set => showIcon = value;
        }

        [Header("Scale & Position Options")]
        /// <summary>
        /// Uses an alternate follow style that works better for very oblong objects
        /// </summary>
        [SerializeField]
        [Tooltip("Uses an alternate follow style that works better for very oblong objects.")]
        private bool useTightFollow = false;
        public bool UseTightFollow
        {
            get { return useTightFollow; }
            set { useTightFollow = value; }
        }

        /// <summary>
        /// Where to display the app bar on the y axis
        /// This can be set to negative values
        /// to force the app bar to appear below the object
        /// </summary>
        [SerializeField]
        [Tooltip("Where to display the app bar on the y axis. This can be set to negative values to force the app bar to appear below the object.")]
        private float hoverOffsetYScale = 0.25f;
        public float HoverOffsetYScale
        {
            get { return hoverOffsetYScale; }
            set { hoverOffsetYScale = value; }
        }

        /// <summary>
        /// Pushes the app bar away from the object
        /// </summary>
        [SerializeField]
        [Tooltip("Pushes the app bar away from the object.")]
        private float hoverOffsetZ = 0f;
        public float HoverOffsetZ
        {
            get { return hoverOffsetZ; }
            set { hoverOffsetZ = value; }
        }


        /// <summary>
        /// The button width for each button
        /// </summary>
        [Tooltip("The button width for each button")]
        [SerializeField]
        private float buttonWidth = 0.032f;
        public float ButtonWidth
        {
            get => buttonWidth;
            set => buttonWidth = value;
        }

        /// <summary>
        /// The button depth for each button
        /// </summary>
        [Tooltip("The button depth for each button")]
        [SerializeField]
        private float buttonDepth = 0.016f;
        public float ButtonDepth
        {
            get => buttonDepth;
            set => buttonDepth = value;
        }

        #endregion

        private List<AppBarButton> buttons = new List<AppBarButton>();
        private Vector3 targetBarSize = Vector3.one;
        private float lastTimeTapped = 0f;
        private float coolDownTime = 0.5f;
        private BoundingBoxHelper helper = new BoundingBoxHelper();
        private List<Vector3> boundsPoints = new List<Vector3>();

        #region MonoBehaviour Functions

        private void OnEnable()
        {
            InitializeButtons();
        }

        private void LateUpdate()
        {
            UpdateAppBar();
        }

        #endregion

        public void Reset()
        {
            State = AppBarStateEnum.Default;
            FollowBoundingBox(false);
            lastTimeTapped = Time.time + coolDownTime;
        }

        public void OnButtonPressed(AppBarButton button)
        {
            if (Time.time < lastTimeTapped + coolDownTime)
                return;

            lastTimeTapped = Time.time;

            switch (button.ButtonType)
            {
                case ButtonTypeEnum.Remove:
                    OnClickRemove();
                    break;

                case ButtonTypeEnum.Adjust:
                    State = AppBarStateEnum.Manipulation;
                    break;

                case ButtonTypeEnum.Hide:
                    State = AppBarStateEnum.Hidden;
                    break;

                case ButtonTypeEnum.Show:
                    State = AppBarStateEnum.Default;
                    break;

                case ButtonTypeEnum.Done:
                    State = AppBarStateEnum.Default;
                    break;

                default:
                    break;
            }
        }

        protected virtual void OnClickRemove()
        {
            // Set the app bar and bounding box to inactive
            TargetBoundingBox.Target.SetActive(false);
            TargetBoundingBox.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        private void InitializeButtons()
        {
            buttons.Clear();

            foreach (Transform child in ButtonParent)
            {
                AppBarButton appBarButton = child.GetComponent<AppBarButton>();
                if (appBarButton == null)
                    throw new Exception("Found a transform without an AppBarButton component under buttonTransforms!");

                appBarButton.InitializeButtonContent(this);
                // Set to invisible initially if not custom
                switch (appBarButton.ButtonType)
                {
                    case ButtonTypeEnum.Custom:
                        break;

                    default:
                        appBarButton.SetVisible(false);
                        break;
                }

                buttons.Add(appBarButton);
            }
        }

        private void UpdateAppBar()
        {
            UpdateButtons();
            UpdateBoundingBox();
            FollowBoundingBox(true);
        }

        private void UpdateButtons()
        {
            // First just count how many buttons are visible
            int activeButtonNum = 0;
            for (int i = 0; i < buttons.Count; i++)
            {
                AppBarButton button = buttons[i];

                switch (button.ButtonType)
                {
                    case ButtonTypeEnum.Custom:
                        break;

                    default:
                        button.SetVisible(GetButtonVisible(button.ButtonType));
                        break;
                }

                if (!buttons[i].Visible)
                {
                    continue;
                }

                activeButtonNum++;
            }

            // Sort the buttons by display order
            buttons.Sort(delegate (AppBarButton b1, AppBarButton b2) { return b2.DisplayOrder.CompareTo(b1.DisplayOrder); });

            // Use active button number to determine background size and offset
            float backgroundBarSize = ButtonWidth * activeButtonNum;
            Vector3 positionOffset = Vector3.right * ((backgroundBarSize / 2) - (ButtonWidth / 2));

            // Go through them again, setting active as
            activeButtonNum = 0;
            for (int i = 0; i < buttons.Count; i++)
            {
                // Set the sibling index and target position so the button will behave predictably when set visible
                buttons[i].transform.SetSiblingIndex(i);
                buttons[i].SetTargetPosition((Vector3.left * ButtonWidth * activeButtonNum) + positionOffset);

                if (!buttons[i].Visible)
                    continue;

                activeButtonNum++;
            }

            targetBarSize.x = backgroundBarSize;
            BackgroundBar.transform.localScale = Vector3.Lerp(BackgroundBar.transform.localScale, targetBarSize, Time.deltaTime * backgroundBarMoveSpeed);
            BackgroundBar.transform.localPosition = Vector3.forward * ButtonDepth / 2;
        }

        private void UpdateBoundingBox()
        {
            if (TargetBoundingBox == null || TargetBoundingBox.Target == null)
            {
                if (DisplayType == AppBarDisplayTypeEnum.Manipulation)
                {
                    // Hide our buttons
                    BaseRenderer.SetActive(false);
                }
                else
                {
                    BaseRenderer.SetActive(true);
                }
                return;
            }

            // BoundingBox can't update in editor mode
            if (!Application.isPlaying)
                return;

            if (TargetBoundingBox == null)
                return;

            switch (State)
            {
                case AppBarStateEnum.Manipulation:
                    TargetBoundingBox.Active = true;
                    break;

                default:
                    TargetBoundingBox.Active = false;
                    break;
            }
        }

        private void FollowBoundingBox(bool smooth)
        {
            if (TargetBoundingBox == null)
                return;

            //calculate best follow position for AppBar
            Vector3 finalPosition = Vector3.zero;
            Vector3 headPosition = CameraCache.Main.transform.position;
            boundsPoints.Clear();

            helper.UpdateNonAABoundingBoxCornerPositions(TargetBoundingBox, boundsPoints);
            int followingFaceIndex = helper.GetIndexOfForwardFace(headPosition);
            Vector3 faceNormal = helper.GetFaceNormal(followingFaceIndex);

            //finally we have new position
            finalPosition = helper.GetFaceBottomCentroid(followingFaceIndex) + (faceNormal * HoverOffsetZ);

            // Follow our bounding box
            transform.position = smooth ? Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * backgroundBarMoveSpeed) : finalPosition;

            // Rotate on the y axis
            Vector3 direction = (TargetBoundingBox.TargetBounds.bounds.center - finalPosition).normalized;
            if (direction != Vector3.zero)
            {
                Vector3 eulerAngles = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
                eulerAngles.x = 0f;
                eulerAngles.z = 0f;
                transform.eulerAngles = eulerAngles;
            }
            else
            {
                transform.eulerAngles = Vector3.zero;
            }
        }

        private bool GetButtonVisible(ButtonTypeEnum buttonType)
        {
            // Set visibility based on button type / options
            switch (buttonType)
            {
                default:
                    break;

                case ButtonTypeEnum.Remove:
                    if (!UseRemove)
                        return false;
                    break;

                case ButtonTypeEnum.Hide:
                    if (!UseHide)
                        return false;
                    break;

                case ButtonTypeEnum.Adjust:
                    if (!UseAdjust)
                        return false;
                    break;
            }

            switch (State)
            {
                case AppBarStateEnum.Default:
                default:
                    switch (buttonType)
                    {
                        // Show hide, adjust, remove buttons
                        // The rest are hidden
                        case AppBar.ButtonTypeEnum.Hide:
                        case AppBar.ButtonTypeEnum.Remove:
                        case AppBar.ButtonTypeEnum.Adjust:
                        case AppBar.ButtonTypeEnum.Custom:
                            return true;

                        default:
                            return false;
                    }

                case AppBarStateEnum.Hidden:
                    switch (buttonType)
                    {
                        // Show show button
                        // The rest are hidden
                        case ButtonTypeEnum.Show:
                            return true;

                        default:
                            return false;
                    }

                case AppBarStateEnum.Manipulation:
                    switch (buttonType)
                    {
                        // Show done button
                        // The rest are hidden
                        case AppBar.ButtonTypeEnum.Done:
                            return true;

                        default:
                            return false;
                    }

            }
        }

        public void GetButtonTextAndIconFromType(ButtonTypeEnum type, out string buttonText, out Texture buttonIcon, out int displayOrder)
        {
            switch (type)
            {
                case ButtonTypeEnum.Show:
                    buttonText = "Show";
                    buttonIcon = ShowIcon;
                    displayOrder = 0;
                    break;

                case ButtonTypeEnum.Hide:
                    buttonText = "Hide";
                    buttonIcon = HideIcon;
                    displayOrder = 1;
                    break;

                case ButtonTypeEnum.Adjust:
                    buttonText = "Adjust";
                    buttonIcon = AdjustIcon;
                    displayOrder = 2;
                    break;

                case ButtonTypeEnum.Remove:
                    buttonText = "Remove";
                    buttonIcon = RemoveIcon;
                    displayOrder = 3;
                    break;

                case ButtonTypeEnum.Done:
                    buttonText = "Done";
                    buttonIcon = DoneIcon;
                    displayOrder = 4;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}
