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

        #region Public properties

        /// <summary>
        /// Where to display the app bar on the y axis
        /// This can be set to negative values
        /// to force the app bar to appear below the object
        /// </summary>
        public float HoverOffsetYScale
        {
            get { return hoverOffsetYScale; }
            set { hoverOffsetYScale = value; }
        }

        /// <summary>
        /// Pushes the app bar away from the object
        /// </summary>
        public float HoverOffsetZ
        {
            get { return hoverOffsetZ; }
            set { hoverOffsetZ = value; }
        }

        /// <summary>
        /// Uses an alternate follow style that works better for very oblong objects
        /// </summary>
        public bool UseTightFollow
        {
            get { return useTightFollow; }
            set { useTightFollow = value; }
        }

        public AppBarDisplayTypeEnum DisplayType
        {
            get { return displayType; }
            set { displayType = value; }
        }

        public AppBarStateEnum State
        {
            get { return state; }
            set { state = value; }
        }

        public bool UseRemove
        {
            get { return useRemove; }
            set { useRemove = value; }
        }

        public bool UseAdjust
        {
            get { return useAdjust; }
            set { useAdjust = value; }
        }

        public bool UseHide
        {
            get { return useHide; }
            set { useHide = value; }
        }

        #endregion

        #region Private Serialized Fields

        [Header("Target Bounding Box")]
        [SerializeField]
        private BoundingBox boundingBox = null;

        [SerializeField]
        private GameObject baseRenderer = null;

        [SerializeField]
        private Transform buttonParent = null;

        [SerializeField]
        private GameObject backgroundBar = null;
               
        [Header("States")]
        [SerializeField]
        private AppBarDisplayTypeEnum displayType = AppBarDisplayTypeEnum.Manipulation;

        [SerializeField]
        private AppBarStateEnum state = AppBarStateEnum.Default;

        [Header("Default Button Options")]
        [SerializeField]
        private bool useRemove = true;

        [SerializeField]
        private bool useAdjust = true;

        [SerializeField]
        private bool useHide = true;

        [Header("Default Button Icons")]
        [SerializeField]
        private Texture adjustIcon = null;

        [SerializeField]
        private Texture doneIcon = null;

        [SerializeField]
        private Texture hideIcon = null;

        [SerializeField]
        private Texture removeIcon = null;

        [SerializeField]
        private Texture showIcon = null;

        [Header("Scale & Position Options")]
        [SerializeField]
        [Tooltip("Uses an alternate follow style that works better for very oblong objects.")]
        private bool useTightFollow = false;

        [SerializeField]
        [Tooltip("Where to display the app bar on the y axis. This can be set to negative values to force the app bar to appear below the object.")]
        private float hoverOffsetYScale = 0.25f;

        [SerializeField]
        [Tooltip("Pushes the app bar away from the object.")]
        private float hoverOffsetZ = 0f;

        [SerializeField]
        private float buttonWidth = 0.032f;

        [SerializeField]
        private float buttonDepth = 0.016f;

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
            state = AppBarStateEnum.Default;
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
                    state = AppBarStateEnum.Manipulation;
                    break;

                case ButtonTypeEnum.Hide:
                    state = AppBarStateEnum.Hidden;
                    break;

                case ButtonTypeEnum.Show:
                    state = AppBarStateEnum.Default;
                    break;

                case ButtonTypeEnum.Done:
                    state = AppBarStateEnum.Default;
                    break;

                default:
                    break;
            }
        }

        protected virtual void OnClickRemove()
        {
            // Set the app bar and bounding box to inactive
            boundingBox.Target.SetActive(false);
            boundingBox.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        private void InitializeButtons()
        {
            buttons.Clear();

            foreach (Transform child in buttonParent)
            {
                AppBarButton appBarButton = child.GetComponent<AppBarButton>();
                if (appBarButton == null)
                    throw new Exception("Found a transform without an AppBarButton component under buttonTransforms!");
                
                appBarButton.InitializeButtonContent(this);
                // Set to invisible initially
                appBarButton.SetVisible(false);

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

                if (button.ButtonType != ButtonTypeEnum.Custom)
                    button.SetVisible(GetButtonVisible(button.ButtonType));

                if (!buttons[i].Visible)
                    continue;

                activeButtonNum++;
            }

            // Sort the buttons by display order
            buttons.Sort(delegate (AppBarButton b1, AppBarButton b2) { return b2.DisplayOrder.CompareTo(b1.DisplayOrder); });

            // Use active button number to determine background size and offset
            float backgroundBarSize = buttonWidth * activeButtonNum;
            Vector3 positionOffset = Vector3.right * ((backgroundBarSize / 2) - (buttonWidth / 2));

            // Go through them again, setting active as
            activeButtonNum = 0;
            for (int i = 0; i < buttons.Count; i++)
            {
                // Set the sibling index and target position so the button will behave predictably when set visible
                buttons[i].transform.SetSiblingIndex(i);
                buttons[i].SetTargetPosition((Vector3.left * buttonWidth * activeButtonNum) + positionOffset);

                if (!buttons[i].Visible)
                    continue;

                activeButtonNum++;
            }

            targetBarSize.x = backgroundBarSize;
            backgroundBar.transform.localScale = Vector3.Lerp(backgroundBar.transform.localScale, targetBarSize, Time.deltaTime * backgroundBarMoveSpeed);
            backgroundBar.transform.localPosition = Vector3.forward * buttonDepth / 2;
        }

        private void UpdateBoundingBox()
        {
            if (boundingBox == null || boundingBox.Target == null)
            {
                if (displayType == AppBarDisplayTypeEnum.Manipulation)
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

            // BoundingBox can't update in editor mode
            if (!Application.isPlaying)
                return;

            if (boundingBox == null)
                return;

            switch (state)
            {
                case AppBarStateEnum.Manipulation:
                    boundingBox.Active = true;
                    break;

                default:
                    boundingBox.Active = false;
                    break;
            }
        }

        private void FollowBoundingBox(bool smooth)
        {
            if (boundingBox == null)
                return;

            //calculate best follow position for AppBar
            Vector3 finalPosition = Vector3.zero;
            Vector3 headPosition = Camera.main.transform.position;
            boundsPoints.Clear();

            helper.UpdateNonAABoundingBoxCornerPositions(boundingBox, boundsPoints);
            int followingFaceIndex = helper.GetIndexOfForwardFace(headPosition);
            Vector3 faceNormal = helper.GetFaceNormal(followingFaceIndex);

            //finally we have new position
            finalPosition = helper.GetFaceBottomCentroid(followingFaceIndex) + (faceNormal * HoverOffsetZ);

            // Follow our bounding box
            transform.position = smooth ? Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * backgroundBarMoveSpeed) : finalPosition;

            // Rotate on the y axis
            Vector3 direction = (boundingBox.TargetBounds.bounds.center - finalPosition).normalized;
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
                    if (!useRemove)
                        return false;
                    break;

                case ButtonTypeEnum.Hide:
                    if (!useHide)
                        return false;
                    break;

                case ButtonTypeEnum.Adjust:
                    if (!useAdjust)
                        return false;
                    break;
            }

            switch (state)
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
                    buttonIcon = showIcon;
                    displayOrder = 0;
                    break;

                case ButtonTypeEnum.Hide:
                    buttonText = "Hide";
                    buttonIcon = hideIcon;
                    displayOrder = 1;
                    break;

                case ButtonTypeEnum.Adjust:
                    buttonText = "Adjust";
                    buttonIcon = adjustIcon;
                    displayOrder = 2;
                    break;

                case ButtonTypeEnum.Remove:
                    buttonText = "Remove";
                    buttonIcon = removeIcon;
                    displayOrder = 3;
                    break;

                case ButtonTypeEnum.Done:
                    buttonText = "Done";
                    buttonIcon = doneIcon;
                    displayOrder = 4;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}
