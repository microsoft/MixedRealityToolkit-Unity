// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountOfPlayersOnMasterProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine.UI;

using Photon.Realtime;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// PhotonNetwork.CountOfPlayersOnMaster UI property.
    /// </summary>
    public class CountOfPlayersOnMasterProperty : PropertyListenerBase
    {
        public Text Text;

        int _cache = -1;

        void Update()
        {
            if (PhotonNetwork.NetworkingClient.Server == ServerConnection.MasterServer)
            {
                if (PhotonNetwork.CountOfPlayersOnMaster != _cache)
                {
                    _cache = PhotonNetwork.CountOfPlayersOnMaster;
                    Text.text = _cache.ToString();
                    this.OnValueChanged();
                }
            }
            else
            {
                if (_cache != -1)
                {
                    _cache = -1;
                    Text.text = "n/a";
                }
            }
        }
    }
}