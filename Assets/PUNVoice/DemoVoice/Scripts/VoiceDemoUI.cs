// ----------------------------------------------------------------------------
// <copyright file="VoiceDemoUI.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// UI manager class for the PUN Voice Demo
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using ExitGames.Client.Photon;

namespace ExitGames.Demos.DemoPunVoice {

    using UnityEngine;
    using UnityEngine.UI;

    public class VoiceDemoUI : MonoBehaviour {
        [SerializeField]
        private Text punState;
        [SerializeField]
        private Text voiceState;

        private Canvas canvas;

        [SerializeField]
        private Button punSwitch;
        private Text punSwitchText;
        [SerializeField]
        private Button voiceSwitch;
        private Text voiceSwitchText;
        [SerializeField]
        private Button calibrateButton;
        private Text calibrateText;

        [SerializeField]
        private Text voiceDebugText;

        private PhotonVoiceRecorder rec;

        [SerializeField]
        private GameObject inGameSettings;

        [SerializeField]
        private GameObject globalSettings;

        [SerializeField]
        private Text devicesInfoText;

        private GameObject debugGO;

        private bool debugMode;

        private float volumeBeforeMute;

        private DebugLevel previousDebugLevel;

        //MI Code: for background music toggle
        private AudioSource musicSource;

        public bool DebugMode {
            get {
                return debugMode;
            }
            set {
                debugMode = value;
                debugGO.SetActive(debugMode);
                voiceDebugText.text = "";
                if (debugMode)
                {
                    previousDebugLevel = PhotonVoiceNetwork.Client.loadBalancingPeer.DebugOut;
                    PhotonVoiceNetwork.Client.loadBalancingPeer.DebugOut = Client.Photon.DebugLevel.ALL;
                } else
                {
                    PhotonVoiceNetwork.Client.loadBalancingPeer.DebugOut = previousDebugLevel;
                }
                if (DebugToggled != null) {
                    DebugToggled(debugMode);
                }
            }
        }

        public delegate void OnDebugToggle(bool debugMode);

        public static event OnDebugToggle DebugToggled;

        [SerializeField]
        private int calibrationMilliSeconds = 2000;

        private void OnEnable() {
            ChangePOV.CameraChanged += OnCameraChanged;
            CharacterInstantiation.CharacterInstantiated += CharacterInstantiation_CharacterInstantiated;
            BetterToggle.ToggleValueChanged += BetterToggle_ToggleValueChanged;
        }

        private void OnDisable() {
            ChangePOV.CameraChanged -= OnCameraChanged;
            CharacterInstantiation.CharacterInstantiated -= CharacterInstantiation_CharacterInstantiated;
            BetterToggle.ToggleValueChanged -= BetterToggle_ToggleValueChanged;
        }

