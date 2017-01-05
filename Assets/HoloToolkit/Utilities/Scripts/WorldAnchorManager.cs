// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA.Persistence;
using System.Collections.Generic;
using UnityEngine.VR.WSA;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Wrapper around world anchor store to streamline some of the
    /// persistence api busy work.
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
        }

        /// <summary>
        /// The queue mentioned above.
        /// </summary>
        private Queue<AnchorAttachmentInfo> anchorOperations = new Queue<AnchorAttachmentInfo>();

        /// <summary>
        /// The WorldAnchorStore for the current application.  
        /// Can be null when the application starts.
        /// </summary>
        public WorldAnchorStore AnchorStore { get; private set; }

        /// <summary>
        /// Callback function that contains the WorldAnchorStore object.
        /// </summary>
        /// <param name="Store">The WorldAnchorStore to cache.</param>
        private void AnchorStoreReady(WorldAnchorStore Store)
        {
            AnchorStore = Store;
        }

        /// <summary>
        /// When the app starts grab the anchor store immediately.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            AnchorStore = null;
            WorldAnchorStore.GetAsync(AnchorStoreReady);
        }

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
                new AnchorAttachmentInfo()
                {
                    GameObjectToAnchor = gameObjectToAnchor,
                    AnchorName = anchorName
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
            // This case is unexpected, but just in case.
            if (AnchorStore == null)
            {
                Debug.LogError("remove anchor called before anchor store is ready.");
            }

            WorldAnchor anchor = gameObjectToUnanchor.GetComponent<WorldAnchor>();

            if (anchor != null)
            {
                AnchorStore.Delete(anchor.name);
                DestroyImmediate(anchor);
            }
        }

        /// <summary>
        /// Function that actually adds the anchor to the game object.
        /// </summary>
        /// <param name="anchorAttachmentInfo">Parameters for attaching the anchor.</param>
        private void DoAnchorOperation(AnchorAttachmentInfo anchorAttachmentInfo)
        {
            string AnchorName = anchorAttachmentInfo.AnchorName;
            GameObject gameObjectToAnchor = anchorAttachmentInfo.GameObjectToAnchor;

            if (gameObjectToAnchor == null)
            {
                Debug.Log("GameObject must have been destroyed before we got a chance to anchor it.");
                return;
            }

            // Try to load a previously saved world anchor.
            WorldAnchor savedAnchor = AnchorStore.Load(AnchorName, gameObjectToAnchor);
            if (savedAnchor == null)
            {
                // Either world anchor was not saved / does not exist or has a different name.
                Debug.Log(gameObjectToAnchor.name + " : World anchor could not be loaded for this game object. Creating a new anchor.");

                // Create anchor since one does not exist.
                CreateAnchor(gameObjectToAnchor, AnchorName);
            }
            else
            {
                savedAnchor.name = AnchorName;
                Debug.Log(gameObjectToAnchor.name + " : World anchor loaded from anchor store and updated for this game object.");
            }
        }

        /// <summary>
        /// Creates an anchor, attaches it to the gameObjectToAnchor, and saves the anchor to the anchor store.
        /// </summary>
        /// <param name="gameObjectToAnchor">The GameObject to attach the anchor to.</param>
        /// <param name="anchorName">The name to give to the anchor.</param>
        private void CreateAnchor(GameObject gameObjectToAnchor, string anchorName)
        {
            WorldAnchor anchor = gameObjectToAnchor.AddComponent<WorldAnchor>();
            anchor.name = anchorName;

            // Sometimes the anchor is located immediately. In that case it can be saved immediately.
            if (anchor.isLocated)
            {
                SaveAnchor(anchor);
            }
            else
            {
                // Othertimes we must wait for the 
                anchor.OnTrackingChanged += Anchor_OnTrackingChanged;
            }
        }

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
    }
}
