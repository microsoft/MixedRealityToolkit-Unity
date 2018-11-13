// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR || UNITY_WSA
using QRCodesTrackerPlugin;
#endif // UNITY_EDITOR || UNITY_WSA

namespace HoloToolkit.Unity.QRTracking
{
    public static class QRCodeEventArgs
    {
        public static QRCodeEventArgs<TData> Create<TData>(TData data)
        {
            return new QRCodeEventArgs<TData>(data);
        }
    }

    [Serializable]
    public class QRCodeEventArgs<TData> : EventArgs
    {
        public TData Data { get; private set; }

        public QRCodeEventArgs(TData data)
        {
            Data = data;
        }
    }

    public class QRCodesManager : Singleton<QRCodesManager>
    {
        [Tooltip("Determines if the QR codes scanner should be automatically started.")]
        public bool AutoStartQRTracking = true;

        public bool IsTrackerRunning { get; private set; }

#if UNITY_EDITOR || UNITY_WSA
        public QRTrackerStartResult StartResult { get; private set; }

        public event EventHandler<QRCodeEventArgs<QRCodesTrackerPlugin.QRCode>> QRCodeAdded;
        public event EventHandler<QRCodeEventArgs<QRCodesTrackerPlugin.QRCode>> QRCodeUpdated;
        public event EventHandler<QRCodeEventArgs<QRCodesTrackerPlugin.QRCode>> QRCodeRemoved;

        private SortedDictionary<Guid, QRCodesTrackerPlugin.QRCode> qrCodesList = new SortedDictionary<Guid, QRCodesTrackerPlugin.QRCode>();

        private QRTracker qrTracker;

        public Guid GetIdForQRCode(string qrCodeData)
        {
            lock (qrCodesList)
            {
                foreach (var ite in qrCodesList)
                {
                    if (ite.Value.Code == qrCodeData)
                    {
                        return ite.Key;
                    }
                }
            }
            return Guid.Empty;
        }

        public IList<QRCodesTrackerPlugin.QRCode> GetList()
        {
            lock (qrCodesList)
            {
                return new List<QRCodesTrackerPlugin.QRCode>(qrCodesList.Values);
            }
        }
#endif // UNITY_EDITOR || UNITY_WSA

        protected override void Awake()
        {
            base.Awake();
            IsTrackerRunning = false;
        }

        protected virtual void Start()
        {
#if UNITY_EDITOR || UNITY_WSA
            qrTracker = new QRTracker();
            qrTracker.Added += QrTracker_Added;
            qrTracker.Updated += QrTracker_Updated;
            qrTracker.Removed += QrTracker_Removed;

            if (AutoStartQRTracking)
            {
                StartQRTracking();
            }
#endif // UNITY_EDITOR || UNITY_WSA
        }

#if UNITY_EDITOR || UNITY_WSA
        public QRTrackerStartResult StartQRTracking()
        {
            if (!IsTrackerRunning)
            {
                StartResult = (qrTracker.Start());
                if (StartResult == QRTrackerStartResult.Success)
                {
                    IsTrackerRunning = true;
                }
            }
            return StartResult;
        }

        public void StopQRTracking()
        {
            if (IsTrackerRunning)
            {
                IsTrackerRunning = false;
                qrTracker.Stop();
                StartResult = QRTrackerStartResult.DeviceNotConnected;
            }
        }

        private void QrTracker_Removed(QRCodeRemovedEventArgs args)
        {
            lock (qrCodesList)
            {
                qrCodesList.Remove(args.Code.Id);
            }
            var handlers = QRCodeRemoved;
            if (handlers != null)
            {
                handlers(this, QRCodeEventArgs.Create(args.Code));
            }
        }

        private void QrTracker_Updated(QRCodeUpdatedEventArgs args)
        {
            lock (qrCodesList)
            {
                qrCodesList[args.Code.Id] = args.Code;
            }
            var handlers = QRCodeUpdated;
            if (handlers != null)
            {
                handlers(this, QRCodeEventArgs.Create(args.Code));
            }
        }

        private void QrTracker_Added(QRCodeAddedEventArgs args)
        {
            lock (qrCodesList)
            {
                qrCodesList.Add(args.Code.Id, args.Code);
            }
            var handlers = QRCodeAdded;
            if (handlers != null)
            {
                handlers(this, QRCodeEventArgs.Create(args.Code));
            }
        }
#endif // UNITY_EDITOR || UNITY_WSA
    }
}