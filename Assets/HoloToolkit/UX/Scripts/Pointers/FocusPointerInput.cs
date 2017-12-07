using HoloToolkit.Unity.InputModule;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif

namespace HoloToolkit.Unity.UX
{
    /// <summary>
    /// Routes controller input to a physics pointer
    /// </summary>
    public class FocusPointerInput : AttachToController
    {
        [SerializeField]
        [DropDownComponent(true,true)]
        private ControllerPointerBase pointer = null;
        [SerializeField]
        private InteractionSourcePressType activeHoldType = InteractionSourcePressType.Select;
        [SerializeField]
        private InteractionSourcePressType selectPressType = InteractionSourcePressType.Select;

        [SerializeField]
        private bool interactionRequiresHold = false;

        protected override void OnAttachToController()
        {
            if (pointer == null)
            {
                Debug.LogError("Pointer cannot be null.");
                return;
            }
            
            // Subscribe to interaction events
            InteractionManager.InteractionSourcePressed += InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionSourceReleased;
        }

        protected override void OnDetachFromController()
        {
            if (pointer == null)
            {
                Debug.LogError("Pointer cannot be null.");
                return;
            }

            // Subscribe to interaction events
            InteractionManager.InteractionSourcePressed -= InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased -= InteractionSourceReleased;
        }

        private void Update()
        {
            if (pointer.InteractionEnabled)
            {
                InputManager.Instance.ApplyEventOrigin(controller.SourceId, pointer.EventOrign);
            }
            else
            {
                InputManager.Instance.RemoveEventOrigin(controller.SourceId, pointer.EventOrign);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InputManager.Instance.RemoveEventOrigin(controller.SourceId, pointer.EventOrign);

        }

        /// <summary>
        /// Presses active
        /// </summary>
        /// <param name="obj"></param>
        private void InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
        {
            if ((InputModule.InteractionSourceHandedness)obj.state.source.handedness == handedness)
            {
                if (obj.pressType == activeHoldType && interactionRequiresHold)
                {
                    pointer.InteractionEnabled = true;
                }

                if (obj.pressType == selectPressType)
                {
                    pointer.OnSelectPressed();
                }
            }
        }

        /// <summary>
        /// Releases active
        /// </summary>
        /// <param name="obj"></param>
        private void InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
        {
            if ((InputModule.InteractionSourceHandedness)obj.state.source.handedness == handedness)
            {
                if (obj.pressType == activeHoldType && interactionRequiresHold)
                {
                    pointer.InteractionEnabled = false;
                }

                if (obj.pressType == selectPressType)
                {
                    pointer.OnSelectReleased();
                }
            }
        }

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(FocusPointerInput))]
        public class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}