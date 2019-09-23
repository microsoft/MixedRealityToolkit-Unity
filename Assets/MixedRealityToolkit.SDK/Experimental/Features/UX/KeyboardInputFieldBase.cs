// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.UI;
#if WINDOWS_UWP
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    public abstract class KeyboardInputFieldBase<T> : MixedRealityKeyboardBase<T>
#if WINDOWS_UWP
    , IDeselectHandler, IMixedRealityPointerHandler
# endif
    where T : Selectable
    {
        void OnValidate()
        {
            T inputField = GetComponent<T>();

            DisableRaycastTarget(Text(inputField));
            DisableRaycastTarget(PlaceHolder(inputField));
        }

        private void DisableRaycastTarget(Graphic graphic)
        {
            if (graphic != null)
            {
                graphic.raycastTarget = false;
            }
        }

#if WINDOWS_UWP

#region IDeselectHandler implementation

        public void OnDeselect(BaseEventData eventData) => HideKeyboard();

#endregion

#region IMixedRealityPointerHandler implementation

        public void OnPointerDown(MixedRealityPointerEventData eventData) { }

        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        public void OnPointerUp(MixedRealityPointerEventData eventData) { }

        public void OnPointerClicked(MixedRealityPointerEventData eventData) => ShowKeyboard();

#endregion

#endif
        protected abstract Graphic Text(T inputField);
        protected abstract Graphic PlaceHolder(T inputField);
    }
}
