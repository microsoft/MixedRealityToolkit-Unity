using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    public abstract class ControllerPointerBase : BaseInputSource, IPointingSource
    {
        [Header("Interaction")]
        [SerializeField]
        protected bool interactionEnabled = true;
        [SerializeField]
        [Range(0f, 360f)]
        protected float pointerOrientation = 0f;
        [SerializeField]
        [Range(0.5f, 50f)]
        protected float pointerExtent = 2f;
        [Tooltip("Source transform for raycast origin - leave null to use default transform")]
        [SerializeField]
        protected Transform raycastOrigin;
        [SerializeField]
        protected BaseCursor cursorOverride;

        // True if select is pressed right now
        protected bool selectPressed = false;
        // True if select has been pressed once since startup
        protected bool selectPressedOnce = false;

        protected virtual void OnEnable()
        {
            selectPressed = false;
        }

        protected virtual void OnDisable()
        {
            selectPressed = false;
        }

        #region IPointingSource implementation

        float? IPointingSource.ExtentOverride { get; set; }

        public RayStep[] Rays { get { return rays; } }

        LayerMask[] IPointingSource.PrioritizedLayerMasksOverride { get; set; }
        public IFocusHandler FocusTarget { get; set; }

        public BaseCursor BaseCursor { get; set; }
        public CursorModifier CursorModifier { get; set; }

        public virtual bool InteractionEnabled
        {
            get { return interactionEnabled; }
            set { interactionEnabled = value; }
        }

        public virtual float? ExtentOverride { get { return PointerExtent; } }

        public PointerResult Result { get; set; }

        public BaseRayStabilizer RayStabilizer { get; set; }

        public BaseCursor CursorOverride { get { return cursorOverride; } }

        [SerializeField]
        protected LayerMask[] prioritizedLayerMasksOverride = new LayerMask[1] { new LayerMask() };

        public LayerMask[] PrioritizedLayerMasksOverride { get { return prioritizedLayerMasksOverride; } }

        protected RayStep[] rays = new RayStep[1] { new RayStep(Vector3.zero, Vector3.forward) };

        public bool FocusLocked { get; set; }

        public abstract void OnPreRaycast();

        public abstract void OnPostRaycast();

        public bool TryGetPointerPosition(out Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPointingRay(out Ray pointingRay)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPointerRotation(out Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Call to initiate a select action for this pointer
        /// </summary>
        public virtual void OnSelectPressed()
        {
            selectPressed = true;
            selectPressedOnce = true;
        }

        public virtual void OnSelectReleased()
        {
            selectPressed = false;
        }

        /// <summary>
        /// The world origin of the targeting ray
        /// </summary>
        public virtual Vector3 PointerOrigin
        {
            get { return raycastOrigin != null ? raycastOrigin.position : transform.position; }
        }

        /// <summary>
        /// The forward direction of the targeting ray
        /// </summary>
        public virtual Vector3 PointerDirection
        {
            get { return raycastOrigin != null ? raycastOrigin.forward : transform.forward; }
        }

        /// <summary>
        /// The Y orientation of the pointer target - used for touchpad rotation and navigation
        /// </summary>
        public virtual float PointerOrientation
        {
            get
            {
                return pointerOrientation + (raycastOrigin != null ? raycastOrigin.eulerAngles.y : transform.eulerAngles.y);
            }
            set
            {
                pointerOrientation = value;
            }
        }

        public virtual float PointerExtent { get { return pointerExtent; } }

        [SerializeField]
        private SupportedInputInfo supportedInputInfo;

        public override SupportedInputInfo GetSupportedInputInfo()
        {
            return supportedInputInfo;
        }

        protected virtual void OnDrawGizmos() { }

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(ControllerPointerBase))]
        public class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}
