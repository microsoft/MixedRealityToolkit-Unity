// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Service that manages <see cref="TransformBroadcaster"/>s
    /// </summary>
    public class TransformBroadcasterService : ComponentBroadcasterService<TransformBroadcasterService, TransformObserver>, IComponentBroadcasterService
    {
        static readonly ShortID SynchronizedSceneChangeTypeTransform = new ShortID("TRA");

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<TransformBroadcaster>(typeof(Transform)));
        }

        /// <inheritdoc />
        public override ShortID GetID()
        {
            return SynchronizedSceneChangeTypeTransform;
        }

        /// <summary>
        /// Cleans up state synchronization logic for the provided game object
        /// </summary>
        /// <param name="mirror"></param>
        public override void Destroy(GameObject mirror)
        {
            base.Destroy(mirror);

            StateSynchronizationSceneManager.Instance.DestroyMirror(mirror.GetComponent<TransformObserver>().Id);
        }

        /// <summary>
        /// Provides a network message to the game objects transform observer for interpolation
        /// </summary>
        /// <param name="message">network message</param>
        /// <param name="mirror">game object that has transform observer</param>
        /// <param name="lerpVal">interpolation value</param>
        public override void LerpRead(BinaryReader message, GameObject mirror, float lerpVal)
        {
            TransformObserver TransformObserver = mirror.GetComponent<TransformObserver>();
            if (TransformObserver != null)
            {
                TransformObserver.LerpRead(message, lerpVal);
            }
        }
    }
}
