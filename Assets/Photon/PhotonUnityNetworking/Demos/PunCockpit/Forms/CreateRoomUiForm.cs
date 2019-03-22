// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateRoomUiForm.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit Demo
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Photon.Realtime;

namespace Photon.Pun.Demo.Cockpit.Forms
{
    /// <summary>
    /// Create room user interface form.
    /// </summary>
    public class CreateRoomUiForm : MonoBehaviour
    {
        public InputField RoomNameInput;
        public InputField LobbyNameInput;
        public InputField ExpectedUsersInput;
        public Dropdown LobbyTypeInput;

        [System.Serializable]
        public class OnSubmitEvent : UnityEvent<string, string, LobbyType, string[]> { }

        public OnSubmitEvent OnSubmit;

        public void Start()
        {

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
            LobbyType _t = LobbyType.Default;

            if (LobbyTypeInput.value == 1)
            {
                _t = LobbyType.SqlLobby;
            }
            else if (LobbyTypeInput.value == 2)
            {
                _t = LobbyType.AsyncRandomLobby;
            }

            string[] _expectedUsers = string.IsNullOrEmpty(ExpectedUsersInput.text) ? null : ExpectedUsersInput.text.Split(',').Select(t => t.Trim()).ToArray();

            OnSubmit.Invoke(
                string.IsNullOrEmpty(RoomNameInput.text) ? null : RoomNameInput.text,
                string.IsNullOrEmpty(LobbyNameInput.text) ? null : LobbyNameInput.text,
                _t,
                _expectedUsers);
        }
    }
}