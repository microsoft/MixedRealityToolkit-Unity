using MixedRealityToolkit.InputModule.EventData;
using System.Collections;
using UnityEngine;


namespace MixedRealityToolkit.Examples.UX
{
    public class LaunchDialogScript : Interactive
    {
        //set to false to reuse existing instance of SimpleDialogShell called TestDialog
        private const bool INSTANTIATE_DIALOGS = true;

        public GameObject dialogInstance = null;
        public GameObject resultText;
        private bool isDialogLaunched = false;

        protected override void Start()
        {
            if (dialogInstance == null)
            {
                dialogInstance = GameObject.Find("TestDialog");
                dialogInstance.SetActive(false);
            }
        }

        protected IEnumerator LaunchDialog(SimpleDialog.ButtonTypeEnum buttons, string title, string message)
        {
            isDialogLaunched = true;
            SimpleDialog dialog;

            if (INSTANTIATE_DIALOGS)
            {
                GameObject dialogPrefab = Resources.Load("Dialog") as GameObject;
                dialog = SimpleDialog.Open(dialogPrefab, buttons, title, message);
                dialog.OnClosed += OnClosed;
            }
            else
            {
                dialogInstance.SetActive(true);
                dialog = SimpleDialog.Open(dialogInstance, buttons, title, message);
                dialog.OnClosed += OnClosed;
                dialogInstance.SetActive(false);
            }

            // Wait for dialog to close
            while (dialog.State < SimpleDialog.StateEnum.InputReceived)
            {
                yield return null;
            }


            //only let button create one dialog at a time
            isDialogLaunched = false;

            yield break;
        }

        public override void OnInputClicked(InputClickedEventData eventData)
        {
            if (isDialogLaunched == false)
            {
                StartCoroutine(LaunchDialog(SimpleDialog.ButtonTypeEnum.Yes | SimpleDialog.ButtonTypeEnum.No, "Title Text", "Dialogs and flyouts are transient UI elements that appear when something happens that requires notification, approval, or additional information from the user."));
            }
        }

        protected void OnClosed(SimpleDialogResult result)
        {
            resultText.GetComponent<TextMesh>().text = result.Result.ToString();
        }
    }
}
