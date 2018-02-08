using MixedRealityToolkit.InputModule.EventData;



namespace MixedRealityToolkit.Examples.UX
{
    public class DialogButton : Interactive
    {
        public SimpleDialogShell ParentDialog { get; set; }

        public SimpleDialog.ButtonTypeEnum ButtonTypeEnum;

        public void SetCaption(string title)
        {
            this.SetTitle(title);
        }

        public override void OnInputClicked(InputClickedEventData eventData)
        {
            if (ParentDialog != null)
            {
                ParentDialog.Result.Result = ButtonTypeEnum;
                ParentDialog.DismissDialog();
            }
        }
    }
}