// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfflineModeProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
	/// PhotonNetwork.OfflineMode UI property
    /// </summary>
	public class OfflineModeProperty : PropertyListenerBase
    {

        public Text Text;

        int _cache = -1;

        void Update()
        {
			if ((PhotonNetwork.OfflineMode && _cache != 1) || (!PhotonNetwork.OfflineMode && _cache != 0))
            {
				_cache = PhotonNetwork.OfflineMode ? 1 : 0;
				Text.text = PhotonNetwork.OfflineMode ? "true" : "false";
                this.OnValueChanged();
            }
        }
    }
}