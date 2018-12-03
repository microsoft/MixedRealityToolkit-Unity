// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrcCheckToggle.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit Demo
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Int UI InputField.
    /// </summary>
    public class IntInputField : MonoBehaviour
    {
        public InputField PropertyValueInput;

        [System.Serializable]
        public class OnSubmitEvent : UnityEvent<int> { }

        public OnSubmitEvent OnSubmit;

        bool registered;

        void OnEnable()
        {
            if (!registered)
            {
                registered = true;
                PropertyValueInput.onEndEdit.AddListener(EndEditOnEnter);
            }
        }

        void OnDisable()
        {
            registered = false;
            PropertyValueInput.onEndEdit.RemoveListener(EndEditOnEnter);
        }

        public void SetValue(int value)
        {
            PropertyValueInput.text = value.ToString();
        }

        public void EndEditOnEnter(string value)
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
            int _value = 0;
            int.TryParse(value, out _value);
            OnSubmit.Invoke(_value);
        }
    }
}