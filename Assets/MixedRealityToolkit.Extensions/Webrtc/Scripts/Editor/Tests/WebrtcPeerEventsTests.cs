// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Extensions.WebRTC;
using Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Delegates;

namespace Tests
{
    /// <summary>
    /// Zero-dependency tests for <see cref="WebrtcPeerEvents"/>
    /// </summary>
    /// <remarks>
    /// We define our own mock impls, as not all mocking frameworks work well in different Unity builds
    /// and we're trying to be as agnostic as possible here.
    /// </remarks>
    public class WebrtcPeerEventsTests
    {
        private class MockWebrtc : Webrtc
        {
            public void SetPeer(IPeer mockPeer)
            {
                this.Peer = mockPeer;
            }
        }

        private class EmptyMockPeer : IPeer
        {
            // Toggle "Event is never used" warnings off for these test events
#pragma warning disable 67
            public virtual event Action LocalDataChannelReady;
            public virtual event Action<string> DataFromDataChannelReady;
            public virtual event Action<string> FailureMessage;
            public virtual event AudioBusReadyHandler AudioBusReady;
            public virtual event I420FrameReadyHandler LocalI420FrameReady;
            public virtual event I420FrameReadyHandler RemoteI420FrameReady;
            public virtual event Action<string, string> LocalSdpReadytoSend;
            public virtual event Action<string, int, string> IceCandiateReadytoSend;
#pragma warning restore 67
            public virtual void AddDataChannel()
            {
                throw new NotImplementedException();
            }

            public virtual void AddIceCandidate(string candidate, int sdpMlineindex, string sdpMid)
            {
                throw new NotImplementedException();
            }

            public virtual void AddStream(bool audioOnly)
            {
                throw new NotImplementedException();
            }

            public virtual void ClosePeerConnection()
            {
                throw new NotImplementedException();
            }

            public virtual void CreateAnswer()
            {
                throw new NotImplementedException();
            }

            public virtual void CreateOffer()
            {
                throw new NotImplementedException();
            }

            public virtual int GetUniqueId()
            {
                throw new NotImplementedException();
            }

            public virtual void SendDataViaDataChannel(string data)
            {
                throw new NotImplementedException();
            }

            public virtual void SetAudioControl(bool isMute, bool isRecord)
            {
                throw new NotImplementedException();
            }

            public virtual void SetRemoteDescription(string type, string sdp)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Allocate a test gameobject with a <see cref="WebrtcPeerEvents"/> component attached
        /// </summary>
        /// <param name="mockPeer">the <see cref="IPeer"/> implementation to use</param>
        /// <returns>the peer events instance</returns>
        private WebrtcPeerEvents AllocWithGameObject(IPeer mockPeer)
        {
            var go = new GameObject();
            go.AddComponent<MockWebrtc>().SetPeer(mockPeer);
            return go.AddComponent<WebrtcPeerEvents>();
        }

