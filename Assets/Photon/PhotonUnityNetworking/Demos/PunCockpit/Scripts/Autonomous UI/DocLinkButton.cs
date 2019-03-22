// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocLinkButton.cs" company="Exit Games GmbH">
//   Part of: Pun demos
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Pun.Demo.Shared;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Open an Url on Pointer Click.
    /// </summary>
    public class DocLinkButton : MonoBehaviour, IPointerClickHandler
    {
		public DocLinks.DocTypes Type = DocLinks.DocTypes.Doc;

        public string Reference = "getting-started/pun-intro";


		// Just so that Unity expose the enable Check Box inside the Component Inspector Header.
		public void Start(){}

        //Detect if a click occurs
        public void OnPointerClick(PointerEventData pointerEventData)
        {
			Application.OpenURL(DocLinks.GetLink(Type,Reference));
        }
    }
}