using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Procedural
{
    /// <summary>
    /// The Ingame Control Panel basically controls the WorldGenerator.
    /// It is only interactable for the current MasterClient in the room.
    /// </summary>
    public class IngameControlPanel : MonoBehaviourPunCallbacks
    {
        private bool isSeedValid;

        private InputField seedInputField;
        private Dropdown worldSizeDropdown;
        private Dropdown clusterSizeDropdown;
        private Dropdown worldTypeDropdown;
        private Button generateWorldButton;

        #region UNITY

        /// <summary>
        /// When the object gets created, all necessary references are set up.
        /// Also the UI elements get set up properly in order to control the WorldGenerator.
        /// </summary>
        public void Awake()
        {
            isSeedValid = false;

            seedInputField = GetComponentInChildren<InputField>();
            seedInputField.characterLimit = 10;
            seedInputField.characterValidation = InputField.CharacterValidation.Integer;
            seedInputField.interactable = PhotonNetwork.PhotonServerSettings.StartInOfflineMode;
            seedInputField.onValueChanged.AddListener((string value) =>
            {
                int seed;
                if (int.TryParse(value, out seed))
                {
                    isSeedValid = true;
                    WorldGenerator.Instance.Seed = seed;
                }
                else
                {
                    isSeedValid = false;
                    Debug.LogError("Invalid Seed entered. Only numeric Seeds are allowed.");
                }
            });

            worldSizeDropdown = GetComponentsInChildren<Dropdown>()[0];
            worldSizeDropdown.interactable = PhotonNetwork.PhotonServerSettings.StartInOfflineMode;
            worldSizeDropdown.onValueChanged.AddListener((int value) =>
            {
                switch (value)
                {
                    case 0:
                    {
                        WorldGenerator.Instance.WorldSize = WorldSize.Tiny;
                        break;
                    }
                    case 1:
                    {
                        WorldGenerator.Instance.WorldSize = WorldSize.Small;
                        break;
                    }
                    case 2:
                    {
                        WorldGenerator.Instance.WorldSize = WorldSize.Medium;
                        break;
                    }
                    case 3:
                    {
                        WorldGenerator.Instance.WorldSize = WorldSize.Large;
                        break;
                    }
                }
            });

            clusterSizeDropdown = GetComponentsInChildren<Dropdown>()[1];
            clusterSizeDropdown.interactable = PhotonNetwork.PhotonServerSettings.StartInOfflineMode;
            clusterSizeDropdown.onValueChanged.AddListener((int value) =>
            {
                switch (value)
                {
                    case 0:
                    {
                        WorldGenerator.Instance.ClusterSize = ClusterSize.Small;
                        break;
                    }
                    case 1:
                    {
                        WorldGenerator.Instance.ClusterSize = ClusterSize.Medium;
                        break;
                    }
                    case 2:
                    {
                        WorldGenerator.Instance.ClusterSize = ClusterSize.Large;
                        break;
                    }
                }
            });

            worldTypeDropdown = GetComponentsInChildren<Dropdown>()[2];
            worldTypeDropdown.interactable = PhotonNetwork.PhotonServerSettings.StartInOfflineMode;
            worldTypeDropdown.onValueChanged.AddListener((int value) =>
            {
                switch (value)
                {
                    case 0:
                    {
                        WorldGenerator.Instance.WorldType = WorldType.Flat;
                        break;
                    }
                    case 1:
                    {
                        WorldGenerator.Instance.WorldType = WorldType.Standard;
                        break;
                    }
                    case 2:
                    {
                        WorldGenerator.Instance.WorldType = WorldType.Mountain;
                        break;
                    }
                }
            });

            generateWorldButton = GetComponentInChildren<Button>();
            generateWorldButton.interactable = PhotonNetwork.PhotonServerSettings.StartInOfflineMode;
            generateWorldButton.onClick.AddListener(() =>
            {
                if (!PhotonNetwork.InRoom)
                {
                    Debug.LogError("Client is not in a room.");
                    return;
                }

                if (!isSeedValid)
                {
                    Debug.LogError("Invalid Seed entered. Only numeric Seeds are allowed.");
                    return;
                }

                WorldGenerator.Instance.ConfirmAndUpdateProperties();
            });
        }

        #endregion

        #region PUN CALLBACKS

        /// <summary>
        /// Gets called when the local client has joined the room.
        /// Since only the MasterClient can control the WorldGenerator,
        /// we are checking if we have to make the UI controls available for the local client.
        /// </summary>
        public override void OnJoinedRoom()
        {
            seedInputField.interactable = PhotonNetwork.IsMasterClient;
            worldSizeDropdown.interactable = PhotonNetwork.IsMasterClient;
            clusterSizeDropdown.interactable = PhotonNetwork.IsMasterClient;
            worldTypeDropdown.interactable = PhotonNetwork.IsMasterClient;
            generateWorldButton.interactable = PhotonNetwork.IsMasterClient;
        }

        /// <summary>
        /// Gets called whenever the current MasterClient has left the room and a new one is selected.
        /// If the local client is the new MasterClient, we make the UI controls available for him.
        /// </summary>
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            seedInputField.interactable = newMasterClient.IsLocal;
            worldSizeDropdown.interactable = newMasterClient.IsLocal;
            clusterSizeDropdown.interactable = newMasterClient.IsLocal;
            worldTypeDropdown.interactable = newMasterClient.IsLocal;
            generateWorldButton.interactable = newMasterClient.IsLocal;
        }

        /// <summary>
        /// Gets called whenever the Custom Room Properties are updated.
        /// In this callback we are interested in the settings the MasterClient can apply to the WorldGenerator.
        /// If all possible settings are updated (and available within the updated properties), these settings are also used
        /// to update the Ingame Control Panel as well as the WorldGenerator on all clients.
        /// Afterwards the WorldGenerator creates a new world with the new settings.
        /// </summary>
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(WorldGenerator.Instance.SeedPropertiesKey) && propertiesThatChanged.ContainsKey(WorldGenerator.Instance.WorldSizePropertiesKey) && propertiesThatChanged.ContainsKey(WorldGenerator.Instance.ClusterSizePropertiesKey) && propertiesThatChanged.ContainsKey(WorldGenerator.Instance.WorldTypePropertiesKey))
            {
                // Updating Seed
                int seed = (int) propertiesThatChanged[WorldGenerator.Instance.SeedPropertiesKey];
                seedInputField.text = seed.ToString();

                // Updating World Size
                WorldSize worldSize = (WorldSize) propertiesThatChanged[WorldGenerator.Instance.WorldSizePropertiesKey];
                switch (worldSize)
                {
                    case WorldSize.Tiny:
                    {
                        worldSizeDropdown.value = 0;
                        break;
                    }
                    case WorldSize.Small:
                    {
                        worldSizeDropdown.value = 1;
                        break;
                    }
                    case WorldSize.Medium:
                    {
                        worldSizeDropdown.value = 2;
                        break;
                    }
                    case WorldSize.Large:
                    {
                        worldSizeDropdown.value = 3;
                        break;
                    }
                }

                // Updating Cluster Size
                ClusterSize clusterSize = (ClusterSize) propertiesThatChanged[WorldGenerator.Instance.ClusterSizePropertiesKey];
                switch (clusterSize)
                {
                    case ClusterSize.Small:
                    {
                        clusterSizeDropdown.value = 0;
                        break;
                    }
                    case ClusterSize.Medium:
                    {
                        clusterSizeDropdown.value = 1;
                        break;
                    }
                    case ClusterSize.Large:
                    {
                        clusterSizeDropdown.value = 2;
                        break;
                    }
                }

                // Updating World Type
                WorldType worldType = (WorldType) propertiesThatChanged[WorldGenerator.Instance.WorldTypePropertiesKey];
                switch (worldType)
                {
                    case WorldType.Flat:
                    {
                        worldTypeDropdown.value = 0;
                        break;
                    }
                    case WorldType.Standard:
                    {
                        worldTypeDropdown.value = 1;
                        break;
                    }
                    case WorldType.Mountain:
                    {
                        worldTypeDropdown.value = 2;
                        break;
                    }
                }

                // Generating a new world
                WorldGenerator.Instance.CreateWorld();
            }
        }

        #endregion
    }
}