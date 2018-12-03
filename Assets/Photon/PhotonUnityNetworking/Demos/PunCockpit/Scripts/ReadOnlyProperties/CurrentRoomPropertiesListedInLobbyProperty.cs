// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomListView.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine.UI;

using System.Linq;

namespace Photon.Pun.Demo.Cockpit
{

    /// <summary>
    /// PhotonNetwork.CurrentRoom.PropertiesListedInLobby UI property.
    /// </summary>
    public class CurrentRoomPropertiesListedInLobbyProperty : PropertyListenerBase
    {
        public Text Text;

        string[] _cache = null;

        void Update()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {

                if (_cache == null || !PhotonNetwork.CurrentRoom.PropertiesListedInLobby.SequenceEqual(_cache))
                {

                    _cache = PhotonNetwork.CurrentRoom.PropertiesListedInLobby.Clone() as string[];
                    Text.text = string.Join("\n", PhotonNetwork.CurrentRoom.PropertiesListedInLobby);
                    this.OnValueChanged();
                }
            }
            else
            {
                if (_cache != null)
                {
                    _cache = null;
                    Text.text = "n/a";
                }
            }
        }
    }
}