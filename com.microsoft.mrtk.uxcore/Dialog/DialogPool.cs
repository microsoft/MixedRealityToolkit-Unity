// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Spawns <see cref="Microsoft.MixedReality.Toolkit.UX.IDialog">IDialog</see> with the requested parameters and manages
    /// the lifecycle of the resulting <see cref="Microsoft.MixedReality.Toolkit.UX.IDialog">IDialog</see> component.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("MRTK/UX/Dialog Pool")]
    public class DialogPool : MonoBehaviour
    {
        /// <summary>
        /// Specifies the <see cref="DialogPool"/>'s behavior
        /// when opening a dialog while one is already active.
        /// </summary>
        public enum Policy
        {
            /// <summary> Dismisses any existing dialog, and then spawns its own. </summary>
            DismissExisting,

            /// <summary> Aborts spawning if a dialog is already active. </summary>
            AbortIfExisting
        }

        /// <summary>
        /// The default prefab to instantiate when spawning a dialog. This prefab must
        /// contain a <see cref="Microsoft.MixedReality.Toolkit.UX.IDialog">IDialog</see> component.
        /// </summary>
        [field: SerializeField, Tooltip("The default prefab to instantiate when spawning a dialog. This prefab must contain a IDialog component.")]
        public GameObject DialogPrefab { get; set; }

        // Static reference to the dialog instance, if active and spawned.
        // Will be null when no dialog is open.
        private static IDialog dialogInstance = null;

        // Previously-dismissed dialogs pooled by type.
        private static Dictionary<Type, Queue<IDialog>> dialogPool = new Dictionary<Type, Queue<IDialog>>();

        // Used to pre-populate the prefab slot at edit-time.
        // User can always set the prefab themselves, either in inspector
        // or through the API itself.
        private const string CanvasDialogPrefabGUID = "cca6164bb2744884a92a100266f5f3aa";

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (DialogPrefab == null)
            {
                // This is all editor-specific, locked behind the UNITY_EDITOR ifdef.
                DialogPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    AssetDatabase.GUIDToAssetPath(CanvasDialogPrefabGUID));
            }
#endif // UNITY_EDITOR
        }

        /// <summary>
        /// Retrieves or creates a new dialog instance. Specify a <paramref name="spawnPolicy"/> to
        /// determine how existing dialogs are handled. By default, the spawner will use its <see cref="DialogPrefab"/>,
        /// but a custom <paramref name="prefab"/> can also be specified. Dialogs of different types
        /// will be pooled independently. Only one dialog can be visible at a time, globally.
        /// </summary>
        /// <remarks>
        /// To build your dialog, call the fluent builder methods on the <see cref="IDialog"/> instance retuirned
        /// by this method.
        /// </remarks>
        /// <param name="spawnPolicy">How the spawner should deal with existing dialogs (dismiss or abort)</param>
        /// <param name="prefab">The prefab to use for the dialog. If null, the spawner's <see cref="DialogPrefab"/> will be used.</param>
        /// <returns>A dialog instance, or null if the spawner was unable to spawn a dialog.</returns>
        public IDialog Get(Policy spawnPolicy = Policy.DismissExisting, GameObject prefab = null)
        {
            if (prefab == null) { prefab = DialogPrefab; }

            if (prefab == null)
            {
                Debug.LogError("DialogPool's default dialog prefab is null.", this);
                return null;
            }

            // Is there a dialog already open, and is our policy to dismiss the existing dialog?
            if (dialogInstance != null && !IsDialogDestroyed(dialogInstance) && spawnPolicy == Policy.DismissExisting)
            {
                // Dismiss the existing dialog.
                dialogInstance.Dismiss();
            }
            else if (dialogInstance != null && !IsDialogDestroyed(dialogInstance) && spawnPolicy == Policy.AbortIfExisting)
            {
                // Otherwise, we abort.
                Debug.LogWarning("Tried to open a dialog, but one is already open. " +
                                 "To dismiss existing dialogs when opening a new one, " + 
                                 "use DialogPool.Policy.DismissExisting.", this);
                return null;
            }

            IDialog dialog = null;

            // Do we have a pooled dialog available for use?
            // Dialog pool is keyed by type, so we can pool derived dialog types.
            Type dialogType = prefab.GetComponent<IDialog>().GetType();
            if (dialogPool.ContainsKey(dialogType))
            {
                // Pop through our pooled queue until we find a valid dialog;
                // these might have been destroyed by some external event (scene load, etc)
                while ((dialog == null || IsDialogDestroyed(dialog)) && dialogPool[dialogType].Count > 0)
                {
                    dialog = dialogPool[dialogType].Dequeue();
                }
            }
            
            // If we didn't find a pooled dialog, instantiate a new one.
            if (dialog == null || IsDialogDestroyed(dialog))
            {
                // The pool is empty. We need to instantiate a new one!
                dialog = Instantiate(prefab).GetComponent<IDialog>();
            }
            
            dialog.VisibleRoot.SetActive(false);
            dialog.OnDismissed += OnDialogDismissed;
            
            // Put it on the pile of dialogs we're managing.
            dialogInstance = dialog;
            return dialog;
        }

        private void OnDialogDismissed(DialogDismissedEventArgs args)
        {
            args.Dialog.Reset(); // Reset the dialog to its default state for next use out of the pool.
            args.Dialog.OnDismissed -= OnDialogDismissed;
            if (dialogInstance == args.Dialog) dialogInstance = null;

            // Return dialog instance back to pool.
            Type dialogType = args.Dialog.GetType();
            if (!dialogPool.ContainsKey(dialogType))
            {
                dialogPool[dialogType] = new Queue<IDialog>();
            }
            
            dialogPool[dialogType].Enqueue(args.Dialog);
        }

        // Checks the MonoBehaviour-specific null check, which
        // asks Unity whether the object is actually destroyed.
        private bool IsDialogDestroyed(IDialog dialog)
        {
            if (dialog is MonoBehaviour dialogBehaviour)
            {
                return dialogBehaviour == null;
            }

            return dialog == null;
        }
    }
}