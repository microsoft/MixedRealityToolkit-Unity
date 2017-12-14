using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class GamePadHandlerBase : MonoBehaviour, ISourceStateHandler
    {
        [SerializeField]
        [Tooltip("True, if gaze is not required for Input")]
        protected bool IsGlobalListener = true;

        protected string GamePadName = string.Empty;

        private void OnEnable()
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

        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
            // Override and name your GamePad source.
        }

        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            GamePadName = string.Empty;
        }
    }
}
