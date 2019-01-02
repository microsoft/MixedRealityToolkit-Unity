using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl.Photon
{
    public class SystemPrefabPool : MonoBehaviour, ISystemPrefabPool//, IPunPrefabPool
    {
        public Action<GameObject> OnSystemObjectSpawned { get; set; }

        [SerializeField]
        private GameObject userObjectPrefab;
        [SerializeField]
        private GameObject deviceObjectPrefab;

        /*private Dictionary<string, GameObject> prefabLookup = new Dictionary<string, GameObject>();

        private void OnEnable()
        {
            PhotonNetwork.PrefabPool = this;
        }

        private void Awake()
        {
            prefabLookup.Add(userObjectPrefab.name, userObjectPrefab);
            prefabLookup.Add(deviceObjectPrefab.name, deviceObjectPrefab);
        }

        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }

        public new GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            GameObject systemObject = GameObject.Instantiate(prefabLookup[prefabId], position, rotation, null);
            PhotonView pv = systemObject.GetComponent<PhotonView>();
            if (pv != null)
                Debug.Log("Instantiated " + systemObject.name + " with photon view ID " + pv.ViewID);

            //systemObject.SetActive(false);

            return systemObject;
        }*/

        public GameObject InstantiateUser()
        {
            // User prefabs are currently local and don't sync any values
            // That may change in the future
            return GameObject.Instantiate(userObjectPrefab);
        }

        public GameObject InstantiateDevice()
        {
            // Device prefabs have a pun view component and sync values
            string prefabName = deviceObjectPrefab.name;
            GameObject go = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);
            Debug.Log("Instantiated device " + go.name);

            if (OnSystemObjectSpawned != null)
                OnSystemObjectSpawned(go);

            return go;
        }
    }
}