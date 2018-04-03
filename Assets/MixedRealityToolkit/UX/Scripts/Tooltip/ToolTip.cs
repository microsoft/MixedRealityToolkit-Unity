//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;
using MixedRealityToolkit.UX.Buttons;
using MixedRealityToolkit.UX.Lines;

namespace MixedRealityToolkit.UX.ToolTips
{
    [RequireComponent(typeof(ToolTipConnector))]
    public class ToolTip : Button
    {
        #region fields
        // TipState - Set locally
        // GroupState - Set by GroupManager (class available TBD)
        // Global State - Set by MasterManager (class available TBD)
        [Serializable]
        public enum TipDisplayModeEnum
        {
            /// <summary>
            /// No state to have from Manager
            /// </summary>
            None,
            /// <summary>
            /// Tips are always on
            /// </summary>
            On,
            /// <summary>
            /// Looking at Object Activates tip (Object must be interactive)
            /// </summary>
            OnFocus,
            /// <summary>
            /// Tips are always off
            /// </summary>
            Off
        }

        [SerializeField]
        private bool showBackground = true;

        [SerializeField]
        private bool showOutline = false;

        [SerializeField]
        private bool showConnector = true;

        [SerializeField]
        protected TipDisplayModeEnum tipState;

        [SerializeField]
        protected TipDisplayModeEnum groupTipState;

        [SerializeField]
        protected TipDisplayModeEnum masterTipState;
        #endregion

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

        public Vector3 AnchorPosition
        {
            get
            {
                return anchor.transform.position;
            }
        }

        public Transform ContentParentTransform
        {
            get
            {
                return contentParent.transform;
            }
        }

        public Vector2 LocalContentSize
        {
            get
            {
                return localContentSize;
            }
        }

        public Vector3 LocalContentOffset
        {
            get
            {
                return backgroundOffset;
            }
        }

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

        public GameObject Pivot
        {
            get
            {
                return pivot;
            }
        }

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

        public bool HasFocus
        {
            get
            {
                switch (ButtonState)
                {
                    case Buttons.Enums.ButtonStateEnum.Targeted:
                    case Buttons.Enums.ButtonStateEnum.ObservationTargeted:
                    case Buttons.Enums.ButtonStateEnum.Pressed:
                        return true;

                    default:
                        return false;
                }
            }
        }

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

        public ToolTipUtility.AttachPointType PivotType
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

        // Used for Content Change update in text
        public Action ContentChange;

        [Tooltip("GameObject that the line and text are attached to")]
        [SerializeField]
        protected GameObject anchor;

        [Tooltip("Pivot point that text will rotate around as well as the point where the Line will be rendered to.")]
        [SerializeField]
        protected GameObject pivot;

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

        [Tooltip("The padding around the content (height / width)")]
        [SerializeField]
        protected Vector2 backgroundPadding;

        [Tooltip("The offset of the background (x / y / z)")]
        [SerializeField]
        protected Vector3 backgroundOffset;

        [Tooltip("The scale of all the content (label, backgrounds, etc.)")]
        [SerializeField]
        [Range(0.01f, 3f)]
        protected float contentScale = 1f;

        [Tooltip("The font size of the tooltip.)")]
        [SerializeField]
        [Range(10, 60)]
        protected int fontSize = 30;

        [SerializeField]
        protected ToolTipUtility.AttachPointType attachPointType = ToolTipUtility.AttachPointType.Closest;

        [Tooltip("The line connecting the anchor to the pivot. If present, this component will be updated automatically.")]
        [SerializeField]
        protected LineBase toolTipLine;

        protected Vector2 localContentSize;

        protected Vector3 localAttachPoint;

        protected Vector3 attachPointOffset;

        protected Vector3[] localAttachPointPositions;

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

        protected virtual bool EnforceHeirarchy() {

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
        void OnDrawGizmos() {

            if (Application.isPlaying)
                return;

            if (!EnforceHeirarchy()) {
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