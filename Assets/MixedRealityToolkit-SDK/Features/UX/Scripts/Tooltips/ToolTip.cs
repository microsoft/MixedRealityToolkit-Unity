//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.DataProviders;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// Class for Tooltip object
    /// Creates a floating tooltip that is attached to an object and moves to stay in view as object rotates with respect to the view.
    /// </summary>
    [RequireComponent(typeof(ToolTipConnector))]
    public class ToolTip : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Show the opaque background of tooltip.")]
        private bool showBackground = true;

        /// <summary>
        /// Show the opaque background of tooltip.
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
        [Tooltip("Shows white trim around edge of tooltip.")]
        private bool showOutline = false;

        /// <summary>
        /// Shows white trim around edge of tooltip.
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
                GameObject tipBackground = contentParent.transform.GetChild(1).gameObject;
                var rectangle = tipBackground.GetComponent<RectangleLineDataProvider>();
                rectangle.enabled = value;
            }
        }

        [SerializeField]
        [Tooltip("Show the connecting stem between the tooltip and its parent GameObject.")]
        private bool showConnector = true;

        /// <summary>
        /// Show the connecting stem between the tooltip and its parent GameObject.
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

                var lineScript = GetComponent<BaseMixedRealityLineDataProvider>();
                if (lineScript)
                { lineScript.enabled = value; }
            }
        }

        [SerializeField]
        [Tooltip("Display the state of the tooltip.")]
        private DisplayModeType tipState;

        /// <summary>
        /// The display the state of the tooltip.
        /// </summary>
        public DisplayModeType TipState
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
        [Tooltip("Display the state of a group of tooltips.")]
        private DisplayModeType groupTipState;

        /// <summary>
        /// Display the state of a group of tooltips.
        /// </summary>
        public DisplayModeType GroupTipState
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
        [Tooltip("Display the state of the master tooltip.")]
        private DisplayModeType masterTipState;

        /// <summary>
        /// Display the state of the master tooltip.
        /// </summary>
        public DisplayModeType MasterTipState
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

        [SerializeField]
        [Tooltip("GameObject that the line and text are attached to")]
        private GameObject anchor;
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
        private GameObject pivot;

        /// <summary>
        /// Pivot point that text will rotate around as well as the point where the Line will be rendered to. 
        /// </summary>
        public GameObject Pivot => pivot;

        [SerializeField]
        [Tooltip("GameObject text that is displayed on the tooltip.")]
        private GameObject label;

        [SerializeField]
        [Tooltip("Parent of the Text and Background")]
        private GameObject contentParent;

        [TextArea]
        [SerializeField]
        [Tooltip("Text for the ToolTip to display")]
        private string toolTipText;

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
                    ContentChange?.Invoke();
                }
            }
            get
            {
                return toolTipText;
            }
        }

        [SerializeField]
        [Tooltip("The padding around the content (height / width)")]
        private Vector2 backgroundPadding = Vector2.zero;

        [SerializeField]
        [Tooltip("The offset of the background (x / y / z)")]
        private Vector3 backgroundOffset = Vector3.zero;

        /// <summary>
        /// The offset of the background (x / y / z)
        /// </summary>
        public Vector3 LocalContentOffset => backgroundOffset;

        [SerializeField]
        [Range(0.01f, 3f)]
        [Tooltip("The scale of all the content (label, backgrounds, etc.)")]
        private float contentScale = 1f;

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

        [SerializeField]
        [Range(10, 60)]
        [Tooltip("The font size of the tooltip.")]
        private int fontSize = 30;

        [SerializeField]
        private ToolTipAttachPointType attachPointType = ToolTipAttachPointType.Closest;

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

        [SerializeField]
        [Tooltip("The line connecting the anchor to the pivot. If present, this component will be updated automatically.\n\nRecommended: SimpleLine, Spline, and ParabolaConstrainted")]
        private BaseMixedRealityLineDataProvider toolTipLine;

        private Vector2 localContentSize;

        /// <summary>
        /// getter/setter for size of tooltip.
        /// </summary>
        public Vector2 LocalContentSize => localContentSize;

        private Vector3 localAttachPoint;

        private Vector3 attachPointOffset;

        private Vector3[] localAttachPointPositions;

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
        public Vector3 AnchorPosition => anchor.transform.position;

        /// <summary>
        /// Transform of object to which ToolTip is attached
        /// </summary>
        public Transform ContentParentTransform => contentParent.transform;

        /// <summary>
        /// is ToolTip active and displaying
        /// </summary>
        public bool IsOn
        {
            get
            {
                switch (masterTipState)
                {
                    case DisplayModeType.None:
                    default:
                        // Use our group state
                        switch (groupTipState)
                        {
                            case DisplayModeType.None:
                            default:
                                // Use our local State
                                switch (tipState)
                                {
                                    case DisplayModeType.None:
                                    case DisplayModeType.Off:
                                    default:
                                        return false;

                                    case DisplayModeType.On:
                                        return true;

                                    case DisplayModeType.OnFocus:
                                        return HasFocus;
                                }

                            case DisplayModeType.On:
                                return true;

                            case DisplayModeType.Off:
                                return false;

                            case DisplayModeType.OnFocus:
                                return HasFocus;
                        }

                    case DisplayModeType.On:
                        return true;

                    case DisplayModeType.Off:
                        return false;

                    case DisplayModeType.OnFocus:
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
                return false;
            }
        }

        /// <summary>
        /// Used for Content Change update in text
        /// </summary>
        public Action ContentChange;

        /// <summary>
        /// virtual functions
        /// </summary>
        protected virtual void OnEnable()
        {
            // Get our line if it exists
            if (toolTipLine == null)
            {
                toolTipLine = gameObject.GetComponent<BaseMixedRealityLineDataProvider>();
            }

            RefreshLocalContent();
            contentParent.SetActive(false);
            ShowBackground = showBackground;
            ShowOutline = showOutline;
            ShowConnector = showConnector;
        }

        protected virtual void Update()
        {
            // Enable / disable our line if it exists
            if (toolTipLine != null)
            {
                if (!(toolTipLine is ParabolaConstrainedLineDataProvider))
                {
                    toolTipLine.enabled = IsOn;
                    toolTipLine.FirstPoint = AnchorPosition;
                }

                toolTipLine.LastPoint = AttachPointPosition;
            }

            if (IsOn)
            {
                contentParent.SetActive(true);
                localAttachPoint = ToolTipUtility.FindClosestAttachPointToAnchor(anchor.transform, contentParent.transform, localAttachPointPositions, PivotType);
            }
            else
            {
                contentParent.SetActive(false);
            }
        }

        protected virtual void RefreshLocalContent()
        {
            // Set the scale of the pivot
            contentParent.transform.localScale = Vector3.one * contentScale;
            label.transform.localScale = Vector3.one * 0.005f;
            // Set the content using a text mesh by default
            // This function can be overridden for tooltips that use Unity UI

            TextMesh text = label.GetComponent<TextMesh>();
            if (text != null && !string.IsNullOrEmpty(toolTipText))
            {
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
            localAttachPoint = ToolTipUtility.FindClosestAttachPointToAnchor(anchor.transform, contentParent.transform, localAttachPointPositions, PivotType);
        }

        protected virtual bool EnforceHierarchy()
        {
            Transform pivotTransform = transform.Find("Pivot");
            Transform anchorTransform = transform.Find("Anchor");

            if (pivotTransform == null || anchorTransform == null)
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("Found error in hierarchy, disabling.");
                    enabled = false;
                }

                return false;
            }
            Transform contentParentTransform = pivotTransform.Find("ContentParent");

            if (contentParentTransform == null)
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("Found error in hierarchy, disabling.");
                    enabled = false;
                }

                return false;
            }

            Transform labelTransform = contentParentTransform.Find("Label");
            if (labelTransform == null)
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("Found error in hierarchy, disabling.");
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
        private void OnDrawGizmos()
        {

            if (Application.isPlaying)
                return;

            if (!EnforceHierarchy())
            {
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
