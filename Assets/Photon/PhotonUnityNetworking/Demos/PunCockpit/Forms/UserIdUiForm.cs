// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserIdUiForm.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit Demo
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Photon.Pun.Demo.Cockpit.Forms
{
    /// <summary>
    /// User Id UI form.
    /// </summary>
    public class UserIdUiForm : MonoBehaviour
    {
        public const string UserIdPlayerPref = "PunUserId";

        public InputField idInput;

        [System.Serializable]
        public class OnSubmitEvent : UnityEvent<string> { }

        public OnSubmitEvent OnSubmit;

        public void Start()
        {

            string prefsName = PlayerPrefs.GetString(UserIdUiForm.UserIdPlayerPref);
            if (!string.IsNullOrEmpty(prefsName))
            {
                this.idInput.text = prefsName;
            }
        }


        // new UI will fire "EndEdit" event also when loosing focus. So check "enter" key and only then StartChat.
        public void EndEditOnEnter()
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                this.SubmitForm();
            }
        }

        public void SubmitForm()
        {
            PlayerPrefs.SetString(UserIdUiForm.UserIdPlayerPref, idInput.text);
            OnSubmit.Invoke(idInput.text);
        }
    }
}