// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Collections
{

    public class ScrollingObjectCollection : BaseObjectCollection, IMixedRealityPointerHandler
    {
        public event Action<ScrollingObjectCollection, GameObject> OnTouchRelease;
        
        public enum VelocityType
        {
            FalloffPerFrame = 0,
            FalloffPerItem,
            NoVeloctySnapToItem,
            None
        }

        public enum ScrollDirectionType
        {
            UpAndDown = 0,
            LeftAndRight,
        }

        /// <summary>
        /// Automatically set up scroller at runtime 
        /// </summary>
        [SerializeField]
        [Tooltip("Automatically set up scroller at runtime")]
        private bool setUpAtRuntime = true;

        public bool SetUpAtRuntime
        {
            get => setUpAtRuntime;
            set => setUpAtRuntime = value;
        }

        /// <summary>
        /// Number of items visible in scroller 
        /// </summary>
        [SerializeField]
        [Tooltip("Number of items visible in scroller")]
        private int viewableArea = 4;

        public int ViewableArea
        {
            get => viewableArea;
            set => viewableArea = value;
        }

        /// <summary>
        /// The amount of movement the user's pointer can make before its considerd a drag gesture
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of movement the user's pointer can make before its considerd a drag gesture")]
        [Range(0.0f, 1.0f)]
        private float handDeltaMagThreshold;

        public float HandDeltaMagThreshold
        {
            get => handDeltaMagThreshold;
            set => handDeltaMagThreshold = value;
        }

        /// <summary>
        /// The amount of time the user's hand can intersect a controller item before its considerd a drag gesture
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of time the user's hand can intersect a controller item before its considerd a drag gesture")]
        [Range(0.0f, 2.0f)]
        private float dragTimeThreshold = 0.25f;

        public float DragTimeThreshold
        {
            get => DragTimeThreshold;
            set => DragTimeThreshold = value;
        }

        /// <summary>
        /// Snaps the items in the list to be entirely visible
        /// </summary>
        [SerializeField]
        [Tooltip("Snaps the items in the list to be entirely visible")]
        private bool snapListItems = false;

        public bool SnapListItems
        {
            get => snapListItems;
            set => snapListItems = value;
        }

        /// <summary>
        /// The direction in which content should scroll 
        /// </summary>
        [SerializeField]
        [Tooltip("The direction in which content should scroll")]
        private ScrollDirectionType scrollDirection;

        public ScrollDirectionType ScrollDirection
        {
            get => scrollDirection;
            set => scrollDirection = value;
        }

        /// <summary>
        /// Amount of velocity to be applied to scroller 
        /// </summary>
        [SerializeField]
        [Tooltip("Amount of velocity to be applied to scroller")]
        [Range(0.0f, 2.0f)]
        private float velocityMultiplier = 0.8f;

        public float VelocityMultiplier
        {
            get => velocityMultiplier;
            set => velocityMultiplier = value;
        }

        /// <summary>
        /// Amount of falloff applied to velocity 
        /// </summary>
        [SerializeField]
        [Tooltip("Amount of falloff applied to velocity")]
        //[Range(0.0f, 0.90f)]
        private float velocityFalloff = 0.90f;

        public float VelocityFalloff
        {
            get => velocityFalloff;
            set => velocityFalloff = value;
        }

        /// <summary>
        /// The desired type of velocity for the scroller 
        /// </summary>
        [SerializeField]
        [Tooltip("The desired type of velocity for the scroller")]
        private VelocityType typeOfVelocity;

        public VelocityType TypeOfVelocity
        {
            get => typeOfVelocity;
            set => typeOfVelocity = value;
        }


        /// <summary>
        /// Animation curve for pagination 
        /// </summary>
        [SerializeField]
        [Tooltip("Animation curve for pagination")]
        private AnimationCurve paginationCurve = new AnimationCurve(
                                                                    new Keyframe(0, 0),
                                                                    new Keyframe(1, 1));

        public AnimationCurve PaginationCurve
        {
            get => paginationCurve;
            set => paginationCurve = value;
        }

        private AnimationCurve falloffCurve = new AnimationCurve(
                                                                    new Keyframe(0f, 0f),
                                                                    new Keyframe(1f, 1f, 0.5f, 0.0f));

        /// <summary>
        /// Animation length 
        /// </summary>
        [SerializeField]
        [Tooltip("Animation length")]
        private float animationLength = 0.25f;

        public float AnimationLength
        {
            get => animationLength;
            set => animationLength = value;
        }


        private int rows = 3;

        /// <summary>
        /// Number of rows per column, column number is automatically determined
        /// </summary>
        public int Rows
        {
            get => rows;
            set => rows = value;
        }

        [Tooltip("Number of rows per column")]
        [SerializeField]
        private int columns = 3;

        /// <summary>
        /// Number of rows per column, column number is automatically determined
        /// </summary>
        public int Columns
        {
            get => columns;
            set => columns = value;
        }

        /// <summary>
        /// Multiplier to fix any mismatch in scale calculation of the occlusion objects 
        /// </summary>
        [SerializeField]
        [Tooltip("Multiplier to fix any mismatch in scale calculation of the occlusion objects")]
        private Vector3 occlusionScalePadding = new Vector3(0.0f, 0.0f, 0.001f);

        public Vector3 OcclusionScalePadding
        {
            get => occlusionScalePadding;
            set => occlusionScalePadding = value;
        }

        /// <summary>
        /// Multiplier to fix any mismatch in position calculation of the occlusion objects 
        /// </summary>
        [SerializeField]
        [Tooltip("Multiplier to fix any mismatch in position calculation of the occlusion objects")]
        private Vector3 occlusionPositionPadding = Vector3.zero;

        public Vector3 OcclusionPositionPadding
        {
            get => occlusionPositionPadding;
            set => occlusionPositionPadding = value;
        }

        [Tooltip("Width of cell per object")]
        [SerializeField]
        private float cellWidth = 0.5f;

        /// <summary>
        /// Width of the cell per object in the collection.
        /// </summary>
        public float CellWidth
        {
            get => cellWidth;
            set => cellWidth = value;
        }

        [Tooltip("Height of cell per object")]
        [SerializeField]
        private float cellHeight = 0.5f;

        /// <summary>
        /// Height of the cell per object in the collection.
        /// </summary>
        public float CellHeight
        {
            get => cellHeight;
            set => cellHeight = value;
        }


        /// <summary>
        /// Event that is fired on the target object when the scrollingCollection deems event as a press.
        /// </summary>
        [Tooltip("Event that is fired on the target object when the scrollingCollection deems event as a press.")]
        //public UnityEvent<ScrollingObjectCollection, GameObject> PressEvent;
        public UnityEvent PressEvent;

        /// <summary>
        /// First item in the viewable area. 
        /// </summary>
        private int firstItemInview = 0;

        public int FirstItemInView
        {
            get
            {
                if (scrollDirection == ScrollDirectionType.UpAndDown)
                {
                    return (int)(workingScrollerPos.y / cellHeight) - ((int)(workingScrollerPos.y / cellHeight) % columns);
                }
                else
                {
                    return (int)(workingScrollerPos.x / cellHeight) - ((int)(workingScrollerPos.x / cellHeight) % columns);
                }
            }

            set => firstItemInview = value - (value % columns);
        }

        private Vector2 HalfCell => new Vector2(CellWidth * 0.5f, CellHeight * 0.5f);

        private float maxY => (StepMultiplier(NodeList.Count, columns) + ModCheck(NodeList.Count, columns) - viewableArea) * CellHeight;
        private float minY => 0.0f; //here only for readability

        private float maxX => (StepMultiplier(NodeList.Count, rows) + ModCheck(NodeList.Count, rows) - viewableArea) * CellWidth;
        private float minX => 0.0f; //here only for readability

        //number of items per frame the scroller would travel @ current velocity
        private float velPerCell => (scrollDirection == ScrollDirectionType.UpAndDown) ? cellHeight / (avgVelocity * velocityFalloff) : cellWidth / (avgVelocity * velocityFalloff);

        //item index for items that should be visible
        private int numItemsPrevView
        {
            get
            {
                if (scrollDirection == ScrollDirectionType.UpAndDown)
                {
                    return (int)Mathf.Ceil((scrollContainer.transform.localPosition.y + occlusionPositionPadding.y) / cellHeight) * columns;
                }
                else
                {
                    return (int)Mathf.Ceil((scrollContainer.transform.localPosition.x + occlusionPositionPadding.x) / cellWidth) * columns;
                }
            }
        }

        //Take the previous view and then subtract the column remainde and we add the viewable area as a multiplier (minus 1 since the index is zero based).
        private int numItemsPostView
        {
            get
            {
                if (scrollDirection == ScrollDirectionType.UpAndDown)
                {
                    return ((int)Mathf.Floor((scrollContainer.transform.localPosition.y + occlusionPositionPadding.y) / cellHeight) * columns) + (viewableArea * columns) - 1;
                }
                else
                {
                    return ((int)Mathf.Floor((scrollContainer.transform.localPosition.x + occlusionPositionPadding.x) / cellWidth) * columns) + (viewableArea * columns) - 1;
                }

            }
        }

        private bool isInit = false; // first run flag for LateUpdate loop

        [SerializeField]
        [HideInInspector]
        private GameObject scrollContainer;
        [SerializeField]
        [HideInInspector]
        private GameObject clippingObject;
        [SerializeField]
        [HideInInspector]
        private ClippingBox clippingBox;
        [SerializeField]
        [HideInInspector]
        private Bounds clippingBounds;

        private bool isEngaged = false; // Tracks whether an item in the list is being interacted with
        private bool isDragging = false; // Tracks whether a movement action resulted in dragging the list  

        private float thresholdOffset;
        private Vector3 initialScrollerPos;
        private Vector3 workingScrollerPos;

        private GameObject focusedObject;

        #region event variables

        private Transform currentPointerTransform; // The currently tracked finger transform

        private MixedRealityPose currentPose;
        private Vector3 currentPointerPos
        {
            get
            {
                if (currentPointer == null) { return Vector3.zero; }

                if (currentPointer.Controller.Visualizer is IMixedRealityHandVisualizer)
                {
                    //this is a hand pointer
                    return currentPointerTransform.position;
                }
                else
                {
                    //Ray controllerRay;
                    //currentPointer.TryGetPointingRay(out controllerRay);
                    return GetTrackedPoint(currentPointerTransform.position, hitPoint, currentPointer.Position + currentPointer.Rotation * Vector3.forward, RotationConstraintType.YAxisOnly);
                }

            }
        }

        private IMixedRealityPointer currentPointer;
        private bool isTouched = false;  // we need to know if the pointer was a touch so we can do the threshold test

        #endregion

        #region drag position calculation variables

        private Vector3 handDelta;
        private float absAxisHandDelta;
        private Vector3 initialHandPos;
        private Vector3 finalOffset;
        private Vector3 thresholdPoint;

        private float initialPressTime;

        private Vector3 lastHandPos;
        private bool animatingToPosition = false;
        private IEnumerator AnimateScroller;

        #endregion

        private float scrollVelocity = 0.0f;
        private float avgVelocity = 0.0f;
        private float velocityFilterWeight = 0.9f;

        #region ObjectCollection methods

        public override void UpdateCollection()
        {
            //Generate our scroll specific objects
            if (scrollContainer == null)
            {
                scrollContainer = new GameObject();
                scrollContainer.name = "Container";
                scrollContainer.transform.parent = transform;
                scrollContainer.transform.localPosition = Vector3.zero;
                scrollContainer.transform.localRotation = Quaternion.identity;
            }

            if (clippingObject == null)
            {
                clippingObject = new GameObject();
                clippingObject.name = "Clipping Bounds";
                clippingObject.transform.parent = transform;
                clippingObject.transform.localPosition = Vector3.zero;
            }

            if (clippingBox == null)
            {
                clippingBox = clippingObject.AddComponent<ClippingBox>();
                clippingBox.ClippingSide = ClippingPrimitive.Side.Outside;
            }

            //move any objects to the scroll container
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);

                if (child != scrollContainer.transform && child != clippingObject.transform)
                {
                    child.parent = scrollContainer.transform;
                }
            }

            // Check for empty nodes and remove them
            List<ObjectCollectionNode> emptyNodes = new List<ObjectCollectionNode>();

            for (int i = 0; i < NodeList.Count; i++)
            {
                if (NodeList[i].Transform == null
                    || (IgnoreInactiveTransforms && !NodeList[i].Transform.gameObject.activeSelf)
                    || NodeList[i].Transform.parent == null
                    || !(NodeList[i].Transform.parent.gameObject == scrollContainer))
                {
                    emptyNodes.Add(NodeList[i]);
                }
            }

            // Now delete the empty nodes
            for (int i = 0; i < emptyNodes.Count; i++)
            {
                NodeList.Remove(emptyNodes[i]);
            }

            emptyNodes.Clear();

            // Check when children change and adjust
            for (int i = 0; i < scrollContainer.transform.childCount; i++)
            {
                Transform child = scrollContainer.transform.GetChild(i);

                if (!ContainsNode(child) && (child.gameObject.activeSelf || !IgnoreInactiveTransforms))
                {
                    NodeList.Add(new ObjectCollectionNode { Name = child.name, Transform = child });
                }
            }

            switch (SortType)
            {
                case CollationOrder.ChildOrder:
                    NodeList.Sort((c1, c2) => (c1.Transform.GetSiblingIndex().CompareTo(c2.Transform.GetSiblingIndex())));
                    break;

                case CollationOrder.Alphabetical:
                    NodeList.Sort((c1, c2) => (string.CompareOrdinal(c1.Name, c2.Name)));
                    break;

                case CollationOrder.AlphabeticalReversed:
                    NodeList.Sort((c1, c2) => (string.CompareOrdinal(c1.Name, c2.Name)));
                    NodeList.Reverse();
                    break;

                case CollationOrder.ChildOrderReversed:
                    NodeList.Sort((c1, c2) => (c1.Transform.GetSiblingIndex().CompareTo(c2.Transform.GetSiblingIndex())));
                    NodeList.Reverse();
                    break;
            }

            LayoutChildren();

            OnCollectionUpdated?.Invoke(this);
        }

        protected override void LayoutChildren()
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                ObjectCollectionNode node = NodeList[i];

                Vector3 newPos;

                if (scrollDirection == ScrollDirectionType.UpAndDown)
                {
                    newPos.x = (ModCheck(i, columns) != 0) ? (ModCheck(i, columns) * cellWidth) + HalfCell.x : HalfCell.x;
                    newPos.y = ((StepMultiplier(i, columns) * CellHeight) + HalfCell.y) * -1;
                    newPos.z = 0.0f;

                }
                else //left or right
                {
                    newPos.x = (StepMultiplier(i, columns) * CellWidth) + HalfCell.x;
                    newPos.y = ((ModCheck(i, columns) != 0) ? (ModCheck(i, columns) * cellHeight) + HalfCell.y : HalfCell.y) * -1;
                    newPos.z = 0.0f;
                }
                node.Transform.localPosition = newPos;
                NodeList[i] = node;
            }

            ResolveLayout();
            AddItemsToClippingObject();
            HideItems();
        }

        #endregion

        private void ResolveLayout()
        {
            float tempWorkingPos;
            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                tempWorkingPos = (StepMultiplier(FirstItemInView, columns) * cellHeight);

                workingScrollerPos.y = tempWorkingPos;
                initialScrollerPos.y = tempWorkingPos;
            }
            else
            {
                tempWorkingPos = (StepMultiplier(FirstItemInView, rows) * cellWidth);

                workingScrollerPos.x = tempWorkingPos;
                initialScrollerPos.x = tempWorkingPos;
            }

            //create the offset for our thesholdCalculation -- grab the first item in the list
            TryGetObjectAlignedBoundsSize(NodeList[0].Transform, RotationConstraintType.ZAxisOnly, out thresholdOffset);

            //Use the first element for collection bounds -> for occluder positioning
            //temporarily zero out the rotation so we can get an accurate bounds
            Quaternion origRot = NodeList[0].Transform.rotation;
            NodeList[0].Transform.rotation = Quaternion.identity;

            clippingBounds.center = NodeList[0].Transform.GetColliderBounds().center;

            // lets check whether the collection cell dimensions are a better fit
            // this prevents negative offset from ruining the scroll effect
            Vector3 tempClippingSize = NodeList[0].Transform.GetColliderBounds().size;

            //put the rotation back
            NodeList[0].Transform.rotation = origRot;

            tempClippingSize.x = (tempClippingSize.x > cellWidth) ? tempClippingSize.x : cellWidth;
            tempClippingSize.y = (tempClippingSize.x > cellHeight) ? tempClippingSize.x : cellHeight;

            clippingBounds.size = tempClippingSize;


            Vector3 viewableCenter = new Vector3();

            //Adjust scale and position of our occluder objects
            switch (scrollDirection)
            {
                case ScrollDirectionType.UpAndDown:
                default:

                    //apply the viewable area and column/row multiplier
                    //use a dummy bounds of one to get the local scale to match;
                    clippingBounds.size = new Vector3((clippingBounds.size.x * columns), (clippingBounds.size.y * viewableArea), clippingBounds.size.z);
                    Debug.Log(clippingBounds.size);
                    clippingBox.transform.localScale = CalculateScale(new Bounds(Vector3.zero, Vector3.one), clippingBounds, occlusionScalePadding);

                    //adjust where the center of the clipping box is
                    viewableCenter.x = (clippingBox.transform.localScale.x * 0.5f) - (occlusionScalePadding.x * 0.5f) + occlusionPositionPadding.x;
                    viewableCenter.y = (((clippingBox.transform.localScale.y * 0.5f) - (occlusionScalePadding.y * 0.5f)) * -1) + occlusionPositionPadding.y;

                    viewableCenter.z = occlusionPositionPadding.z;

                    break;

                case ScrollDirectionType.LeftAndRight:

                    //Same as above for L <-> R
                    clippingBounds.size = new Vector3((clippingBounds.size.x * viewableArea) + HalfCell.x, (clippingBounds.size.y * columns) - HalfCell.y, clippingBounds.size.z);
                    clippingBox.transform.localScale = CalculateScale(new Bounds(Vector3.zero, Vector3.one), clippingBounds, OcclusionScalePadding);

                    //Same as above for L <-> R
                    viewableCenter.x = (((clippingBox.transform.localScale.x * 0.5f) - (occlusionScalePadding.x * 0.5f)) * -1) + occlusionPositionPadding.x;

                    viewableCenter.y = (clippingBox.transform.localScale.y * 0.5f) - (occlusionScalePadding.y * 0.5f) + occlusionPositionPadding.y;
                    viewableCenter.z = occlusionPositionPadding.z;

                    break;
            }

            clippingBox.transform.localPosition = viewableCenter;

            //mark the initilization flag for LateUpdate()        
            isInit = true;
        }

        #region Monobehavior Implementation

        private void Start()
        {
            if (setUpAtRuntime)
            {
                UpdateCollection();
            }
        }

        private void Update()
        {
            //Debug the position of the scroller
            //workingScrollerPos.y = Mathf.Lerp(scrollContainer.transform.localPosition.y, maxY * debugScrollPos, 0.5f);

            if (scrollContainer == null) { return; } //early bail, the component isn't set up properly

            if (isEngaged && currentPointer != null)
            {
                handDelta = initialHandPos - currentPointerPos;
                Debug.DrawLine(initialHandPos, currentPointerPos, Color.red);

                //Lets see if this is gonna be a click or a drag
                //Check the var's state to prevent resetting calculation
                if (!isDragging)
                {
                    absAxisHandDelta = (scrollDirection == ScrollDirectionType.UpAndDown) ? Mathf.Abs(handDelta.y) : Mathf.Abs(handDelta.x);

                    // Check for hand delta (on a single axis) or a long time intersecting the list
                    if (absAxisHandDelta > (handDeltaMagThreshold * 0.1f) || TimeTest(initialPressTime, Time.time, dragTimeThreshold))
                    {
                        isDragging = true;

                        //reset initialHandPos to prevent the scroller from jumping
                        initialHandPos = currentPointerPos;
                    }
                }

                //Use child delta to determine offset
                Vector3 finalOffset = transform.forward * thresholdOffset;
                Vector3 thresholdPoint = transform.position - finalOffset;

                if (isTouched && TouchPassedThreshold(-transform.forward, thresholdPoint, currentPointerPos))
                {
                    //We're on the other side of the original touch position. This is a release.

                    Debug.DrawLine(currentPointerPos, thresholdPoint, Color.white, .5f);

                    if (!isDragging)
                    {
                        //Its a release
                        //Fire the OnRelease even with the previously selected object
                        Collider[] c = focusedObject.GetComponentsInChildren<Collider>();
                        bool isColliderActive = false;

                        foreach (Collider col in c)
                        {
                            if (col.enabled)
                            {
                                isColliderActive = true;
                                break;
                            }
                        }

                        if (isColliderActive)
                        {
                            //Make a method call to the child object
                            focusedObject.GetComponentInChildren<IScrollingChildObject>().OnScrollPressRelease();

                            //Fire the UnityEvent
                            //PressEvent?.Invoke(this, focusedObject);
                        }
                    }
                    else
                    {
                        //Its a drag release
                        initialScrollerPos = workingScrollerPos;
                    }

                    //Release the pointer
                    if (currentPointer.GetType() != typeof(PokePointer))
                    {
                        currentPointer.IsFocusLocked = false;
                        currentPointer = null;
                    }

                    //Clear our states
                    isTouched = false;
                    isEngaged = false;
                    isDragging = false;
                }
                else if (isDragging)
                {
                    //We're still engaged and dragging
                    Debug.DrawLine(initialHandPos, currentPointerPos, Color.green);
                    DebugUtilities.DrawPoint(currentPointerPos, Color.red, .2f);


                    workingScrollerPos = initialScrollerPos - handDelta;

                    workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);
                    workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                    CalculateDragMove(workingScrollerPos);

                    Debug.DrawLine(initialScrollerPos, workingScrollerPos, Color.magenta);

                    //do our velocity stuff
                    scrollVelocity = (scrollDirection == ScrollDirectionType.UpAndDown)
                                     ? (currentPointerPos.y - lastHandPos.y) / Time.deltaTime * (velocityMultiplier * 0.01f)
                                     : (currentPointerPos.x - lastHandPos.x) / Time.deltaTime * (velocityMultiplier * 0.01f);
                    avgVelocity = (avgVelocity * (1.0f - velocityFilterWeight)) + (scrollVelocity * velocityFilterWeight);

                    lastHandPos = currentPointerPos;
                }
            }
            else if (!animatingToPosition)//Prevent the Animation coroutine from being overridden
            {
                //We're not engaged, so handle any not touching behavior

                switch (typeOfVelocity)
                {
                    case VelocityType.NoVeloctySnapToItem:
                        avgVelocity = 0.0f;
                        //Round to the nearest list item
                        if (scrollDirection == ScrollDirectionType.UpAndDown)
                        {
                            workingScrollerPos.y = StepMultiplier((int)Mathf.Round(scrollContainer.transform.localPosition.y / cellHeight), columns) * CellHeight;
                        }
                        else
                        {
                            workingScrollerPos.x = StepMultiplier((int)Mathf.Round(scrollContainer.transform.localPosition.x / cellWidth), columns) * cellWidth;
                        }
                        break;

                    case VelocityType.None:
                        avgVelocity = 0.0f;
                        //apply no velocity
                        break;

                    case VelocityType.FalloffPerItem:
                        StopAllCoroutines();
                        AnimateScroller = AnimateTo(scrollContainer.transform.localPosition, PreCalculateRestAfterVelocity(scrollContainer.transform.localPosition), falloffCurve, velPerCell * Time.deltaTime);
                        StartCoroutine(AnimateScroller);
                        avgVelocity = 0.0f; // clear the velocity value now that we've calulated our new position
                        return; // bail out of the update loop so animatingToPosition will evaluate to true now that we spun up a animation coroutine

                    case VelocityType.FalloffPerFrame:
                    default:
                        if (Mathf.Abs(avgVelocity) > Mathf.Epsilon)
                        {
                            if (scrollDirection == ScrollDirectionType.UpAndDown)
                            {
                                avgVelocity *= velocityFalloff;
                                workingScrollerPos.y = initialScrollerPos.y + avgVelocity;
                            }
                            else
                            {
                                avgVelocity *= velocityFalloff;
                                workingScrollerPos.x = initialScrollerPos.x + avgVelocity;
                            }
                        }
                        break;
                }

                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                initialScrollerPos = workingScrollerPos;

                CalculateDragMove(workingScrollerPos);
            }
        }

        private void LateUpdate()
        {

            if (isInit)
            {
                //check and see if any children have Near Touch components.
                //We need to remove them since events won't bubble up to ScrollingObject when children handle them

                NearInteractionTouchable[] touchables = GetComponentsInChildren<NearInteractionTouchable>();
                foreach (NearInteractionTouchable touchable in touchables)
                {
                    //    touchable.enabled = false;
                }

                //This has to be here otherwise Text mesh pro objects could be hidden
                //before initialization and the Clipping box will miss special characters
                AddItemsToClippingObject();
                isInit = false;
            }

            //Hide the items not in view
            HideItems();
        }

        private IEnumerator AnimateTo(Vector3 initialPos, Vector3 finalPos, AnimationCurve curve = null, float? time = null)
        {
            animatingToPosition = true;

            if (curve == null)
            {
                curve = paginationCurve;
            }

            if (time == null)
            {
                time = animationLength;
            }

            float counter = 0.0f;
            while (counter <= time)
            {
                scrollContainer.transform.localPosition = Vector3.Lerp(initialPos, finalPos, curve.Evaluate(counter / (float)time));

                //update our values so they stick
                initialScrollerPos = scrollContainer.transform.localPosition;
                workingScrollerPos = scrollContainer.transform.localPosition;

                counter += Time.deltaTime;
                yield return null;
            }

            animatingToPosition = false;
        }

        #endregion

        #region private methods

        private void AddItemsToClippingObject()
        {
            //Register all of the renderers to be clipped by the clippingBox
            foreach (ObjectCollectionNode node in NodeList)
            {
                Renderer[] childRends = node.Transform.gameObject.transform.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < childRends.Length; i++)
                {
                    clippingBox.AddRenderer(childRends[i]);
                }
            }
        }

        private float PreCalculateRestAfterVelocity(float pos)
        {
            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                return Mathf.Pow(pos * (1 - (avgVelocity * velocityFalloff) / Time.deltaTime), velPerCell);
            }
            else
            {
                return Mathf.Pow(pos * (1 - (avgVelocity * velocityFalloff) / Time.deltaTime), velPerCell);
            }
        }

        private Vector3 PreCalculateRestAfterVelocity(Vector3 pos)
        {
            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                pos.y = Mathf.Pow(pos.y * (1 - (avgVelocity * velocityFalloff) / Time.deltaTime), velPerCell);
            }
            else
            {
                pos.x = Mathf.Pow(pos.x * (1 - (avgVelocity * velocityFalloff) / Time.deltaTime), velPerCell);
            }
            return pos;
        }

        private static int ModCheck(int itemIndex, int divisor)
        {
            return itemIndex % divisor;
        }

        private static int StepMultiplier(int itemIndex, int divisor)
        {
            return (divisor != 0) ? itemIndex / divisor : 1; //prevent divide by 0
        }

        private void HideItems()
        {
            if (NodeList.Count == 0) { return; }

            DebugUtilities.DrawPoint(NodeList[numItemsPrevView].Transform.position, Color.cyan, 0.1f);
            if (numItemsPostView > NodeList.Count - 1)
            {
                DebugUtilities.DrawPoint(NodeList[NodeList.Count - 1].Transform.position, Color.cyan, 0.1f);
            }
            else
            {
                DebugUtilities.DrawPoint(NodeList[numItemsPostView].Transform.position, Color.cyan, 0.1f);
            }



            for (int i = 0; i < NodeList.Count; i++)
            {
                //hide the items that have no chance of being seen
                if (i < numItemsPrevView - columns || i > numItemsPostView + columns)
                {
                    NodeList[i].Transform.gameObject.SetActive(false);
                }
                else
                {
                    //Disable colliders on items that will be scrolling in and out of view
                    //todo eventually we should put the Colliders in the collection node to cut down on redundant GetComponent calls
                    if (i < numItemsPrevView || i > ((numItemsPostView > NodeList.Count) ? NodeList.Count : numItemsPostView))
                    {
                        Collider[] cols = NodeList[i].Transform.GetComponentsInChildren<Collider>();
                        foreach (Collider c in cols)
                        {
                            c.enabled = false;
                        }
                    }
                    else
                    {
                        Collider[] cols = NodeList[i].Transform.GetComponentsInChildren<Collider>();
                        foreach (Collider c in cols)
                        {
                            c.enabled = true;
                        }
                    }

                    NodeList[i].Transform.gameObject.SetActive(true);
                }
            }
        }


        private void HandleScrollRelease()
        {

        }

        private void CalculateDragMove(Vector3 workingPos)
        {
            Vector3 newScrollPos;

            switch (scrollDirection)
            {
                case ScrollDirectionType.UpAndDown:
                default:

                    newScrollPos = new Vector3(scrollContainer.transform.localPosition.x, workingPos.y, scrollContainer.transform.localPosition.z);
                    break;

                case ScrollDirectionType.LeftAndRight:

                    newScrollPos = new Vector3(workingPos.x, scrollContainer.transform.localPosition.y, scrollContainer.transform.localPosition.z);

                    break;
            }
            scrollContainer.transform.localPosition = newScrollPos;
        }

        private bool TryGetPokePointer(IMixedRealityPointer[] pointers, out IMixedRealityPointer pokePointer)
        {
            pokePointer = null;

            for (int i = 0; i < pointers.Length; i++)
            {
                if (pointers[i].GetType() == typeof(PokePointer))
                {
                    pokePointer = pointers[i];
                }
            }

            return (pokePointer != null) ? true : false;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Moves scroller by a multiplier of <see cref="ViewableArea"/>
        /// </summary>
        /// <param name="numOfPages">number of <see cref="ViewableArea"/> to move scroller by</param>
        /// <param name="animateToPage">if <see cref="true"/>, scroller will animate to new position</param>
        public void PageBy(int numOfPages, bool animateToPage = true)
        {
            StopAllCoroutines();

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = (FirstItemInView + (numOfPages * viewableArea)) * cellHeight;
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = (FirstItemInView + (numOfPages * viewableArea)) * cellHeight;
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPage)
            {
                AnimateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength);
                StartCoroutine(AnimateScroller);
            }

        }

        /// <summary>
        /// Moves scroller a relative number of items
        /// </summary>
        /// <param name="numberOfItemsToMove">number of items to move by</param>
        /// <param name="animateToPosition">if <see cref="true"/>, scroller will animate to new position</param>
        public void MoveByItems(int numberOfItemsToMove, bool animateToPosition = true)
        {
            StopAllCoroutines();

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = FirstItemInView + (StepMultiplier(numberOfItemsToMove, columns) * cellHeight);
                //clamp the working pos since we already have calculated it
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = FirstItemInView + (StepMultiplier(numberOfItemsToMove, columns) * cellWidth);
                //clamp the working pos since we already have calculated it
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }


            if (animateToPosition)
            {
                AnimateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength);
                StartCoroutine(AnimateScroller);
            }
        }

        /// <summary>
        /// Moves scroller a relative number of rows
        /// </summary>
        /// <param name="numberOfRowsToMove">number of rows to move by</param>
        /// <param name="animateToPosition">if <see cref="true"/>, scroller will animate to new position</param>
        public void MoveByRows(int numberOfRowsToMove, bool animateToPosition = true)
        {
            StopAllCoroutines();

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = (FirstItemInView + (numberOfRowsToMove * columns)) * cellHeight;
                //clamp the working pos since we already have calculated it
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = (FirstItemInView + (numberOfRowsToMove * columns)) * cellWidth;
                //clamp the working pos since we already have calculated it
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPosition)
            {
                AnimateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength);
                StartCoroutine(AnimateScroller);
            }
        }


        /// <summary>
        /// Moves scroller to an absolute position where <param name"indexOfItem"/> is in the first column of the viewable area
        /// </summary>
        /// <param name="indexOfItem">item to move to, will be first (or closest to in respect to scroll maxiumum) in viewable area</param>
        /// <param name="animateToPosition">if <see cref="true"/>, scroller will animate to new position</param>
        public void MoveTo(int indexOfItem, bool animateToPosition = true)
        {
            StopAllCoroutines();

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = StepMultiplier(indexOfItem, columns) * cellHeight;
                //clamp the working pos since we already have calculated it
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = StepMultiplier(indexOfItem, columns) * cellWidth;
                //clamp the working pos since we already have calculated it
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPosition)
            {
                AnimateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, 0.5f);
                StartCoroutine(AnimateScroller);
            }
        }

        #endregion

        #region static methods

        public static Vector3 GetTrackedPoint(Vector3 origin, Vector3 hitPoint, Vector3 newDir, RotationConstraintType axisConstraint)
        {
            Vector3 hitDir = hitPoint - origin;
            float mag = Vector3.Dot(hitDir, newDir);

            return origin + (newDir * mag);
        }

        /// <summary>
        /// Simple time threshold check
        /// </summary>
        /// <param name="initTime">Initial time</param>
        /// <param name="currTime">Current time</param>
        /// <param name="pressMargin">Time threshold</param>
        /// <returns>true if amount of time surpasses threshold</returns>
        public static bool TimeTest(float initTime, float currTime, float pressMargin)
        {
            if (currTime - initTime > pressMargin)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates mow much scale is required for objBounds to match otherbounds.
        /// </summary>
        /// <param name="objBounds">Object representation to be scaled</param>
        /// <param name="otherBounds">Object representation to be scaled to</param>
        /// <param name="padding">padding multitplied into otherbounds</param>
        /// <returns>scale represented as a Vector3</returns>
        public static Vector3 CalculateScale(Bounds objBounds, Bounds otherBounds, Vector3 padding = default)
        {
            //Optional padding (especially z)
            Vector3 szA = otherBounds.size + new Vector3(otherBounds.size.x * padding.x, otherBounds.size.y * padding.y, otherBounds.size.z * padding.z);
            Vector3 szB = objBounds.size;
            return new Vector3(szA.x / szB.x, szA.y / szB.y, szA.z / szB.z);
        }

        /// <summary>
        /// Calculates which side a point in space is on
        /// </summary>
        /// <param name="initialDirection">The plane normal direction. </param>
        /// <param name="initialPosition">The point representing the plane's origin</param>
        /// <param name="pointToCompare">The point compared to the normal and origin</param>
        /// <returns>true when the compared point is on the other side of the plane</returns>
        public static bool TouchPassedThreshold(Vector3 initialDirection, Vector3 initialPosition, Vector3 pointToCompare)
        {
            Vector3 delta = pointToCompare - initialPosition;
            float dot = Vector3.Dot(delta, initialDirection);

            return (dot > 0) ? true : false;
        }

        /// <summary>
        /// Finds the object-aligned size of a Transform 
        /// </summary>
        /// <param name="obj"><see cref="Transform"/> representing the object to get offset from</param>
        /// <returns></returns>
        public static bool TryGetObjectAlignedBoundsSize(Transform obj, RotationConstraintType axis, out float offset)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            Vector3 size = Vector3.zero;
            offset = 0.0f;

            //store and clear the original rotation
            Quaternion origRot = obj.rotation;
            obj.rotation = Quaternion.identity;

            if (rend != null)
            {
                size = rend.bounds.size;
            }
            else
            {
                BoxCollider bC = obj.GetComponent<Collider>() as BoxCollider;

                if (bC == null)
                {
                    return false;
                }

                size = bC.bounds.size;
            }
            //reapply our rotation
            obj.rotation = origRot;

            switch (axis)
            {
                case RotationConstraintType.None:
                    offset = 0.0f;
                    break;
                case RotationConstraintType.XAxisOnly:
                    offset = size.x;
                    break;
                case RotationConstraintType.YAxisOnly:
                    offset = size.y;
                    break;
                case RotationConstraintType.ZAxisOnly:
                    offset = size.z;
                    break;
            }

            return true;
        }

        #endregion

        #region IMixedRealityPointerHandler Implementation

        private Vector3 hitPoint;

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            //Do a check to prevent a new touch event from overriding our current touch values
            if (!isEngaged && !animatingToPosition)
            {
                if (eventData.Pointer.Controller.IsPositionAvailable)
                {
                    currentPointer = eventData.Pointer;

                    IMixedRealityControllerVisualizer controllerViz = eventData.Pointer.Controller.Visualizer as IMixedRealityControllerVisualizer;

                    currentPointerTransform = controllerViz.GameObjectProxy.transform;

                    currentPointer.IsFocusLocked = true;

                    hitPoint = currentPointer.Result.Details.Point;

                    focusedObject = eventData.selectedObject;

                    scrollVelocity = 0.0f;

                    initialHandPos = currentPointerPos;
                    initialPressTime = Time.time;
                    initialScrollerPos = scrollContainer.transform.localPosition;

                    isTouched = false;
                    isEngaged = true;
                    isDragging = false;
                }
                else
                {
                    //not sure what to do with this pointer
                    Debug.Log(gameObject.name + "'s " + name + " intercepted a pointer, " + eventData.Pointer.PointerName + ", but doesn't know what to do with it.");
                }
            }
        }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {

            if (!isTouched && isEngaged && !animatingToPosition)
            {
                //Its a drag release
                initialScrollerPos = workingScrollerPos;

                //Release the pointer
                Debug.Log("Current FOCUS: " + currentPointer.IsFocusLocked);
                currentPointer.IsFocusLocked = false;
                currentPointer = null;

                //Clear our states
                isTouched = false;
                isEngaged = false;
                isDragging = false;

            }

            Debug.Log("Pointer up!");

        }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            //Do a check to prevent a new touch event from overriding our current touch values
            if (!animatingToPosition)
            {
                if (eventData.Pointer.Controller.IsPositionAvailable)
                {
                    // need to figure out how to handle this
                }
                else
                {
                    //not sure what to do with this pointer
                    Debug.Log(gameObject.name + "'s " + name + " intercepted a pointer, " + eventData.Pointer.PointerName + ", but doesn't know what to do with it.");
                }
            }
        }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            //we ignore this event and calculate click in the Update() loop;
        }

        #endregion

        #region IMixedRealityTouchHandler Implementation
        /*
        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            if (TryGetPokePointer(eventData.InputSource.Pointers, out currentPointer))
            {
                focusedObject = currentPointer.Result.CurrentPointerTarget;

                if (!isTouched && !isEngaged && !animatingToPosition)
                {

                    IMixedRealityHandVisualizer currentHand = eventData.Controller.Visualizer as IMixedRealityHandVisualizer;

                    HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, eventData.Handedness, out MixedRealityPose currentPose);

                    //currentHand.TryGetJointTransform(TrackedHandJoint.IndexTip, out currentPointerTransform);

                    scrollVelocity = 0.0f;

                    initialHandPos = Vector3.zero;
                    initialPressTime = Time.time;
                    initialScrollerPos = scrollContainer.transform.localPosition;

                    isTouched = true;
                    isEngaged = true;
                    isDragging = false;

                    Debug.Log(focusedObject);

                    //We don't know if this is a scroll or not so we're going to pass the event along to the child
                    focusedObject.GetComponentInChildren<IScrollingChildObject>().OnTouchStarted(eventData);
                }
            }
        }
        
        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            //Might be able to get away with using the touch started pointer reference but lets try this first
            IMixedRealityPointer p;
            if (TryGetPokePointer(eventData.InputSource.Pointers, out p))
            {
                if (!isDragging & p == currentPointer)
                {
                    //We don't know if this is a scroll or not so we're going to pass the event along to the child
                    currentPointer.Result.CurrentPointerTarget.GetComponentInChildren<IScrollingChildObject>().OnTouchUpdated(eventData);
                }
            }
        }


        void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            //
        }

        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData) { /*This is determined in the Update loop*/
    /*}
    */
        #endregion

    }
}
