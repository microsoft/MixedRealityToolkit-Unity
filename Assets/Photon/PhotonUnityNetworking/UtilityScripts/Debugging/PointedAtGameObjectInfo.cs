// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointedAtGameObjectInfo.cs" company="Exit Games GmbH">
// </copyright>
// <summary>
//  Display ViewId, OwnerActorNr, IsCeneView and IsMine when clicked using the old UI system
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Display ViewId, OwnerActorNr, IsCeneView and IsMine when clicked.
    /// </summary>
    public class PointedAtGameObjectInfo : MonoBehaviour
    {
        public static PointedAtGameObjectInfo Instance;

        public Text text;

        Transform focus;

        void Start()
        {
            if (Instance != null)
            {
                Debug.LogWarning("PointedAtGameObjectInfo is already featured in the scene, gameobject is destroyed");
                Destroy(this.gameObject);
            }

            Instance = this;
        }

        public void SetFocus(PhotonView pv)
        {

            focus = pv != null ? pv.transform : null;

            if (pv != null)
            {
                text.text = string.Format("id {0} own: {1} {2}{3}", pv.ViewID, pv.OwnerActorNr, (pv.IsSceneView) ? "scn" : "", (pv.IsMine) ? " mine" : "");
                //GUI.Label (new Rect (Input.mousePosition.x + 5, Screen.height - Input.mousePosition.y - 15, 300, 30), );
            }
            else
            {
                text.text = string.Empty;

            }
        }

        public void RemoveFocus(PhotonView pv)
        {
            if (pv == null)
            {
                text.text = string.Empty;
                return;
            }

            if (pv.transform == focus)
            {
                text.text = string.Empty;
                return;
            }

        }

        void LateUpdate()
        {
            if (focus != null)
            {
                this.transform.position = Camera.main.WorldToScreenPoint(focus.position);
            }
        }
    }
}