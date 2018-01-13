using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.XR.WSA.Input;

namespace HoloToolkit.Unity.UX
{
    /// <summary>
    /// Routes controller input to a physics pointer
    /// </summary>
    public class NavigationPointerInput : AttachToController, IInputHandler
    {
        // Pressing 'forward' on the thumbstick gives us an angle that doesn't quite feel like
        // the forward direction, so we apply this offset to make navigation feel more natural
        const float thumbstickAngleOffset = -82.5f;

        [SerializeField]
        [DropDownComponent(true, true)]
        private ControllerPointerBase pointer = null;

        [SerializeField]
        private float thumbstickThreshold = 0.05f;

        private bool processingInput = false;
        private Vector2 thumbstickPosition = Vector2.zero;

        protected override void OnAttachToController()
        {
            if (pointer == null)
            {
                Debug.LogError("Pointer cannot be null.");
                return;
            }

            // Subscribe to interaction events
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        protected override void OnDetachFromController()
        {
            if (pointer == null)
            {
                Debug.LogError("Pointer cannot be null.");
                return;
            }

            // Unsubscribe from interaction events
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        private void Update()
        {
            processingInput = false;

            if (Mathf.Abs(thumbstickPosition.y) > thumbstickThreshold || Mathf.Abs(thumbstickPosition.x) > thumbstickThreshold)
            {
                // Get the angle of the pointer input
                float angle = Mathf.Atan2(thumbstickPosition.y, thumbstickPosition.x) * Mathf.Rad2Deg;
                // Offset the angle so it's 'forward' facing
                angle += thumbstickAngleOffset;
                pointer.PointerOrientation = angle;
                processingInput = true;
            }

            if (processingInput)
            {
                if (!pointer.InteractionEnabled)
                {
                    pointer.InteractionEnabled = true;
                    pointer.OnSelectPressed();
                }
            }
            else
            {
                if (pointer.InteractionEnabled)
                {
                    pointer.InteractionEnabled = false;
                    pointer.OnSelectReleased();
                }
            }
        }

        public void OnInputUp(InputEventData eventData) { }

        public void OnInputDown(InputEventData eventData) { }

        public void OnInputPressed(InputPressedEventData eventData) { }

        /// <summary>
        /// Updates target point orientation via thumbstick
        /// </summary>
        public void OnInputPositionChanged(InputPositionEventData eventData)
        {
            if (eventData.SourceId == pointer.SourceId)
            {
#if UNITY_WSA
                if (eventData.PressType == InteractionSourcePressType.Thumbstick)
#endif
                {
                    if (eventData.Handedness == handedness)
                    {
                        thumbstickPosition = eventData.InputPosition;
                    }
                }
            }
        }

        #region custom editor
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(NavigationPointerInput))]
        public class CustomEditor : MRTKEditor { }
#endif
        #endregion
    }
}