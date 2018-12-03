// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnClickInstantiate.cs" company="Exit Games GmbH">
// Part of: Photon Unity Utilities
// </copyright>
// <summary>A compact script for prototyping.</summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


namespace Photon.Pun.UtilityScripts
{
    using UnityEngine;
    using UnityEngine.EventSystems;


    /// <summary>
    /// Instantiates a networked GameObject on click.
    /// </summary>
    /// <remarks>
    /// Gets OnClick() calls by Unity's IPointerClickHandler. Needs a PhysicsRaycaster on the camera.
    /// See: https://docs.unity3d.com/ScriptReference/EventSystems.IPointerClickHandler.html
    /// </remarks>
    public class OnClickInstantiate : MonoBehaviour, IPointerClickHandler
    {
        public enum InstantiateOption { Mine, Scene }


        public PointerEventData.InputButton Button;
        public KeyCode ModifierKey;

        public GameObject Prefab;

        [SerializeField]
        private InstantiateOption InstantiateType;


        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!PhotonNetwork.InRoom || (this.ModifierKey != KeyCode.None && !Input.GetKey(this.ModifierKey)) || eventData.button != this.Button)
            {
                return;
            }


            switch (this.InstantiateType)
            {
                case InstantiateOption.Mine:
                    PhotonNetwork.Instantiate(this.Prefab.name, eventData.pointerCurrentRaycast.worldPosition + new Vector3(0, 0.5f, 0), Quaternion.identity, 0);
                    break;
                case InstantiateOption.Scene:
                    PhotonNetwork.InstantiateSceneObject(this.Prefab.name, eventData.pointerCurrentRaycast.worldPosition + new Vector3(0, 0.5f, 0), Quaternion.identity, 0, null);
                    break;
            }
        }
    }
}