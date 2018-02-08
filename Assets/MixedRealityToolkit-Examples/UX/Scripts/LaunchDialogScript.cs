using MixedRealityToolkit.InputModule.EventData;
using System.Collections;
using UnityEngine;


namespace MixedRealityToolkit.Examples.UX
{
    public class LaunchDialogScript : Interactive
    {
        public GameObject resultText;
        public int numButtons;
        private bool isDialogLaunched = false;

        protected IEnumerator LaunchDialog(SimpleDialog.ButtonTypeEnum buttons, string title, string message)
        {
            isDialogLaunched = true;
            //instantiate a dialog instance from the prefab
            SimpleDialog dialog;

            GameObject dialogPrefab = Resources.Load("Dialog") as GameObject;
            dialog = SimpleDialog.Open(dialogPrefab, buttons, title, message);
            dialog.OnClosed += OnClosed;

            // Wait for dialog to close
            while (dialog.State < SimpleDialog.StateEnum.InputReceived)
            {
                yield return null;
            }

            //only let one dialog be created at a time
            isDialogLaunched = false;

            yield break;
        }

        public override void OnInputClicked(InputClickedEventData eventData)
        {
            if (isDialogLaunched == false)
            {
                if (numButtons == 1)
                {
                    // Launch Dialog with single button
                    StartCoroutine(LaunchDialog(SimpleDialog.ButtonTypeEnum.OK, "Single Button Dialog", "Dialogs and flyouts are transient UI elements that appear when something happens that requires notification, approval, or additional information from the user."));
                }
                else if (numButtons == 2)
                {
                    // Launch Dialog with two buttons
                    StartCoroutine(LaunchDialog(SimpleDialog.ButtonTypeEnum.Yes | SimpleDialog.ButtonTypeEnum.No, "Two Buttons Dialog", "Dialogs and flyouts are transient UI elements that appear when something happens that requires notification, approval, or additional information from the user."));
                }
            }
        }

        protected void OnClosed(SimpleDialogResult result)
        {
            resultText.GetComponent<TextMesh>().text = result.Result.ToString();
        }
    }
}
