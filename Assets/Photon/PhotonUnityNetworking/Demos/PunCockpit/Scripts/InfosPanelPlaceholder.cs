// <copyright file="PlayerDetailsController.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Infos panel placeholder. Defines the place where the infos panel should go. It will request InfoPanel when Component is enabled.
    /// </summary>
    public class InfosPanelPlaceholder : MonoBehaviour
    {
        public PunCockpit Manager;

        // Use this for initialization
        void OnEnable()
        {
            Manager.RequestInfosPanel(this.gameObject);
        }
    }
}
