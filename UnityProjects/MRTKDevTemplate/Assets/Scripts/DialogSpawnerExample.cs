// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class DialogSpawnerExample : MonoBehaviour
    {
        public DialogSpawner dialogSpawner;

        public void Awake()
        {
            if (dialogSpawner == null)
            {
                dialogSpawner = GetComponent<DialogSpawner>();
            }
        }

        public void SpawnDialogFromCode()
        {
            Dialog dialog = dialogSpawner.Build()
                .SetHeader("This dialog is spawned from code.")
                .SetBody("All of the dialog's properties can be set from code, using a friendly API.")
                .SetPositive("Yes, please!", ( args ) => Debug.Log("Code-driven dialog says " + args.ButtonType))
                .SetNegative("No, thanks.", ( args ) => Debug.Log("Code-driven dialog says " + args.ButtonType));

            dialog.Show();
        }

        public void SpawnNeutralDialogFromCode()
        {
            Dialog dialog = dialogSpawner.Build()
                .SetHeader("Demonstration of a neutral optioned dialog")
                .SetBody("As you can see, only the options requested will be shown in the dialog. " +
                         "Here's a neutral option, neither negative nor positive.")
                .SetNeutral("Neutral option!", ( args ) => Debug.Log("Code-driven dialog says " + args.ButtonType));

            dialog.Show();
        }

        public void SpawnAllThreeDialogFromCode()
        {
            Dialog dialog = dialogSpawner.Build()
                .SetHeader("You can even have three!")
                .SetBody("Yes, in fact, you can request all three option types and they'll still be laid out correctly.")
                .SetPositive("Yes, please!", ( args ) => Debug.Log("Code-driven dialog says " + args.ButtonType))
                .SetNegative("No, thanks.", ( args ) => Debug.Log("Code-driven dialog says " + args.ButtonType))
                .SetNeutral("Neutral option!", ( args ) => Debug.Log("Code-driven dialog says " + args.ButtonType));

            dialog.Show();
        }
    }
}