        /// <summary>
        /// Invoke a provide "Update" method on a given target
        /// </summary>
        /// <typeparam name="TBehaviour">target type</typeparam>
        /// <param name="behaviour">target instance</param>
        private void InvokePrivateUpdate<TBehaviour>(TBehaviour behaviour) where TBehaviour : MonoBehaviour
        {
            behaviour
                .GetType()
                .GetMethod("Update", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(behaviour, null);
        }

        /// <summary>
        /// Ensure that the WebrtcPeerEvents instance is able to Initialize
        /// </summary>
        [Test]
        public void WebrtcPeerEvents_InitializeSuccess()
        {
            var instance = AllocWithGameObject(mockPeer: new EmptyMockPeer());

            var wasPeerReady = false;
            instance.OnPeerReady.AddListener(() =>
            {
                wasPeerReady = true;
            });

            instance.OnInitialized();

            Assert.IsTrue(wasPeerReady);
        }

        class MockPeerWithDataFromDataChannelReady : EmptyMockPeer
        {
            public override event Action<string> DataFromDataChannelReady;

            public void Fire(string data)
            {
                this.DataFromDataChannelReady(data);
            }
        }

        /// <summary>
        /// Ensure that DataFromDataChannelReady works
        /// </summary>
        [Test]
        public void WebrtcPeerEvents_Fires_DataFromDataChannelReady()
        {
            var mockPeer = new MockPeerWithDataFromDataChannelReady();
            var instance = AllocWithGameObject(mockPeer);
            instance.OnInitialized();

            var expectedString = "test fire";
            var wasFired = false;
            instance.OnDataFromDataChannelReady.AddListener((string actual) =>
            {
                Assert.AreEqual(expectedString, actual);
                wasFired = true;
            });

            mockPeer.Fire(expectedString);
            InvokePrivateUpdate(instance);

            Assert.IsTrue(wasFired);
        }

        class MockPeerWithFailureMessage : EmptyMockPeer
        {
            public override event Action<string> FailureMessage;

            public void Fire(string data)
            {
                this.FailureMessage(data);
            }
        }

        /// <summary>
        /// Ensure that FailureMessage works (with autolog)
        /// </summary>
        [Test]
        public void WebrtcPeerEvents_AutoLogErrors_Fires_FailureMessage()
        {
            var mockPeer = new MockPeerWithFailureMessage();
            var instance = AllocWithGameObject(mockPeer);
            instance.OnInitialized();

            var expectedString = "Expected Error Message";
            var wasFired = false;
            instance.OnFailure.AddListener((string actual) =>
            {
                Assert.AreEqual(expectedString, actual);
                wasFired = true;
            });

            mockPeer.Fire(expectedString);
            InvokePrivateUpdate(instance);

            Assert.IsTrue(wasFired);
            LogAssert.Expect(LogType.Error, expectedString);
        }

        /// <summary>
        /// Ensure that FailureMessage works (without autolog)
        /// </summary>
        [Test]
        public void WebrtcPeerEvents_Fires_FailureMessage()
        {
            var mockPeer = new MockPeerWithFailureMessage();
            var instance = AllocWithGameObject(mockPeer);
            instance.AutoLogErrors = false;
            instance.OnInitialized();

            var expectedString = "test fire";
            var wasFired = false;
            instance.OnFailure.AddListener((string actual) =>
            {
                Assert.AreEqual(expectedString, actual);
                wasFired = true;
            });

            mockPeer.Fire(expectedString);
            InvokePrivateUpdate(instance);

            Assert.IsTrue(wasFired);
        }

        class MockPeerWithIceCandiateReadytoSend : EmptyMockPeer
        {
            public override event Action<string, int, string> IceCandiateReadytoSend;

            public void Fire(string arg0, int arg1, string arg2)
            {
                this.IceCandiateReadytoSend(arg0, arg1, arg2);
            }
        }

        /// <summary>
        /// Ensure that IceCandiateReadytoSend works
        /// </summary>
        [Test]
        public void WebrtcPeerEvents_Fires_IceCandiateReadytoSend()
        {
            var mockPeer = new MockPeerWithIceCandiateReadytoSend();
            var instance = AllocWithGameObject(mockPeer);
            instance.OnInitialized();

            var expectedArg1 = "test fire";
            var expectedArg2 = 0;
            var expectedArg3 = "fire test";
            var wasFired = false;
            instance.OnIceCandiateReadyToSend.AddListener((string arg1, int arg2, string arg3) =>
            {
                Assert.AreEqual(expectedArg1, arg1);
                Assert.AreEqual(expectedArg2, arg2);
                Assert.AreEqual(expectedArg3, arg3);
                wasFired = true;
            });

            mockPeer.Fire(expectedArg1, expectedArg2, expectedArg3);
            InvokePrivateUpdate(instance);

            Assert.IsTrue(wasFired);
        }

        class MockPeerWithLocalDataChannelReady : EmptyMockPeer
        {
            public override event Action LocalDataChannelReady;

            public void Fire()
            {
                this.LocalDataChannelReady();
            }
        }

        /// <summary>
        /// Ensure that LocalDataChannelReady works
        /// </summary>
        [Test]
        public void WebrtcPeerEvents_Fires_LocalDataChannelReady()
        {
            var mockPeer = new MockPeerWithLocalDataChannelReady();
            var instance = AllocWithGameObject(mockPeer);
            instance.OnInitialized();

            var wasFired = false;
            instance.OnLocalDataChannelReady.AddListener(() =>
            {
                wasFired = true;
            });

            mockPeer.Fire();
            InvokePrivateUpdate(instance);

            Assert.IsTrue(wasFired);
        }

        class MockPeerWithLocalSdpReadytoSend : EmptyMockPeer
        {
            public override event Action<string, string> LocalSdpReadytoSend;

            public void Fire(string arg0, string arg1)
            {
                this.LocalSdpReadytoSend(arg0, arg1);
            }
        }

        /// <summary>
        /// Ensure that LocalSdpReadytoSend works
        /// </summary>
        [Test]
        public void WebrtcPeerEvents_Fires_LocalSdpReadytoSend()
        {
            var mockPeer = new MockPeerWithLocalSdpReadytoSend();
            var instance = AllocWithGameObject(mockPeer);
            instance.OnInitialized();

            var expectedArg1 = "test fire";
            var expectedArg2 = "fire test";
            var wasFired = false;
            instance.OnLocalSdpReadyToSend.AddListener((string arg1, string arg2) =>
            {
                Assert.AreEqual(expectedArg1, arg1);
                Assert.AreEqual(expectedArg2, arg2);
                wasFired = true;
            });

            mockPeer.Fire(expectedArg1, expectedArg2);
            InvokePrivateUpdate(instance);

            Assert.IsTrue(wasFired);
        }

        /// <summary>
        /// Ensure that LocalSdpReadytoSend works with type = "answer"
        /// </summary>
        [Test]
        public void WebrtcPeerEvents_Answer_Fires_LocalSdpReadytoSend()
        {
            var mockPeer = new MockPeerWithLocalSdpReadytoSend();
            var instance = AllocWithGameObject(mockPeer);
            instance.OnInitialized();

            var expectedArg1 = "answer";
            var expectedArg2 = "fire test";
            var wasFired = false;
            instance.OnSdpAnswerReadyToSend.AddListener((string arg2) =>
            {
                Assert.AreEqual(expectedArg2, arg2);
                wasFired = true;
            });

            mockPeer.Fire(expectedArg1, expectedArg2);
            InvokePrivateUpdate(instance);

            Assert.IsTrue(wasFired);
        }

        /// <summary>
        /// Ensure that LocalSdpReadytoSend works with type = "offer"
        /// </summary>
        [Test]
        public void WebrtcPeerEvents_Offer_Fires_LocalSdpReadytoSend()
        {
            var mockPeer = new MockPeerWithLocalSdpReadytoSend();
            var instance = AllocWithGameObject(mockPeer);
            instance.OnInitialized();

            var expectedArg1 = "offer";
            var expectedArg2 = "fire test";
            var wasFired = false;
            instance.OnSdpOfferReadyToSend.AddListener((string arg2) =>
            {
                Assert.AreEqual(expectedArg2, arg2);
                wasFired = true;
            });

            mockPeer.Fire(expectedArg1, expectedArg2);
            InvokePrivateUpdate(instance);

            Assert.IsTrue(wasFired);
        }
    }
}
