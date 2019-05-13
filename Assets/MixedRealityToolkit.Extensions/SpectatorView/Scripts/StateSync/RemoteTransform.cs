// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class RemoteTransform : RemoteComponent<Transform>
    {
        private const string temporaryGameObjectName = "TEMP-0B111914-CE43-43B0-A7FB-7D546CE69FEE";

        /// <summary>
        /// Unique id for the transform
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Reads a network message and updates local state data
        /// </summary>
        /// <param name="sendingEndpoint">Sender endpoint</param>
        /// <param name="message">Received payload</param>
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            short id = message.ReadInt16();

            SynchronizedTransformChangeType changeType = (SynchronizedTransformChangeType)message.ReadByte();

            GameObject mirror = null;
            RectTransform rectTransform = null;

            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.RectTransform))
            {
                mirror = mirror ?? SynchronizedSceneManager.Instance.GetOrCreateMirror(id);
                rectTransform = mirror.GetComponent<RectTransform>();
                if (rectTransform == null)
                    rectTransform = mirror.AddComponent<RectTransform>();
            }

            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Name))
            {
                mirror = mirror ?? SynchronizedSceneManager.Instance.GetOrCreateMirror(id);
                mirror.name = message.ReadString();
            }
            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Layer))
            {
                mirror = mirror ?? SynchronizedSceneManager.Instance.GetOrCreateMirror(id);
                mirror.layer = message.ReadInt32();
            }
            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Parent))
            {
                mirror = mirror ?? SynchronizedSceneManager.Instance.GetOrCreateMirror(id);
                short parentId = message.ReadInt16();
                int siblingIndex = message.ReadInt32();
                Transform newParent;
                if (parentId == SynchronizedTransform.NullTransformId)
                {
                    newParent = SynchronizedSceneManager.Instance.RootTransform;
                }
                else
                {
                    newParent = SynchronizedSceneManager.Instance.GetOrCreateMirror(parentId).transform;
                }

                if (siblingIndex < newParent.childCount)
                {
                    Transform existingChildAtIndex = newParent.GetChild(siblingIndex);
                    if (existingChildAtIndex.gameObject.name == temporaryGameObjectName)
                    {
                        Destroy(existingChildAtIndex.gameObject);
                    }
                }
                else
                {
                    for (int i = newParent.childCount; i < siblingIndex; i++)
                    {
                        GameObject temp = new GameObject(temporaryGameObjectName);
                        temp.SetActive(false);
                        temp.transform.SetParent(newParent);
                        temp.transform.SetSiblingIndex(i);
                    }
                }

                mirror.transform.SetParent(newParent, worldPositionStays: false);
                mirror.transform.SetSiblingIndex(siblingIndex);
            }
            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Position))
            {
                mirror = mirror ?? SynchronizedSceneManager.Instance.GetOrCreateMirror(id);
                mirror.transform.localPosition = message.ReadVector3();
            }
            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Rotation))
            {
                mirror = mirror ?? SynchronizedSceneManager.Instance.GetOrCreateMirror(id);
                mirror.transform.localRotation = message.ReadQuaternion();
            }
            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Scale))
            {
                mirror = mirror ?? SynchronizedSceneManager.Instance.GetOrCreateMirror(id);
                mirror.transform.localScale = message.ReadVector3();
            }
            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.IsActive))
            {
                bool isActive = message.ReadBoolean();
                if (isActive)
                {
                    mirror = mirror ?? SynchronizedSceneManager.Instance.GetOrCreateMirror(id);
                    mirror.SetActive(true);
                }
                else
                {
                    mirror = SynchronizedSceneManager.Instance.FindGameObjectWithId(id);
                    if (mirror != null)
                    {
                        mirror.SetActive(false);
                    }
                }
            }

            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.RectTransform))
            {
                SynchronizedRectTransform.Read(rectTransform, message);
                ComponentExtensions.EnsureComponent<RemoteRectTransform>(mirror).CaptureRectTransform();
            }
        }

        /// <summary>
        /// Reads a network message and updates local state data using interpolation
        /// </summary>
        /// <param name="message">Received payload</param>
        /// <param name="lerpVal">interpolation value</param>
        public void LerpRead(BinaryReader message, float lerpVal)
        {
            short id = message.ReadInt16();

            SynchronizedTransformChangeType changeType = (SynchronizedTransformChangeType)message.ReadByte();
            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Name) ||
                SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Layer) ||
                SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Parent) ||
                SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.IsActive))
            {
                return;
            }

            GameObject mirror = SynchronizedSceneManager.Instance.FindGameObjectWithId(id);
            if (mirror == null)
            {
                return;
            }


            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Position))
            {
                mirror.transform.localPosition = Vector3.Lerp(mirror.transform.localPosition, message.ReadVector3(), lerpVal);
            }
            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Rotation))
            {
                mirror.transform.localRotation = Quaternion.Slerp(mirror.transform.localRotation, message.ReadQuaternion(), lerpVal);
            }
            if (SynchronizedTransform.HasFlag(changeType, SynchronizedTransformChangeType.Scale))
            {
                mirror.transform.localScale = Vector3.Lerp(mirror.transform.localScale, message.ReadVector3(), lerpVal);
            }
        }
    }
}