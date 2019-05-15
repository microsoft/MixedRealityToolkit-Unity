// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Service that manages <see cref="SynchronizedTransform"/>s
    /// </summary>
    public class SynchronizedTransformService : SynchronizedComponentService<SynchronizedTransformService, RemoteTransform>, ISynchronizedComponentService
    {
        static readonly ShortID SynchronizedSceneChangeTypeTransform = new ShortID("TRA");

        private void Start()
        {
            SynchronizedSceneManager.Instance.RegisterService(this, new SynchronizedComponentDefinition<SynchronizedTransform>(typeof(Transform)));
        }

        /// <inheritdoc />
        public override ShortID GetID()
        {
            return SynchronizedSceneChangeTypeTransform;
        }

        /// <summary>
        /// Cleans synchronization logic for the provided game object
        /// </summary>
        /// <param name="mirror"></param>
        public override void Destroy(GameObject mirror)
        {
            base.Destroy(mirror);

            SynchronizedSceneManager.Instance.DestroyMirror(mirror.GetComponent<RemoteTransform>().Id);
        }

        /// <summary>
        /// Provides a network message to the game objects remote transform for interpolation
        /// </summary>
        /// <param name="message">network message</param>
        /// <param name="mirror">game object that has remote transform</param>
        /// <param name="lerpVal">interpolation value</param>
        public override void LerpRead(BinaryReader message, GameObject mirror, float lerpVal)
        {
            RemoteTransform remoteTransform = mirror.GetComponent<RemoteTransform>();
            if (remoteTransform != null)
            {
                remoteTransform.LerpRead(message, lerpVal);
            }
        }
    }
}
