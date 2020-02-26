// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A set of child objects organized in a series of Rows/Columns that can scroll in either the X or Y direction.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/ScrollingObjectCollection")]
    public class ScrollingObjectCollection : BaseObjectCollection, IMixedRealityPointerHandler, IMixedRealityTouchHandler, IMixedRealitySourceStateHandler, IMixedRealityInputHandler
    {
        /// <summary>
        /// How velocity is applied to a <see cref="ScrollingObjectCollection"/> when a scroll is released.
        /// </summary>
        public enum VelocityType
        {
            FalloffPerFrame = 0,
            FalloffPerItem,
            NoVelocitySnapToItem,
            None
        }

        /// <summary>
        /// The direction in which a <see cref="ScrollingObjectCollection"/> can scroll.
        /// </summary>
        public enum ScrollDirectionType
        {
            UpAndDown = 0,
            LeftAndRight,
        }

        [Experimental]
        [SerializeField]
        [Tooltip("Enables/disables scrolling with near/far interaction.")]
        private bool canScroll = true;

        /// <summary>
        /// Enables/disables scrolling with near/far interaction.
        /// </summary>
        /// <remarks>Helpful for controls where you may want pagination or list movement without freeform scrolling.</remarks>
        public bool CanScroll
        {
            get { return canScroll; }
            set { canScroll = value; }
        }

        [SerializeField]
        [Tooltip("Automatically set up scroller at runtime.")]
        private bool setUpAtRuntime = true;

        /// <summary>
        /// Automatically set up scroller at runtime.
        /// </summary>
        public bool SetUpAtRuntime
        {
            get { return setUpAtRuntime; }
            set { setUpAtRuntime = value; }
        }

        [SerializeField]
        [Tooltip("Number of lines visible in scroller. Orthogonal to tiers.")]
        private int viewableArea = 4;

        /// <summary>
        /// Number of lines visible in scroller. Orthogonal to <see cref="tiers"/>.
        /// </summary>
        public int ViewableArea
        {
            get { return (viewableArea > 0) ? viewableArea : 1; }
            set { viewableArea = value; }
        }

        [SerializeField]
        [Tooltip("The distance the user's pointer can make before its considered a drag.")]
        [Range(0.0f, 2.0f)]
        private float handDeltaMagThreshold = 0.4f;

        /// <summary>
        /// The distance the user's pointer can make before its considered a drag.
        /// </summary>
        public float HandDeltaMagThreshold
        {
            get { return handDeltaMagThreshold; }
            set { handDeltaMagThreshold = value; }
        }

        [SerializeField]
        [Tooltip("Seconds the user's pointer can intersect a controller item before it is considered a drag.")]
        [Range(0.0f, 2.0f)]
        private float dragTimeThreshold = 0.75f;

        /// <summary>
        /// Seconds the user's pointer can intersect a controller item before it is considered a drag.
        /// </summary>
        public float DragTimeThreshold
        {
            get { return dragTimeThreshold; }
            set { dragTimeThreshold = value; }
        }

        [SerializeField]
        [Tooltip("Determines whether a near scroll gesture is released when the engaged fingertip is dragged outside of the viewable area.")]
        private bool useNearScrollBoundary = false;

        /// <summary>
        /// Determines whether a near scroll gesture is released when the engaged fingertip is dragged outside of the viewable area.
        /// </summary>
        public bool UseNearScrollBoundary
        {
            get { return useNearScrollBoundary; }
            set { useNearScrollBoundary = value; }
        }

        [SerializeField]
        [Tooltip("The direction in which content should scroll.")]
        private ScrollDirectionType scrollDirection;

        /// <summary>
        /// The direction in which content should scroll.
        /// </summary>
        public ScrollDirectionType ScrollDirection
        {
            get { return scrollDirection; }
            set { scrollDirection = value; }
        }

        [SerializeField]
        [Tooltip("Toggles whether the scrollingObjectCollection will use the Camera OnPreRender event to hide items in the list.")]
        private bool useOnPreRender;

        /// <summary>
        /// Toggles whether the ScrollingObjectCollection" will use the Camera OnPreRender
        /// event to hide items in the list. The fallback is MonoBehaviour.LateUpdate().
        /// </summary>
        /// <remarks>
        /// This is especially helpful if you're trying to scroll dynamically created objects that may be added to the list after LateUpdate,
        /// </remarks>
        public bool UseOnPreRender
        {
            get { return useOnPreRender; }
            set
            {
                if (useOnPreRender == value) { return; }

                if (cameraMethods == null)
                {
                    cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
                }

                if (clipBox != null)
                {
                    clipBox.UseOnPreRender = true;
                }

                if (value)
                {
                    cameraMethods.OnCameraPreRender += OnCameraPreRender;
                }
                else
                {
                    cameraMethods.OnCameraPreRender -= OnCameraPreRender;
                }

                useOnPreRender = value;
            }
        }

        [SerializeField]
        [Tooltip("Amount of (extra) velocity to be applied to scroller")]
        [Range(0.0f, 2.0f)]
        private float velocityMultiplier = 0.8f;

        /// <summary>
        /// Amount of (extra) velocity to be applied to scroller.
        /// </summary>
        /// <remarks>Helpful if you want a small movement to fling the list.</remarks>
        public float VelocityMultiplier
        {
            get { return velocityMultiplier; }
            set { velocityMultiplier = value; }
        }

        [SerializeField]
        [Tooltip("Amount of falloff applied to velocity")]
        [Range(0.0001f, 0.9999f)]
        private float velocityDampen = 0.90f;

        /// <summary>
        /// Amount of drag applied to velocity.
        /// </summary>
        /// <remarks>This can't be 0.0f since that won't allow ANY velocity - set <see cref="TypeOfVelocity"/> to <see cref="VelocityType.None"/>. It can't be 1.0f since that won't allow ANY drag.</remarks>
        public float VelocityDampen
        {
            get { return velocityDampen; }
            set { velocityDampen = value; }
        }

        [SerializeField]
        [Tooltip("The desired type of velocity for the scroller.")]
        private VelocityType typeOfVelocity;

        /// <summary>
        /// The desired type of velocity for the scroller.
        /// </summary>
        public VelocityType TypeOfVelocity
        {
            get { return typeOfVelocity; }
            set { typeOfVelocity = value; }
        }

        [SerializeField]
        [Tooltip("Animation curve for pagination.")]
        private AnimationCurve paginationCurve = new AnimationCurve(
                                                                    new Keyframe(0, 0),
                                                                    new Keyframe(1, 1));
        /// <summary>
        /// Animation curve used to interpolate the pagination and movement methods.
        /// </summary>
        public AnimationCurve PaginationCurve
        {
            get { return paginationCurve; }
            set { paginationCurve = value; }
        }

        [SerializeField]
        [Tooltip("The amount of time (in seconds) the PaginationCurve will take to evaluate.")]
        private float animationLength = 0.25f;

        /// <summary>
        /// The amount of time (in seconds) the <see cref="PaginationCurve"/> will take to evaluate.
        /// </summary>
        public float AnimationLength
        {
            get { return (animationLength < 0) ? 0 : animationLength; }
            set { animationLength = value; }
        }

        [Tooltip("Number of columns or rows in respect to ViewableArea and ScrollDirection.")]
        [SerializeField]
        [Range(1, 500)]
        private int tiers = 2;

        /// <summary>
        /// Number of columns or rows in respect to <see cref="ViewableArea"/> and <see cref="ScrollDirection"/>.
        /// </summary>
        public int Tiers
        {
            get { return (tiers > 0) ? tiers : 1; }
            set { tiers = value; }
        }

        [Tooltip("Whether items that are partially clipped are disabled for input hit testing")]
        [SerializeField]
        private bool disableClippedItems = true;
        public bool DisableClippedItems {
            get => disableClippedItems;
            set => disableClippedItems = value;
        }

        [SerializeField]
        [Tooltip("Manual offset adjust the scale calculation of the ClippingBox.")]
        private Vector3 occlusionScalePadding = new Vector3(0.0f, 0.0f, 0.001f);

        /// <summary>
        /// Manual offset adjust the scale calculation of the <see cref="Microsoft.MixedReality.Toolkit.Utilities.ClippingBox"/>.
        /// </summary>
        /// <remarks>Setting to zero may result in z fighting."</remarks>
        public Vector3 OcclusionScalePadding
        {
            get { return occlusionScalePadding; }
            set { occlusionScalePadding = value; }
        }

        [SerializeField]
        [Tooltip("Manual offset adjust the position calculation of the ClippingBox.")]
        private Vector3 occlusionPositionPadding = Vector3.zero;

        /// <summary>
        /// Manual offset adjust the position calculation of the <see cref="Microsoft.MixedReality.Toolkit.Utilities.ClippingBox"/>.
        /// </summary>
        public Vector3 OcclusionPositionPadding
        {
            get { return occlusionPositionPadding; }
            set { occlusionPositionPadding = value; }
        }

        [Tooltip("Width of cell per object.")]
        [SerializeField]
        [Range(0.00001f, 100.0f)]
        private float cellWidth = 0.25f;

        /// <summary>
        /// Width of the cell per object in the collection.
        /// </summary>
        public float CellWidth
        {
            get { return (cellWidth > 0) ? cellWidth : 0.00001f; }
            set { cellWidth = value; }
        }

        [Tooltip("Height of cell per object.")]
        [SerializeField]
        [Range(0.00001f, 100.0f)]
        private float cellHeight = 0.25f;

        /// <summary>
        /// Height of the cell per object in the collection.
        /// </summary>
        public float CellHeight
        {
            get { return (cellHeight > 0) ? cellHeight : 0.00001f; }
            set { cellHeight = value; }
        }

        [SerializeField]
        [Tooltip("Multiplier to add more bounce to the overscroll of a list when using VelocityType.FalloffPerFrame or VelocityType.FalloffPerItem.")]
        private float bounceMultiplier = 0.1f;

        /// <summary>
        /// Multiplier to add more bounce to the overscroll of a list when using <see cref="VelocityType.FalloffPerFrame"/> or <see cref="VelocityType.FalloffPerItem"/>.
        /// </summary>
        public float BounceMultiplier
        {
            get { return bounceMultiplier; }
            set { bounceMultiplier = value; }
        }

        /// <summary>
        /// The UnityEvent type the ScrollingObjectCollection sends.
        /// GameObject is the object the fired the scroll.
        /// </summary>
        [System.Serializable]
        public class ScrollEvent : UnityEvent<GameObject> { }

        /// <summary>
        /// Event that is fired on the target object when the ScrollingObjectCollection deems event as a Click.
        /// </summary>
        [Tooltip("Event that is fired on the target object when the ScrollingObjectCollection deems event as a Click.")]
        public ScrollEvent ClickEvent = new ScrollEvent();

        /// <summary>
        /// Event that is fired on the target object when the ScrollingObjectCollection is touched.
        /// </summary>
        [Tooltip("Event that is fired on the target object when the ScrollingObjectCollection is touched.")]
        public ScrollEvent TouchStarted = new ScrollEvent();

        /// <summary>
        /// Event that is fired on the target object when the ScrollingObjectCollection is no longer touched.
        /// </summary>
        [Tooltip("Event that is fired on the target object when the ScrollingObjectCollection is no longer touched.")]
        public ScrollEvent TouchEnded = new ScrollEvent();

        /// <summary>
        /// Event that is fired on the target object when the ScrollingObjectCollection is no longer in motion from velocity
        /// </summary>
        [Tooltip("Event that is fired on the target object when the ScrollingObjectCollection is no longer in motion from velocity.")]
        public UnityEvent ListMomentumEnded = new UnityEvent();

        /// <summary>
        /// First item (visible) in the <see cref="ViewableArea"/>. 
        /// </summary>
        public int FirstItemInViewIndex
        {
            get
            {
                if (scrollDirection == ScrollDirectionType.UpAndDown)
                {
                    return (scrollContainer.transform.localPosition.y != 0.0f) ? (int)Mathf.Ceil(scrollContainer.transform.localPosition.y / CellHeight) : 0;
                }
                else
                {
                    return (scrollContainer.transform.localPosition.x != 0.0f) ? (int)Mathf.Ceil(scrollContainer.transform.localPosition.x / CellWidth) : 0;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        private CameraEventRouter cameraMethods;

        // Maximum amount the scroller can travel (vertically)
        private float maxY
        {
            get
            {
                int hasMod = (ModuloCheck(NodeList.Count, Tiers) != 0) ? 1 : 0;
                return NodeList.Count != 0 ? (StepMultiplier(NodeList.Count - (ViewableArea * Tiers), Tiers) + hasMod) * CellHeight : 0.0f;
            }
        }

        // Minimum amount the scroller can travel (vertically) - this will always be zero. Here for readability
        private readonly float minY = 0.0f;

        // Maximum amount the scroller can travel (horizontally) - this will always be zero. Here for readability
        private readonly float maxX = 0.0f;

        // Minimum amount the scroller can travel (horizontally)
        private float minX
        {
            get
            {
                int hasMod = (ModuloCheck(NodeList.Count, Tiers) != 0) ? 1 : 0;
                return NodeList.Count != 0 ? -((StepMultiplier(NodeList.Count - (ViewableArea * Tiers), Tiers) + hasMod) * CellWidth) : 0.0f;
            }

        }

        // Item index for items that should be visible
        private int numItemsPrevView
        {
            get
            {
                if (scrollDirection == ScrollDirectionType.UpAndDown)
                {
                    return (int)Mathf.Ceil(scrollContainer.transform.localPosition.y / CellHeight) * Tiers;
                }
                else
                {
                    return -((int)Mathf.Ceil(scrollContainer.transform.localPosition.x / CellWidth) * Tiers);
                }
            }
        }

        //Take the previous view and then subtract the column remainder and we add the viewable area as a multiplier (minus 1 since the index is zero based).
        //The first item not viewable in the list
        private int numItemsPostView
        {
            get
            {
                if (scrollDirection == ScrollDirectionType.UpAndDown)
                {
                    return ((int)Mathf.Floor((scrollContainer.transform.localPosition.y + occlusionPositionPadding.y) / CellHeight) * Tiers) + (ViewableArea * Tiers) - 1;
                }
                else
                {
                    return ((int)Mathf.Floor((-(scrollContainer.transform.localPosition.x) + occlusionPositionPadding.x) / CellWidth) * Tiers) + (ViewableArea * Tiers) - 1;
                }

            }
        }

        // The empty game object that contains our nodes and be scrolled
        [SerializeField]
        [HideInInspector]
        private GameObject scrollContainer;

        // The empty game object that contains the ClipppingBox
        [SerializeField]
        [HideInInspector]
        private GameObject clippingObject;

        /// <summary>
        /// The empty GameObject containing the ScrollingObjectCollection's <see cref="Microsoft.MixedReality.Toolkit.Utilities.ClippingBox"/>.
        /// </summary>
        public GameObject ClippingObject
        {
            get { return clippingObject; }
        }

        [SerializeField]
        [HideInInspector]
        private ClippingBox clipBox;

        /// <summary>
        /// The ScrollingObjectCollection's <see cref="Microsoft.MixedReality.Toolkit.Utilities.ClippingBox"/> 
        /// that is used for clipping items in and out of the list.
        /// </summary>
        public ClippingBox ClipBox
        {
            get { return clipBox; }
        }

        #region scroll state variables

        // Tracks whether an item in the list is being interacted with
        private bool isEngaged = false;

        // Tracks whether a movement action resulted in dragging the list  
        private bool isDragging = false;

        // we need to know if the pointer was a touch so we can do the threshold test (dot product test)
        private bool isTouched = false;

        // The position of the scollContainer before we do any updating to it
        private Vector3 initialScrollerPos;

        // The new of the scollContainer before we've set the position / finished the updateloop
        private Vector3 workingScrollerPos;

        // A list of new child nodes that have new child renderers that need to be added to the clippingBox
        private List<ObjectCollectionNode> nodesToClip = new List<ObjectCollectionNode>();

        // A list of new child nodes that have new child renderers that need to be removed to the clippingBox
        private List<ObjectCollectionNode> nodesToUnclip = new List<ObjectCollectionNode>();

        private IMixedRealityPointer currentPointer;

        //The initial contact object for the list. this may not always be currentPointer.Result.CurrentPointerTarget
        private GameObject initialFocusedObject;

        // The point where the original PointerDown occurred
        private Vector3 pointerHitPoint;

        // The ray length of original pointer down
        private float pointerHitDistance;

        // This flag is set by PointerUp to prevent InputUp from continuing to propagate. e.g. Interactables
        private bool shouldSwallowEvents = false;

        #endregion scroll state variables

        #region drag position calculation variables

        // Hand position when starting a motion
        private Vector3 initialPointerPos;

        // Hand position previous frame
        private Vector3 lastPointerPos;

        [SerializeField]
        [HideInInspector]
        /// <summary>
        /// The distance in front of the scroller, in local space, for a touch release.
        /// We serialize and then hide this because we want to use the value
        /// when drawing the touch plane, but not actually expose the value publicly.
        /// </summary>
        private float releaseDistance;

        // Current time at initial press
        private float initialPressTime;

        #endregion drag position calculation variables

        #region velocity calculation variables

        // Simple velocity of the scroller: current - last / timeDelta
        private float scrollVelocity = 0.0f;

        // Filtered weight of scroll velocity
        private float avgVelocity = 0.0f;

        // How much we should filter the velocity - yes this is a magic number. Its been tuned so lets leave it.
        private readonly float velocityFilterWeight = 0.97f;

        // Simple state enum to handle velocity logic
        private enum VelocityState
        {
            None = 0,
            Resolving,
            Calculating,
            Bouncing
        }

        // Internal enum for tracking the velocity state of the list
        private VelocityState velocityState = VelocityState.None;

        // Pre calculated destination with velocity and falloff when using per item snapping
        private Vector3 velocityDestinationPos;

        // Velocity container for storing previous filtered velocity
        private float velocitySnapshot;

        #endregion velocity calculation variables

        // The Animation CoRoutine
        private IEnumerator animateScroller;

        #region ObjectCollection methods

        /// <inheritdoc/>
        public override void UpdateCollection()
        {
            // Generate our scroll specific objects
            SetUpScrollContainer();
            SetUpClippingPrimitive();

            // ensure IgnoreInactiveTransforms is set to false, otherwise the node prune will remove any hidden items in the list.g
            IgnoreInactiveTransforms = false;
            // stash our children in a list so the count doesn't change or reverse the order if we were to count down
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child != scrollContainer.transform && child != clippingObject.transform)
                {
                    children.Add(child);
                }
            }

            // move any objects to the scrollContainer
            for (int i = 0; i < children.Count; i++)
            {
                children[i].parent = scrollContainer.transform;
            }

            // Check for empty nodes and remove them
            List<ObjectCollectionNode> emptyNodes = new List<ObjectCollectionNode>();

            for (int i = 0; i < NodeList.Count; i++)
            {
                // Make sure we respect our special scroll objects
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
                
                if (ContainsNode(child, out int nodeIndex) && NodeList[nodeIndex].GetType() != typeof(ScrollingObjectCollectionNode))
                {
                    //This node is in the list, but of the wrong type
                    NodeList[nodeIndex] = new ScrollingObjectCollectionNode(NodeList[nodeIndex]);
                }

                if (!ContainsNode(child) && (child.gameObject.activeSelf || !IgnoreInactiveTransforms))
                {
                    NodeList.Add( new ScrollingObjectCollectionNode { Name = child.name, Transform = child, Colliders = child.GetComponentsInChildren<Collider>() });
                }
            }

            if (NodeList.Count <= 0)
            {
                Debug.LogWarning(gameObject.name + " ScrollingObjectCollection needs a NodeList greater than zero");
                return;
            }

            SortNodes();

            LayoutChildren();

            OnCollectionUpdated?.Invoke(this);
        }

        private void SetUpScrollContainer()
        {
            // ScrollContainer empty game object null check - ensure its set up properly
            if (scrollContainer == null)
            {
                Transform oldContainer = transform.Find("Container");

                if (oldContainer != null)
                {
                    scrollContainer = oldContainer.gameObject;
                    Debug.LogWarning(name + " ScrollingObjectCollection found an existing Container object, using it for the list");
                }
                else
                {
                    scrollContainer = new GameObject();
                }

                scrollContainer.name = "Container";
                scrollContainer.transform.parent = transform;
                scrollContainer.transform.localPosition = Vector3.zero;
                scrollContainer.transform.localRotation = Quaternion.identity;
            }
        }

        private void SetUpClippingPrimitive()
        {
            // ClippingObject empty game object null check - ensure its set up properly
            if (clippingObject == null)
            {
                Transform oldClippingObj = transform.Find("Clipping Bounds");

                if (oldClippingObj != null)
                {
                    clippingObject = oldClippingObj.gameObject;
                    Debug.LogWarning(name + " ScrollingObjectCollection found an existing Clipping object, using it for the list");
                }
                else
                {
                    clippingObject = new GameObject();
                }

                clippingObject.name = "Clipping Bounds";
                clippingObject.transform.parent = transform;
                clippingObject.transform.localRotation = Quaternion.identity;
                clippingObject.transform.localPosition = Vector3.zero;
            }

            // ClippingBox component null check - ensure its set up properly
            if (clipBox == null)
            {
                clipBox = clippingObject.GetComponent<ClippingBox>();

                if (clipBox == null)
                {
                    clipBox = clippingObject.AddComponent<ClippingBox>();
                }

                clipBox.ClippingSide = ClippingPrimitive.Side.Outside;

                if (useOnPreRender)
                {
                    clipBox.UseOnPreRender = true;

                    // Subscribe to the preRender callback on the main camera so we can intercept it and make sure we catch
                    // any dynamically created children in our list
                    cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
                    cameraMethods.OnCameraPreRender += OnCameraPreRender;
                }
            }
        }

        /// <summary>
        /// Arranges our child objects in the scrollContainer per our set up instructions
        /// The layout method uses modulo with Columns / Rows
        /// </summary>
        protected override void LayoutChildren()
        {
            Vector2 halfCell = new Vector2(CellWidth * 0.5f, CellHeight * 0.5f);

            if (NodeList.Count <= 0)
            {
                Debug.LogWarning(gameObject.name + " ScrollingObjectCollection needs a NodeList greater than zero");
                return;
            }

            for (int i = 0; i < NodeList.Count; i++)
            {
                ObjectCollectionNode node = NodeList[i];

                Vector3 newPos;

                if (scrollDirection == ScrollDirectionType.UpAndDown)
                {
                    newPos.x = (ModuloCheck(i, Tiers) != 0) ? (ModuloCheck(i, Tiers) * CellWidth) + halfCell.x : halfCell.x;
                    newPos.y = ((StepMultiplier(i, Tiers) * CellHeight) + halfCell.y) * -1;
                    newPos.z = 0.0f;

                }
                else //left or right
                {
                    newPos.x = (StepMultiplier(i, Tiers) * CellWidth) + halfCell.x;
                    newPos.y = ((ModuloCheck(i, Tiers) != 0) ? (ModuloCheck(i, Tiers) * CellHeight) + halfCell.y : halfCell.y) * -1;
                    newPos.z = 0.0f;
                }
                node.Transform.localPosition = newPos;
                NodeList[i] = node;
            }

            ResolveLayout();
            HideItems();
        }

        /// <summary>
        /// Sets up the initial position of the scroller object as well as the configuration for the clippingBox
        /// </summary>
        private void ResolveLayout()
        {
            if (NodeList.Count <= 0)
            {
                Debug.LogWarning(gameObject.name + " ScrollingObjectCollection needs a NodeList greater than zero");
                return;
            }

            // temporarily turn on the first item in the list if its inactive
            bool resetActiveState = false;
            if (!NodeList[FirstItemInViewIndex].Transform.gameObject.activeSelf)
            {
                resetActiveState = true;
                NodeList[FirstItemInViewIndex].Transform.gameObject.SetActive(true);
            }

            // create the offset for our thresholdCalculation -- grab the first item in the list

            TryGetObjectAlignedBoundsSize(NodeList[FirstItemInViewIndex].Transform, out Vector3 offsetSize);
            releaseDistance = offsetSize.z * 0.5f;

            // Use the first element for collection bounds for occluder positioning
            // temporarily zero out the rotation so we can get an accurate bounds
            Quaternion origRot = NodeList[FirstItemInViewIndex].Transform.rotation;
            NodeList[FirstItemInViewIndex].Transform.rotation = Quaternion.identity;

            // The bounds of the clipping object, this is to make helper math easier later, it doesn't matter that its AABB since we're really not using it for bounds operations
            Bounds clippingBounds = new Bounds();
            clippingBounds.size = Vector3.zero;

            List<Vector3> boundsPoints = new List<Vector3>();
            BoundsExtensions.GetColliderBoundsPoints(NodeList[FirstItemInViewIndex].Transform.gameObject, boundsPoints, 0);
            clippingBounds.center = boundsPoints[0];

            foreach (Vector3 point in boundsPoints)
            {
                clippingBounds.Encapsulate(point);
            }

            // lets check whether the collection cell dimensions are a better fit
            // this prevents negative offset from ruining the scroll effect
            Vector3 tempClippingSize = clippingBounds.size;

            tempClippingSize.x = (tempClippingSize.x > CellWidth) ? tempClippingSize.x : CellWidth;
            tempClippingSize.y = (tempClippingSize.y > CellHeight) ? tempClippingSize.y : CellHeight;

            clippingBounds.size = tempClippingSize;
            clippingBounds.center = clippingBounds.size * 0.5f;

            // put the rotation back
            NodeList[FirstItemInViewIndex].Transform.rotation = origRot;

            //set the first item back to its original state
            if (resetActiveState)
            {
                NodeList[FirstItemInViewIndex].Transform.gameObject.SetActive(false);
            }

            Vector3 viewableCenter = new Vector3();

            //Adjust scale and position of our clipping box
            switch (scrollDirection)
            {
                case ScrollDirectionType.UpAndDown:
                default:

                    //apply the viewable area and column/row multiplier
                    //use a dummy bounds of one to get the local scale to match;
                    clippingBounds.size = new Vector3((clippingBounds.size.x * Tiers), (clippingBounds.size.y * ViewableArea), clippingBounds.size.z);
                    clipBox.transform.localScale = new Bounds(Vector3.zero, Vector3.one).GetScaleToMatchBounds(clippingBounds, OcclusionScalePadding);

                    //adjust where the center of the clipping box is
                    viewableCenter.x = (clipBox.transform.localScale.x * 0.5f) - (OcclusionScalePadding.x * 0.5f) + OcclusionPositionPadding.x;
                    viewableCenter.y = (((clipBox.transform.localScale.y * 0.5f) - (OcclusionScalePadding.y * 0.5f)) * -1) + OcclusionPositionPadding.y;
                    viewableCenter.z = OcclusionPositionPadding.z;
                    break;

                case ScrollDirectionType.LeftAndRight:

                    //Same as above for L <-> R
                    clippingBounds.size = new Vector3(clippingBounds.size.x * ViewableArea, clippingBounds.size.y * Tiers, clippingBounds.size.z);
                    clipBox.transform.localScale = new Bounds(Vector3.zero, Vector3.one).GetScaleToMatchBounds(clippingBounds, OcclusionScalePadding);

                    //Same as above for L <-> R
                    viewableCenter.x = (clipBox.transform.localScale.x * 0.5f) - (OcclusionScalePadding.x * 0.5f) + OcclusionPositionPadding.x;
                    viewableCenter.y = ((clipBox.transform.localScale.y * 0.5f) - (OcclusionScalePadding.y * 0.5f) + OcclusionPositionPadding.y) * -1.0f;
                    viewableCenter.z = OcclusionPositionPadding.z;
                    break;
            }

            //Apply our new values
            clipBox.transform.localPosition = viewableCenter;

            //add our objects to the clippingBox queue
            AddAllItemsToClippingObject();
        }

        #endregion ObjectCollection methods

        #region MonoBehaviour Implementation

        private void OnEnable()
        {
            //Register for global input events
            if (CoreServices.InputSystem != null)
            {
                CoreServices.InputSystem.RegisterHandler<IMixedRealityInputHandler>(this);
                CoreServices.InputSystem.RegisterHandler<IMixedRealityTouchHandler>(this);
                CoreServices.InputSystem.RegisterHandler<IMixedRealityPointerHandler>(this);
                CoreServices.InputSystem.RegisterHandler<IMixedRealitySourceStateHandler>(this);
            }

            if (useOnPreRender)
            {
                if (clippingObject == null || clipBox == null)
                {
                    Debug.Log(name + " UseOnPreRender is enabled, but ScrollingObjectCollection needs UpdateCollection() to be called first.");
                    return;
                }

                clipBox.UseOnPreRender = true;

                //Subscribe to the preRender callback on the main camera so we can intercept it and make sure we catch
                //any dynamically created children in our list
                cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
                cameraMethods.OnCameraPreRender += OnCameraPreRender;
            }
        }

        private void Start()
        {
            if (setUpAtRuntime)
            {
                UpdateCollection();
            }
        }

        private void Update()
        {
            //early bail, the component isn't set up properly
            if (scrollContainer == null) { return; }

            bool nodeLengthCheck = NodeList.Count > (ViewableArea * Tiers);

            // Force the position if the total number of items in the list is less than the scrollable area
            if (!nodeLengthCheck)
            {
                workingScrollerPos = Vector3.zero;
                ApplyPosition(workingScrollerPos);
            }

            //The scroller has detected input and has a valid pointer
            if (isEngaged && TryGetPointerPositionOnPlane(out Vector3 currentPointerPos))
            {
                Vector3 handDelta = initialPointerPos - currentPointerPos;
                handDelta = transform.InverseTransformDirection(handDelta);

                //Lets see if this is gonna be a click or a drag
                //Check the scroller's length state to prevent resetting calculation
                if (!isDragging && nodeLengthCheck)
                {
                    //grab the delta value we care about
                    float absAxisHandDelta = (scrollDirection == ScrollDirectionType.UpAndDown) ? Mathf.Abs(handDelta.y) : Mathf.Abs(handDelta.x);

                    //Catch an intentional finger in scroller to stop momentum, this isn't a drag its definitely a stop
                    if (absAxisHandDelta > (handDeltaMagThreshold * 0.1f) || TimeTest(initialPressTime, Time.time, dragTimeThreshold))
                    {
                        scrollVelocity = 0.0f;
                        avgVelocity = 0.0f;

                        isDragging = true;
                        velocityState = VelocityState.None;

                        //now that we're dragging, reset the interacted with interactable if it exists
                        Interactable ixable = initialFocusedObject.GetComponent<Interactable>();
                        if (ixable != null)
                        {
                           ixable.ResetInputTrackingStates();
                        }

                        //TODO: Reset the state of PressableButton, when possible. 

                        //reset initialHandPos to prevent the scroller from jumping
                        initialScrollerPos = workingScrollerPos = scrollContainer.transform.localPosition;
                        initialPointerPos = currentPointerPos;
                    }
                }

                var thresholdPoint = transform.TransformPoint((Vector3.forward * -1.0f) * releaseDistance);
                //Make sure we're actually (near) touched and not a pointer event, do a dot product check            
                bool scrollRelease = UseNearScrollBoundary ? DetectScrollRelease(transform.forward * -1.0f, thresholdPoint, currentPointerPos, clippingObject.transform, transform.worldToLocalMatrix, scrollDirection)
                                                           : DetectScrollRelease(transform.forward * -1.0f, thresholdPoint, currentPointerPos, null, null, null);

                if (isTouched && scrollRelease)
                {
                    //We're on the other side of the original touch position. This is a release.
                    if (isDragging)
                    {
                        //Its a drag release
                        initialScrollerPos = workingScrollerPos;
                        velocityState = VelocityState.Calculating;
                    }
                    else
                    {
                        //Its a click release
                        Collider[] c = NodeList.Find(x => x.Transform.gameObject == initialFocusedObject).Colliders;
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
                            //Fire the UnityEvent
                            ClickEvent?.Invoke(initialFocusedObject);
                        }
                    }

                    ResetState();

                }
                else if (isDragging && canScroll)
                {

                    if (scrollDirection == ScrollDirectionType.UpAndDown)
                    {
                        //Lock X, clamp Y
                        workingScrollerPos.y = MathUtilities.CLampLerp(initialScrollerPos.y - handDelta.y, minY, maxY, 0.5f);
                        workingScrollerPos.x = 0.0f;
                    }
                    else
                    {
                        //Lock Y, clamp X
                        workingScrollerPos.x = MathUtilities.CLampLerp(initialScrollerPos.x - handDelta.x, minX, maxX, 0.5f);
                        workingScrollerPos.y = 0.0f;
                    }

                    //Update the scrollContainer Position
                    ApplyPosition(workingScrollerPos);

                    CalculateVelocity();

                    //Update the prev val for velocity
                    lastPointerPos = currentPointerPos;
                }
            }
            else if (animateScroller == null && nodeLengthCheck)// Prevent the Animation coroutine from being overridden
            {
                //We're not engaged, so handle any not touching behavior
                HandleVelocityFalloff();

                //Apply our position
                ApplyPosition(workingScrollerPos);
            }
        }

        private void LateUpdate()
        {
            //Hide the items not in view
            HideItems();
        }

        private void OnDisable()
        {
            //Unregister global input events
            if (CoreServices.InputSystem != null)
            {
                CoreServices.InputSystem.UnregisterHandler<IMixedRealityInputHandler>(this);
                CoreServices.InputSystem.UnregisterHandler<IMixedRealityTouchHandler>(this);
                CoreServices.InputSystem.UnregisterHandler<IMixedRealityPointerHandler>(this);
                CoreServices.InputSystem.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
            }

            if (useOnPreRender && cameraMethods != null)
            {
                CameraEventRouter cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
                cameraMethods.OnCameraPreRender -= OnCameraPreRender;
            }
        }

        #endregion MonoBehaviour Implementation

        #region private methods

        /// <summary>
        /// When <see cref="UseOnPreRender"/>, the <see cref="ScrollingObjectCollection"/> subscribes to the <see cref="CameraEventRouter"/> call back for OnCameraPreRender
        /// </summary>
        /// <param name="router">The active <see cref="CameraEventRouter"/> on the camera.</param>
        private void OnCameraPreRender(CameraEventRouter router)
        {
            //clip any new items that may have shown up
            if (nodesToClip.Count > 0)
            {
                AddItemsToClippingObject(nodesToClip);

                //reset the list for next time
                nodesToClip.Clear();
            }

            //unclip any new items that may have shown up
            if (nodesToUnclip.Count > 0)
            {
                RemoveItemsFromClippingObject(nodesToUnclip);

                //reset the list for next time
                nodesToUnclip.Clear();
            }

        }

        /// <summary>
        /// Gets the cursor position (pointer end point) on the scrollable plane,
        /// projected onto the direction being scrolled.
        /// Returns false if the pointer is null or pointer details is null.
        /// </summary>
        private bool TryGetPointerPositionOnPlane(out Vector3 result)
        {
            result = Vector3.zero;

            if (((MonoBehaviour)currentPointer) == null)
            {
                return false;
            }
            if (currentPointer.GetType() == typeof(PokePointer))
            {
                result = currentPointer.Position;
                return true;
            }
            if (currentPointer.Result?.Details != null)
            {
                var endPoint = RayStep.GetPointByDistance(currentPointer.Rays, pointerHitDistance);
                var scrollVector = (scrollDirection == ScrollDirectionType.UpAndDown) ? transform.up : transform.right;
                result = pointerHitPoint + Vector3.Project(endPoint - pointerHitPoint, scrollVector);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates our <see cref="VelocityType"/> falloff
        /// </summary>
        private void HandleVelocityFalloff()
        {
            switch (typeOfVelocity)
            {
                case VelocityType.FalloffPerFrame:

                    HandleFalloffPerFrame();
                    break;

                case VelocityType.FalloffPerItem:
                default:

                    HandleFalloffPerItem();
                    break;

                case VelocityType.NoVelocitySnapToItem:

                    velocityState = VelocityState.None;

                    avgVelocity = 0.0f;

                    //Round to the nearest list item
                    if (scrollDirection == ScrollDirectionType.UpAndDown)
                    {
                        workingScrollerPos.y = Mathf.Round(scrollContainer.transform.localPosition.y / CellHeight) * CellHeight;
                    }
                    else
                    {
                        workingScrollerPos.x = Mathf.Round(scrollContainer.transform.localPosition.x / CellWidth) * CellWidth;
                    }

                    initialScrollerPos = workingScrollerPos;
                    ListMomentumEnded?.Invoke();

                    break;

                case VelocityType.None:

                    velocityState = VelocityState.None;

                    avgVelocity = 0.0f;
                    ListMomentumEnded?.Invoke();
                    break;
            }

            if (velocityState == VelocityState.None)
            {
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);
            }
        }

        /// <summary>
        /// Handles <see cref="ScrollingObjectCollection"/> drag release behavior when <see cref="TypeOfVelocity"/> is set to <see cref="VelocityType.FalloffPerItem"/>
        /// </summary>
        private void HandleFalloffPerItem()
        {

            switch (velocityState)
            {
                case VelocityState.Calculating:

                    int numSteps;
                    float newPosAfterVelocity;
                    if (scrollDirection == ScrollDirectionType.UpAndDown)
                    {
                        if (avgVelocity == 0.0f)
                        {
                            //velocity was cleared out so we should just snap
                            newPosAfterVelocity = scrollContainer.transform.localPosition.y;
                        }
                        else
                        {
                            //Precalculate where the velocity falloff WOULD land our scrollContainer, then round it to the nearest item so it feels "natural"
                            velocitySnapshot = IterateFalloff(avgVelocity, out numSteps);
                            newPosAfterVelocity = initialScrollerPos.y - velocitySnapshot;
                        }

                        velocityDestinationPos.y = (Mathf.Round(newPosAfterVelocity / CellHeight)) * CellHeight;

                        velocityState = VelocityState.Resolving;
                    }
                    else
                    {
                        if (avgVelocity == 0.0f)
                        {
                            //velocity was cleared out so we should just snap
                            newPosAfterVelocity = scrollContainer.transform.localPosition.x;
                        }
                        else
                        {
                            //Precalculate where the velocity falloff WOULD land our scrollContainer, then round it to the nearest item so it feels "natural"
                            velocitySnapshot = IterateFalloff(avgVelocity, out numSteps);
                            newPosAfterVelocity = initialScrollerPos.x + velocitySnapshot;
                        }

                        velocityDestinationPos.x = (Mathf.Round(newPosAfterVelocity / CellWidth)) * CellWidth;

                        velocityState = VelocityState.Resolving;
                    }

                    workingScrollerPos = Solver.SmoothTo(scrollContainer.transform.localPosition, velocityDestinationPos, Time.deltaTime, 0.9275f);

                    //Clear the velocity now that we've applied a new position
                    avgVelocity = 0.0f;
                    break;

                case VelocityState.Resolving:

                    if (scrollDirection == ScrollDirectionType.UpAndDown)
                    {
                        if (scrollContainer.transform.localPosition.y > maxY + (releaseDistance * bounceMultiplier)
                            || scrollContainer.transform.localPosition.y < minY - (releaseDistance * bounceMultiplier))
                        {
                            velocityState = VelocityState.Bouncing;
                            velocitySnapshot = 0.0f;
                            break;
                        }
                        else
                        {
                            workingScrollerPos = Solver.SmoothTo(scrollContainer.transform.localPosition, velocityDestinationPos, Time.deltaTime, 0.9275f);

                            SnapVelocityFinish();
                        }
                    }
                    else
                    {
                        if (scrollContainer.transform.localPosition.x > maxX + (releaseDistance * bounceMultiplier)
                            || scrollContainer.transform.localPosition.x < minX - (releaseDistance * bounceMultiplier))
                        {
                            velocityState = VelocityState.Bouncing;
                            velocitySnapshot = 0.0f;
                            break;
                        }
                        else
                        {
                            workingScrollerPos = Solver.SmoothTo(scrollContainer.transform.localPosition, velocityDestinationPos, Time.deltaTime, 0.9275f);

                            SnapVelocityFinish();
                        }
                    }
                    break;

                case VelocityState.Bouncing:

                    HandleBounceState();
                    break;

                case VelocityState.None:
                default:
                    // clean up our position for next frame
                    initialScrollerPos = workingScrollerPos;
                    break;
            }
        }

        /// <summary>
        /// Handles <see cref="ScrollingObjectCollection"/> drag release behavior when <see cref="TypeOfVelocity"/> is set to <see cref="VelocityType.FalloffPerFrame"/>
        /// </summary>
        private void HandleFalloffPerFrame()
        {
            switch (velocityState)
            {
                case VelocityState.Calculating:

                    if (scrollDirection == ScrollDirectionType.UpAndDown)
                    {
                        workingScrollerPos.y = initialScrollerPos.y + avgVelocity;
                    }
                    else
                    {
                        workingScrollerPos.x = initialScrollerPos.x + avgVelocity;
                    }

                    velocityState = VelocityState.Resolving;

                    // clean up our position for next frame
                    initialScrollerPos = workingScrollerPos;
                    break;

                case VelocityState.Resolving:

                    if (scrollDirection == ScrollDirectionType.UpAndDown)
                    {
                        if (scrollContainer.transform.localPosition.y > maxY + (releaseDistance * bounceMultiplier)
                            || scrollContainer.transform.localPosition.y < minY - (releaseDistance * bounceMultiplier))
                        {
                            velocityState = VelocityState.Bouncing;
                            avgVelocity = 0.0f;
                            break;
                        }
                        else
                        {
                            avgVelocity *= velocityDampen;
                            workingScrollerPos.y = initialScrollerPos.y + avgVelocity;

                            SnapVelocityFinish();

                        }
                    }
                    else
                    {
                        if (scrollContainer.transform.localPosition.x > maxX + (releaseDistance * bounceMultiplier)
                            || scrollContainer.transform.localPosition.x < minX - (releaseDistance * bounceMultiplier))
                        {
                            velocityState = VelocityState.Bouncing;
                            avgVelocity = 0.0f;
                            break;
                        }
                        else
                        {
                            avgVelocity *= velocityDampen;
                            workingScrollerPos.x = initialScrollerPos.x + avgVelocity;

                            SnapVelocityFinish();
                        }
                    }

                    // clean up our position for next frame
                    initialScrollerPos = workingScrollerPos;

                    break;

                case VelocityState.Bouncing:

                    HandleBounceState();

                    break;
            }
        }

        /// <summary>
        /// Smooths <see cref="ScrollContainer"/>'s position to the proper clamped edge 
        /// while <see cref="velocityState"/> is <see cref="VelocityState.Bouncing"/>.
        /// </summary>
        private void HandleBounceState()
        {
            Vector3 clampedDest = new Vector3(Mathf.Clamp(scrollContainer.transform.localPosition.x, minX, maxX), Mathf.Clamp(scrollContainer.transform.localPosition.y, minY, maxY), 0.0f);
            if ((scrollDirection == ScrollDirectionType.UpAndDown && Mathf.Abs(scrollContainer.transform.localPosition.y - clampedDest.y) < 0.00001)
                || (scrollDirection == ScrollDirectionType.LeftAndRight && Mathf.Abs(scrollContainer.transform.localPosition.x - clampedDest.x) < 0.00001))
            {
                velocityState = VelocityState.None;

                ListMomentumEnded?.Invoke();

                // clean up our position for next frame
                initialScrollerPos = workingScrollerPos = clampedDest;
                return;
            }
            workingScrollerPos.y = Solver.SmoothTo(scrollContainer.transform.localPosition, clampedDest, Time.deltaTime, 0.2f).y;
            workingScrollerPos.x = Solver.SmoothTo(scrollContainer.transform.localPosition, clampedDest, Time.deltaTime, 0.2f).x;
        }

        /// <summary>
        /// Snaps to the final position of the <see cref="ScrollContainer"/> once velocity as resolved.
        /// </summary>
        private void SnapVelocityFinish()
        {
            if (Vector3.Distance(scrollContainer.transform.localPosition, workingScrollerPos) > 0.00001f)
            {
                return;
            }

            if (typeOfVelocity == VelocityType.FalloffPerItem)
            {
                if (scrollDirection == ScrollDirectionType.UpAndDown)
                {
                    //Ensure we've actually snapped the position to prevent an extreme in-between state
                    workingScrollerPos.y = (Mathf.Round(scrollContainer.transform.localPosition.y / CellHeight)) * CellHeight;
                }
                else
                {
                    workingScrollerPos.x = (Mathf.Round(scrollContainer.transform.localPosition.x / CellWidth)) * CellWidth;
                }
            }

            velocityState = VelocityState.None;
            avgVelocity = 0.0f;

            ListMomentumEnded?.Invoke();

            // clean up our position for next frame
            initialScrollerPos = workingScrollerPos;
        }

        /// <summary>
        /// Wrapper for per frame velocity calculation and filtering.
        /// </summary>
        private void CalculateVelocity()
        {
            //update simple velocity
            TryGetPointerPositionOnPlane(out Vector3 newPos);

                scrollVelocity = (scrollDirection == ScrollDirectionType.UpAndDown)
                                 ? (newPos.y - lastPointerPos.y) / Time.deltaTime * (velocityMultiplier * 0.01f)
                                 : (newPos.x - lastPointerPos.x) / Time.deltaTime * (velocityMultiplier * 0.01f);

            //And filter it...
            avgVelocity = (avgVelocity * (1.0f - velocityFilterWeight)) + (scrollVelocity * velocityFilterWeight);
        }

        /// <summary>
        /// The Animation Override to position our scroller based on manual movement <see cref="PageBy(int, bool)"/>, <see cref="MoveTo(int, bool)"/>,
        /// <see cref="MoveByItems(int, bool)"/>, or <see cref="MoveByTiers(int, bool)"/>
        /// </summary>
        /// <param name="initialPos">The start position of the scrollContainer</param>
        /// <param name="finalPos">Where we want the scrollContainer to end up, typically this should be <see cref="workingScrollerPos"/></param>
        /// <param name="curve"><see cref="AnimationCurve"/> representing the easing desired</param>
        /// <param name="time">Time for animation, in seconds</param>
        /// <param name="callback">Optional callback action to be invoked after animation coroutine has finished</param>
        private IEnumerator AnimateTo(Vector3 initialPos, Vector3 finalPos, AnimationCurve curve = null, float? time = null, System.Action callback = null)
        {
            velocityState = VelocityState.None;

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
                workingScrollerPos = Vector3.Lerp(initialPos, finalPos, curve.Evaluate(counter / (float)time));
                scrollContainer.transform.localPosition = workingScrollerPos;

                counter += Time.deltaTime;
                yield return null;
            }

            //update our values so they stick

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = initialScrollerPos.y = finalPos.y;
            }
            else
            {
                workingScrollerPos.x = initialScrollerPos.x = finalPos.x;
            }

            if (callback != null)
            {
                callback?.Invoke();
            }

            animateScroller = null;
        }

        /// <summary>
        /// Checks to see if the engaged joint has released the scrollable list
        /// </summary>
        /// <param name="initialDirection">The plane normal direction.</param>
        /// <param name="initialPosition">The point representing the plane's origin.</param>
        /// <param name="pointToCompare">The point compared to the normal and origin.</param>
        /// <param name="clippingObj">The object representing the maximum scrollable area.</param>
        /// <param name="transformMatrix">The world Matrix for the scrollable area to be compared in.</param>
        /// <param name="direction"><see cref="ScrollDirectionType"/> the list is scrolling in.</param>
        /// <returns><see cref="true"/> if released.</returns>
        private static bool DetectScrollRelease(Vector3 initialDirection, Vector3 initialPosition, Vector3 pointToCompare, Transform clippingObj = null, Matrix4x4? transformMatrix = null, ScrollDirectionType? direction = null)
        {
            Plane testPlane = new Plane(initialDirection.normalized, initialPosition);

            if (testPlane.GetSide(pointToCompare))
            {
                return true;
            }

            bool hasPassedBoundary = false;

            if (clippingObj != null && transformMatrix != null && direction != null)
            {
                Matrix4x4 tMat = (Matrix4x4)transformMatrix;
                Vector3 posToClip = tMat.MultiplyPoint3x4(pointToCompare) - clippingObj.localPosition;
                Vector3 halfScale = clippingObj.localScale * 0.5f;

                if (direction == ScrollDirectionType.UpAndDown)
                {
                    hasPassedBoundary = posToClip.y > halfScale.y || posToClip.y < -halfScale.y;
                }
                else
                {
                    hasPassedBoundary = posToClip.x > halfScale.x || posToClip.x < -halfScale.x;
                }
            }

            return hasPassedBoundary;
        }

        /// <summary>
        /// Grab all child renderers in each node from NodeList and add them to the ClippingBox
        /// </summary>
        private void AddAllItemsToClippingObject()
        {
            //Register all of the renderers to be clipped by the clippingBox
            foreach (ObjectCollectionNode node in NodeList)
            {
                Renderer[] childRends = node.Transform.gameObject.transform.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < childRends.Length; i++)
                {
                    clipBox.AddRenderer(childRends[i]);
                }
            }
        }

        /// <summary>
        /// Grab all child renderers in a list of nodes from NodeList and add them to the ClippingBox
        /// </summary>
        private void AddItemsToClippingObject(List<ObjectCollectionNode> nodes)
        {
            foreach (ObjectCollectionNode node in nodes)
            {
                //Register all of the renderers to be clipped by the clippingBox
                Renderer[] childRends = node.Transform.gameObject.transform.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < childRends.Length; i++)
                {
                    clipBox.AddRenderer(childRends[i]);
                }
            }
        }

        /// <summary>
        /// Grab all child renderers in each node from NodeList and remove them to the ClippingBox
        /// </summary>
        private void RemoveAllItemsFromClippingObject()
        {
            //Register all of the renderers to be clipped by the clippingBox
            foreach (ObjectCollectionNode node in NodeList)
            {
                Renderer[] childRends = node.Transform.gameObject.transform.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < childRends.Length; i++)
                {
                    clipBox.RemoveRenderer(childRends[i]);
                }
            }
        }

        /// <summary>
        /// Grab all child renderers in a list of nodes from NodeList and remove them to the ClippingBox
        /// </summary>
        private void RemoveItemsFromClippingObject(List<ObjectCollectionNode> nodes)
        {
            foreach (ObjectCollectionNode node in nodes)
            {
                //Register all of the renderers to be clipped by the clippingBox
                Renderer[] childRends = node.Transform.gameObject.transform.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < childRends.Length; i++)
                {
                    clipBox.RemoveRenderer(childRends[i]);
                }
            }
        }

        /// <summary>
        /// Helper to get the remainder from an itemindex in the list in relation to rows/columns
        /// </summary>
        /// <param name="itemIndex">Index of node item in 
        /// <see cref="Microsoft.MixedReality.Toolkit.Utilities.BaseObjectCollection.NodeList"/> 
        /// to be compared</param>
        /// <param name="divisor">Rows / Columns</param>
        /// <returns>The remainder from the divisor</returns>
        public static int ModuloCheck(int itemIndex, int divisor)
        {
            //prevent divide by 0
            return (divisor > 0) ? itemIndex % divisor : 0;
        }

        /// <summary>
        /// Helper to get the number of rows / columns deep the item is
        /// </summary>
        /// <param name="itemIndex">Index of node item in <see cref="Microsoft.MixedReality.Toolkit.Utilities.BaseObjectCollection.NodeList"/> to be compared</param>
        /// <param name="divisor">Rows / Columns</param>
        /// <returns>The multiplier to get the row / column index the item is in</returns>
        public static int StepMultiplier(int itemIndex, int divisor)
        {
            //prevent divide by 0
            return (divisor != 0) ? itemIndex / divisor : 0;
        }

        /// <summary>
        /// Iterates the <see cref="BaseObjectCollection.NodeList"/> to determine which <see cref="ObjectCollectionNode"/>s needs to be
        /// disabled (<see cref="GameObject.SetActive(bool)"/>) and their <see cref="Collider"/> disabled.
        /// </summary>
        /// <remarks>When <see cref="useOnPreRender"/> is set to <see cref="true"/>, <see cref="HideItems"/> will populate a list of <see cref="ObjectCollectionNode"/>
        /// to be added to the <see cref="ClipBox"/>.</remarks>
        private void HideItems()
        {
            //Early Bail - our list is empty
            if (NodeList.Count == 0) { return; }

            //Stash the values from numItems to cut down on redundant calculations
            int prevItems = numItemsPrevView;
            int postItems = numItemsPostView;

            int listLength = NodeList.Count;

            for (int i = 0; i < listLength; i++)
            {
                ScrollingObjectCollectionNode node = NodeList[i] as ScrollingObjectCollectionNode;
                if (node == null)
                {
                    //The object we grabbed isn't a ScrollingObjectCollectionNode
                    NodeList[i] = new ScrollingObjectCollectionNode(NodeList[i]);
                    node = NodeList[i] as ScrollingObjectCollectionNode;
                }

                //hide the items that have no chance of being seen
                if (i < prevItems - Tiers || i > postItems + Tiers)
                {
                    //quick check to cut down on the redundant calls
                    if (node.Transform.gameObject.activeSelf)
                    {
                        node.Transform.gameObject.SetActive(false);
                    }

                    //the node is inactive, and has been previously clipped, make sure we remove it from the rendering list, this keeps the calls in ClippingBox as small as possible
                    if (node.isClipped)
                    {
                        nodesToUnclip.Add(NodeList[i]);
                        node.isClipped = false;
                    }
                }
                else
                {
                    bool disableNode = disableClippedItems ?
                        i < prevItems || i > postItems :
                        i < prevItems - Tiers || i > postItems + Tiers;

                    //Disable colliders on items that will be scrolling in and out of view
                    if (disableNode)
                    {
                        foreach (Collider c in node.Colliders)
                        {
                            c.enabled = false;
                        }
                    }
                    else
                    {
                        foreach (Collider c in node.Colliders)
                        {
                            c.enabled = true;
                        }
                    }

                    //Otherwise show the item
                    if (!node.Transform.gameObject.activeSelf)
                    {
                        node.Transform.gameObject.SetActive(true);
                    }

                    //the node has a new item, send it to the clipping box and see if its a renderer
                    if (!node.isClipped)
                    {
                        nodesToClip.Add(node);
                        node.isClipped = true;
                    }
                }
            }
        }

        /// <summary>
        /// Precalculates the total amount of travel given the scroller's current average velocity and drag.
        /// </summary>
        /// <param name="steps"><see cref="out"/> Number of steps to get our <see cref="avgVelocity"/> to effectively "zero" (0.00001).</param>
        /// <returns>The total distance the <see cref="avgVelocity"/> with <see cref="velocityDampen"/> as drag would travel.</returns>
        private float IterateFalloff(float vel, out int steps)
        {
            //Some day this should be a falloff formula, below is the number of steps. Just can't figure out how to get the right velocity.
            //float numSteps = (Mathf.Log(0.00001f)  - Mathf.Log(Mathf.Abs(avgVelocity))) / Mathf.Log(velocityFalloff);

            float newVal = 0.0f;
            float v = vel;
            steps = 0;

            while (Mathf.Abs(v) > 0.00001)
            {
                v *= velocityDampen;
                newVal += v;
                steps++;
            }

            return newVal;
        }

        /// <summary>
        /// Applies <paramref name="workingPos"/> to the <see cref="Transform.localPosition"/> of our <see cref="scrollContainer"/>
        /// </summary>
        /// <param name="workingPos">The new desired position for <see cref="scrollContainer"/> in local space</param>
        private void ApplyPosition(Vector3 workingPos)
        {
            Vector3 newScrollPos;

            switch (scrollDirection)
            {
                case ScrollDirectionType.UpAndDown:
                default:

                    newScrollPos = new Vector3(scrollContainer.transform.localPosition.x, workingPos.y, 0.0f);
                    break;

                case ScrollDirectionType.LeftAndRight:

                    newScrollPos = new Vector3(workingPos.x, scrollContainer.transform.localPosition.y, 0.0f);

                    break;
            }
            scrollContainer.transform.localPosition = newScrollPos;
        }

        /// <summary>
        /// Resets the state of the ScrollingObjectCollection for the next scroll
        /// </summary>
        private void ResetState()
        {
            TouchEnded?.Invoke(initialFocusedObject);

            //Release the pointer
            currentPointer = null;
            initialFocusedObject = null;

            //Clear our states
            isTouched = false;
            isEngaged = false;
            isDragging = false;
        }

        #endregion private methods

        #region public methods

        /// <summary>
        /// Checks whether the given item is visible in the list
        /// </summary>
        /// <param name="indexOfItem">the index of the item in the list</param>
        /// <returns>true when item is visible</returns>
        public bool IsItemVisible(int indexOfItem)
        {
            bool itemLoc = true;

            if (indexOfItem < numItemsPrevView)
            {
                //its above the visible area
                itemLoc = false;
            }
            else if (indexOfItem > numItemsPostView)
            {
                //its below the visible area
                itemLoc = false;
            }
            return itemLoc;
        }

        /// <summary>
        /// Moves scroller by a multiplier of <see cref="ViewableArea"/>
        /// </summary>
        /// <param name="numOfPages">number of <see cref="ViewableArea"/> to move scroller by</param>
        /// <param name="animateToPage">if true, scroller will animate to new position</param>
        /// <param name="callback"> An optional action to pass in to get notified that the <see cref="ScrollingObjectCollection"/> is finished moving</param>
        public void PageBy(int numOfPages, bool animateToPage = true, System.Action callback = null)
        {
            StopAllCoroutines();
            velocityState = VelocityState.None;

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = Mathf.Clamp((FirstItemInViewIndex + (numOfPages * ViewableArea)) * CellHeight, minY, maxY);
                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = Mathf.Clamp((FirstItemInViewIndex + (numOfPages * ViewableArea)) * CellWidth, minX, maxX);
                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPage)
            {
                animateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength, callback);
                StartCoroutine(animateScroller);
            }
            else
            {
                initialScrollerPos = workingScrollerPos;
                if (callback != null)
                {
                    callback?.Invoke();
                }
            }
        }

        /// <summary>
        /// Moves scroller a relative number of items
        /// </summary>
        /// <param name="numberOfItemsToMove">number of items to move by</param>
        /// <param name="animateToPosition">if true, scroller will animate to new position</param>
        /// <param name="callback"> An optional action to pass in to get notified that the ScrollingObjectCollection is finished moving</param>
        public void MoveByItems(int numberOfItemsToMove, bool animateToPosition = true, System.Action callback = null)
        {
            StopAllCoroutines();
            velocityState = VelocityState.None;

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = (FirstItemInViewIndex + StepMultiplier(numberOfItemsToMove, Tiers)) * CellHeight;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = ((FirstItemInViewIndex + StepMultiplier(numberOfItemsToMove, Tiers)) * CellWidth) * -1.0f;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPosition)
            {
                animateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength);
                StartCoroutine(animateScroller);
            }
            else
            {
                initialScrollerPos = workingScrollerPos;
                if (callback != null)
                {
                    callback?.Invoke();
                }
            }
        }

        /// <summary>
        /// Moves scroller a relative number of <see cref="Tiers"/> of items.
        /// </summary>
        /// <param name="numberOfLinesToMove">number of lines to move by</param>
        /// <param name="animateToPosition">if true, scroller will animate to new position</param>
        /// <param name="callback"> An optional action to pass in to get notified that the <see cref="ScrollingObjectCollection"/> is finished moving</param>
        public void MoveByLines(int numberOfLinesToMove, bool animateToPosition = true, System.Action callback = null)
        {
            StopAllCoroutines();
            velocityState = VelocityState.None;

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = (FirstItemInViewIndex + numberOfLinesToMove) * CellHeight;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = ((FirstItemInViewIndex + numberOfLinesToMove) * CellWidth) * -1.0f;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPosition)
            {
                animateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength, callback);
                StartCoroutine(animateScroller);
            }
            else
            {
                initialScrollerPos = workingScrollerPos;
                if (callback != null)
                {
                    callback?.Invoke();
                }
            }
        }

        /// <summary>
        /// Moves scroller to an absolute position where indexOfItem 
        /// is in the first column of the viewable area
        /// </summary>
        /// <param name="indexOfItem">Item to move to, will be first (or closest to in respect to scroll maximum) in viewable area</param>
        /// <param name="animateToPosition">if true, scroller will animate to new position</param>
        /// <param name="callback"> An optional action to pass in to get notified that the ScrollingObjectCollection is finished moving</param>
        public void MoveTo(int indexOfItem, bool animateToPosition = true, System.Action callback = null)
        {
            StopAllCoroutines();
            velocityState = VelocityState.None;

            indexOfItem = (indexOfItem < 0) ? 0 : indexOfItem;

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = StepMultiplier(indexOfItem, Tiers) * CellHeight;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = (StepMultiplier(indexOfItem, Tiers) * CellWidth) * -1.0f;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPosition)
            {
                animateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength, callback);
                StartCoroutine(animateScroller);
            }
            else
            {
                initialScrollerPos = workingScrollerPos;

                if (callback != null)
                {
                    callback?.Invoke();
                }
            }
        }

        /// <summary>
        /// Simple time threshold check
        /// </summary>
        /// <param name="initTime">Initial time</param>
        /// <param name="currTime">Current time</param>
        /// <param name="pressMargin">Time threshold</param>
        /// <returns>true if amount of time surpasses <paramref name="pressMargin"/></returns>
        public static bool TimeTest(float initTime, float currTime, float pressMargin)
        {
            if (currTime - initTime > pressMargin)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Finds the object-aligned size of a <see href="https://docs.unity3d.com/ScriptReference/Transform.html">Transform</see>.
        /// </summary>
        /// <param name="obj">Transform representing the object to get offset from</param>
        /// <param name="alignedSize">the object-aligned size of obj</param>
        /// <returns>true if alignedSize is valid</returns>
        public static bool TryGetObjectAlignedBoundsSize(Transform obj, out Vector3 alignedSize)
        {
            Collider c = obj.GetComponentInChildren<Collider>();
            alignedSize = Vector3.zero;

            //store and clear the original rotation
            Quaternion origRot = obj.rotation;
            obj.rotation = Quaternion.identity;

            bool canGetSize = false;

            if (c != null)
            {
                if (c.GetType() == typeof(BoxCollider))
                {
                    BoxCollider bC = c as BoxCollider;
                    alignedSize = bC.bounds.size;
                    canGetSize = true;
                }
                else if (c.GetType() == typeof(SphereCollider))
                {
                    SphereCollider sC = c as SphereCollider;
                    alignedSize = new Vector3(sC.radius, sC.radius, sC.radius);
                    canGetSize = true;
                }
                else if (c.GetType() == typeof(CapsuleCollider))
                {
                    CapsuleCollider cc = c as CapsuleCollider;
                    Bounds capsuleBounds = new Bounds(cc.center, Vector3.zero);
                    switch (cc.direction)
                    {
                        case 0:
                            alignedSize = new Vector3(cc.height, cc.radius * 2, cc.radius * 2);
                            break;

                        case 1:
                            alignedSize = new Vector3(cc.radius * 2, cc.height, cc.radius * 2);
                            break;

                        case 2:
                            alignedSize = new Vector3(cc.radius * 2, cc.radius * 2, cc.height);
                            break;
                    }
                }
                else
                {
                    canGetSize = false;
                }

            }
            else if (obj.GetComponentInChildren<Renderer>() != null)
            {
                List<Vector3> points = new List<Vector3>();
                Bounds rendBound = new Bounds();
                BoundsExtensions.GetRenderBoundsPoints(obj.gameObject, points, 0);
                rendBound.center = points[0];

                foreach (Vector3 p in points)
                {
                    rendBound.Encapsulate(p);
                }

                alignedSize = rendBound.size;
                canGetSize = true;
            }
            else
            {
                canGetSize = false;
            }

            //reapply our rotation
            obj.rotation = origRot;

            return (canGetSize) ? true : false;
        }

        #endregion public methods

        #region IMixedRealityPointerHandler implementation

        ///<inheritdoc/>
        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            //Quick check for the global listener to bail if the object is not in the list
            if (currentPointer == null || eventData.Pointer.PointerId != currentPointer.PointerId)
            {
                return;
            }

            if (!isTouched && isEngaged && animateScroller == null)
            {   
                if (isDragging)
                {
                    eventData.Use();
                    shouldSwallowEvents = true;
                    //Its a drag release
                    initialScrollerPos = workingScrollerPos;
                    velocityState = VelocityState.Calculating;
                }

                //Release the pointer
                currentPointer.IsTargetPositionLockedOnFocusLock = true;
 
                 ResetState();
            }
        }

        ///<inheritdoc/>
        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer.Controller.IsPositionAvailable)
            {
                //Quick check for the global listener to bail if the object is not in the list
                if (eventData.Pointer?.Result?.CurrentPointerTarget == null 
                    || !ContainsNode(eventData.Pointer.Result.CurrentPointerTarget.transform) || initialFocusedObject != null)
                {
                    return;
                }

                currentPointer = eventData.Pointer;

                currentPointer.IsTargetPositionLockedOnFocusLock = false;

                pointerHitPoint = currentPointer.Result.Details.Point;
                pointerHitDistance = currentPointer.Result.Details.RayDistance;

                initialFocusedObject = currentPointer.Result?.CurrentPointerTarget;

                //Reset the scroll state
                scrollVelocity = 0.0f;

                if (TryGetPointerPositionOnPlane(out initialPointerPos))
                {
                    initialPressTime = Time.time;
                    initialScrollerPos = scrollContainer.transform.localPosition;
                    velocityState = VelocityState.None;

                    isTouched = false;
                    isEngaged = true;
                    isDragging = false;

                    TouchStarted?.Invoke(initialFocusedObject);
                }
            }
            else
            {
                //not sure what to do with this pointer
                Debug.Log(name + " intercepted a pointer from " + gameObject.name + ". " + eventData.Pointer.PointerName + ", but don't know what to do with it.");
            }
        }

        ///<inheritdoc/>
        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            //we ignore this event and calculate click in the Update() loop;
        }

        #endregion IMixedRealityPointerHandler implementation

        #region IMixedRealityTouchHandler implementation

        ///<inheritdoc/>
        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            if (isDragging || initialFocusedObject)
            {
                eventData.Use();
                return;
            }

            currentPointer = PointerUtils.GetPointer<PokePointer>(eventData.Handedness);
            if (currentPointer != null)
            {
                //Quick check for the global listener to bail if the object is not in the list
                if (currentPointer.Result?.CurrentPointerTarget == null ||
                    !ContainsNode(currentPointer.Result?.CurrentPointerTarget.transform))
                {
                    return;
                }

                StopAllCoroutines();
                animateScroller = null;

                if (!isTouched && !isEngaged)
                {
                    initialPointerPos = currentPointer.Position;
                    initialPressTime = Time.time;
                    initialFocusedObject = currentPointer.Result?.CurrentPointerTarget;
                    initialScrollerPos = scrollContainer.transform.localPosition;
                    shouldSwallowEvents = true;

                    isTouched = true;
                    isEngaged = true;
                    isDragging = false;

                    TouchStarted?.Invoke(initialFocusedObject);

                }
            }

        }

        ///<inheritdoc/>
        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            //Quick check for the global listener to bail if the object is not in the list
            if (currentPointer != null && currentPointer.Result?.CurrentPointerTarget != null 
                && ContainsNode(currentPointer.Result.CurrentPointerTarget.transform))
            {
                if (isDragging)
                {
                    eventData.Use();
                }
                return;
            }
        }

        ///<inheritdoc/>
        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            IMixedRealityPointer p = PointerUtils.GetPointer<PokePointer>(eventData.Handedness);

            if (p != null)
            {
                //Quick check for the global listener to bail if the object is not in the list
                if (currentPointer == null || 
                    currentPointer.Result?.CurrentPointerTarget == null ||
                    !ContainsNode(p.Result.CurrentPointerTarget.transform) || initialFocusedObject != p.Result.CurrentPointerTarget)
                {
                    return;
                }

                if (p == currentPointer && isDragging)
                {
                    eventData.Use();
                }
            }
        }

        #endregion IMixedRealityTouchHandler implementation

        #region IMixedRealitySourceStateHandler implementation

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            //We'll consider this a drag release
            if (isEngaged && animateScroller == null && currentPointer != null && currentPointer.InputSourceParent.SourceId == eventData.SourceId)
            {
                if (isTouched || isDragging)
                {
                    //Its a drag release
                    initialScrollerPos = workingScrollerPos;
                }

                ResetState();

                velocityState = VelocityState.Calculating;
            }
        }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData)
        {
            if (shouldSwallowEvents)
            {
                //Prevents the handled event from PointerUp to continue propagating
                eventData.Use();
                shouldSwallowEvents = false;
            }
        }

        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData) { }

        #endregion IMixedRealitySourceStateHandler implementation
    }
}