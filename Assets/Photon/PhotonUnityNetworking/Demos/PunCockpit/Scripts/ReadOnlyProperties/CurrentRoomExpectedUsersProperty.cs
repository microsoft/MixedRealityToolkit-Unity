// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentRoomExpectedUsersProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// PhotonNetwork.CurrentRoom.ExpectedUsers UI property.
    /// </summary>
    public class CurrentRoomExpectedUsersProperty : PropertyListenerBase
    {
        public Text Text;

        string[] _cache = null;

        void Update()
        {

            if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.ExpectedUsers == null)
            {
                if (_cache != null)
                {
                    _cache = null;
                    Text.text = "n/a";
                }

                return;

            }

            if (_cache == null || (PhotonNetwork.CurrentRoom.ExpectedUsers != null && !PhotonNetwork.CurrentRoom.ExpectedUsers.SequenceEqual(_cache)))
            {

                Text.text = string.Join("\n", PhotonNetwork.CurrentRoom.ExpectedUsers);

                this.OnValueChanged();

                return;
            }

            if (PhotonNetwork.CurrentRoom.ExpectedUsers == null && _cache != null)
            {

                Text.text = string.Join("\n", PhotonNetwork.CurrentRoom.ExpectedUsers);

                this.OnValueChanged();

                return;
            }
        }
    }
}