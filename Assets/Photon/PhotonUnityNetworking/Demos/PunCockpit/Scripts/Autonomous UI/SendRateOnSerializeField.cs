// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendRateOnSerializeField.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit Demo
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// PhotonNetwork.SerializationRate InputField
    /// </summary>
    public class SendRateOnSerializeField : MonoBehaviour
    {

        public InputField PropertyValueInput;

        int _cache;

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
            if (PhotonNetwork.SerializationRate != _cache)
            {
                _cache = PhotonNetwork.SerializationRate;
                PropertyValueInput.text = _cache.ToString();
            }
        }

        // new UI will fire "EndEdit" event also when loosing focus. So check "enter" key and only then StartChat.
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
            _cache = int.Parse(PropertyValueInput.text);
            PhotonNetwork.SerializationRate = _cache;
            //Debug.Log("PhotonNetwork.SerializationRate = " + PhotonNetwork.SerializationRate, this);
        }
    }
}