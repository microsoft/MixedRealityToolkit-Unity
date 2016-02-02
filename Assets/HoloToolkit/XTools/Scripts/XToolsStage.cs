using UnityEngine;

namespace HoloToolkit.XTools
{
    public class XToolsStage : MonoBehaviour
    {
        public static XToolsStage Instance = null;

        /// <summary>
        /// Set whether this app should be a Primary or Secondary client.
        /// Primary: Connects directly to the Session Server, can create/join/leave sessions
        /// Secondary: Connects to a Primary client.  Cannot do any session management
        /// </summary>
        public ClientRole Role = ClientRole.Primary;

        public string ServerAddress = "localhost";
        public int ServerPort = 20602;

        private XTools.XToolsManager xtoolsMgr;
        public XTools.XToolsManager Manager { get { return this.xtoolsMgr; } }

        /// <summary>
        /// Set whether this app should provide audio input / output features.
        /// </summary>
        public bool IsAudioEndpoint = true;

        /// <summary>
        /// Pipes XTools console output to Unity's output window for debugging
        /// </summary>
        private XTools.ConsoleLogWriter logWriter;

        private void Awake()
        {
            Instance = this;

            this.logWriter = new XTools.ConsoleLogWriter();

            ClientConfig config = new ClientConfig(this.Role);
            config.SetIsAudioEndpoint(this.IsAudioEndpoint);
            config.SetLogWriter(this.logWriter);
            config.SetServerAddress(this.ServerAddress);
            config.SetServerPort(this.ServerPort);

            this.xtoolsMgr = XTools.XToolsManager.Create(config);
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
                    XTools.Log.Error(string.Format("{0} \n {1}", logString, stackTrace));
                    break;

                case LogType.Warning:
                    XTools.Log.Warning(string.Format("{0} \n {1}", logString, stackTrace));
                    break;

                case LogType.Log:
                default:
                    XTools.Log.Info(logString);
                    break;
            }
        }
    }
}
