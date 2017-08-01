using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    public class InputPositionEventData : InputEventData
    {
        /// <summary>
        /// Two values, from -1.0 to 1.0 in the X-axis and Y-axis, representing where the input control is positioned.
        /// </summary>
        public Vector2 Position;

        public InputPositionEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, InteractionSourcePressType pressType, Vector2 position)
        {
            Initialize(inputSource, sourceId, tag, pressType);
            Position = position;
        }
    }
}