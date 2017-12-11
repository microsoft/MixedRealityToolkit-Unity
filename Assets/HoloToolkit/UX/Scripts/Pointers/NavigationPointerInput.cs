using HoloToolkit.Unity.Controllers;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.UX
{
    /// <summary>
    /// Routes controller input to a physics pointer
    /// </summary>
    public class NavigationPointerInput : AttachToController, IControllerInputHandler
    {
        // Pressing 'forward' on the thumbstick gives us an angle that doesn't quite feel like
        // the forward direction, so we apply this offset to make navigation feel more natural
        const float thumbstickAngleOffset = -82.5f;

        [SerializeField]
        [DropDownComponent(true,true)]
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

            if (Mathf.Abs(thumbstickPosition.y) > thumbstickThreshold || Mathf.Abs(thumbstickPosition.x) > thumbstickThreshold) {
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

        /// <summary>
        /// Updates target point orientation via thumbstick
        /// </summary>
        /// <param name="obj"></param>
        public void OnInputPositionChanged(InputPositionEventData eventData)
        {
            if (eventData.SourceId == SourceId && eventData.PressType == InteractionSourcePressInfo.Thumbstick)
            {
                Vector2 newThumbstick = Vector2.zero;
                bool thumbstickPressed = false;
                if (eventData.InputSource.TryGetThumbstick(SourceId, out thumbstickPressed, out newThumbstick)) {
                    thumbstickPosition = newThumbstick;
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