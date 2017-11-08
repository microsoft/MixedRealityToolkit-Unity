using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class GamePadHandlerBase : MonoBehaviour, ISourceStateHandler
    {
        [SerializeField]
        [Tooltip("True, if gaze is not required for Input")]
        protected bool IsGlobalListener = true;

        protected string GamePadName;

        protected virtual void Start()
        {
            if (IsGlobalListener)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
        }

        public virtual void OnSourceDetected(SourceStateEventData eventData)
        {
            //GamePadName = eventData.SourceName;
        }

        public virtual void OnSourceLost(SourceStateEventData eventData)
        {
            GamePadName = string.Empty;
        }
    }
}
