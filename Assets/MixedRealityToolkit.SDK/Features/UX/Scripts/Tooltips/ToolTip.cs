//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.DataProviders;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.ToolTips
{
    /// <summary>
    /// Class for Tooltip object
    /// Creates a floating tooltip that is attached to an object and moves to stay in view as object rotates with respect to the view.
    /// </summary>
    [RequireComponent(typeof(ToolTipConnector))]
    [ExecuteAlways]
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
        private bool showHighlight = false;

        /// <summary>
        /// Shows white trim around edge of tooltip.
        /// </summary>
        public bool ShowHighlight
        {
            get
            {
                return showHighlight;
            }
            set
            {
                showHighlight = value;
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
            }
        }

        [SerializeField]
        [Tooltip("Display the state of the tooltip.")]
        private DisplayMode tipState;

        /// <summary>
        /// The display the state of the tooltip.
        /// </summary>
        public DisplayMode TipState
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
        private DisplayMode groupTipState;

        /// <summary>
        /// Display the state of a group of tooltips.
        /// </summary>
        public DisplayMode GroupTipState
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
        private DisplayMode masterTipState;

        /// <summary>
        /// Display the state of the master tooltip.
        /// </summary>
        public DisplayMode MasterTipState
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
        [Tooltip("Determines where the line will attach to the tooltip content.")]
        private ToolTipAttachPoint attachPointType = ToolTipAttachPoint.Closest;

        public ToolTipAttachPoint PivotType
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
        private List<IToolTipBackground> backgrounds = new List<IToolTipBackground>();
        private List<IToolTipHighlight> highlights = new List<IToolTipHighlight>();
        private TextMesh cachedLabelText;
        private int prevTextLength = -1;
        private int prevTextHash = -1;

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
                return ResolveTipState(masterTipState, groupTipState, tipState, HasFocus);              
            }
        }
        
        public static bool ResolveTipState(DisplayMode masterTipState, DisplayMode groupTipState, DisplayMode tipState, bool hasFocus)
        {
            switch (masterTipState)
            {
                case DisplayMode.None:
                default:
                    // Use our group state
                    switch (groupTipState)
                    {
                        case DisplayMode.None:
                        default:
                            // Use our local State
                            switch (tipState)
                            {
                                case DisplayMode.None:
                                case DisplayMode.Off:
                                default:
                                    return false;

                                case DisplayMode.On:
                                    return true;

                                case DisplayMode.OnFocus:
                                    return hasFocus;
                            }

                        case DisplayMode.On:
                            return true;

                        case DisplayMode.Off:
                            return false;

                        case DisplayMode.OnFocus:
                            return hasFocus;
                    }

                case DisplayMode.On:
                    return true;

                case DisplayMode.Off:
                    return false;

                case DisplayMode.OnFocus:
                    return hasFocus;
            }
        }

        /// <summary>
        /// does the ToolTip have focus.
        /// </summary>
        public virtual bool HasFocus
        {
            get
            {
                return false;
            }
        }

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

            backgrounds.Clear();
            foreach (IToolTipBackground background in GetComponents(typeof(IToolTipBackground)))
            {
                backgrounds.Add(background);
            }

            highlights.Clear();
            foreach (IToolTipHighlight highlight in GetComponents(typeof(IToolTipHighlight)))
            {
                highlights.Add(highlight);
            }

            RefreshLocalContent();

            contentParent.SetActive(false);
            ShowBackground = showBackground;
            ShowHighlight = showHighlight;
            ShowConnector = showConnector;
        }

        protected virtual void Update()
        {
            // Enable / disable our line if it exists
            if (toolTipLine != null)
            {
                toolTipLine.enabled = showConnector;

                if (!(toolTipLine is ParabolaConstrainedLineDataProvider))
                {
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

            RefreshLocalContent();
        }

        protected virtual void RefreshLocalContent()
        {
            // Set the scale of the pivot
            contentParent.transform.localScale = Vector3.one * contentScale;
            label.transform.localScale = Vector3.one * 0.005f;
            // Set the content using a text mesh by default
            // This function can be overridden for tooltips that use Unity UI

            // Has content changed?
            int currentTextLength = toolTipText.Length;
            int currentTextHash = toolTipText.GetHashCode();

            // If it has, update the content
            if (currentTextLength != prevTextLength || currentTextHash != prevTextHash)
            {
                prevTextHash = currentTextHash;
                prevTextLength = currentTextLength;

                if (cachedLabelText == null)
                    cachedLabelText = label.GetComponent<TextMesh>();

                if (cachedLabelText != null && !string.IsNullOrEmpty(toolTipText))
                {
                    cachedLabelText.fontSize = fontSize;
                    cachedLabelText.text = toolTipText.Trim();
                    cachedLabelText.lineSpacing = 1;
                    cachedLabelText.anchor = TextAnchor.MiddleCenter;
                    // Get the world scale of the text
                    // Convert that to local scale using the content parent
                    Vector3 localScale = GetTextMeshLocalScale(cachedLabelText);
                    localContentSize.x = localScale.x + backgroundPadding.x;
                    localContentSize.y = localScale.y + backgroundPadding.y;
                }

                // Now that we have the size of our content, get our pivots
                ToolTipUtility.GetAttachPointPositions(ref localAttachPointPositions, localContentSize);
                localAttachPoint = ToolTipUtility.FindClosestAttachPointToAnchor(anchor.transform, contentParent.transform, localAttachPointPositions, PivotType);

                foreach (IToolTipBackground background in backgrounds)
                {
                    background.OnContentChange(localContentSize, LocalContentOffset, contentParent.transform);
                }
            }

            foreach (IToolTipBackground background in backgrounds)
            {
                background.IsVisible = showBackground;
            }

            foreach (IToolTipHighlight highlight in highlights)
            {
                highlight.ShowHighlight = ShowHighlight;
            }
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

        public static Vector3 GetTextMeshLocalScale(TextMesh textMesh)
        {
            Vector3 localScale = Vector3.zero;

            if (string.IsNullOrEmpty(textMesh.text))
                return localScale;

            string[] splitStrings = textMesh.text.Split(new string[] { System.Environment.NewLine, "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            // Calculate the width of the text using character info
            float widestLine = 0f;
            foreach (string splitString in splitStrings)
            {
                float lineWidth = 0f;
                foreach (char symbol in splitString)
                {
                    CharacterInfo info;
                    if (textMesh.font.GetCharacterInfo(symbol, out info, textMesh.fontSize, textMesh.fontStyle))
                    {
                        lineWidth += info.advance;
                    }
                }
                if (lineWidth > widestLine)
                    widestLine = lineWidth;
            }
            localScale.x = widestLine;

            // Use this to multiply the character size
            Vector3 transformScale = textMesh.transform.localScale;
            localScale.x = (localScale.x * textMesh.characterSize * 0.1f) * transformScale.x;
            localScale.z = transformScale.z;

            // We could calculate the height based on line height and character size
            // But I've found that method can be flaky and has a lot of magic numbers
            // that may break in future Unity versions
            Vector3 eulerAngles = textMesh.transform.eulerAngles;
            Vector3 rendererScale = Vector3.zero;
            textMesh.transform.rotation = Quaternion.identity;
            rendererScale = textMesh.GetComponent<MeshRenderer>().bounds.size;
            textMesh.transform.eulerAngles = eulerAngles;
            localScale.y = textMesh.transform.worldToLocalMatrix.MultiplyVector(rendererScale).y * transformScale.y;

            return localScale;
        }
    }
}
