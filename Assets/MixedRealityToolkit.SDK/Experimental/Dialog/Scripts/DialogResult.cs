using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Experimental.Dialog
{
    public class DialogResult
    {
        /// <summary>
        /// Title for the dialog to display
        /// </summary>
        private string title = string.Empty;
        /// <summary>
        /// The public property to get and set the Title
        /// string (topmost) on the Dialog.
        /// </summary>
        public string Title
        {
            get => title;
            set => title = value;            
        }

        /// <summary>
        /// Message for the dialog to display
        /// </summary>
        private string message = string.Empty;
        /// <summary>
        /// The public property to get and set the Message
        /// string of the dialog.
        /// </summary>
        public string Message
        {
            get => message;
            set => message = value;            
        }

        /// <summary>
        /// Which buttons to generate
        /// </summary>
        private DialogButtonType buttons = DialogButtonType.Close;
        /// <summary>
        /// Property defining the button type[s]
        /// on the dialog.
        /// </summary>
        public DialogButtonType Buttons
        {
            get => buttons;
            set => buttons = value;            
        }

        /// <summary>
        /// The button press that closed the dialog
        /// </summary>
        private DialogButtonType result = DialogButtonType.Close;
        /// <summary>
        /// Property reporting the Result of the Dialog:
        /// Which button was clicked to dismiss it.
        /// </summary>
        public DialogButtonType Result
        {
            get => result;
            set => result = value;
        }

        /// <summary>
        /// Variable that can be use to pass an object to the dialog
        /// so to have it back with the result
        /// </summary>
        private System.Object variable;
        /// <summary>
        /// The public property to get and set the variable
        /// object of the dialog
        /// </summary>
        public System.Object Variable
        {
            get => variable;
            set => variable = value;
        }
    }
}