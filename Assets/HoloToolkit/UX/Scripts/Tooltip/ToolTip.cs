//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;
using HoloToolkit.Unity.UX;
using HoloToolkit.Unity.Buttons;

namespace HoloToolkit.UX.ToolTips
{
    [RequireComponent(typeof(ToolTipConnector))]

    /// <summary>
    /// Class for Tooltip object
    /// Creates a floating tooltip that is attached to an object and moves to stay in view as object rotates with respect to the view.
    /// </summary>
    public class ToolTip : Button
    {
        [SerializeField]
        private bool showBackground = true;
        /// <summary>
        /// getter/setter for showing the opaque background of tooltip.
        /// </summary>
        public bool ShowBackground
        {
            get
            {
                return showBackground;
            }
            set
            {
                showBackground = value;
                GetComponent<ToolTipBackgroundMesh>().IsVisible = value;
            }
        }

        [SerializeField]
        private bool showOutline = false;
        /// <summary>
        /// getter/setter for showing white trim around edge of tooltip
        /// </summary>
        public bool ShowOutline
        {
            get
            {
                return showOutline;
            }
            set
            {
                showOutline = value;
                GameObject TipBackground = contentParent.transform.GetChild(1).gameObject;
                Rectangle rectangle = TipBackground.GetComponent<Rectangle>();
                rectangle.enabled = value;
            }
        }

        [SerializeField]
        private bool showConnector = true;
        /// <summary>
        /// getter/setter for showing connecting stem between tooltip and parent object
        /// </summary>
        public bool ShowConnector
        {
            get
            {
                return showConnector;
            }
            set
            {
                showConnector = value;
                //todo fix this
                Line lineScript = GetComponent<Line>();
                lineScript.enabled = value;
            }
        }

        [SerializeField]
        protected TipDisplayModeEnum tipState;
        /// <summary>
        /// getter/setter for the display state of a tooltip
        /// </summary>
        public TipDisplayModeEnum TipState
        {
            get
            {
                return tipState;
            }
            set
            {
                tipState = value;
            }
        }

        [SerializeField]
        protected TipDisplayModeEnum groupTipState;
        /// <summary>
        /// getter/setter for display state of group of tooltips
        /// </summary>
        public TipDisplayModeEnum GroupTipState
        {
            set
            {
                groupTipState = value;
            }
            get
            {
                return groupTipState;
            }
        }

        [SerializeField]
        protected TipDisplayModeEnum masterTipState;
        /// <summary>
        /// getter/setter for display state of master tooltip
        /// </summary>
        public TipDisplayModeEnum MasterTipState
        {
            set
            {
                masterTipState = value;
            }
            get
            {
                return masterTipState;
            }
        }

