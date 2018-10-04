// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using QRCodesTrackerPlugin;
namespace HoloToolkit.Unity.QRTracking
{
    public class QRCodesVisualizer : MonoBehaviour
    {
        public GameObject qrCodePrefab;

        private SortedDictionary<System.Guid, GameObject> qrCodesObjectsList;

        struct ActionData
        {
            public enum Type
            {
                Added,
                Updated,
                Removed
            };
            public Type type;
            public QRCodesTrackerPlugin.QRCode qrCode;

            public ActionData(Type type, QRCodesTrackerPlugin.QRCode qRCode) : this()
            {
                this.type = type;
                qrCode = qRCode;
            }
        }

        private Queue<ActionData> pendingActions = new Queue<ActionData>();
        private void Awake()
        {
            qrCodesObjectsList = new SortedDictionary<System.Guid, GameObject>();
            QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += Instance_QRCodeRemoved;
        }

        private void Start()
        {
            if (qrCodePrefab == null)
            {
                throw new System.Exception("Prefab not assigned");
            }
        }

        private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Added, e.Data));
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Updated, e.Data));
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Removed, e.Data));
            }
        }

        private void HandleEvents()
        {
            lock (pendingActions)
            {
                while (pendingActions.Count > 0)
                {
                    var action = pendingActions.Dequeue();
                    switch (action.type)
                    {
                        case ActionData.Type.Added:
                            {
                                GameObject qrCodeObject = Instantiate(qrCodePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                                qrCodeObject.GetComponent<SpatialGraphCoordinateSystem>().Id = action.qrCode.Id;
                                qrCodeObject.GetComponent<QRCode>().qrCode = action.qrCode;
                                qrCodesObjectsList.Add(action.qrCode.Id, qrCodeObject);
                                break;
                            }
                        case ActionData.Type.Removed:
                            {
                                if (qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                                {
                                    Destroy(qrCodesObjectsList[action.qrCode.Id]);
                                    qrCodesObjectsList.Remove(action.qrCode.Id);
                                }
                                break;
                            }
                    }
                }
            }
        }

        private void Update()
        {
            HandleEvents();
        }
    }

}