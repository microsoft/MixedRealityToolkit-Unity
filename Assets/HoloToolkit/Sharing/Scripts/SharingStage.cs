using UnityEngine;

namespace HoloToolkit.Sharing
{
    public class SharingStage : MonoBehaviour
    {
        public static SharingStage Instance = null;

        /// <summary>
        /// Set whether this app should be a Primary or Secondary client.
        /// Primary: Connects directly to the Session Server, can create/join/leave sessions
        /// Secondary: Connects to a Primary client.  Cannot do any session management
        /// </summary>
        public ClientRole ClientRole = ClientRole.Primary;

        public string ServerAddress = "localhost";
        public int ServerPort = 20602;

        private XToolsManager xtoolsMgr;
        public XToolsManager Manager { get { return this.xtoolsMgr; } }

        /// <summary>
        /// Set whether this app should provide audio input / output features.
        /// </summary>
        public bool IsAudioEndpoint = true;

        /// <summary>
        /// Pipes XTools console output to Unity's output window for debugging
        /// </summary>
        private ConsoleLogWriter logWriter;

        private void Awake()
        {
            Instance = this;

            this.logWriter = new ConsoleLogWriter();

            ClientConfig config = new ClientConfig(this.ClientRole);
            config.SetIsAudioEndpoint(this.IsAudioEndpoint);
            config.SetLogWriter(this.logWriter);
            config.SetServerAddress(this.ServerAddress);
            config.SetServerPort(this.ServerPort);

            this.xtoolsMgr = XToolsManager.Create(config);
        }

        protected void OnDestroy()
        {
            Instance = null;

            // Force a disconnection so that we can stop and start Unity without connections hanging around
            this.xtoolsMgr.GetPairedConnection().Disconnect();
            this.xtoolsMgr.GetServerConnection().Disconnect();

            // Release the XTools manager so that it cleans up the C++ copy
            this.xtoolsMgr.Dispose();
            this.xtoolsMgr = null;

            // Forces a garbage collection to try to clean up any additional reference to SWIG-wrapped objects
            System.GC.Collect();
        }


        private void LateUpdate()
        {
            if (this.xtoolsMgr != null)
            {
                // Update the XToolsManager to processes any network messages that have arrived
                this.xtoolsMgr.Update();
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    Log.Error(string.Format("{0} \n {1}", logString, stackTrace));
                    break;

                case LogType.Warning:
                    Log.Warning(string.Format("{0} \n {1}", logString, stackTrace));
                    break;

                case LogType.Log:
                default:
                    Log.Info(logString);
                    break;
            }
        }
    }
}
