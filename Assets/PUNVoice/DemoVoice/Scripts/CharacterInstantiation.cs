// ----------------------------------------------------------------------------
// <copyright file="CharacterInstantiation.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// Class that handles character instantiation when the actor is joined.
// It adds multiple prefabs support to OnJoinedInstantiate.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace ExitGames.Demos.DemoPunVoice {

    using UnityEngine;

    public class CharacterInstantiation : OnJoinedInstantiate {

        public delegate void OnCharacterInstantiated(GameObject character);

        public static event OnCharacterInstantiated CharacterInstantiated;

        public new void OnJoinedRoom() {
            if (this.PrefabsToInstantiate != null) {
                GameObject o = PrefabsToInstantiate[(PhotonNetwork.player.ID - 1) % 4];
                //Debug.Log("Instantiating: " + o.name);
                Vector3 spawnPos = Vector3.zero;
                if (this.SpawnPosition != null) {
                    spawnPos = this.SpawnPosition.position;
                }
                Vector3 random = Random.insideUnitSphere;
                random = this.PositionOffset * random.normalized;
                spawnPos += random;
                spawnPos.y = 0;
                Camera.main.transform.position += spawnPos;                

                o = PhotonNetwork.Instantiate(o.name, spawnPos, Quaternion.identity, 0);
                if (CharacterInstantiated != null) {
                    CharacterInstantiated(o);
                }
            }
        }
    }
}