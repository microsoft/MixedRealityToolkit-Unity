// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineDocButton.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit Demo
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Open an Url on Pointer Click.
    /// </summary>
    public class OnlineDocButton : MonoBehaviour, IPointerClickHandler
    {
        public string Url = "https://doc.photonengine.com/en-us/pun/v2/getting-started/pun-intro";

        //Detect if a click occurs
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            Application.OpenURL(Url);
        }

    }
}