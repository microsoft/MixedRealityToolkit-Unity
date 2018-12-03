// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnStartDelete.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  This component will destroy the GameObject it is attached to (in Start()).
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

namespace Photon.Chat.UtilityScripts
{
    /// <summary>This component will destroy the GameObject it is attached to (in Start()).</summary>
    public class OnStartDelete : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            Destroy(this.gameObject);
        }
    }
}