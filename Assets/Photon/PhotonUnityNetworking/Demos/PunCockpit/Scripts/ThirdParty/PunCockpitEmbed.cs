// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PunCockpitEmbed.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Use this in scenes you want to leave Control for connection and pun related commands to Cockpit.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun.UtilityScripts;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Use this in scenes you want to leave Control for connection and pun related commands to Cockpit.
    /// It requires ConnectAndJoinRandom, which it will control for connecting should the Cockpit scene not be present or succesfully loaded.
    /// </summary>
    public class PunCockpitEmbed : MonoBehaviourPunCallbacks
    {

        string PunCockpit_scene = "PunCockpit-Scene";

        public bool EmbeddCockpit = true;
        public string CockpitGameTitle = "";

        public GameObject LoadingIndicator;
        public ConnectAndJoinRandom AutoConnect;

        void Awake()
        {
            if (LoadingIndicator != null)
            {
                LoadingIndicator.SetActive(false);
            }
        }

        // Use this for initialization
        IEnumerator Start()
        {


            PunCockpit.Embedded = EmbeddCockpit;
            PunCockpit.EmbeddedGameTitle = CockpitGameTitle;

            //Debug.Log (SceneManager.GetSceneByName (PunCockpit_scene).IsValid());

            SceneManager.LoadScene(PunCockpit_scene, LoadSceneMode.Additive);

            yield return new WaitForSeconds(1f);

            if (SceneManager.sceneCount == 1)
            {

                AutoConnect.ConnectNow();

                if (LoadingIndicator != null)
                {
                    LoadingIndicator.SetActive(true);
                }
            }
            else
            {
                Destroy(AutoConnect);
            }

            yield return 0;
        }

        #region MonoBehaviourPunCallbacks implementation

        public override void OnJoinedRoom()
        {
            //Debug.Log("OnJoinedRoom", this);

            if (LoadingIndicator != null)
            {
                LoadingIndicator.SetActive(false);
            }

            if (PunCockpit.Instance != null)
            {
                //Debug.Log("switch to minimal panel", this);
                PunCockpit.Instance.SwitchtoMinimalPanel();

            }
        }
        #endregion





    }


}