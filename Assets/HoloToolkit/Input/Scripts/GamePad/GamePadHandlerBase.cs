using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class GamePadHandlerBase : MonoBehaviour, IGamePadHandler
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

        public virtual void OnGamePadDetected(GamePadEventData eventData)
        {
            GamePadName = eventData.GamePadName;
        }

        public virtual void OnGamePadLost(GamePadEventData eventData)
        {
            GamePadName = string.Empty;
        }
    }
}
