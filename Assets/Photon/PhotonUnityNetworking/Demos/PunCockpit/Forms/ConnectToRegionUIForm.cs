// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectToRegionUIForm.cs" company="Exit Games GmbH">
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
    /// Manager for ConnectToRegion UI Form
    /// </summary>
	public class ConnectToRegionUIForm : MonoBehaviour
    {
		public InputField RegionInput;
		public Dropdown RegionListInput;

		[System.Serializable]
		public class OnSubmitEvent : UnityEvent<string>{}

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

		public void SetRegionFromDropDown(int index)
		{
			if (index == 0) {
				return;
			}

			RegionInput.text =	RegionListInput.options[index].text;
			RegionListInput.value = 0;

		}

		public void SubmitForm()
		{
			OnSubmit.Invoke (RegionInput.text);
		}
	}
}