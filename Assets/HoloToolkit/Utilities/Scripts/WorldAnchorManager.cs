// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;

#if UNITY_EDITOR || UNITY_WSA
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Wrapper around world anchor store to streamline some of the persistence api busy work.
    /// </summary>
    public class WorldAnchorManager : Singleton<WorldAnchorManager>
    {
        /// <summary>
        /// To prevent initializing too many anchors at once
        /// and to allow for the WorldAnchorStore to load asyncronously
        /// without callers handling the case where the store isn't loaded yet
        /// we'll setup a queue of anchor attachment operations.  
        /// The AnchorAttachmentInfo struct has the data needed to do this.
        /// </summary>
        private struct AnchorAttachmentInfo
        {
            public GameObject GameObjectToAnchor { get; set; }
            public string AnchorName { get; set; }
            public AnchorOperation Operation { get; set; }
        }

        private enum AnchorOperation
        {
            Create,
            Delete
        }

        /// <summary>
        /// The queue mentioned above.
        /// </summary>
        private Queue<AnchorAttachmentInfo> anchorOperations = new Queue<AnchorAttachmentInfo>();

#if UNITY_EDITOR || UNITY_WSA
        /// <summary>
        /// The WorldAnchorStore for the current application.
        /// Can be null when the application starts.
        /// </summary>
        public WorldAnchorStore AnchorStore { get; private set; }

        /// <summary>
        /// Callback function that contains the WorldAnchorStore object.
        /// </summary>
        /// <param name="anchorStore">The WorldAnchorStore to cache.</param>
        private void AnchorStoreReady(WorldAnchorStore anchorStore)
        {
            AnchorStore = anchorStore;
        }
#endif

        /// <summary>
        /// When the app starts grab the anchor store immediately.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            Debug.LogWarning("World Anchor Manager does not work in the editor. Anchor Store will never be ready.");
#endif
#if UNITY_EDITOR || UNITY_WSA
            AnchorStore = null;
            WorldAnchorStore.GetAsync(AnchorStoreReady);
#endif
        }

#if UNITY_EDITOR || UNITY_WSA
        /// <summary>
        /// Each frame see if there is work to do and if we can do a unit, do it.
        /// </summary>
        private void Update()
        {
            if (AnchorStore != null && anchorOperations.Count > 0)
            {
                DoAnchorOperation(anchorOperations.Dequeue());
            }
        }
