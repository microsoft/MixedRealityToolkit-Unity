// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListCell.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Player list cell representing a given PhotonPlayer
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Player list cell representing a given PhotonPlayer
    /// </summary>
    public class PlayerListCell : MonoBehaviour
    {

        public PlayerListView ListManager;

        public Text NumberText;
        public Text NameText;
        public Image ActiveFlag;
        public Color InactiveColor;
        public Color ActiveColor;

        public Text isLocalText;
        public Image isMasterFlag;

        public LayoutElement LayoutElement;

        Player _player;

        public bool isInactiveCache;



        public void RefreshInfo(ExitGames.Client.Photon.Hashtable changedProps)
        {
            UpdateInfo();
        }

        public void AddToList(Player info, bool animate = false)
        {
            //Debug.Log("AddToList " + info.ToStringFull());

            _player = info;

            UpdateInfo();

            if (animate)
            {

                StartCoroutine("Add");
            }
            else
            {
                LayoutElement.minHeight = 30f;
            }
        }

        public void RemoveFromList()
        {
            StartCoroutine("Remove");
        }


        public void OnClick()
        {
            ListManager.SelectPlayer(_player);
        }

        void UpdateInfo()
        {
            if (string.IsNullOrEmpty(_player.NickName))
            {
                NameText.text = _player.ActorNumber.ToString();
            }

            int _index = _player.GetPlayerNumber();
            NumberText.text = "#" + _index.ToString("00"); // if this function was not called on every update, we would need to listen to the PlayerNumbering delegate

            NameText.text = _player.NickName;

            ActiveFlag.color = _player.IsInactive ? InactiveColor : ActiveColor;

            isLocalText.gameObject.SetActive(_player.IsLocal);

            isMasterFlag.gameObject.SetActive(_player.IsMasterClient);


            // reorder the list to match player number
            if (_index >= 0 && this.transform.GetSiblingIndex() != _index)
            {
                this.transform.SetSiblingIndex(_index + 1);
            }
        }


        IEnumerator Add()
        {
            this.isInactiveCache = false;

            LayoutElement.minHeight = 0f;

            while (LayoutElement.minHeight != 30f)
            {

                LayoutElement.minHeight = Mathf.MoveTowards(LayoutElement.minHeight, 30f, 2f);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Remove()
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