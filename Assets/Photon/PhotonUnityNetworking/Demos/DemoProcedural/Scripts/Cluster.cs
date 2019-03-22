using System.Collections.Generic;

using UnityEngine;

using ExitGames.Client.Photon;

namespace Photon.Pun.Demo.Procedural
{
    /// <summary>
    /// The Cluster component has references to all Blocks that are part of this Cluster.
    /// It provides functions for modifying single Blocks inside this Cluster.
    /// It also handles modifications made to the world by other clients.
    /// </summary>
    public class Cluster : MonoBehaviourPunCallbacks
    {
        private string propertiesKey;

        private Dictionary<int, float> propertiesValue;

        public int ClusterId { get; set; }

        public Dictionary<int, GameObject> Blocks { get; private set; }

        #region UNITY

        public void Awake()
        {
            Blocks = new Dictionary<int, GameObject>();

            propertiesValue = new Dictionary<int, float>();
        }

        /// <summary>
        /// Sets the unique key of this Cluster used for storing modifications within the Custom Room Properties.
        /// </summary>
        private void Start()
        {
            propertiesKey = "Cluster " + ClusterId;
        }

        #endregion

        #region CLASS FUNCTIONS

        /// <summary>
        /// Adds a Block to the Cluster.
        /// This is called by the WorldGenerator while the generation process is running.
        /// In order to modify Blocks directly, we are storing the ID as well as a reference to the certain GameObject.
        /// </summary>
        public void AddBlock(int blockId, GameObject block)
        {
            Blocks.Add(blockId, block);
        }

        /// <summary>
        /// Gets called before a new world can be generated.
        /// Destroys each Block from this Cluster and removes the data stored in the Custom Room Properties.
        /// </summary>
        public void DestroyCluster()
        {
            foreach (GameObject block in Blocks.Values)
            {
                Destroy(block);
            }

            Blocks.Clear();

            if (PhotonNetwork.IsMasterClient)
            {
                RemoveClusterFromRoomProperties();
            }
        }

        /// <summary>
        /// Decreases a Block's height locally before applying the modification to the Custom Room Properties.
        /// </summary>
        public void DecreaseBlockHeight(int blockId)
        {
            float height = Blocks[blockId].transform.localScale.y;
            height = Mathf.Max((height - 1.0f), 0.0f);

            SetBlockHeightLocal(blockId, height);
        }

        /// <summary>
        /// Increases a Block's height locally before applying the modification to the Custom Room Properties.
        /// </summary>
        public void IncreaseBlockHeight(int blockId)
        {
            float height = Blocks[blockId].transform.localScale.y;
            height = Mathf.Min((height + 1.0f), 8.0f);

            SetBlockHeightLocal(blockId, height);
        }

        /// <summary>
        /// Gets called when a remote client has modified a certain Block within this Cluster.
        /// Called by the WorldGenerator or the Cluster itself after the Custom Room Properties have been updated.
        /// </summary>
        public void SetBlockHeightRemote(int blockId, float height)
        {
            GameObject block = Blocks[blockId];

            Vector3 scale = block.transform.localScale;
            Vector3 position = block.transform.localPosition;

            block.transform.localScale = new Vector3(scale.x, height, scale.z);
            block.transform.localPosition = new Vector3(position.x, height / 2.0f, position.z);
        }

        /// <summary>
        /// Gets called whenever the local client modifies any Block within this Cluster.
        /// The modification will be applied to the Block first before it is published to the Custom Room Properties.
        /// </summary>
        private void SetBlockHeightLocal(int blockId, float height)
        {
            GameObject block = Blocks[blockId];

            Vector3 scale = block.transform.localScale;
            Vector3 position = block.transform.localPosition;

            block.transform.localScale = new Vector3(scale.x, height, scale.z);
            block.transform.localPosition = new Vector3(position.x, height / 2.0f, position.z);

            UpdateRoomProperties(blockId, height);
        }

        /// <summary>
        /// Gets called in order to update the Custom Room Properties with the modification made by the local client.
        /// </summary>
        private void UpdateRoomProperties(int blockId, float height)
        {
            propertiesValue[blockId] = height;

            Hashtable properties = new Hashtable {{propertiesKey, propertiesValue}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }

        /// <summary>
        /// Removes the modifications of this Cluster from the Custom Room Properties.
        /// </summary>
        private void RemoveClusterFromRoomProperties()
        {
            Hashtable properties = new Hashtable {{propertiesKey, null}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }

        #endregion

        #region PUN CALLBACKS

        /// <summary>
        /// Gets called whenever the Custom Room Properties are updated.
        /// When the changed properties contain the previously set PropertiesKey (basically the Cluster ID),
        /// the Cluster and its Blocks will be updated accordingly.
        /// </summary>
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(propertiesKey))
            {
                if (propertiesThatChanged[propertiesKey] == null)
                {
                    propertiesValue = new Dictionary<int, float>();
                    return;
                }

                propertiesValue = (Dictionary<int, float>) propertiesThatChanged[propertiesKey];

                foreach (KeyValuePair<int, float> pair in propertiesValue)
                {
                    SetBlockHeightRemote(pair.Key, pair.Value);
                }
            }
        }

        #endregion
    }
}