#endif

        /// <summary>
        /// Attaches an anchor to the game object.  If the anchor store has
        /// an anchor with the specified name it will load the acnhor, otherwise
        /// a new anchor will be saved under the specified name.
        /// </summary>
        /// <param name="gameObjectToAnchor">The Gameobject to attach the anchor to.</param>
        /// <param name="anchorName">Name of the anchor.</param>
        public void AttachAnchor(GameObject gameObjectToAnchor, string anchorName)
        {
            if (gameObjectToAnchor == null)
            {
                Debug.LogError("Must pass in a valid gameObject");
                return;
            }

            if (string.IsNullOrEmpty(anchorName))
            {
                Debug.LogError("Must supply an AnchorName.");
                return;
            }

            anchorOperations.Enqueue(
                new AnchorAttachmentInfo
                {
                    GameObjectToAnchor = gameObjectToAnchor,
                    AnchorName = anchorName,
                    Operation = AnchorOperation.Create
                }
            );
        }

        /// <summary>
        /// Removes the anchor from the game object and deletes the anchor
        /// from the anchor store.
        /// </summary>
        /// <param name="gameObjectToUnanchor">gameObject to remove the anchor from.</param>
        public void RemoveAnchor(GameObject gameObjectToUnanchor)
        {
            if (gameObjectToUnanchor == null)
            {
                Debug.LogError("Invalid GameObject");
                return;
            }

#if UNITY_EDITOR || UNITY_WSA
            // This case is unexpected, but just in case.
            if (AnchorStore == null)
            {
                Debug.LogError("remove anchor called before anchor store is ready.");
                return;
            }
#endif

            anchorOperations.Enqueue(
                new AnchorAttachmentInfo
                {
                    GameObjectToAnchor = gameObjectToUnanchor,
                    AnchorName = string.Empty,
                    Operation = AnchorOperation.Delete
                });
        }

        /// <summary>
        /// Removes all anchors from the scene and deletes them from the anchor store.
        /// </summary>
        public void RemoveAllAnchors()
        {
#if UNITY_EDITOR || UNITY_WSA
            SpatialMappingManager spatialMappingManager = SpatialMappingManager.Instance;

            // This case is unexpected, but just in case.
            if (AnchorStore == null)
            {
                Debug.LogError("remove all anchors called before anchor store is ready.");
            }

            WorldAnchor[] anchors = FindObjectsOfType<WorldAnchor>();

            if (anchors != null)
            {
                foreach (WorldAnchor anchor in anchors)
                {
                    // Don't remove SpatialMapping anchors if exists
                    if (spatialMappingManager == null ||
                        anchor.gameObject.transform.parent.gameObject != spatialMappingManager.gameObject)
                    {
                        anchorOperations.Enqueue(new AnchorAttachmentInfo()
                        {
                            AnchorName = anchor.name,
                            GameObjectToAnchor = anchor.gameObject,
                            Operation = AnchorOperation.Delete
                        });
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Function that actually adds the anchor to the game object.
        /// </summary>
        /// <param name="anchorAttachmentInfo">Parameters for attaching the anchor.</param>
        private void DoAnchorOperation(AnchorAttachmentInfo anchorAttachmentInfo)
        {
#if UNITY_EDITOR || UNITY_WSA
            switch (anchorAttachmentInfo.Operation)
            {
                case AnchorOperation.Create:
                    string anchorName = anchorAttachmentInfo.AnchorName;
                    GameObject gameObjectToAnchor = anchorAttachmentInfo.GameObjectToAnchor;

                    if (gameObjectToAnchor == null)
                    {
                        Debug.LogError("GameObject must have been destroyed before we got a chance to anchor it.");
                        break;
                    }

                    // Try to load a previously saved world anchor.
                    WorldAnchor savedAnchor = AnchorStore.Load(anchorName, gameObjectToAnchor);
                    if (savedAnchor == null)
                    {
                        // Either world anchor was not saved / does not exist or has a different name.
                        Debug.LogWarning(gameObjectToAnchor.name + " : World anchor could not be loaded for this game object. Creating a new anchor.");

                        // Create anchor since one does not exist.
                        CreateAnchor(gameObjectToAnchor, anchorName);
                    }
                    else
                    {
                        savedAnchor.name = anchorName;
                        Debug.Log(gameObjectToAnchor.name + " : World anchor loaded from anchor store and updated for this game object.");
                    }

                    break;
                case AnchorOperation.Delete:
                    if (AnchorStore == null)
                    {
                        Debug.LogError("Remove anchor called before anchor store is ready.");
                        break;
                    }

                    GameObject gameObjectToUnanchor = anchorAttachmentInfo.GameObjectToAnchor;
                    var anchor = gameObjectToUnanchor.GetComponent<WorldAnchor>();

                    if (anchor != null)
                    {
                        AnchorStore.Delete(anchor.name);
                        DestroyImmediate(anchor);
                    }
                    else
                    {
                        Debug.LogError("Cannot get anchor while deleting");
                    }

                    break;
            }
#endif
        }

        /// <summary>
        /// Creates an anchor, attaches it to the gameObjectToAnchor, and saves the anchor to the anchor store.
        /// </summary>
        /// <param name="gameObjectToAnchor">The GameObject to attach the anchor to.</param>
        /// <param name="anchorName">The name to give to the anchor.</param>
        private void CreateAnchor(GameObject gameObjectToAnchor, string anchorName)
        {
#if UNITY_EDITOR || UNITY_WSA
            var anchor = gameObjectToAnchor.AddComponent<WorldAnchor>();
            anchor.name = anchorName;

            // Sometimes the anchor is located immediately. In that case it can be saved immediately.
            if (anchor.isLocated)
            {
                SaveAnchor(anchor);
            }
            else
            {
                // Other times we must wait for the tracking system to locate the world.
                anchor.OnTrackingChanged += Anchor_OnTrackingChanged;
            }
#endif
        }

#if UNITY_EDITOR || UNITY_WSA
        /// <summary>
        /// When an anchor isn't located immediately we subscribe to this event so
        /// we can save the anchor when it is finally located.
        /// </summary>
        /// <param name="self">The anchor that is reporting a tracking changed event.</param>
        /// <param name="located">Indicates if the anchor is located or not located.</param>
        private void Anchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            if (located)
            {
                Debug.Log(gameObject.name + " : World anchor located successfully.");

                SaveAnchor(self);

                // Once the anchor is located we can unsubscribe from this event.
                self.OnTrackingChanged -= Anchor_OnTrackingChanged;
            }
            else
            {
                Debug.LogError(gameObject.name + " : World anchor failed to locate.");
            }
        }

        /// <summary>
        /// Saves the anchor to the anchor store.
        /// </summary>
        /// <param name="anchor"></param>
        private void SaveAnchor(WorldAnchor anchor)
        {
            // Save the anchor to persist holograms across sessions.
            if (AnchorStore.Save(anchor.name, anchor))
            {
                Debug.Log(gameObject.name + " : World anchor saved successfully.");
            }
            else
            {
                Debug.LogError(gameObject.name + " : World anchor save failed.");
            }
        }
#endif
    }
}
