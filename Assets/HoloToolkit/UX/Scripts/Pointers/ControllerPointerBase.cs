using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.WSA.Input;

namespace HoloToolkit.Unity.UX
{
    public abstract class ControllerPointerBase : MonoBehaviour, InputModule.IPointingSource
    {
        public string EventOrign { get { return eventOrigin; } }

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
        protected InputModule.Cursor cursorOverride;
        [SerializeField]
        private string eventOrigin = "Pointer";

        // True if select is pressed right now
        protected bool selectPressed = false;
        // True if select has been pressed once since startup
        protected bool selectPressedOnce = false;

        protected virtual void OnEnable()
        {
            if (FocusManager.Instance != null)
            {
                FocusManager.Instance.RegisterFocuser(this);
            }

            selectPressed = false;
        }

        protected virtual void OnDisable()
        {
            if (FocusManager.Instance != null)
            {
                FocusManager.Instance.UnregisterFocuser(this);
            }

            selectPressed = false;
        }

        #region IPointingSource implementation

        public RayStep[] Rays
        {
            get
            {
                return rays;
            }
        }

        public virtual bool InteractionEnabled
        {
            get
            {
                return interactionEnabled;
            }
            set
            {
                interactionEnabled = value;
            }
        }

        public virtual float? ExtentOverride { get { return PointerExtent; } }

        public FocusResult Result { get; set; }

        public InputModule.Cursor CursorOverride
        {
            get
            {
                return cursorOverride;
            }
        }

        public LayerMask[] PrioritizedLayerMasksOverride { get { return prioritizedLayerMasksOverride; } }

        [SerializeField]
        protected LayerMask[] prioritizedLayerMasksOverride = new LayerMask[1] { new LayerMask() };

        protected RayStep[] rays = new RayStep[1] { new RayStep(Vector3.zero, Vector3.forward) };

        public bool FocusLocked { get; set; }

        public abstract void OnPreRaycast();

        public abstract void OnPostRaycast();

        public abstract bool OwnsInput(BaseInputEventData eventData);

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

        protected virtual void OnDrawGizmos()
        {

        }

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(ControllerPointerBase))]
        public class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}
