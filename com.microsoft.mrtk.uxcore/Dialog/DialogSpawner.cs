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
        [SerializeField]
        [Tooltip("The prefab to instantiate when spawning a dialog.")]
        private GameObject dialogPrefab = null;

        /// <summary>
        /// The prefab to instantiate when spawning a dialog.
        /// </summary>
        public GameObject DialogPrefab
        {
            get => dialogPrefab;
            set => dialogPrefab = value;
        }

        [SerializeField]
        [Tooltip("Should this spawner only be allowed to spawn one dialog at a time?")]
        private bool enforceSingleInstance = true;

        /// <summary>
        /// Should this spawner only be allowed to spawn one dialog at a time?
        /// </summary>
        public bool EnforceSingleInstance
        {
            get => enforceSingleInstance;
            set
            {
                if (enforceSingleInstance != value)
                {
                    enforceSingleInstance = value;

                    // If we've just set it to enforce only a single dialog,
                    // we should dismiss all active dialogs except for one.
                    if (enforceSingleInstance)
                    {
                        List<Dialog> dialogsToDismiss = new List<Dialog>(dialogInstances);
                        for (int i = 0; i < dialogsToDismiss.Count - 1; i++)
                        {
                            dialogsToDismiss[i].Dismiss();
                        }
                    }
                }
            }
        }

        private HashSet<Dialog> dialogInstances = new HashSet<Dialog>();

        public Dialog Build()
        {
            if (dialogPrefab == null)
            {
                Debug.LogError("Dialog prefab is null.", this);
                return null;
            }

            if (enforceSingleInstance && dialogInstances.Count > 0)
            {
                // Dismiss all. Copy to a list beforehand as Dismiss()ing
                // dialogs will mutate the HashSet.
                List<Dialog> dialogsToDismiss = new List<Dialog>(dialogInstances);
                foreach (Dialog dialogToDismiss in dialogsToDismiss)
                {
                    dialogToDismiss.Dismiss();
                }
            }

            Dialog dialog = Instantiate(dialogPrefab).GetComponent<Dialog>();
            dialog.gameObject.SetActive(false);
            dialog.OnDismissed.AddListener(OnDialogDismissed);
            
            // Put it on the pile of dialogs we're managing.
            dialogInstances.Add(dialog);
            return dialog;
        }

        private void OnDialogDismissed(Dialog dismissedDialog)
        {
            dismissedDialog.OnDismissed.RemoveListener(OnDialogDismissed);
            dialogInstances.Remove(dismissedDialog);
            Destroy(dismissedDialog.gameObject);
            dismissedDialog = null;
        }
    }
}