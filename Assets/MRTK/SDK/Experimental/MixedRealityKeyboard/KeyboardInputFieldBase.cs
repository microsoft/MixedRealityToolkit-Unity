// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.UI;
#if WINDOWS_UWP
using UnityEngine;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Base class explicitly launching Windows Mixed Reality's system keyboard for InputField and TMP_InputField
    /// To be attached to the same GameObject with either of the components.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class KeyboardInputFieldBase<T> : MixedRealityKeyboardBase
#if WINDOWS_UWP
    , IDeselectHandler, IMixedRealityPointerHandler
#endif
    where T : Selectable
    {
        [Experimental]
        protected T inputField;

        void OnValidate()
        {
            inputField = GetComponent<T>();

            if (inputField != null)
            {
                DisableRaycastTarget(TextGraphic(inputField));
                DisableRaycastTarget(PlaceHolderGraphic(inputField));
            }
        }

        private void DisableRaycastTarget(Graphic graphic)
        {
            if (graphic != null)
            {
                graphic.raycastTarget = false;
            }
        }

#if WINDOWS_UWP

        protected virtual void Awake()
        {
            if ((inputField = GetComponent<T>()) == null)
            {
                Destroy(this);
                Debug.LogWarning($"There is no {typeof(T).ToString()} on GameObject {name}, removing this component");
            }
        }

        #region IDeselectHandler implementation

        public void OnDeselect(BaseEventData eventData) => HideKeyboard();

        #endregion

        #region IMixedRealityPointerHandler implementation

        public void OnPointerDown(MixedRealityPointerEventData eventData) { }
        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
        public void OnPointerUp(MixedRealityPointerEventData eventData) { }
        public void OnPointerClicked(MixedRealityPointerEventData eventData) => ShowKeyboard(Text);

        #endregion

#endif
        protected abstract Graphic TextGraphic(T inputField);
        protected abstract Graphic PlaceHolderGraphic(T inputField);
    }
}
