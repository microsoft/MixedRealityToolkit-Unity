// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class DynamicGameObjectHierarchyObserver<TComponentService> : MonoBehaviour, IComponentObserver where TComponentService : Singleton<TComponentService>, IComponentBroadcasterService
    {
        private GameObject dynamicObject;

        protected GameObject DynamicObject
        {
            get { return dynamicObject; }
            set
            {
                if (dynamicObject != value)
                {
                    dynamicObject = value;

                    if (dynamicObject != null)
                    {
                        OnDynamicObjectCreated();
                    }
                }
            }
        }

        protected abstract void CreateRemoteObject(BinaryReader message);

        private void OnDynamicObjectCreated()
        {
            DynamicObject.SetActive(false);

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                Singleton<TComponentService>.Instance.WriteHeader(writer, GetComponent<TransformObserver>());

                writer.Write(DynamicGameObjectHierarchyBroadcaster<TComponentService>.ChangeType.RemoteObjectCreated);

                writer.Flush();
                Observer.Instance.SendComponentMessage(stream.ToArray());
            }
        }

        public void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            byte changeType = message.ReadByte();

            Read(sendingEndpoint, message, changeType);
        }

        protected virtual void Read(SocketEndpoint sendingEndpoint, BinaryReader message, byte changeType)
        {
            switch (changeType)
            {
                case DynamicGameObjectHierarchyBroadcaster<TComponentService>.ChangeType.CreateRemoteObject:
                    CreateRemoteObject(message);
                    break;
                case DynamicGameObjectHierarchyBroadcaster<TComponentService>.ChangeType.BindTransformHierarchy:
                    BindRemoteHierarchy(message);
                    break;
            }
        }

        private void BindRemoteHierarchy(BinaryReader message)
        {
            var remoteHierarchy = ReadRemoteHierarchyTransformIDs(message);
            ApplyChildTransforms(DynamicObject.transform, remoteHierarchy);

            DynamicObject.SetActive(true);

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                Singleton<TComponentService>.Instance.WriteHeader(writer, GetComponent<TransformObserver>());

                writer.Write(DynamicGameObjectHierarchyBroadcaster<TComponentService>.ChangeType.RemoteHierarchyBound);

                writer.Flush();
                Observer.Instance.SendComponentMessage(stream.ToArray());
            }
        }

        public static TransformObserverInfo[] ReadRemoteHierarchyTransformIDs(BinaryReader message)
        {
            int childCount = message.ReadInt32();
            TransformObserverInfo[] list = new TransformObserverInfo[childCount];
            for (int i = 0; i < childCount; i++)
            {
                short id = message.ReadInt16();
                string name = message.ReadString();
                TransformObserverInfo childTransformInfo = new TransformObserverInfo
                {
                    Name = name,
                    Id = id
                };
                    
                childTransformInfo.Children = ReadRemoteHierarchyTransformIDs(message);
                list[i] = childTransformInfo;

            }

            return list;
        }

        public static void ApplyChildTransforms(Transform transform, TransformObserverInfo[] childTransformInfos)
        {
            if (transform.childCount != childTransformInfos.Length)
            {
                Debug.LogError("Client dynamic object does not have the same number of children as the remote dynamic object at child " + transform.name);
            }
            else
            {
                for (int i = 0; i < childTransformInfos.Length; i++)
                {
                    Transform childTransform = transform.GetChild(i);
                    if (childTransform.name != childTransformInfos[i].Name)
                    {
                        Debug.LogError("Client dynamic object " + transform.name + " has child object named " + childTransformInfos[i].Name + " but remote dynamic object has child named " + childTransform.name);
                    }
                    else
                    {
                        StateSynchronizationSceneManager.Instance.AssignMirror(childTransform.gameObject, childTransformInfos[i].Id);

                        ApplyChildTransforms(childTransform, childTransformInfos[i].Children);
                    }
                }
            }
        }

        public class TransformObserverInfo
        {
            public string Name { get; set; }
            public short Id { get; set; }
            public TransformObserverInfo[] Children { get; set; }
        }
    }
}