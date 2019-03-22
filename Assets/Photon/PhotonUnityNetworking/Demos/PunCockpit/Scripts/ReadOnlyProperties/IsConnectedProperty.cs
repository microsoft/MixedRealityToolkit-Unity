// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsConnectedProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
	/// PhotonNetwork.IsConnected UI property
    /// </summary>
	public class IsConnectedProperty : PropertyListenerBase
    {

        public Text Text;

        int _cache = -1;

        void Update()
        {
			if ((PhotonNetwork.IsConnected && _cache != 1) || (!PhotonNetwork.IsConnected && _cache != 0))
            {
				_cache = PhotonNetwork.IsConnected ? 1 : 0;
				Text.text = PhotonNetwork.IsConnected ? "true" : "false";
                this.OnValueChanged();
            }
        }
    }
}