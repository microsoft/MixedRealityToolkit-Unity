// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Spawns dialogs with the requested parameters and manages
    /// the lifecycle of the resulting dialog component.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("MRTK/UX/Dialog Spawner")]
    public class DialogSpawner : MonoBehaviour
    {
        /// <summary>
        /// Specifies the <see cref="DialogSpawner"/>'s behavior
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
        /// The default prefab to instantiate when spawning a dialog.
        /// </summary>
        [field: SerializeField, Tooltip("The default prefab to instantiate when spawning a dialog.")]
        public GameObject DialogPrefab { get; set; }

        // Static reference to the dialog instance, if active and spawned.
        // Will be null when no dialog is open.
        private static Dialog dialogInstance = null;

        // Previously-dismissed dialogs pooled by type.
        private static Dictionary<Type, Queue<Dialog>> dialogPool = new Dictionary<Type, Queue<Dialog>>();

        /// <summary>
        /// Retrieves or creates a new dialog instance. Specify a <paramref name="spawnPolicy"/> to
        /// determine how existing dialogs are handled. By default, the spawner will use its <see cref="DialogPrefab"/>,
        /// but a custom <paramref name="prefab"/> can also be specified. Dialogs of different types
        /// will be pooled independently. Only one dialog can be visible at a time, globally.
        /// </summary>
        /// <remarks>
        /// To build your dialog, call the fluent builder methods on the <see cref="Dialog"/> instance retuirned
        /// by this method.
        /// </remarks>
        /// <param name="spawnPolicy">How the spawner should deal with existing dialogs (dismiss or abort)</param>
        /// <param name="prefab">The prefab to use for the dialog. If null, the spawner's <see cref="DialogPrefab"/> will be used.</param>
        /// <returns>A dialog instance, or null if the spawner was unable to spawn a dialog.</returns>
        public Dialog Get(Policy spawnPolicy = Policy.DismissExisting, GameObject prefab = null)
        {
            if (prefab == null) { prefab = DialogPrefab; }

            if (prefab == null)
            {
                Debug.LogError("DialogSpawner's default dialog prefab is null.", this);
                return null;
            }

            // Is there a dialog already open, and is our policy to dismiss the existing dialog?
            if (dialogInstance != null && spawnPolicy == Policy.DismissExisting)
            {
                // Dismiss the existing dialog.
                dialogInstance.Dismiss();
            }
            else if (dialogInstance != null && spawnPolicy == Policy.AbortIfExisting)
            {
                // Otherwise, we abort.
                Debug.LogWarning("Tried to open a dialog, but one is already open. " +
                                 "To dismiss existing dialogs when opening a new one, " + 
                                 "use DialogSpawner.Policy.DismissExisting.", this);
                return null;
            }

            Dialog dialog = null;

            // Do we have a pooled dialog available for use?
            // Dialog pool is keyed by type, so we can pool derived dialog types.
            Type dialogType = prefab.GetComponent<Dialog>().GetType();
            if (dialogPool.ContainsKey(dialogType))
            {
                // Pop through our pooled queue until we find a valid dialog;
                // these might have been destroyed by some external event (scene load, etc)
                while (dialog == null && dialogPool[dialogType].Count > 0)
                {
                    dialog = dialogPool[dialogType].Dequeue();
                }
            }
            
            // If we didn't find a pooled dialog, instantiate a new one.
            if (dialog == null)
            {
                // The pool is empty. We need to instantiate a new one!
                dialog = Instantiate(prefab).GetComponent<Dialog>();
            }
            
            dialog.gameObject.SetActive(false);
            dialog.OnDismissed.AddListener(OnDialogDismissed);
            
            // Put it on the pile of dialogs we're managing.
            dialogInstance = dialog;
            return dialog;
        }

        private void OnDialogDismissed(Dialog dismissedDialog)
        {
            dismissedDialog.Reset(); // Reset the dialog to its default state for next use out of the pool.
            dismissedDialog.OnDismissed?.RemoveListener(OnDialogDismissed);
            if (dialogInstance == dismissedDialog) dialogInstance = null;

            // Return dialog instance back to pool.
            Debug.Log("Adding dialog back to pool.");
            Type dialogType = dismissedDialog.GetType();
            if (!dialogPool.ContainsKey(dialogType))
            {
                dialogPool[dialogType] = new Queue<Dialog>();
            }
            
            dialogPool[dialogType].Enqueue(dismissedDialog);
        }
    }
}