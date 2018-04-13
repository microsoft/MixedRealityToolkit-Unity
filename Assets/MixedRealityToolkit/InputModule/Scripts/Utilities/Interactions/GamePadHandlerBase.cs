using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Utilities.Interactions
{
    public class GamePadHandlerBase : MonoBehaviour, ISourceStateHandler, IInputHandler
    {
        [SerializeField]
        [Tooltip("True, if gaze is not required for Input")]
        protected bool IsGlobalListener = true;

        protected virtual void OnEnable()
        {
            if (IsGlobalListener)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
        }

        protected virtual void OnDisable()
        {
            if (IsGlobalListener && InputManager.Instance != null)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        public virtual void OnSourceDetected(SourceStateEventData eventData) { }

        public virtual void OnSourceLost(SourceStateEventData eventData) { }

        public virtual void OnSourcePositionChanged(SourcePositionEventData eventData) { }

        public virtual void OnSourceRotationChanged(SourceRotationEventData eventData) { }

        public virtual void OnInputUp(InputEventData eventData) { }

        public virtual void OnInputDown(InputEventData eventData) { }

        public virtual void OnInputPressed(InputPressedEventData eventData) { }

        public virtual void OnInputPositionChanged(InputPositionEventData eventData) { }
    }
}
