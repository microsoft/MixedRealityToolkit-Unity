// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomListCell.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Region list cell.
    /// </summary>
    public class RegionListCell : MonoBehaviour
    {
        public RegionListView ListManager;

        public Text CodeText;
        public Text IpText;
        public Text PingText;

		public LayoutElement LayoutElement;

		int _index;

		Region info;

		public void RefreshInfo(Region info)
        {
            this.info = info;
			CodeText.text = this.info.Code;
			IpText.text = this.info.HostAndPort;
			PingText.text = this.info.Ping +"ms";
        }

		public void AddToList(Region info,int index)
        {
            RefreshInfo(info);
			_index = index;

            StartCoroutine("AnimateAddition");
  
        }

        public void RemoveFromList()
        {
            StartCoroutine("AnimateRemove");
        }

        IEnumerator AnimateAddition()
        {
			LayoutElement.minHeight = 0f;

			yield return new WaitForSeconds(_index * 0.04f);

            while (LayoutElement.minHeight != 30f)
            {

                LayoutElement.minHeight = Mathf.MoveTowards(LayoutElement.minHeight, 30f, 2f);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator AnimateRemove()
        {
            while (LayoutElement.minHeight != 0f)
            {
                LayoutElement.minHeight = Mathf.MoveTowards(LayoutElement.minHeight, 0f, 2f);
                yield return new WaitForEndOfFrame();
            }
            Destroy(this.gameObject);
        }

    }
}