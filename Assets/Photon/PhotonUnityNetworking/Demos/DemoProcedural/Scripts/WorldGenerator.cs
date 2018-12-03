using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.Demo.Procedural
{
    /// <summary>
    /// Describes the Size of the World.
    /// A 'Tiny' world for example will be created with 16 x 16 Blocks.
    /// </summary>
    public enum WorldSize
    {
        Tiny = 16,
        Small = 32,
        Medium = 64,
        Large = 128
    }

    /// <summary>
    /// Describes the type of the generated world.
    /// This basically influences the maximum height of a Block.
    /// </summary>
    public enum WorldType
    {
        Flat = 4,
        Standard = 8,
        Mountain = 16
    }

    /// <summary>
    /// Describes how many Blocks are part of one CLuster.
    /// Having a 'Small' ClusterSize increases the amount of Clusters being created,
    /// whereat generating a world with a 'Large' ClusterSize doesn't create that many Clusters.
    /// </summary>
    public enum ClusterSize
    {
        Small = 4,
        Medium = 16,
        Large = 64
    }

    /// <summary>
    /// The World Generator creates a world based on four options the current MasterClient can set.
    /// These options are available on the Ingame Control Panel. If those options are confirmed by the current MasterClient,
    /// they will be stored in the Custom Room Properties to make them available on all clients.
    /// These options are:
    /// 1) a numerical Seed to make sure that each client generates the same world and to avoid Random functions and 'network-instantiate' everything
    /// 2) the World Size to describe how large the generated world should be
    /// 3) the Cluster Size to describe how many Blocks are inside each Cluster
    /// 4) the World Type to make the generated world appear in different 'looks'.
    /// </summary>
    public class WorldGenerator : MonoBehaviour
    {
        public readonly string SeedPropertiesKey = "Seed";
        public readonly string WorldSizePropertiesKey = "WorldSize";
        public readonly string ClusterSizePropertiesKey = "ClusterSize";
        public readonly string WorldTypePropertiesKey = "WorldType";

        private static WorldGenerator instance;

        public static WorldGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<WorldGenerator>();
                }

                return instance;
            }
        }

        public int Seed { get; set; }

        public WorldSize WorldSize { get; set; }

        public ClusterSize ClusterSize { get; set; }

        public WorldType WorldType { get; set; }

        private Dictionary<int, GameObject> clusterList;

        public Material[] WorldMaterials;

        #region UNITY

        public void Awake()
        {
            clusterList = new Dictionary<int, GameObject>();

            WorldSize = WorldSize.Tiny;
            ClusterSize = ClusterSize.Small;
            WorldType = WorldType.Standard;
        }

        #endregion

        #region CLASS FUNCTIONS

        /// <summary>
        /// Called whenever a client receives a Custom Room Properties update containing all necessary information for creating a world.
        /// If there is currently a world generation process running, it will be stopped automatically.
        /// Also if there is a world already existing, it will be destroyed before the new one gets created.
        /// </summary>
        public void CreateWorld()
        {
            StopAllCoroutines();
            DestroyWorld();
            StartCoroutine(GenerateWorld());
        }

        /// <summary>
        /// Destroys each Block from each Cluster before actually destroying the Cluster itself.
        /// </summary>
        private void DestroyWorld()
        {
            foreach (GameObject cluster in clusterList.Values)
            {
                Cluster clusterComponent = cluster.GetComponent<Cluster>();
                clusterComponent.DestroyCluster();

                Destroy(cluster);
            }

            clusterList.Clear();
        }

        /// <summary>
        /// Whenever the 'Confirm' button on the Control Panel is clicked by the MasterClient,
        /// the Room Properties will be updated with the settings he defined.
        /// </summary>
        public void ConfirmAndUpdateProperties()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Hashtable properties = new Hashtable
            {
                {SeedPropertiesKey, Seed},
                {WorldSizePropertiesKey, (int) WorldSize},
                {ClusterSizePropertiesKey, (int) ClusterSize},
                {WorldTypePropertiesKey, (int) WorldType}
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }

        /// <summary>
        /// Decreases the height of a certain Block from a certain Cluster.
        /// </summary>
        public void DecreaseBlockHeight(int clusterId, int blockId)
        {
            Cluster c = clusterList[clusterId].GetComponent<Cluster>();

            if (c != null)
            {
                c.DecreaseBlockHeight(blockId);
            }
        }

        /// <summary>
        /// Increases the height of a certain Block from a certain Cluster.
        /// </summary>
        public void IncreaseBlockHeight(int clusterId, int blockId)
        {
            Cluster c = clusterList[clusterId].GetComponent<Cluster>();

            if (c != null)
            {
                c.IncreaseBlockHeight(blockId);
            }
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Generates a new world based on the settings made either by the MasterClient on the 
        /// Ingame Control Panel or after receiving the new settings from the Custom Room Properties update.
        /// </summary>
        private IEnumerator GenerateWorld()
        {
            Debug.Log(string.Format("<b>Procedural Demo</b>: Creating world using Seed: {0}, World Size: {1}, Cluster Size: {2} and World Type: {3}", Seed, WorldSize, ClusterSize, WorldType));

            Simplex.Noise.Seed = Seed;

            int clusterId = 0;

            // Instantiating all necessary clusters at their target position
            for (int x = 0; x < (int) WorldSize; x += (int) Mathf.Sqrt((int) ClusterSize))
            {
                for (int z = 0; z < (int) WorldSize; z += (int) Mathf.Sqrt((int) ClusterSize))
                {
                    GameObject cluster = new GameObject();
                    cluster.name = "Cluster " + clusterId;

                    cluster.transform.SetParent(transform);
                    cluster.transform.position = new Vector3(x, 0.0f, z);

                    Cluster clusterComponent = cluster.AddComponent<Cluster>();
                    clusterComponent.ClusterId = clusterId;

                    clusterList.Add(clusterId++, cluster);
                }
            }

            yield return new WaitForEndOfFrame();

            // Instantiating all necessary blocks as a child of a certain cluster
            foreach (GameObject cluster in clusterList.Values)
            {
                Vector3 clusterPosition = cluster.transform.position;

                int blockId = 0;

                for (int x = 0; x < (int) Mathf.Sqrt((int) ClusterSize); ++x)
                {
                    for (int z = 0; z < (int) Mathf.Sqrt((int) ClusterSize); ++z)
                    {
                        float noiseValue = Simplex.Noise.CalcPixel2D((int) clusterPosition.x + x, (int) clusterPosition.z + z, 0.02f);

                        int height = (int) noiseValue / (int) (256.0f / (float) WorldType);
                        int materialIndex = (int) noiseValue / (int) (256.0f / WorldMaterials.Length);

                        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        block.name = "Block " + blockId;

                        block.transform.SetParent(cluster.transform);
                        block.transform.localScale = new Vector3(1.0f, height, 1.0f);
                        block.transform.position = new Vector3(clusterPosition.x + x, height / 2.0f, clusterPosition.z + z);
                        block.GetComponent<MeshRenderer>().material = WorldMaterials[materialIndex];

                        Block blockComponent = block.AddComponent<Block>();
                        blockComponent.BlockId = blockId;
                        blockComponent.ClusterId = cluster.GetComponent<Cluster>().ClusterId;

                        cluster.GetComponent<Cluster>().AddBlock(blockId++, block);
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            // Applying modifications made to the world when joining the room later or while it is created
            foreach (DictionaryEntry entry in PhotonNetwork.CurrentRoom.CustomProperties)
            {
                if (entry.Value == null)
                {
                    continue;
                }

                string key = entry.Key.ToString();

                if ((key == SeedPropertiesKey) || (key == WorldSizePropertiesKey) || (key == ClusterSizePropertiesKey) || (key == WorldTypePropertiesKey))
                {
                    continue;
                }

                int indexOfBlank = key.IndexOf(' ');
                key = key.Substring(indexOfBlank + 1, (key.Length - (indexOfBlank + 1)));

                int.TryParse(key, out clusterId);

                GameObject cluster;
                if (clusterList.TryGetValue(clusterId, out cluster))
                {
                    Cluster c = cluster.GetComponent<Cluster>();

                    if (c != null)
                    {
                        Dictionary<int, float> clusterModifications = (Dictionary<int, float>) entry.Value;

                        foreach (KeyValuePair<int, float> pair in clusterModifications)
                        {
                            c.SetBlockHeightRemote(pair.Key, pair.Value);
                        }
                    }
                }
            }
        }

        #endregion
    }
}