using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves a trigger press.
    /// </summary>
    public class TriggerEventData : BaseInputEventData
    {
        /// <summary>
        /// The amount, from 0.0 to 1.0, that the trigger was pressed.
        /// </summary>
        public double PressedValue { get; private set; }

        public TriggerEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, double pressedValue)
        {
            BaseInitialize(inputSource, sourceId, tag);
            PressedValue = pressedValue;
        }
    }
}