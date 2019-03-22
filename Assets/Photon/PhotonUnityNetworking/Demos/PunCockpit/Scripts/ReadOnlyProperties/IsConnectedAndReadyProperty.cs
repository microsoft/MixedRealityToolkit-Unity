// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsConnectedAndReady.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
	/// PhotonNetwork.IsConnectedAndReady UI property
    /// </summary>
	public class IsConnectedAndReadyProperty : PropertyListenerBase
    {

        public Text Text;

        int _cache = -1;

        void Update()
        {

			if ((PhotonNetwork.IsConnectedAndReady && _cache != 1) || (!PhotonNetwork.IsConnectedAndReady && _cache != 0))
            {
				_cache = PhotonNetwork.IsConnectedAndReady ? 1 : 0;
				Text.text = PhotonNetwork.IsConnectedAndReady ? "true" : "false";
                this.OnValueChanged();
            }
        }
    }
}