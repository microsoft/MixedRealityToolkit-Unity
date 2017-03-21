using UnityEngine.EventSystems;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    public class InputXYEventData : InputEventData
    {
        /// <summary>
        /// The amount, from -1.0 to 1.0, that the input control was moved.
        /// </summary>
        public double X { get; private set; }
        public double Y { get; private set; }

        public InputXYEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, InteractionPressKind pressKind, double x, double y, object tag = null)
        {
            Initialize(inputSource, sourceId, tag, pressKind);
            X = x;
            Y = y;
        }
    }
}