        [Tooltip("GameObject that the line and text are attached to")]
        [SerializeField]
        protected GameObject anchor;
        /// <summary>
        /// getter/setter for ameObject that the line and text are attached to
        /// </summary>
        public GameObject Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                anchor = value;
            }
        }

        [Tooltip("Pivot point that text will rotate around as well as the point where the Line will be rendered to.")]
        [SerializeField]
        protected GameObject pivot;
        /// <summary>
        /// Pivot point that text will rotate around as well as the point where the Line will be rendered to. 
        /// </summary>
        public GameObject Pivot
        {
            get
            {
                return pivot;
            }
        }

        [Tooltip("GameObject text that is displayed on the tooltip.")]
        [SerializeField]
        protected GameObject label;

        [Tooltip("Parent of the Text and Background")]
        [SerializeField]
        protected GameObject contentParent;

        [Tooltip("Text for the ToolTip to say")]
        [SerializeField]
        [TextArea]
        protected string toolTipText;
        /// <summary>
        /// Text for the ToolTip to display
        /// </summary>
        public string ToolTipText
        {
            set
            {
                if (value != toolTipText)
                {
                    toolTipText = value;
                    RefreshLocalContent();
                    if (ContentChange != null)
                        ContentChange.Invoke();
                }
            }
            get
            {
                return toolTipText;
            }
        }

        [Tooltip("The padding around the content (height / width)")]
        [SerializeField]
        protected Vector2 backgroundPadding;

        [Tooltip("The offset of the background (x / y / z)")]
        [SerializeField]
        protected Vector3 backgroundOffset;
        /// <summary>
        /// The offset of the background (x / y / z)
        /// </summary>
        public Vector3 LocalContentOffset
        {
            get
            {
                return backgroundOffset;
            }
        }

        [Tooltip("The scale of all the content (label, backgrounds, etc.)")]
        [SerializeField]
        [Range(0.01f, 3f)]
        protected float contentScale = 1f;
        /// <summary>
        /// The scale of all the content (label, backgrounds, etc.)
        /// </summary>
        public float ContentScale
        {
            get
            {
                return contentScale;
            }
            set
            {
                contentScale = value;
                RefreshLocalContent();
            }
        }

        [Tooltip("The font size of the tooltip.)")]
        [SerializeField]
        [Range(10, 60)]
        protected int fontSize = 30;

        [SerializeField]
        protected ToolTipAttachPointType attachPointType = ToolTipAttachPointType.Closest;
        /// <summary>
        /// getter/setter for type of pivot
        /// </summary>
        public ToolTipAttachPointType PivotType
        {
            get
            {
                return attachPointType;
            }
            set
            {
                attachPointType = value;
            }
        }

        [Tooltip("The line connecting the anchor to the pivot. If present, this component will be updated automatically.")]
        [SerializeField]
        protected LineBase toolTipLine;

        protected Vector2 localContentSize;
        /// <summary>
        /// getter/setter for size of tooltip.
        /// </summary>
        public Vector2 LocalContentSize
        {
            get
            {
                return localContentSize;
            }
        }

        protected Vector3 localAttachPoint;

        protected Vector3 attachPointOffset;

        protected Vector3[] localAttachPointPositions;

        /// <summary>
        /// point about which ToolTip pivots to face camera
        /// </summary>
        public Vector3 PivotPosition
        {
            get
            {
                return pivot.transform.position;
            }
            set
            {
                pivot.transform.position = value;
            }
        }

        /// <summary>
        /// point where ToolTip is attached
        /// </summary>
        public Vector3 AttachPointPosition
        {
            get
            {
                return contentParent.transform.TransformPoint(localAttachPoint) + attachPointOffset;
            }
            set
            {
                // apply the difference to the offset
                attachPointOffset = value - contentParent.transform.TransformPoint(localAttachPoint);
            }
        }

        /// <summary>
        /// point where ToolTip connector is attached
        /// </summary>
        public Vector3 AnchorPosition
        {
            get
            {
                return anchor.transform.position;
            }
        }

        /// <summary>
        /// Tramsform of object to which ToolTip is attached
        /// </summary>
        public Transform ContentParentTransform
        {
            get
            {
                return contentParent.transform;
            }
        }

        /// <summary>
        /// is ToolTip active and displaying
        /// </summary>
        public bool IsOn
        {
            get
            {
                switch (masterTipState)
                {
                    case TipDisplayModeEnum.None:
                    default:
                        // Use our group state
                        switch (groupTipState)
                        {
                            case TipDisplayModeEnum.None:
                            default:
                                // Use our local State
                                switch (tipState)
                                {
                                    case TipDisplayModeEnum.None:
                                    case TipDisplayModeEnum.Off:
                                    default:
                                        return false;

                                    case TipDisplayModeEnum.On:
                                        return true;

                                    case TipDisplayModeEnum.OnFocus:
                                        return HasFocus;
                                }

                            case TipDisplayModeEnum.On:
                                return true;

                            case TipDisplayModeEnum.Off:
                                return false;

                            case TipDisplayModeEnum.OnFocus:
                                return HasFocus;
                        }

                    case TipDisplayModeEnum.On:
                        return true;

                    case TipDisplayModeEnum.Off:
                        return false;

                    case TipDisplayModeEnum.OnFocus:
                        return HasFocus;
                }
            }
        }

        /// <summary>
        /// does the ToolTip have focus.
        /// </summary>
        public bool HasFocus
        {
            get
            {
                switch (ButtonState)
                {
                    case ButtonStateEnum.Targeted:
                    case ButtonStateEnum.ObservationTargeted:
                    case ButtonStateEnum.Pressed:
                        return true;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Used for Content Change update in text
        /// </summary>
        public Action ContentChange;

        /// <summary>
        /// virtual functions
        /// </summary>
        protected virtual void OnEnable() {

            // Get our line if it exists
            if (toolTipLine == null)
                toolTipLine = gameObject.GetComponent<LineBase>();

            //EnforceHeirarchy();
            RefreshLocalContent();
            contentParent.SetActive(false);
            ShowBackground = showBackground;
            ShowOutline = showOutline;
            ShowConnector = showConnector;
        }

        protected virtual void Update() {
            // Enable / disable our line if it exists
            if (toolTipLine != null)
            {
                toolTipLine.enabled = IsOn;
                toolTipLine.FirstPoint = AnchorPosition;
                toolTipLine.LastPoint = AttachPointPosition;
            }

            if (IsOn) {
                contentParent.SetActive(true);
                localAttachPoint = ToolTipUtility.FindClosestAttachPointToAnchor(anchor.transform, contentParent.transform, localAttachPointPositions, attachPointType);
            } else {
                contentParent.SetActive(false);
            }
        }

        protected virtual void RefreshLocalContent() {

            // Set the scale of the pivot
            contentParent.transform.localScale = Vector3.one * contentScale;
            label.transform.localScale = Vector3.one * 0.005f;
            // Set the content using a text mesh by default
            // This function can be overridden for tooltips that use Unity UI
            TextMesh text = label.GetComponent<TextMesh>();
            if (text != null && !string.IsNullOrEmpty(toolTipText)) {
                text.fontSize = fontSize;
                text.text = toolTipText.Trim();
                text.lineSpacing = 1;
                text.anchor = TextAnchor.MiddleCenter;
                // Get the world scale of the text
                // Convert that to local scale using the content parent
                Vector3 localScale = text.transform.localScale;
                localContentSize.x = localScale.x + backgroundPadding.x;
                localContentSize.y = localScale.y + backgroundPadding.y;
            }
            // Now that we have the size of our content, get our pivots
            ToolTipUtility.GetAttachPointPositions(ref localAttachPointPositions, localContentSize);
            localAttachPoint = ToolTipUtility.FindClosestAttachPointToAnchor(anchor.transform, contentParent.transform, localAttachPointPositions, attachPointType);
        }

        protected virtual bool EnforceHierarchy() {

            Transform pivotTransform = transform.Find("Pivot");
            Transform anchorTransform = transform.Find("Anchor");
            if (pivotTransform == null || anchorTransform == null) {
                if (Application.isPlaying) {
                    Debug.LogError("Found error in heirarchy, disabling.");
                    enabled = false;
                }
                return false;
            }
            Transform contentParentTransform = pivotTransform.Find("ContentParent");
            if (contentParentTransform == null) {
                if (Application.isPlaying) {
                    Debug.LogError("Found error in heirarchy, disabling.");
                    enabled = false;
                }
                return false;
            }
            Transform labelTransform = contentParentTransform.Find("Label");
            if (labelTransform == null) {
                if (Application.isPlaying) {
                    Debug.LogError("Found error in heirarchy, disabling.");
                    enabled = false;
                }
                return false;
            }

            contentParentTransform.localPosition = Vector3.zero;
            contentParentTransform.localRotation = Quaternion.identity;
            contentParentTransform.localScale = Vector3.one * contentScale;
            labelTransform.localPosition = Vector3.zero;
            labelTransform.localScale = Vector3.one * 0.025f;
            labelTransform.localRotation = Quaternion.identity;
            pivotTransform.localScale = Vector3.one;

            pivot = pivotTransform.gameObject;
            anchor = anchorTransform.gameObject;
            contentParent = contentParentTransform.gameObject;
            label = labelTransform.gameObject;

            return true;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {

            if (Application.isPlaying)
                return;

            if (!EnforceHierarchy()) {
                return;
            }

            RefreshLocalContent();

            if (toolTipLine != null)
            {
                toolTipLine.FirstPoint = AnchorPosition;
                toolTipLine.LastPoint = AttachPointPosition;
            }
        }
        #endif
    }
}