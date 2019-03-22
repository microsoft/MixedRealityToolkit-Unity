// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserIdField.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
	/// <summary>
	/// User identifier InputField.
	/// </summary>
    public class UserIdField : MonoBehaviour
    {

        public PunCockpit Manager;

        public InputField PropertyValueInput;

        string _cache;

        bool registered;

        void OnEnable()
        {
            if (!registered)
            {
                registered = true;
                PropertyValueInput.onEndEdit.AddListener(OnEndEdit);
            }
        }

        void OnDisable()
        {
            registered = false;
            PropertyValueInput.onEndEdit.RemoveListener(OnEndEdit);
        }

        void Update()
        {
            if (Manager.UserId != _cache)
            {
                _cache = Manager.UserId;
                PropertyValueInput.text = _cache;
            }
        }

        // new UI will fire "EndEdit" event also when loosing focus. So check "enter" key and only then submit form.
        public void OnEndEdit(string value)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Tab))
            {
                this.SubmitForm(value.Trim());
            }
            else
            {
                this.SubmitForm(value);
            }
        }

        public void SubmitForm(string value)
        {
            _cache = value;
            Manager.UserId = _cache;
            //Debug.Log("PunCockpit.UserId = " + Manager.UserId, this);
        }
    }
}