// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// A set of child objects organized in a series of Rows/Columns that can scroll in either the X or Y direction
    /// </summary>
    public class ScrollingObjectCollection : BaseObjectCollection, IMixedRealityPointerHandler, IMixedRealityTouchHandler, IMixedRealitySourceStateHandler
    {

        public enum VelocityType
        {
            FalloffPerFrame = 0,
            FalloffPerItem,
            NoVelocitySnapToItem,
            None
        }

        public enum ScrollDirectionType
        {
            UpAndDown = 0,
            LeftAndRight,
        }


        /// <summary>
        /// Enables/disables scrolling with near/far interaction 
        /// </summary>
        /// <remarks>Helpful for controls where you may want pagination or list movement without freeform scrolling.</remarks>
        [SerializeField]
        [Tooltip("Enables/disables scrolling with near/far interaction")]
        private bool canScroll = true;

        public bool CanScroll
        {
            get { return canScroll; }
            set { canScroll = value; }
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
        /// Number of lines visible in scroller. orthagonal to <see cref="tiers"/>
        /// </summary>
        [SerializeField]
        [Tooltip("Number of lines visible in scroller, orthagonal to tiers")]
        private int viewableArea = 4;

        public int ViewableArea
        {
            get => (viewableArea > 0) ? viewableArea : 1;
            set => viewableArea = value;
        }

        /// <summary>
        /// The amount of movement the user's pointer can make before its considerd a drag gesture
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of movement the user's pointer can make before its considerd a drag gesture")]
        [Range(0.0f, 2.0f)]
        private float handDeltaMagThreshold = 0.1f;

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
            get => dragTimeThreshold;
            set => dragTimeThreshold = value;
        }

        /// <summary>
        /// Determines whether a near scroll gesture is released when the engaged fingertip is dragged outside of the viewable area.
        /// </summary>
        [SerializeField]
        [Tooltip("Determines whether a near scroll gesture is released when the engaged fingertip is dragged outside of the viewable area.")]
        private bool useNearScrollBoundary = false;

        public bool UseNearScrollBoundary
        {
            get => useNearScrollBoundary;
            set => useNearScrollBoundary = value;
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
        /// Toggles whether the <see cref="ScrollingObjectCollection"/> will use the Camera <see cref="Camera.onPreRender"/> event to hide items in the list. The fallback is <see cref"MonoBehaviour.LateUpdate()"/>.
        /// </summary>
        /// <remarks>This is especially helpful if you're trying to scroll dynamically created objects that may be added to the list after <see cref"MonoBehaviour.LateUpdate()"/> such as <see cref"MonoBehaviour.OnWillRender()"/></remarks>
        [SerializeField]
        [Tooltip("Toggles whether the scrollingObjectCollection will use the Camera OnPreRender event to hide items in the list")]
        private bool useOnPreRender;

        public bool UseOnPreRender
        {
            get => useOnPreRender;
            set
            {
                if (cameraMethods == null)
                {
                    cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
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

        /// <summary>
        /// Amount of (extra) velocity to be applied to scroller.
        /// </summary>
        [SerializeField]
        [Tooltip("Amount of (extra) velocity to be applied to scroller")]
        [Range(0.0f, 2.0f)]
        private float velocityMultiplier = 0.8f;

        public float VelocityMultiplier
        {
            get => velocityMultiplier;
            set => velocityMultiplier = value;
        }

        /// <summary>
        /// Amount of falloff (drag) applied to velocity 
        /// </summary>
        /// <remarks>This can't be 0.0f since that won't allow ANY velocity, and it can't be 1.0f since that won't allow ANY drag.</remarks>
        [SerializeField]
        [Tooltip("Amount of falloff applied to velocity")]
        [Range(0.0001f, 0.9999f)]
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
        /// Animation curve used to interpolate the pagination and movement methods
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

        /// <summary>
        /// The amount of time the <see cref="PaginationCurve"/> will take to evaulaute
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of time the PaginationCurve will take to evaulaute")]
        private float animationLength = 0.25f;

        public float AnimationLength
        {
            get => (animationLength < 0) ? 0 : animationLength;
            set => animationLength = value;
        }

        /// <summary>
        /// Number of columns or rows in respect to <see cref="ViewableArea"/> and <see cref="ScrollDirection"/>
        /// </summary>
        [Tooltip("Number of columns or rows in respect to ViewableArea and ScrollDirection")]
        [SerializeField]
        [Range(1, 500)]
        private int tiers = 2;

        public int Tiers
        {
            get => (tiers > 0) ? tiers : 1;
            set => tiers = value;
        }

        /// <summary>
        /// Manual offset adjust the scale calculation of the <see cref="ClippingBox"/>
        /// </summary>
        /// <remarks>Setting to zero may result in z fighting."/></remarks>
        [SerializeField]
        [Tooltip("Manual offset adjust the scale calculation of the ClippingBox")]
        private Vector3 occlusionScalePadding = new Vector3(0.0f, 0.0f, 0.001f);

        public Vector3 OcclusionScalePadding
        {
            get => occlusionScalePadding;
            set => occlusionScalePadding = value;
        }

        /// <summary>
        /// Manual offset adjust the position calculation of the <see cref="ClippingBox"/>
        /// </summary>
        [SerializeField]
        [Tooltip("Manual offset adjust the position calculation of the ClippingBox")]
        private Vector3 occlusionPositionPadding = Vector3.zero;

        public Vector3 OcclusionPositionPadding
        {
            get => occlusionPositionPadding;
            set => occlusionPositionPadding = value;
        }

        /// <summary>
        /// Width of the cell per object in the collection.
        /// </summary>
        [Tooltip("Width of cell per object")]
        [SerializeField]
        [Range(0.00001f, 100.0f)]
        private float cellWidth = 0.25f;

        public float CellWidth
        {
            get => (cellWidth > 0) ? cellWidth : 0.00001f;
            set => cellWidth = value;
        }

        /// <summary>
        /// Height of the cell per object in the collection.
        /// </summary>
        [Tooltip("Height of cell per object")]
        [SerializeField]
        [Range(0.00001f, 100.0f)]
        private float cellHeight = 0.25f;

        public float CellHeight
        {
            get => (cellHeight > 0) ? cellHeight : 0.00001f;
            set => cellHeight = value;
        }

        /// <summary>
        /// Multiplier to add more bounce to the overscroll of a list when using <see cref="VelocityType.FalloffPerFrame"/> or <see cref="VelocityType.FalloffPerItem"/>.
        /// </summary>
        [SerializeField]
        [Tooltip("Multiplier to add more bounce to the overscroll of a list when using VelocityType.FalloffPerFrame or VelocityType.FalloffPerItem")]
        private float bounceMultiplier = 0.1f;

        public float BounceMultiplier
        {
            get => bounceMultiplier;
            set => bounceMultiplier = value;
        }

        /// <summary>
        /// The UnityEvent type the ScrollingObjectCollection sends 
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
        public int FirstItemInView
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

        //Half of a cell
        private Vector2 HalfCell;

        //Maximum amount the scroller can travel (vertically)
        private float maxY => NodeList.Count != 0 ? (StepMultiplier(NodeList.Count + Tiers - ModuloCheck(NodeList.Count, Tiers), Tiers) - ViewableArea) * CellHeight : 0.0f;

        //Minimum amount the scroller can travel (vertically) - this will always be zero. Here for readability
        private readonly float minY = 0.0f;

        //Maximum amount the scroller can travel (horizontally) - this will always be zero. Here for readability
        private readonly float maxX = 0.0f;

        //Minimum amount the scroller can travel (horizontally)
        private float minX => NodeList.Count != 0 ? -((StepMultiplier(NodeList.Count + Tiers - ModuloCheck(NodeList.Count, Tiers), Tiers) - ViewableArea) * CellWidth) : 0.0f;

        //item index for items that should be visible
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

        //The empty game object that contains our nodes and be scrolled
        [SerializeField]
        [HideInInspector]
        private GameObject scrollContainer;

        //The empty game object that contains the ClipppingBox
        [SerializeField]
        [HideInInspector]
        private GameObject clippingObject;

        /// <summary>
        /// The empty <see cref="GameObject"/> containing the <see cref="ScrollingObjectCollection"/>'s <see cref="ClippingBox"/>.
        /// </summary>
        public GameObject ClippingObject => clippingObject;

        //A reference to the ClippingBox on the clippingObject
        [SerializeField]
        [HideInInspector]
        private ClippingBox clipBox;

        /// <summary>
        /// The <see cref="ScrollingObjectCollection"/>'s <see cref="ClippingBox"/> that is used for clipping items in and out of the list.
        /// </summary>
        public ClippingBox ClipBox => clipBox;

        //The bounds of the clipping object, this is to make helper math easier later, it doesn't matter that its AABB since we're really not using it for bounds operations
        [SerializeField]
        [HideInInspector]
        private Bounds clippingBounds;

        // Tracks whether an item in the list is being interacted with
        private bool isEngaged = false;

        // Tracks whether a movement action resulted in dragging the list  
        private bool isDragging = false;

        // we need to know if the pointer was a touch so we can do the threshold test (dot product test)
        private bool isTouched = false;

        //The postion of the scollContainer before we do any updating to it
        private Vector3 initialScrollerPos;

        //The new of the scollContainer before we've set the postion / finished the updateloop
        private Vector3 workingScrollerPos;

        //The node in the list currently being interacted with
        private GameObject focusedObject;

        //A list of new child nodes that have new child renderers that need to be added to the clippingBox
        private List<ObjectCollectionNode> nodesToClip = new List<ObjectCollectionNode>();

        //A list of new child nodes that have new child renderers that need to be removed to the clippingBox
        private List<ObjectCollectionNode> nodesToUnclip = new List<ObjectCollectionNode>();

        #region scroll state variables

        private IMixedRealityPointer currentPointer;

        // The point where the original PointerDown occured
        private Vector3 pointerHitPoint;
        // The ray length of original pointer down
        private float pointerHitDistance;

        Vector3 currentPointerPos;

        /// <summary>
        /// Gets the cursor position (pointer end point) on the scrollable plane,
        /// projected onto the direction being scrolled.
        /// Returns false if the pointer is null or pointer details is null.
        /// </summary>
        private bool TryGetPointerPositionOnPlane(out Vector3 result)
        {
            if (currentPointer.GetType() == typeof(PokePointer))
            {
                result = currentPointer.Position;
                return true;
            }
            if (currentPointer?.Result?.Details != null) 
            { 
                var endPoint = RayStep.GetPointByDistance(currentPointer.Rays, pointerHitDistance);
                var scrollVector = (scrollDirection == ScrollDirectionType.UpAndDown) ? transform.up : transform.right;
                result =  pointerHitPoint + Vector3.Project(endPoint - pointerHitPoint, scrollVector);
                return true;
            } 
            result = Vector3.zero;
            return false;
        }

        #endregion scroll state variables

        #region drag position calculation variables

        //The simple value scroll movement delta
        private Vector3 handDelta;

        //Absolute value scroll axis delta
        private float absAxisHandDelta;

        //Hand position when starting a motion
        private Vector3 initialHandPos;

        //Hand position previous frame
        private Vector3 lastHandPos;

        //the amount for finalOffset to travel
        [SerializeField]
        [HideInInspector]
        private float thresholdOffset;

        //A point along the collectionForward of the scroll container to calculate the threshold point
        [SerializeField]
        [HideInInspector]
        private Vector3 finalOffset = new Vector3();

        //A point in front of the scroller to use for dot product comparison
        [SerializeField]
        [HideInInspector]
        private Vector3 thresholdPoint = new Vector3();

        //Current time at initial press
        private float initialPressTime;

        #endregion drag position calculation variables

        #region velocity calculation variables

        //simple velocity of the scroller: current - last / timeDelta
        private float scrollVelocity = 0.0f;

        //Filtered weight of scroll velocity
        private float avgVelocity = 0.0f;

        //How much we should filter the velocity - yes this is a magic number. Its been tuned so lets leave it.
        private readonly float velocityFilterWeight = 0.97f;

        //Simple state enum to handle velocity logic
        private enum VelocityState
        {
            None = 0,
            Resolving,
            Calculating,
            Bouncing
        }

        //Internal enum for tracking the velocity state of the list
        private VelocityState velocityState = VelocityState.None;

        //Pre calculated destination with velocity and falloff when using per item snapping
        private Vector3 velocityDestinationPos;

        //velocity container for storing previous filtered velocity
        private float velocitySnapshot;

        #endregion velocity calculation variables

        //Whether a Animation CoRoutine is running / animating the scrollContainer
        private bool animatingToPosition = false;

        //the Animation CoRoutine
        private IEnumerator AnimateScroller;

        #region ObjectCollection methods

        /// <inheritdoc/>
        public override void UpdateCollection()
        {
            //Generate our scroll specific objects

            //scrollContainer empty game object null check - ensure its set up properly
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

            //ClippingObject empty game object null check - ensure its set up properly
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

            //ClippingBox  component null check - ensure its set up properly
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

                    //Subscribe to the preRender callback on the main camera so we can intercept it and make sure we catch
                    //any dynamically created children in our list
                    cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
                    cameraMethods.OnCameraPreRender += OnCameraPreRender;
                }
            }

            //ensure IgnoreInactiveTransforms is set to false, otherwise the node prune will remove any hidden items in the list.g
            IgnoreInactiveTransforms = false;
            //stash our children in a list so the count doesn't change or reverse the order if we were to count down
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child != scrollContainer.transform && child != clippingObject.transform)
                {
                    children.Add(child);
                }
            }

            //move any objects to the scrollContainer
            for (int i = 0; i < children.Count; i++)
            {
                children[i].parent = scrollContainer.transform;
            }

            // Check for empty nodes and remove them
            List<ObjectCollectionNode> emptyNodes = new List<ObjectCollectionNode>();

            for (int i = 0; i < NodeList.Count; i++)
            {
                //Make sure we respect our special scroll objects
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
                    NodeList.Add(new ObjectCollectionNode { Name = child.name, Transform = child, GameObject = child.gameObject, Colliders = child.GetComponentsInChildren<Collider>() });
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

        /// <summary>
        /// Arranges our child objects in the scrollContainer per our set up instructions
        /// The layout method uses modulo with Columns / Rows
        /// </summary>
        protected override void LayoutChildren()
        {
            HalfCell = new Vector2(CellWidth * 0.5f, CellHeight * 0.5f);

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
                    newPos.x = (ModuloCheck(i, Tiers) != 0) ? (ModuloCheck(i, Tiers) * CellWidth) + HalfCell.x : HalfCell.x;
                    newPos.y = ((StepMultiplier(i, Tiers) * CellHeight) + HalfCell.y) * -1;
                    newPos.z = 0.0f;

                }
                else //left or right
                {
                    newPos.x = (StepMultiplier(i, Tiers) * CellWidth) + HalfCell.x;
                    newPos.y = ((ModuloCheck(i, Tiers) != 0) ? (ModuloCheck(i, Tiers) * CellHeight) + HalfCell.y : HalfCell.y) * -1;
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

            //temporarily turn on the first item in the list if its inactive
            bool resetActiveState = false;
            if (!NodeList[FirstItemInView].Transform.gameObject.activeSelf)
            {
                resetActiveState = true;
                NodeList[FirstItemInView].Transform.gameObject.SetActive(true);
            }

            //create the offset for our thesholdCalculation -- grab the first item in the list

            BoundsExtensions.TryGetObjectAlignedBoundsSize(NodeList[FirstItemInView].Transform, out Vector3 offsetSize);
            thresholdOffset = offsetSize.z * 0.5f;

            //get a point in front of the scrollContainer to use for the dot product check
            finalOffset = (Vector3.forward * -1.0f) * thresholdOffset;
            thresholdPoint = transform.TransformPoint(finalOffset);

            //Use the first element for collection bounds for occluder positioning
            //temporarily zero out the rotation so we can get an accurate bounds
            Quaternion origRot = NodeList[FirstItemInView].Transform.rotation;
            NodeList[FirstItemInView].Transform.rotation = Quaternion.identity;

            clippingBounds.size = Vector3.zero;

            List<Vector3> boundsPoints = new List<Vector3>();
            BoundsExtensions.GetColliderBoundsPoints(NodeList[FirstItemInView].GameObject, boundsPoints, 0);

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

            //put the rotation back
            NodeList[FirstItemInView].Transform.rotation = origRot;

            //set the first item back to its original state
            if (resetActiveState)
            {
                NodeList[FirstItemInView].Transform.gameObject.SetActive(false);
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
                    clipBox.transform.localScale = VectorExtensions.ScaleFromBounds(new Bounds(Vector3.zero, Vector3.one), clippingBounds, OcclusionScalePadding);

                    //adjust where the center of the clipping box is
                    viewableCenter.x = (clipBox.transform.localScale.x * 0.5f) - (OcclusionScalePadding.x * 0.5f) + OcclusionPositionPadding.x;
                    viewableCenter.y = (((clipBox.transform.localScale.y * 0.5f) - (OcclusionScalePadding.y * 0.5f)) * -1) + OcclusionPositionPadding.y;
                    viewableCenter.z = OcclusionPositionPadding.z;
                    break;

                case ScrollDirectionType.LeftAndRight:

                    //Same as above for L <-> R
                    clippingBounds.size = new Vector3(clippingBounds.size.x * ViewableArea, clippingBounds.size.y * Tiers, clippingBounds.size.z);
                    clipBox.transform.localScale = VectorExtensions.ScaleFromBounds(new Bounds(Vector3.zero, Vector3.one), clippingBounds) + OcclusionScalePadding;

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

        #region Monobehavior Implementation

        private void OnEnable()
        {
            //Register for global input events
            MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out IMixedRealityInputSystem inputSys);

            if (inputSys != null)
            {
                inputSys.RegisterHandler<IMixedRealityTouchHandler>(this);
                inputSys.RegisterHandler<IMixedRealityPointerHandler>(this);
            }

            if (useOnPreRender)
            {
                if (clippingObject == null || clipBox == null)
                {
                    UpdateCollection();
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

            if (useOnPreRender && clipBox != null)
            {
                clipBox.UseOnPreRender = true;

                //Subscribe to the preRender callback on the main camera so we can intercept it and make sure we catch
                //any dynamically created children in our list
                cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
                cameraMethods.OnCameraPreRender += OnCameraPreRender;
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

            //recalcualte every frame to prevent any weirdness from moving the scrolling list.
            thresholdPoint = transform.TransformPoint(finalOffset);

            //The scroller has detected input and has a valid pointer
            if (isEngaged && TryGetPointerPositionOnPlane(out currentPointerPos))
            {
                handDelta = initialHandPos - currentPointerPos;

                //Lets see if this is gonna be a click or a drag
                //Check the scroller's length state to prevent resetting calculation
                if (!isDragging && nodeLengthCheck)
                {
                    //grab the delta value we care about
                    absAxisHandDelta = (scrollDirection == ScrollDirectionType.UpAndDown) ? Mathf.Abs(handDelta.y) : Mathf.Abs(handDelta.x);

                    //Catch an intentional finger in scroller to stop momentum, this isn't a drag its definitely a stop
                    if (absAxisHandDelta > (handDeltaMagThreshold * 0.1f) || TimeTest(initialPressTime, Time.time, dragTimeThreshold))
                    {
                        scrollVelocity = 0.0f;
                        avgVelocity = 0.0f;

                        isDragging = true;
                        velocityState = VelocityState.None;

                        //reset initialHandPos to prevent the scroller from jumping
                        initialScrollerPos = scrollContainer.transform.localPosition;
                        initialHandPos = currentPointerPos;
                    }
                }

                //Make sure we're actually (near) touched and not a pointer event, do a dot product check            
                bool scrollRelease = UseNearScrollBoundary ? DetectScrollRelease(transform.forward * -1.0f, thresholdPoint, currentPointerPos, clippingObject.transform, transform.worldToLocalMatrix, scrollDirection)
                                                           : DetectScrollRelease(transform.forward * -1.0f, thresholdPoint, currentPointerPos, null, null, null);

                if (isTouched && scrollRelease)
                {
                    //We're on the other side of the original touch position. This is a release.
                    if (!isDragging)
                    {
                        //Its a click release
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
                            //Fire the UnityEvent
                            ClickEvent?.Invoke(focusedObject);
                        }
                    }
                    else
                    {
                        //Its a drag release
                        initialScrollerPos = workingScrollerPos;
                        velocityState = VelocityState.Calculating;
                    }

                    //Release the pointer
                    if (currentPointer.GetType() != typeof(PokePointer))
                    {
                        currentPointer = null;
                    }

                    //Let everyone know the scroller is no longer engaged
                    TouchEnded?.Invoke(focusedObject);

                    focusedObject = null;

                    //Clear our states
                    isTouched = false;
                    isEngaged = false;
                    isDragging = false;

                }
                else if (isDragging && canScroll)
                {

                    if (scrollDirection == ScrollDirectionType.UpAndDown)
                    {
                        //Lock X, clamp Y
                        workingScrollerPos.y = MathUtilities.SoftClamp(initialScrollerPos.y - handDelta.y, minY, maxY, 0.5f);
                        workingScrollerPos.x = 0.0f;
                    }
                    else
                    {
                        //Lock Y, clamp X
                        workingScrollerPos.x = MathUtilities.SoftClamp(initialScrollerPos.x - handDelta.x, minX, maxX, 0.5f);
                        workingScrollerPos.y = 0.0f;
                    }

                    //Update the scrollContainer Position
                    ApplyPosition(workingScrollerPos);

                    CalculateVelocity();

                    //Update the prev val for velocity
                    lastHandPos = currentPointerPos;
                }
            }
            else if (!animatingToPosition && nodeLengthCheck)//Prevent the Animation coroutine from being overridden
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
            MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out IMixedRealityInputSystem inputSys);
            if (inputSys != null)
            {
                inputSys.UnregisterHandler<IMixedRealityTouchHandler>(this);
                inputSys.UnregisterHandler<IMixedRealityPointerHandler>(this);
            }

            if (useOnPreRender && cameraMethods != null)
            {
                CameraEventRouter cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
                cameraMethods.OnCameraPreRender -= OnCameraPreRender;
            }
        }

        #endregion Monobehavior Implementation

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
                        workingScrollerPos.y = (Mathf.Floor(scrollContainer.transform.localPosition.y / CellHeight)) * CellHeight;
                    }
                    else
                    {
                        workingScrollerPos.x = (Mathf.Floor(scrollContainer.transform.localPosition.x / CellWidth)) * CellWidth;
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
                            //calculate velocity one more time to prevent any sort of edge case where we didn't have enough frames to calculate properly
                            CalculateVelocity();

                            //Precalculate where the velocity falloff WOULD land our scrollContainer, then round it to the nearest item so it feels "natural"
                            velocitySnapshot = IterateFalloff(avgVelocity, out numSteps);
                            newPosAfterVelocity = initialScrollerPos.y + velocitySnapshot;
                        }

                        velocityDestinationPos.y = (Mathf.Floor(newPosAfterVelocity / CellHeight)) * CellHeight;

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
                            //calculate velocity one more time to prevent any sort of edge case where we didn't have enough frames to calculate properly
                            CalculateVelocity();

                            //Precalculate where the velocity falloff WOULD land our scrollContainer, then round it to the nearest item so it feels "natural"
                            velocitySnapshot = IterateFalloff(avgVelocity, out numSteps);
                            newPosAfterVelocity = initialScrollerPos.x + velocitySnapshot;
                        }

                        velocityDestinationPos.x = (Mathf.Floor(newPosAfterVelocity / CellWidth)) * CellWidth;

                        velocityState = VelocityState.Resolving;
                    }

                    workingScrollerPos = Solver.SmoothTo(scrollContainer.transform.localPosition, velocityDestinationPos, Time.deltaTime, 0.9275f);

                    //Clear the velocity now that we've applied a new position
                    avgVelocity = 0.0f;
                    break;

                case VelocityState.Resolving:

                    if (scrollDirection == ScrollDirectionType.UpAndDown)
                    {
                        if (scrollContainer.transform.localPosition.y > maxY + (thresholdOffset * bounceMultiplier)
                            || scrollContainer.transform.localPosition.y < minY - (thresholdOffset * bounceMultiplier))
                        {
                            velocityState = VelocityState.Bouncing;
                            velocitySnapshot = 0.0f;
                            break;
                        }
                        else
                        {
                            workingScrollerPos = Solver.SmoothTo(scrollContainer.transform.localPosition, velocityDestinationPos, Time.deltaTime, 0.9275f);

                            if (Vector3.Distance(scrollContainer.transform.localPosition, workingScrollerPos) < 0.00001f)
                            {
                                //Ensure we've actually snapped the position to prevent an extreme in-between state
                                workingScrollerPos.y = (Mathf.Floor(scrollContainer.transform.localPosition.y / CellHeight)) * CellHeight;

                                velocityState = VelocityState.None;

                                ListMomentumEnded?.Invoke();

                                // clean up our position for next frame
                                initialScrollerPos = workingScrollerPos;
                            }
                        }
                    }
                    else
                    {
                        if (scrollContainer.transform.localPosition.x > maxX + (thresholdOffset * bounceMultiplier)
                            || scrollContainer.transform.localPosition.x < minX - (thresholdOffset * bounceMultiplier))
                        {
                            velocityState = VelocityState.Bouncing;
                            velocitySnapshot = 0.0f;
                            break;
                        }
                        else
                        {
                            workingScrollerPos = Solver.SmoothTo(scrollContainer.transform.localPosition, velocityDestinationPos, Time.deltaTime, 0.9275f);

                            if (Vector3.Distance(scrollContainer.transform.localPosition, workingScrollerPos) < 0.00001f)
                            {
                                //Ensure we've actually snapped the position to prevent an extreme in-between state
                                workingScrollerPos.y = (Mathf.Floor(scrollContainer.transform.localPosition.x / CellWidth)) * CellWidth;

                                velocityState = VelocityState.None;

                                ListMomentumEnded?.Invoke();

                                // clean up our position for next frame
                                initialScrollerPos = workingScrollerPos;
                            }
                        }
                    }
                    break;

                case VelocityState.Bouncing:
                    bool smooth = false;
                    if (Vector3.Distance(scrollContainer.transform.localPosition, workingScrollerPos) < 0.00001f)
                    {
                        smooth = true;
                    }
                    if (scrollDirection == ScrollDirectionType.UpAndDown
                        && (scrollContainer.transform.localPosition.y - minY > -0.00001f
                        && scrollContainer.transform.localPosition.y - maxY < 0.00001f))
                    {
                        velocityState = VelocityState.None;

                        ListMomentumEnded?.Invoke();

                        // clean up our position for next frame
                        initialScrollerPos = workingScrollerPos;
                    }
                    else if (scrollDirection == ScrollDirectionType.LeftAndRight
                             && (scrollContainer.transform.localPosition.x - minX > -0.00001f
                             && scrollContainer.transform.localPosition.x + maxX < 0.00001f))
                    {
                        velocityState = VelocityState.None;

                        ListMomentumEnded?.Invoke();

                        // clean up our position for next frame
                        initialScrollerPos = workingScrollerPos;
                    }
                    else
                    {
                        smooth = true;
                    }

                    if (smooth)
                    {
                        Vector3 clampedDest = new Vector3(Mathf.Clamp(scrollContainer.transform.localPosition.x, minX, maxX), Mathf.Clamp(scrollContainer.transform.localPosition.y, minY, maxY), 0.0f);
                        workingScrollerPos.y = Solver.SmoothTo(scrollContainer.transform.localPosition, clampedDest, Time.deltaTime, 0.2f).y;
                        workingScrollerPos.x = Solver.SmoothTo(scrollContainer.transform.localPosition, clampedDest, Time.deltaTime, 0.2f).x;
                    }

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
                        if (scrollContainer.transform.localPosition.y > maxY + (thresholdOffset * bounceMultiplier)
                            || scrollContainer.transform.localPosition.y < minY - (thresholdOffset * bounceMultiplier))
                        {
                            velocityState = VelocityState.Bouncing;
                            avgVelocity = 0.0f;
                            break;
                        }
                        else
                        {
                            avgVelocity *= velocityFalloff;
                            workingScrollerPos.y = initialScrollerPos.y + avgVelocity;

                            if (Vector3.Distance(scrollContainer.transform.localPosition, workingScrollerPos) < 0.00001f)
                            {
                                velocityState = VelocityState.None;
                                avgVelocity = 0.0f;

                                ListMomentumEnded?.Invoke();
                            }
                        }
                    }
                    else
                    {
                        if (scrollContainer.transform.localPosition.x > maxX + (thresholdOffset * bounceMultiplier)
                            || scrollContainer.transform.localPosition.x < minX - (thresholdOffset * bounceMultiplier))
                        {
                            velocityState = VelocityState.Bouncing;
                            avgVelocity = 0.0f;
                            break;
                        }
                        else
                        {

                            avgVelocity *= velocityFalloff;
                            workingScrollerPos.x = initialScrollerPos.x + avgVelocity;

                            if (Vector3.Distance(scrollContainer.transform.localPosition, workingScrollerPos) < 0.00001f)
                            {
                                velocityState = VelocityState.None;
                                avgVelocity = 0.0f;

                                ListMomentumEnded?.Invoke();
                            }
                        }
                    }

                    // clean up our position for next frame
                    initialScrollerPos = workingScrollerPos;

                    break;

                case VelocityState.Bouncing:

                    bool smooth = false;

                    if (Vector3.Distance(scrollContainer.transform.localPosition, workingScrollerPos) < 0.00001f)
                    {
                        smooth = true;
                    }
                    if (scrollDirection == ScrollDirectionType.UpAndDown
                    && (scrollContainer.transform.localPosition.y - minY > 0.00001f
                    && scrollContainer.transform.localPosition.y - maxY < 0.00001f))
                    {
                        velocityState = VelocityState.None;

                        ListMomentumEnded?.Invoke();

                        // clean up our position for next frame
                        initialScrollerPos = workingScrollerPos;
                    }
                    else if (scrollDirection == ScrollDirectionType.LeftAndRight
                             && (scrollContainer.transform.localPosition.x + minX > -0.00001f
                             && scrollContainer.transform.localPosition.x - maxX > 0.00001f))
                    {
                        velocityState = VelocityState.None;

                        ListMomentumEnded?.Invoke();

                        // clean up our position for next frame
                        initialScrollerPos = workingScrollerPos;
                    }
                    else
                    {
                        smooth = true;
                    }

                    if (smooth)
                    {
                        Vector3 clampedDest = new Vector3(Mathf.Clamp(scrollContainer.transform.localPosition.x, minX, maxX), Mathf.Clamp(scrollContainer.transform.localPosition.y, minY, maxY), 0.0f);
                        workingScrollerPos.y = Solver.SmoothTo(scrollContainer.transform.localPosition, clampedDest, Time.deltaTime, 0.2f).y;
                        workingScrollerPos.x = Solver.SmoothTo(scrollContainer.transform.localPosition, clampedDest, Time.deltaTime, 0.2f).x;
                    }
                    break;
            }
        }

        /// <summary>
        /// Wrapper for per frame velocity calculation and filtering.
        /// </summary>
        private void CalculateVelocity()
        {
            //update simple velocity
            scrollVelocity = (scrollDirection == ScrollDirectionType.UpAndDown)
                             ? (currentPointerPos.y - lastHandPos.y) / Time.deltaTime * (velocityMultiplier * 0.01f)
                             : (currentPointerPos.x - lastHandPos.x) / Time.deltaTime * (velocityMultiplier * 0.01f);

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
        /// <returns></returns>
        private IEnumerator AnimateTo(Vector3 initialPos, Vector3 finalPos, AnimationCurve curve = null, float? time = null, System.Action callback = null)
        {
            animatingToPosition = true;
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

            animatingToPosition = false;

            if (callback != null)
            {
                callback?.Invoke();
            }
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
            //true if finger is on the other side (Z) of the initial contact point of the collection
            if (pointToCompare.IsOtherSideOfPoint(initialDirection, initialPosition))
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
                    hasPassedBoundary = (posToClip.y > halfScale.y || posToClip.y < -halfScale.y) ? true : false;
                }
                else
                {
                    hasPassedBoundary = (posToClip.x > halfScale.x || posToClip.x < -halfScale.x) ? true : false;
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
        /// <param name="itemIndex">Index of node item in <see cref="BaseObjectCollection.NodeList"/> to be compared</param>
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
        /// <param name="itemIndex">Index of node item in <see cref="BaseObjectCollection.NodeList"/> to be compared</param>
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
            int col = Tiers;

            for (int i = 0; i < listLength; i++)
            {
                ObjectCollectionNode node = NodeList[i];
                //hide the items that have no chance of being seen
                if (i < prevItems - col || i > postItems + col)
                {
                    //quick check to cut down on the redundant calls
                    if (node.GameObject.activeSelf)
                    {
                        node.GameObject.SetActive(false);
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
                    //Disable colliders on items that will be scrolling in and out of view
                    if (i < prevItems || i > postItems)
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
                    if (!node.GameObject.activeSelf)
                    {
                        node.GameObject.SetActive(true);
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
        /// <returns>The total distance the <see cref="avgVelocity"/> with <see cref="velocityFalloff"/> as drag would travel.</returns>
        private float IterateFalloff(float vel, out int steps)
        {
            //Some day this should be a falloff formula, below is the number of steps. Just can't figure out how to get the right velocity.
            //float numSteps = (Mathf.Log(0.00001f)  - Mathf.Log(Mathf.Abs(avgVelocity))) / Mathf.Log(velocityFalloff);

            float newVal = 0.0f;
            float v = vel;
            steps = 0;

            while (Mathf.Abs(v) > 0.00001)
            {
                v *= velocityFalloff;
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
        /// Helper get near hand joint in world space
        /// </summary>
        /// <param name="joint">Joint to get</param>
        /// <param name="handedness">Desired hand of <see cref="TrackedHandJoint"/></param>
        /// <returns></returns>
        private Vector3 UpdateFingerPosition(TrackedHandJoint joint, Handedness handedness)
        {
            if (HandJointUtils.TryGetJointPose(joint, handedness, out MixedRealityPose handJoint))
            {
                return handJoint.Position;
            }
            return Vector3.zero;
        }

        #endregion private methods

        #region public methods

        /// <summary>
        /// Checks whether the given item is visible in the list
        /// </summary>
        /// <param name="indexOfItem">the index of the item in the list</param>
        /// <returns><see cref="true"/> when item is visible</returns>
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
                //its below the visable area
                itemLoc = false;
            }
            return itemLoc;
        }

        /// <summary>
        /// Moves scroller by a multiplier of <see cref="ViewableArea"/>
        /// </summary>
        /// <param name="numOfPages">number of <see cref="ViewableArea"/> to move scroller by</param>
        /// <param name="animateToPage">if <see cref="true"/>, scroller will animate to new position</param>
        /// <param name="callback"> An optional action to pass in to get notified that the <see cref="ScrollingObjectCollection"/> is finished moving</param>
        public void PageBy(int numOfPages, bool animateToPage = true, System.Action callback = null)
        {
            StopAllCoroutines();
            animatingToPosition = false;
            velocityState = VelocityState.None;

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = Mathf.Clamp((FirstItemInView + (numOfPages * ViewableArea)) * CellHeight, minY, maxY);
                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = Mathf.Clamp((FirstItemInView + (numOfPages * ViewableArea)) * CellWidth, minX, maxX);
                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPage)
            {
                AnimateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength, callback);
                StartCoroutine(AnimateScroller);
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
        /// <param name="animateToPosition">if <see cref="true"/>, scroller will animate to new position</param>
        /// <param name="callback"> An optional action to pass in to get notified that the <see cref="ScrollingObjectCollection"/> is finished moving</param>
        public void MoveByItems(int numberOfItemsToMove, bool animateToPosition = true, System.Action callback = null)
        {
            StopAllCoroutines();
            animatingToPosition = false;
            velocityState = VelocityState.None;

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = (FirstItemInView + StepMultiplier(numberOfItemsToMove, Tiers)) * CellHeight;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = ((FirstItemInView + StepMultiplier(numberOfItemsToMove, Tiers)) * CellWidth) * -1.0f;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPosition)
            {
                AnimateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength);
                StartCoroutine(AnimateScroller);
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
        /// <param name="animateToPosition">if <see cref="true"/>, scroller will animate to new position</param>
        /// <param name="callback"> An optional action to pass in to get notified that the <see cref="ScrollingObjectCollection"/> is finished moving</param>
        public void MoveByLines(int numberOfLinesToMove, bool animateToPosition = true, System.Action callback = null)
        {
            StopAllCoroutines();
            animatingToPosition = false;
            velocityState = VelocityState.None;

            if (scrollDirection == ScrollDirectionType.UpAndDown)
            {
                workingScrollerPos.y = (FirstItemInView + numberOfLinesToMove) * CellHeight;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.y = Mathf.Clamp(workingScrollerPos.y, minY, maxY);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.up);
            }
            else
            {
                workingScrollerPos.x = ((FirstItemInView + numberOfLinesToMove) * CellWidth) * -1.0f;

                //clamp the working pos since we already have calculated it
                workingScrollerPos.x = Mathf.Clamp(workingScrollerPos.x, minX, maxX);

                //zero out the other axes
                workingScrollerPos = workingScrollerPos.Mul(Vector3.right);
            }

            if (animateToPosition)
            {
                AnimateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength, callback);
                StartCoroutine(AnimateScroller);
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
        /// Moves scroller to an absolute position where <param name"indexOfItem"/> is in the first column of the viewable area
        /// </summary>
        /// <param name="indexOfItem">item to move to, will be first (or closest to in respect to scroll maxiumum) in viewable area</param>
        /// <param name="animateToPosition">if <see cref="true"/>, scroller will animate to new position</param>
        /// <param name="callback"> An optional action to pass in to get notified that the <see cref="ScrollingObjectCollection"/> is finished moving</param>
        public void MoveTo(int indexOfItem, bool animateToPosition = true, System.Action callback = null)
        {
            StopAllCoroutines();
            animatingToPosition = false;
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
                AnimateScroller = AnimateTo(scrollContainer.transform.localPosition, workingScrollerPos, paginationCurve, animationLength, callback);
                StartCoroutine(AnimateScroller);
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
        /// <returns><see cref="true"/> if amount of time surpasses <paramref name="pressMargin"/></returns>
        public static bool TimeTest(float initTime, float currTime, float pressMargin)
        {
            if (currTime - initTime > pressMargin)
            {
                return true;
            }
            return false;
        }

        #endregion public methods

        #region IMixedRealityPointerHandler implementation

        ///</inheritdoc>
        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
        {
            //Quick check for the global listener to bail if the object is not in the list
            if (eventData.Pointer.Result.CurrentPointerTarget == null || !ContainsNode(eventData.Pointer.Result.CurrentPointerTarget.transform))
            {
                return;
            }

            if (!isTouched && isEngaged && !animatingToPosition)
            {
                //Its a drag release
                initialScrollerPos = workingScrollerPos;

                //Release the pointer
                currentPointer.IsTargetPositionLockedOnFocusLock = true;
                currentPointer = null;

                //Clear our states
                isTouched = false;
                isEngaged = false;
                isDragging = false;
            }
        }

        ///</inheritdoc>
        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
        {
            //Do a check to prevent a new touch event from overriding our current touch values
            if (!isEngaged && !animatingToPosition)
            {
                if (eventData.Pointer.Controller.IsPositionAvailable)
                {
                    //Quick check for the global listener to bail if the object is not in the list
                    if (eventData.Pointer.Result.CurrentPointerTarget == null || !ContainsNode(eventData.Pointer.Result.CurrentPointerTarget.transform))
                    {
                        return;
                    }

                    currentPointer = eventData.Pointer;

                    currentPointer.IsTargetPositionLockedOnFocusLock = false;

                    pointerHitPoint = currentPointer.Result.Details.Point;
                    pointerHitDistance = currentPointer.Result.Details.RayDistance;

                    focusedObject = eventData.Pointer.Result.CurrentPointerTarget;


                    //Reset the scroll state
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
                    Debug.Log(name + " intercepted a pointer from " + gameObject.name + ". " + eventData.Pointer.PointerName + ", but don't know what to do with it.");
                }
            }
        }

        ///</inheritdoc>
        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            //we ignore this event and calculate click in the Update() loop;
        }

        #endregion IMixedRealityPointerHandler implementation

        #region IMixedRealityTouchHandler implementation

        ///</inheritdoc>
        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            currentPointer = PointerUtils.GetPointer<PokePointer>(eventData.Handedness);
            if (currentPointer != null)
            {
                //Quick check for the global listener to bail if the object is not in the list
                if (!ContainsNode(currentPointer.Result.CurrentPointerTarget.transform))
                {
                    return;
                }

                StopAllCoroutines();
                animatingToPosition = false;

                focusedObject = currentPointer.Result.CurrentPointerTarget;

                //Let everyone know the scroller has been engaged
                TouchStarted?.Invoke(focusedObject);

                if (focusedObject != currentPointer.Result.CurrentPointerTarget || focusedObject == null) { }

                if (!isTouched && !isEngaged)
                {
                    initialHandPos = UpdateFingerPosition(TrackedHandJoint.IndexTip, eventData.Controller.ControllerHandedness);
                    initialPressTime = Time.time;

                    initialScrollerPos = scrollContainer.transform.localPosition;

                    isTouched = true;
                    isEngaged = true;
                    isDragging = false;
                }
            }

        }

        ///</inheritdoc>
        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            //Quick check for the global listener to bail if the object is not in the list
            if (!ContainsNode(currentPointer.Result.CurrentPointerTarget.transform))
            {
                if (isDragging)
                {
                    eventData.Use();
                }
                return;
            }
        }

        ///</inheritdoc>
        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            IMixedRealityPointer p = PointerUtils.GetPointer<PokePointer>(eventData.Handedness);

            if (p != null)
            {
                //Quick check for the global listener to bail if the object is not in the list
                if (!ContainsNode(p.Result.CurrentPointerTarget.transform))
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
            if (isEngaged && !animatingToPosition)
            {
                if (isTouched)
                {
                    //Its a drag release
                    initialScrollerPos = workingScrollerPos;

                }

                if (isDragging)
                {
                    initialScrollerPos = workingScrollerPos;
                }

                //Release the pointer
                if (currentPointer.GetType() != typeof(PokePointer))
                {
                    currentPointer = null;
                }

                //Let everyone know the scroller is no longer engaged
                TouchEnded?.Invoke(focusedObject);

                //Clear our states
                isTouched = false;
                isEngaged = false;
                isDragging = false;

                velocityState = VelocityState.Calculating;
            }
        }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        #endregion IMixedRealitySourceStateHandler implementation
    }
}