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
    /// Roomlist cell.
    /// </summary>
    public class RoomListCell : MonoBehaviour
    {
        public RoomListView ListManager;

        public Text RoomNameText;
        public Text PlayerCountText;
        public Text OpenText;
        public CanvasGroup JoinButtonCanvasGroup;
        public LayoutElement LayoutElement;

        public RoomInfo info;

        public void RefreshInfo(RoomInfo info)
        {
            this.info = info;
            RoomNameText.text = info.Name;
            PlayerCountText.text = info.PlayerCount + "/" + info.MaxPlayers;
            if (info.IsOpen)
            {
                OpenText.text = "Open";
                OpenText.color = Color.green;
                JoinButtonCanvasGroup.blocksRaycasts = true;
                JoinButtonCanvasGroup.alpha = 1f;
            }
            else
            {
                OpenText.text = "Closed";
                OpenText.color = Color.red;
                JoinButtonCanvasGroup.blocksRaycasts = false;
                JoinButtonCanvasGroup.alpha = 0f;
            }

        }

        public void OnJoinRoomButtonClick()
        {
            ListManager.OnRoomCellJoinButtonClick(info.Name);
        }


        public void AddToList(RoomInfo info, bool animate = false)
        {
            RefreshInfo(info);

            if (animate)
            {
                StartCoroutine("AnimateAddition");
            }
            else
            {
                LayoutElement.minHeight = 30f;
            }
        }

        public void RemoveFromList()
        {
            StartCoroutine("AnimateRemove");
        }

        IEnumerator AnimateAddition()
        {
            LayoutElement.minHeight = 0f;

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