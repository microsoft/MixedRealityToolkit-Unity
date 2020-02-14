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

        [Tooltip("Object the app bar is controlling - This object must implement the IBoundsTargetProvider.")]
        [SerializeField]
        private MonoBehaviour boundingBox = null;

        /// <summary>
        /// Object the app bar is controlling - This object must implement the IBoundsTargetProvider.
        /// </summary>
        public MonoBehaviour Target
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        [Tooltip("The parent game object for the renderable objects in the app bar")]
        [SerializeField]
        private GameObject baseRenderer = null;

        /// <summary>
        /// The parent game object for the renderable objects in the AppBar
        /// </summary>
        public GameObject BaseRenderer
        {
            get => baseRenderer;
            set => baseRenderer = value;
        }

        [Tooltip("The parent transform for the button collection")]
        [SerializeField]
        private Transform buttonParent = null;

        /// <summary>
        /// The parent transform for the button collection
        /// </summary>
        public Transform ButtonParent
        {
            get => buttonParent;
            set => buttonParent = value;
        }

        [Tooltip("The background gameobject, scales to fill area behind buttons")]
        [SerializeField]
        private GameObject backgroundBar = null;

        /// <summary>
        /// The background gameobject, scales to fill area behind buttons
        /// </summary>
        public GameObject BackgroundBar
        {
            get => backgroundBar;
            set => backgroundBar = value;
        }
        
        [Header("States")]

        [Tooltip("The AppBar's display type; default is Manipulation")]
        [SerializeField]
        private AppBarDisplayTypeEnum displayType = AppBarDisplayTypeEnum.Manipulation;

        /// <summary>
        /// The AppBar's display type; default is Manipulation
        /// </summary>
        public AppBarDisplayTypeEnum DisplayType
        {
            get { return displayType; }
            set { displayType = value; }
        }

        [Tooltip("The AppBar's current state")]
        [SerializeField]
        private AppBarStateEnum state = AppBarStateEnum.Default;

        /// <summary>
        /// The AppBar's current state
        /// </summary>
        public AppBarStateEnum State
        {
            get { return state; }
            set { state = value; }
        }


        [Header("Default Button Options")]

        [Tooltip("Should the AppBar have a remove button")]
        [SerializeField]
        private bool useRemove = true;

        /// <summary>
        /// Should the AppBar have a remove button
        /// </summary>
        public bool UseRemove
        {
            get { return useRemove; }
            set { useRemove = value; }
        }

        [Tooltip("Should the AppBar have an adjust button")]
        [SerializeField]
        private bool useAdjust = true;

        /// <summary>
        /// Should the AppBar have an adjust button
        /// </summary>
        public bool UseAdjust
        {
            get { return useAdjust; }
            set { useAdjust = value; }
        }

        [Tooltip("Should the AppBar have a hide button")]
        [SerializeField]
        private bool useHide = true;

        /// <summary>
        /// Should the AppBar have a hide button
        /// </summary>
        public bool UseHide
        {
            get { return useHide; }
            set { useHide = value; }
        }

        [Header("Default Button Icons")]

        [Tooltip("The adjust button texture")]
        [SerializeField]
        private Texture adjustIcon = null;

        /// <summary>
        /// The adjust button texture
        /// </summary>
        public Texture AdjustIcon
        {
            get => adjustIcon;
            set => adjustIcon = value;
        }

        [Tooltip("The done button texture")]
        [SerializeField]
        private Texture doneIcon = null;

        /// <summary>
        /// The done button texture
        /// </summary>
        public Texture DoneIcon
        {
            get => doneIcon;
            set => doneIcon = value;
        }

        [Tooltip("The hide button texture")]
        [SerializeField]
        private Texture hideIcon = null;

        /// <summary>
        /// The hide button texture
        /// </summary>
        public Texture HideIcon
        {
            get => hideIcon;
            set => hideIcon = value;
        }

        [Tooltip("The Remove button texture")]
        [SerializeField]
        private Texture removeIcon = null;

        /// <summary>
        /// The remove button texture
        /// </summary>
        public Texture RemoveIcon
        {
            get => removeIcon;
            set => removeIcon = value;
        }

        [Tooltip("The show button texture")]
        [SerializeField]
        private Texture showIcon = null;

        /// <summary>
        /// The show button texture
        /// </summary>
        public Texture ShowIcon
        {
            get => showIcon;
            set => showIcon = value;
        }

        [Header("Scale & Position Options")]

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

        [SerializeField]
        [Tooltip("Where to display the app bar on the y axis. This can be set to negative values to force the app bar to appear below the object.")]
        private float hoverOffsetYScale = 0.25f;

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

        [SerializeField]
        [Tooltip("Pushes the app bar away from the object.")]
        private float hoverOffsetZ = 0f;

        /// <summary>
        /// Pushes the app bar away from the object
        /// </summary>
        public float HoverOffsetZ
        {
            get { return hoverOffsetZ; }
            set { hoverOffsetZ = value; }
        }

        [Tooltip("The button width for each button")]
        [SerializeField]
        private float buttonWidth = 0.032f;

        /// <summary>
        /// The button width for each button
        /// </summary>
        public float ButtonWidth
        {
            get => buttonWidth;
            set => buttonWidth = value;
        }

        [Tooltip("The button depth for each button")]
        [SerializeField]
        private float buttonDepth = 0.016f;

        /// <summary>
        /// The button depth for each button
        /// </summary>
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
            FollowTargetObject(false);
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
            var boundsProvider = Target as IBoundsTargetProvider;
            if (boundsProvider != null)
            {
                boundsProvider.Target.SetActive(false);
            }
            Target.gameObject.SetActive(false);
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
            UpdateTargetObject();
            FollowTargetObject(true);
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

        private void UpdateTargetObject()
        {
            var boundsProvider = Target as IBoundsTargetProvider;
            if (boundsProvider == null || boundsProvider.Target == null)
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

            // Target can't be updated in editor mode
            if (!Application.isPlaying)
                return;

            if (boundsProvider == null)
                return;

            switch (State)
            {
                case AppBarStateEnum.Manipulation:
                    boundsProvider.Active = true;
                    break;

                default:
                    boundsProvider.Active = false;
                    break;
            }
        }

        private void FollowTargetObject(bool smooth)
        {
            var boundsProvider = Target as IBoundsTargetProvider;
            if (boundsProvider == null)
                return;

            //calculate best follow position for AppBar
            Vector3 finalPosition = Vector3.zero;
            Vector3 headPosition = CameraCache.Main.transform.position;
            boundsPoints.Clear();

            helper.UpdateNonAABoundsCornerPositions(boundsProvider.TargetBounds, boundsPoints);
            int followingFaceIndex = helper.GetIndexOfForwardFace(headPosition);
            Vector3 faceNormal = helper.GetFaceNormal(followingFaceIndex);

            //finally we have new position
            finalPosition = helper.GetFaceBottomCentroid(followingFaceIndex) + (faceNormal * HoverOffsetZ);

            // Follow our bounding box
            transform.position = smooth ? Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * backgroundBarMoveSpeed) : finalPosition;

            // Rotate on the y axis
            Vector3 direction = (boundsProvider.TargetBounds.bounds.center - finalPosition).normalized;
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
