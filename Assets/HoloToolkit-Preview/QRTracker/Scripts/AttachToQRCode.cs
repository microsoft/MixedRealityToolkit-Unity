// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.QRTracking
{
    public class AttachToQRCode : MonoBehaviour
    {
        [Tooltip("Object that need to be attached to the QR code.")]
        public GameObject gameObjectToAttach;

        private System.Guid qrCodeId;

        private bool updatedId = false;

        private SpatialGraphCoordinateSystem coordSystem = null;

        /// <summary>
        /// Data of the QR code to which we want to attach the game object."
        /// </summary>
        [SerializeField]
        [Tooltip("Data of the QR code to which we want to attach the game object.")]
        private string qRCodeData = string.Empty;
        public string QRCodeData
        {
            get
            {
                return qRCodeData;
            }
            set
            {
                if (qRCodeData != value)
                {
                    qRCodeData = value;
                    if (qRCodeData == null)
                    {
                        qRCodeData = string.Empty;
                    }

#if UNITY_EDITOR || UNITY_WSA
                    qrCodeId = QRCodesManager.Instance.GetIdForQRCode(qRCodeData);
                    updatedId = true;
#endif // UNITY_EDITOR || UNITY_WSA
                }
            }
        }

        private SortedDictionary<System.Guid, GameObject> qrCodesObjectsList;

        private void Awake()
        {
#if UNITY_EDITOR || UNITY_WSA
            QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += Instance_QRCodeRemoved;
#endif // UNITY_EDITOR || UNITY_WSA
        }

        private void Start()
        {
            if (gameObjectToAttach == null)
            {
                // default use the scripts object
                gameObjectToAttach = gameObject;
            }

#if UNITY_EDITOR || UNITY_WSA
            qrCodeId = QRCodesManager.Instance.GetIdForQRCode(qRCodeData);
            updatedId = true;
#endif // UNITY_EDITOR || UNITY_WSA
        }

#if UNITY_EDITOR || UNITY_WSA
        private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            if (qrCodeId == System.Guid.Empty)
            {
                if (e.Data.Code == QRCodeData)
                {
                    qrCodeId = e.Data.Id;
                    updatedId = true;
                }
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            if (qrCodeId == System.Guid.Empty)
            {
                if (e.Data.Code == QRCodeData)
                {
                    qrCodeId = e.Data.Id;
                    updatedId = true;
                }
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            if (qrCodeId == e.Data.Id)
            {
                qrCodeId = System.Guid.Empty;
                updatedId = true;
            }
        }

        private void Update()
        {
            if (updatedId)
            {
                if (coordSystem == null)
                {
                    coordSystem = gameObjectToAttach.GetComponent<SpatialGraphCoordinateSystem>();
                    if (coordSystem == null)
                    {
                        coordSystem = gameObjectToAttach.AddComponent<SpatialGraphCoordinateSystem>();
                    }
                }
                coordSystem.Id = qrCodeId;
                updatedId = false;
            }
        }
#endif // UNITY_EDITOR || UNITY_WSA
    }
}