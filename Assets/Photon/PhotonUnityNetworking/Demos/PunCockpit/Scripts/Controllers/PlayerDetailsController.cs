// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerDetailsController.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
 
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Controller for the Playerdetails UI view
    /// </summary>
    public class PlayerDetailsController : MonoBehaviourPunCallbacks
    {

        public GameObject ContentPanel;
        public PropertyCell PropertyCellPrototype;
        public Text UpdateStatusText;

        public Transform BuiltInPropertiesPanel;
        public Transform PlayerNumberingExtensionPanel;
        public Transform ScoreExtensionPanel;
        public Transform TeamExtensionPanel;
        public Transform TurnExtensionPanel;
        public Transform CustomPropertiesPanel;

        public GameObject MasterClientToolBar;



        public GameObject NotInRoomLabel;


        Dictionary<string, PropertyCell> builtInPropsCellList = new Dictionary<string, PropertyCell>();

        Player _player;

        void Awake()
        {
            this.PropertyCellPrototype.gameObject.SetActive(false);

        }

        public override void OnEnable()
        {

            base.OnEnable();

            UpdateStatusText.text = string.Empty;
            NotInRoomLabel.SetActive(false);

            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;

        }

        public void SetPlayerTarget(Player player)
        {
            //Debug.Log("SetPlayerTarget " + player);
            this._player = player;

            ContentPanel.SetActive(true);
            NotInRoomLabel.SetActive(false);

            this.ResetList();

            foreach (DictionaryEntry item in this.GetAllPlayerBuiltIntProperties())
            {
                this.AddProperty(ParseKey(item.Key), item.Value.ToString(), this.BuiltInPropertiesPanel);
            }

			// PlayerNumbering extension
            this.AddProperty("Player Number", "#" + player.GetPlayerNumber().ToString("00"), this.PlayerNumberingExtensionPanel);


			// Score extension
			this.AddProperty(PunPlayerScores.PlayerScoreProp, player.GetScore().ToString(), this.ScoreExtensionPanel);


            foreach (DictionaryEntry item in _player.CustomProperties)
            {
                this.AddProperty(ParseKey(item.Key), item.Value.ToString(), this.CustomPropertiesPanel);
            }

			MasterClientToolBar.SetActive(PhotonNetwork.CurrentRoom.PlayerCount>1 && PhotonNetwork.LocalPlayer.IsMasterClient);
        }

        void AddProperty(string property, string value, Transform parent)
        {
            PropertyCell _cell = Instantiate(PropertyCellPrototype);
            builtInPropsCellList.Add(property, _cell);
            _cell.transform.SetParent(parent, false);
            _cell.gameObject.SetActive(true);
            _cell.AddToList(property, value, false);
        }


        string ParseKey(object key)
        {
            if (key.GetType() == typeof(byte))
            {
                byte _byteKey = (byte)key;

                switch (_byteKey)
                {
                    case (byte)255:
                        return "PlayerName";
                    case (byte)254:
                        return "Inactive";
                    case (byte)253:
                        return "UserId";
                }

            }
            return key.ToString();
        }

        public void KickOutPlayer()
        {
            PhotonNetwork.CloseConnection(_player);
        }

        public void SetAsMaster()
        {
            PhotonNetwork.SetMasterClient(_player);
        }

        #region Photon CallBacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            NotInRoomLabel.SetActive(otherPlayer == _player);
            ContentPanel.SetActive(otherPlayer != _player);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            MasterClientToolBar.SetActive(_player == newMasterClient);
        }

        public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
        {
            //Debug.Log("OnPlayerPropertiesUpdate " + target.ActorNumber + " " + target.ToStringFull() + " " + changedProps.ToStringFull());

            //Debug.Log("_player.ID " + _player.ActorNumber);
            if (_player.ActorNumber == target.ActorNumber)
            {

                foreach (DictionaryEntry entry in changedProps)
                {
                    string _key = this.ParseKey(entry.Key);
                    if (this.builtInPropsCellList.ContainsKey(_key))
                    {
                        this.builtInPropsCellList[_key].UpdateInfo(entry.Value.ToString());
                    }
                    else
                    {
                        this.AddProperty(_key, entry.Value.ToString(), this.CustomPropertiesPanel);
                    }

                }

            }

            StartCoroutine("UpdateUIPing");
        }

        #endregion


        private void OnPlayerNumberingChanged()
        {
            if (_player != null)
            { // we might be called before player is setup
                this.builtInPropsCellList["Player Number"].UpdateInfo("#" + _player.GetPlayerNumber().ToString("00"));
            }

        }

        IEnumerator UpdateUIPing()
        {
            UpdateStatusText.text = "Updated";

            yield return new WaitForSeconds(1f);

            UpdateStatusText.text = string.Empty;
        }


        public void ResetList()
        {
            foreach (KeyValuePair<string, PropertyCell> entry in builtInPropsCellList)
            {
                if (entry.Value != null)
                {
                    Destroy(entry.Value.gameObject);
                }
            }

            builtInPropsCellList = new Dictionary<string, PropertyCell>();
        }


        Hashtable GetAllPlayerBuiltIntProperties()
        {
            Hashtable allProps = new Hashtable();

            if (_player != null)
            {

                allProps["ID"] = _player.ActorNumber;
                allProps[ActorProperties.UserId] = _player.UserId != null ? _player.UserId : string.Empty;
                allProps["NickName"] = _player.NickName != null ? _player.NickName : string.Empty;
                allProps["IsLocal"] = _player.IsLocal;
                allProps[ActorProperties.IsInactive] = _player.IsInactive;
                allProps["IsMasterClient"] = _player.IsMasterClient;
            }

            Debug.Log(allProps.ToStringFull());

            return allProps;
        }
    }
}