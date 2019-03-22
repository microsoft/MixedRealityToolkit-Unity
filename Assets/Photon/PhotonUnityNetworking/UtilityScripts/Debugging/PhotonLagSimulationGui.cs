// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonLagSimulationGui.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
// This MonoBehaviour is a basic GUI for the Photon client's network-simulation feature.
// It can modify lag (fixed delay), jitter (random lag) and packet loss.
// Part of the [Optional GUI](@ref optionalGui).
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// This MonoBehaviour is a basic GUI for the Photon client's network-simulation feature.
    /// It can modify lag (fixed delay), jitter (random lag) and packet loss.
    /// </summary>
    /// \ingroup optionalGui
    public class PhotonLagSimulationGui : MonoBehaviour
    {
        /// <summary>Positioning rect for window.</summary>
        public Rect WindowRect = new Rect(0, 100, 120, 100);

        /// <summary>Unity GUI Window ID (must be unique or will cause issues).</summary>
        public int WindowId = 101;

        /// <summary>Shows or hides GUI (does not affect settings).</summary>
        public bool Visible = true;

        /// <summary>The peer currently in use (to set the network simulation).</summary>
        public PhotonPeer Peer { get; set; }

        public void Start()
        {
            this.Peer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
        }

        public void OnGUI()
        {
            if (!this.Visible)
            {
                return;
            }

            if (this.Peer == null)
            {
                this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, this.NetSimHasNoPeerWindow, "Netw. Sim.");
            }
            else
            {
                this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, this.NetSimWindow, "Netw. Sim.");
            }
        }

        private void NetSimHasNoPeerWindow(int windowId)
        {
            GUILayout.Label("No peer to communicate with. ");
        }

        private void NetSimWindow(int windowId)
        {
            GUILayout.Label(string.Format("Rtt:{0,4} +/-{1,3}", this.Peer.RoundTripTime, this.Peer.RoundTripTimeVariance));

            bool simEnabled = this.Peer.IsSimulationEnabled;
            bool newSimEnabled = GUILayout.Toggle(simEnabled, "Simulate");
            if (newSimEnabled != simEnabled)
            {
                this.Peer.IsSimulationEnabled = newSimEnabled;
            }

            float inOutLag = this.Peer.NetworkSimulationSettings.IncomingLag;
            GUILayout.Label("Lag " + inOutLag);
            inOutLag = GUILayout.HorizontalSlider(inOutLag, 0, 500);

            this.Peer.NetworkSimulationSettings.IncomingLag = (int)inOutLag;
            this.Peer.NetworkSimulationSettings.OutgoingLag = (int)inOutLag;

            float inOutJitter = this.Peer.NetworkSimulationSettings.IncomingJitter;
            GUILayout.Label("Jit " + inOutJitter);
            inOutJitter = GUILayout.HorizontalSlider(inOutJitter, 0, 100);

            this.Peer.NetworkSimulationSettings.IncomingJitter = (int)inOutJitter;
            this.Peer.NetworkSimulationSettings.OutgoingJitter = (int)inOutJitter;

            float loss = this.Peer.NetworkSimulationSettings.IncomingLossPercentage;
            GUILayout.Label("Loss " + loss);
            loss = GUILayout.HorizontalSlider(loss, 0, 10);

            this.Peer.NetworkSimulationSettings.IncomingLossPercentage = (int)loss;
            this.Peer.NetworkSimulationSettings.OutgoingLossPercentage = (int)loss;

            // if anything was clicked, the height of this window is likely changed. reduce it to be layouted again next frame
            if (GUI.changed)
            {
                this.WindowRect.height = 100;
            }

            GUI.DragWindow();
        }
    }
}