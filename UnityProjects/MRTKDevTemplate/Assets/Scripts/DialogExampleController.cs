// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UX;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This class is used as an example controller to show how to instantiate and launch two different kind of Dialog.
    /// Each one of the public methods are called by the buttons in the scene at the OnClick event.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Dialog Example Controller")]
    public class DialogExampleController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Assign Dialog_168x140mm.prefab")]
        private Dialog dialogPrefabLarge;

        /// <summary>
        /// Large Dialog example prefab to display
        /// </summary>
        public Dialog DialogPrefabLarge
        {
            get => dialogPrefabLarge;
            set => dialogPrefabLarge = value;
        }

        [SerializeField]
        [Tooltip("Assign Dialog_168x108mm.prefab")]
        private Dialog dialogPrefabMedium;

        /// <summary>
        /// Medium Dialog example prefab to display
        /// </summary>
        public Dialog DialogPrefabMedium
        {
            get => dialogPrefabMedium;
            set => dialogPrefabMedium = value;
        }

        [SerializeField]
        [Tooltip("Assign Dialog_168x88mm.prefab")]
        private Dialog dialogPrefabSmall;

        /// <summary>
        /// Small Dialog example prefab to display
        /// </summary>
        public Dialog DialogPrefabSmall
        {
            get => dialogPrefabSmall;
            set => dialogPrefabSmall = value;
        }

        /// <summary>
        /// Opens confirmation dialog example
        /// </summary>
        public void OpenConfirmationDialogLarge()
        {
            Dialog.InstantiateFromPrefab(DialogPrefabLarge, new DialogProperty("Confirmation Dialog, Large, Far", "This is an example of a large dialog with only one button, placed at far interaction range", DialogButtonHelpers.OK), false, true);
        }

        /// <summary>
        /// Opens choice dialog example
        /// </summary>
        public void OpenChoiceDialogLarge()
        {
            Dialog myDialog = Dialog.InstantiateFromPrefab(DialogPrefabLarge, new DialogProperty("Choice Dialog, Large, Near", "This is an example of a large dialog with a choice message for the user, placed at near interaction range", DialogButtonHelpers.YesNo), true, true);
            if (myDialog != null)
            {
                myDialog.OnClosed += OnClosedDialogEvent;
            }
        }

        /// <summary>
        /// Opens confirmation dialog example
        /// </summary>
        public void OpenConfirmationDialogMedium()
        {
            Dialog.InstantiateFromPrefab(DialogPrefabMedium, new DialogProperty("Confirmation Dialog, Medium, Near", "This is an example of a medium dialog with only one button, placed at near interaction range", DialogButtonHelpers.OK), true, true);
        }

        /// <summary>
        /// Opens choice dialog example
        /// </summary>
        public void OpenChoiceDialogMedium()
        {
            Dialog myDialog = Dialog.InstantiateFromPrefab(DialogPrefabMedium, new DialogProperty("Choice Dialog, Medium, Far", "This is an example of a medium dialog with a choice message for the user, placed at far interaction range", DialogButtonHelpers.YesNo), false, true);
            if (myDialog != null)
            {
                myDialog.OnClosed += OnClosedDialogEvent;
            }
        }

        /// <summary>
        /// Opens confirmation dialog example
        /// </summary>
        public void OpenConfirmationDialogSmall()
        {
            Dialog.InstantiateFromPrefab(DialogPrefabSmall, new DialogProperty("Confirmation Dialog, Small, Far", "This is an example of a small dialog with only one button, placed at far interaction range", DialogButtonHelpers.OK), false, true);
        }

        /// <summary>
        /// Opens choice dialog example
        /// </summary>
        public void OpenChoiceDialogSmall()
        {
            Dialog myDialog = Dialog.InstantiateFromPrefab(DialogPrefabSmall, new DialogProperty("Choice Dialog, Small, Near", "This is an example of a small dialog with a choice message for the user, placed at near interaction range", DialogButtonHelpers.YesNo), true, true);
            if (myDialog != null)
            {
                myDialog.OnClosed += OnClosedDialogEvent;
            }
        }

        private void OnClosedDialogEvent(DialogProperty property)
        {
            if (property.ResultContext.ButtonType == DialogButtonType.Yes)
            {
                Debug.Log(property.ResultContext.ButtonType);
            }
        }
    }
}
