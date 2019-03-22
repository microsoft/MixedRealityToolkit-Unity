// ----------------------------------------------------------------------------
// <copyright file="ServerSettings.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// ScriptableObject defining a server setup. An instance is created as <b>PhotonServerSettings</b>.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using System;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;
    using Photon.Realtime;
    using UnityEngine;

    /// <summary>
    /// Collection of connection-relevant settings, used internally by PhotonNetwork.ConnectUsingSettings.
    /// </summary>
    /// <remarks>
    /// Includes the AppSettings class from the Realtime APIs plus some other, PUN-relevant, settings.</remarks>
    [Serializable]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/getting-started/initial-setup")]
    public class ServerSettings : ScriptableObject
    {
        [Tooltip("Core Photon Server/Cloud settings.")]
        public AppSettings AppSettings;

        [Tooltip("Simulates an online connection.\nPUN can be used as usual.")]
        public bool StartInOfflineMode;

        [Tooltip("Log output by PUN.")]
        public PunLogLevel PunLogging = PunLogLevel.ErrorsOnly;

        [Tooltip("Logs additional info for debugging.")]
        public bool EnableSupportLogger;

        [Tooltip("Enables apps to keep the connection without focus.")]
        public bool RunInBackground = true;

        [Tooltip("RPC name list.\nUsed as shortcut when sending calls.")]
        public List<string> RpcList = new List<string>();   // set by scripts and or via Inspector

        [HideInInspector]
        public bool DisableAutoOpenWizard;


        /// <summary>Sets appid and region code in the AppSettings. Used in Editor.</summary>
        public void UseCloud(string cloudAppid, string code = "")
        {
            this.AppSettings.AppIdRealtime = cloudAppid;
            this.AppSettings.Server = null;
            this.AppSettings.FixedRegion = string.IsNullOrEmpty(code) ? null : code;
            Debug.Log("this.AppSettings.IsBestRegion: " + this.AppSettings.IsBestRegion);
        }

        /// <summary>Checks if a string is a Guid by attempting to create one.</summary>
        /// <param name="val">The potential guid to check.</param>
        /// <returns>True if new Guid(val) did not fail.</returns>
        public static bool IsAppId(string val)
        {
            try
            {
                new Guid(val);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>Gets the "best region summary" from the preferences.</summary>
        /// <value>The best region code in preferences.</value>
        public static string BestRegionSummaryInPreferences
        {
            get { return PhotonNetwork.BestRegionSummaryInPreferences; }
        }

        /// <summary>Sets the "best region summary" in the preferences to null. On next start, the client will ping all available.</summary>
        public static void ResetBestRegionCodeInPreferences()
        {
            PhotonNetwork.BestRegionSummaryInPreferences = null;
        }

        /// <summary>String summary of the AppSettings.</summary>
        public override string ToString()
        {
            return "ServerSettings: " + this.AppSettings.ToStringFull();
        }
    }
}