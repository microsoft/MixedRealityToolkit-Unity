// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsConnectedProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine.UI;
using Photon.Realtime;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
	/// PhotonNetwork.Server UI property
    /// </summary>
	public class ServerProperty : PropertyListenerBase
    {

        public Text Text;

		ServerConnection _cache;


        void Update()
        {

			if (PhotonNetwork.Server != _cache)
            {
				_cache = PhotonNetwork.Server;
				Text.text = PhotonNetwork.Server.ToString();
                this.OnValueChanged();
            }
        }
    }
}