        private void InitToggles(Toggle[] toggles) {
            if (toggles == null) { return; }
            for (int i = 0; i < toggles.Length; i++) {
                Toggle toggle = toggles[i];
                switch (toggle.name) {
                    case "Mute":
                        toggle.isOn = (AudioListener.volume <= 0.001f);
                        break;

                    case "AutoTransmit":
                        toggle.isOn = PhotonVoiceSettings.Instance.AutoTransmit;
                        break;

                    case "VoiceDetection":
                        toggle.isOn = PhotonVoiceSettings.Instance.VoiceDetection;
                        break;

                    case "AutoConnect":
                        toggle.isOn = PhotonVoiceSettings.Instance.AutoConnect;
                        break;

                    case "AutoDisconnect":
                        toggle.isOn = PhotonVoiceSettings.Instance.AutoDisconnect;
                        break;

                    case "DebugVoice":
                        DebugMode = PhotonVoiceSettings.Instance.DebugInfo;
                        toggle.isOn = DebugMode;
                        break;

                    case "Transmit":
                        toggle.isOn = (rec != null && rec.Transmit);
                        break;

                    case "DebugEcho":
                        toggle.isOn = PhotonVoiceNetwork.Client.DebugEchoMode;
                        break;
                    //MI Code: background music toggle
                    case "Music":
                        musicSource = Camera.main.GetComponent<AudioSource>();
                        if (musicSource != null) {
                            toggle.isOn = musicSource.isPlaying;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void CharacterInstantiation_CharacterInstantiated(GameObject character) {
            rec = character.GetComponent<PhotonVoiceRecorder>();
            rec.enabled = true;
        }

        private void BetterToggle_ToggleValueChanged(Toggle toggle) {
            switch (toggle.name) {
                case "Mute":
                    //AudioListener.pause = toggle.isOn;
                    if (toggle.isOn)
                    {
                        volumeBeforeMute = AudioListener.volume;
                        AudioListener.volume = 0f;
                    }
                    else
                    {
                        AudioListener.volume = volumeBeforeMute;
                        volumeBeforeMute = 0f;
                    }
                    break;
                case "Transmit":
                    if (rec) {
                        rec.Transmit = toggle.isOn;
                    }
                    break;
                case "VoiceDetection":
                    PhotonVoiceSettings.Instance.VoiceDetection = toggle.isOn;
                    if (rec) {
                        rec.Detect = toggle.isOn;
                    }
                    break;
                case "DebugEcho":
                    PhotonVoiceNetwork.Client.DebugEchoMode = toggle.isOn;
                    break;
                case "AutoConnect":
                    PhotonVoiceSettings.Instance.AutoConnect = toggle.isOn;
                    break;

                case "AutoDisconnect":
                    PhotonVoiceSettings.Instance.AutoDisconnect = toggle.isOn;
                    break;
                case "AutoTransmit":
                    PhotonVoiceSettings.Instance.AutoTransmit = toggle.isOn;
                    break;
                case "DebugVoice":
                    DebugMode = toggle.isOn;
                    PhotonVoiceSettings.Instance.DebugInfo = DebugMode;
                    break;
                //MI Code: background music toggle
                case "Music":
                    if (musicSource != null && musicSource.clip != null) {
                        if (toggle.isOn)
                        {
                            if(!musicSource.isPlaying)
                                Camera.main.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            Camera.main.GetComponent<AudioSource>().Stop();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnCameraChanged(Camera newCamera) {
            canvas.worldCamera = newCamera;
        }

        private void Start()
        {
            canvas = GetComponentInChildren<Canvas>();
            if (punSwitch != null)
            {
                punSwitchText = punSwitch.GetComponentInChildren<Text>();
                punSwitch.onClick.AddListener(PunSwitchOnClick);
            }
            if (voiceSwitch)
            {
                voiceSwitchText = voiceSwitch.GetComponentInChildren<Text>();
                voiceSwitch.onClick.AddListener(VoiceSwitchOnClick);
            }
            if (calibrateButton != null)
            {
                calibrateButton.onClick.AddListener(CalibrateButtonOnClick);
                calibrateText = calibrateButton.GetComponentInChildren<Text>();
            }
            if (punState != null)
            {
                debugGO = punState.transform.parent.gameObject;
            }
            volumeBeforeMute = AudioListener.volume;
            previousDebugLevel = PhotonVoiceNetwork.Client.loadBalancingPeer.DebugOut;
            if (globalSettings != null)
            {
                globalSettings.SetActive(true);
                InitToggles(globalSettings.GetComponentsInChildren<Toggle>());
            }
            if (devicesInfoText != null)
            {
                if (Microphone.devices == null || Microphone.devices.Length == 0)
                {
                    devicesInfoText.enabled = true;
                    devicesInfoText.color = Color.red;
                    devicesInfoText.text = "No microphone device detected!";
                }
                else if (Microphone.devices.Length == 1)
                {
                    devicesInfoText.text = string.Format("Mic.: {0}", Microphone.devices[0]);
                }
                else
                {
                    devicesInfoText.text = string.Format("Multi.Mic.Devices:\n0. {0} (active)\n", Microphone.devices[0]);
                    for (int i = 1; i < Microphone.devices.Length; i++)
                    {
                        devicesInfoText.text = string.Concat(devicesInfoText.text, string.Format("{0}. {1}\n", i, Microphone.devices[i]));
                    }
                }
            }
        }

        private void PunSwitchOnClick() {
            if (PhotonNetwork.connectionStateDetailed == ClientState.Joined) {
                PhotonNetwork.Disconnect();
            }
            else if (PhotonNetwork.connectionStateDetailed == ClientState.Disconnected ||
                PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated) {
#if UNITY_5_3
                PhotonNetwork.ConnectUsingSettings(string.Format("1.{0}", UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex));
#else
                PhotonNetwork.ConnectUsingSettings(string.Format("1.{0}", Application.loadedLevel));
#endif
             }
        }

        private void VoiceSwitchOnClick() {
            if (PhotonVoiceNetwork.ClientState == Client.Photon.LoadBalancing.ClientState.Joined) {
                PhotonVoiceNetwork.Disconnect();
            }
            else if (PhotonVoiceNetwork.ClientState == Client.Photon.LoadBalancing.ClientState.Uninitialized
                || PhotonVoiceNetwork.ClientState == Client.Photon.LoadBalancing.ClientState.Disconnected) {
                PhotonVoiceNetwork.Connect();
            }
        }

        private void CalibrateButtonOnClick() {
            if (rec && !rec.VoiceDetectorCalibrating) {
                rec.VoiceDetectorCalibrate(calibrationMilliSeconds);
            }
        }

        private void Update() {
            // editor only two-ways binding for toggles
#if UNITY_EDITOR
            InitToggles(globalSettings.GetComponentsInChildren<Toggle>());
#endif
            switch (PhotonNetwork.connectionStateDetailed) {
                case ClientState.PeerCreated:
                case ClientState.Disconnected:
                    punSwitch.interactable = true;
                    punSwitchText.text = "PUN Connect";
                    if (rec != null)
                    {
                        rec.enabled = false;
                        rec = null;
                    }
                    break;
                case ClientState.Joined:
                    punSwitch.interactable = true;
                    punSwitchText.text = "PUN Disconnect";
                    break;
                default:
                    punSwitch.interactable = false;
                    punSwitchText.text = "PUN busy";
                    break;
            }
            switch (PhotonVoiceNetwork.ClientState) {
                case Client.Photon.LoadBalancing.ClientState.Joined:
                    voiceSwitch.interactable = true;
                    voiceSwitchText.text = "Voice Disconnect";
                    inGameSettings.SetActive(true);
                    InitToggles(inGameSettings.GetComponentsInChildren<Toggle>());
                    if (rec != null) {
                        calibrateButton.interactable = !rec.VoiceDetectorCalibrating;
                        calibrateText.text = rec.VoiceDetectorCalibrating ? "Calibrating" : string.Format("Calibrate ({0}s)", calibrationMilliSeconds / 1000);
                    }
                    else {
                        calibrateButton.interactable = false;
                        calibrateText.text = "Unavailable";
                    }
                    break;
                case Client.Photon.LoadBalancing.ClientState.Uninitialized:
                case Client.Photon.LoadBalancing.ClientState.Disconnected:
                    if (PhotonNetwork.inRoom)
                    {
                        voiceSwitch.interactable = true;
                        voiceSwitchText.text = "Voice Connect";
                        voiceDebugText.text = "";
                    } else
                    {
                        voiceSwitch.interactable = false;
                        voiceSwitchText.text = "Voice N/A";
                        voiceDebugText.text = "";
                    }
                    calibrateButton.interactable = false;
                    calibrateText.text = "Unavailable";
                    inGameSettings.SetActive(false);
                    break;
                default:
                    voiceSwitch.interactable = false;
                    voiceSwitchText.text = "Voice busy";
                    break;
            }
            if (debugMode) {
                punState.text = string.Format("PUN: {0}", PhotonNetwork.connectionStateDetailed);
                voiceState.text = string.Format("PhotonVoice: {0}", PhotonVoiceNetwork.ClientState);
                if (rec != null && rec.LevelMeter != null) {
                    voiceDebugText.text = string.Format("Amp: avg. {0}, peak {1}",
                            rec.LevelMeter.CurrentAvgAmp.ToString("0.000000"),
                            rec.LevelMeter.CurrentPeakAmp.ToString("0.000000"));
                } 
            }
        }
    }

}