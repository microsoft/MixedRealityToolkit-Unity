using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HoloToolkit.UI.Keyboard
{
    /// <summary>
    /// This is an input field that overrides getting deselected
    /// </summary>
    public class SliderInputField : InputField
    {
        /// <summary>
        /// Override OnDeselect
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDeselect(BaseEventData eventData)
        {
            // Do nothing for deselection
        }
    }
}
