// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Perception.Spatial;
#endif

namespace HoloToolkit.Unity.QRTracking
{
    [RequireComponent(typeof(HoloToolkit.Unity.SpatialGraphCoordinateSystem))]
    public class QRCode : MonoBehaviour
    {
        public QRCodesTrackerPlugin.QRCode qrCode;
        private GameObject qrCodeCube;

        public float PhysicalSize;
        public string CodeText { get; set; }

        private TextMesh QRID;
        private TextMesh QRText;
        private TextMesh QRVersion;
        private TextMesh QRTimeStamp;
        private TextMesh QRSize;
        private GameObject QRInfo;

        private long lastTimeStamp = 0;

        private void Start()
        {
            PhysicalSize = 0.1f;
            CodeText = "Dummy";
            if (qrCode == null)
            {
                throw new System.Exception("QR Code Empty");
            }

            PhysicalSize = qrCode.PhysicalSizeMeters;
            CodeText = qrCode.Code;

            qrCodeCube = gameObject.transform.Find("Cube").gameObject;
            QRInfo = gameObject.transform.Find("QRInfo").gameObject;
            QRID = QRInfo.transform.Find("QRID").gameObject.GetComponent<TextMesh>();
            QRText = QRInfo.transform.Find("QRText").gameObject.GetComponent<TextMesh>();
            QRVersion = QRInfo.transform.Find("QRVersion").gameObject.GetComponent<TextMesh>();
            QRTimeStamp = QRInfo.transform.Find("QRTimeStamp").gameObject.GetComponent<TextMesh>();
            QRSize = QRInfo.transform.Find("QRSize").gameObject.GetComponent<TextMesh>();

            QRID.text = qrCode.Id.ToString();
            QRText.text = CodeText;
            QRVersion.text = "Ver: " + qrCode.Version;
            QRSize.text = "Size:" + qrCode.PhysicalSizeMeters.ToString("F04") + "m";
            QRTimeStamp.text = "Time:" + qrCode.LastDetectedQPCTicks;
            Debug.Log("Id= " + qrCode.Id + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.LastDetectedQPCTicks + " QRVersion = " + qrCode.Version + " QRData = " + CodeText);
        }
        private void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && lastTimeStamp != qrCode.LastDetectedQPCTicks)
            {
                QRSize.text = "Size:" + qrCode.PhysicalSizeMeters.ToString("F04") + "m";

                {
                    long EndingTime = System.Diagnostics.Stopwatch.GetTimestamp();
                    long ElapsedTime = EndingTime - (long)qrCode.LastDetectedQPCTicks;
                    double ElapsedSecs = ElapsedTime * (1.0f / System.Diagnostics.Stopwatch.Frequency);

                    QRTimeStamp.text = "Time:" + System.DateTime.Now.AddSeconds(-ElapsedSecs).ToString("MM/dd/yyyy HH:mm:ss.fff");
                }

                PhysicalSize = qrCode.PhysicalSizeMeters;
                Debug.Log("Id= " + qrCode.Id + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.LastDetectedQPCTicks);

                qrCodeCube.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                qrCodeCube.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);
                lastTimeStamp = qrCode.LastDetectedQPCTicks;
                QRInfo.transform.localScale = new Vector3(PhysicalSize/0.2f, PhysicalSize / 0.2f, PhysicalSize / 0.2f);
            }
        }

        private void Update()
        {
            UpdatePropertiesDisplay();
        }
    }
}