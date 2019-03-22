// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudRegionProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// PhotonNetwork.CloudRegion UI property.
    /// </summary>
	public class CloudRegionProperty : PropertyListenerBase
    {
        public Text Text;

        string _cache;

        void Update()
        {
            if (PhotonNetwork.CloudRegion != _cache)
            {
                _cache = PhotonNetwork.CloudRegion;
				this.OnValueChanged();
                if (string.IsNullOrEmpty(_cache))
                {
                    Text.text = "n/a";
                }
                else
                {
                    Text.text = _cache;
                }
            }
        }
    }
}