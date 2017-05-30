using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

#if NETFX_CORE
    using Windows.Storage;
    using System.Threading.Tasks;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Store;
#endif

public class PairingProcess : MonoBehaviour
{
    //private PairingCryptoManager CryptoManager;

    private GameObject buttonsCanvas;
    private GameObject secretColorsCanas;
    private List<GameObject> buttons;
    private Color[] availableColors;

    [HideInInspector] public Vector3 serverLocation;
    [HideInInspector] public Vector3 clientLocation;

    private string resultsFilename = "";
    private int idPlayerA;
    private int idPlayerB;
    private string[] logTimestamps = { "-1", "-1", "-1", "-1", "-1"};
    private int log_session_overall = 0;
    private int log_session_for_user = 0;
    private bool didSwitchHappen = false;


    // msgVaribales represent network messages between two player objects on a specific device
    // They must only be set by the server using using clientRpc and cmdFunctions.
    // Probably not the best way to implement this feature, but should be OK for now.
    // TODO: for security (to ensure the inability that anyone changes those variables subsequently), we should make those private,
    //     and have getters and setters. Setters must ensure that the variable changes ONLY if it was never set.
    // TODO: we could also just have one "setting" and "getting" variable, so that we don't duplicate the code, and use a dictionary?
    [HideInInspector] public string msgPublicKeyA;
    [HideInInspector] public string msgPublicKeyB;
    [HideInInspector] public string msgHashOfK;
    [HideInInspector] public string msgEncryptedK;
    [HideInInspector] public string msgFinalMessage;
    [HideInInspector] public bool msgProtocolAborted;
    [HideInInspector] public bool msgProtocolRestarted;

    [HideInInspector] public int cnfgNumberOfSecretElements;
    [HideInInspector] public bool cnfgShouldUseSecretColors;
    [HideInInspector] public bool isAttackHappening;


    // Private variables needed in the protocol execution, not shared among players
    [HideInInspector] public string myPrivateK;       // Player A needs to preserve this between states 3 and 5
    [HideInInspector] public string myHashOfK;        // Player B needs to preserve this between states 4 and 6
    [HideInInspector] public string myFinalSharedKey; // Both players compute this independently from their PrivateK and exchanged public keys.
    [HideInInspector] public bool userAConfirmedPairingSuccessful;
    [HideInInspector] public bool userAConfirmedVisualACK;
    [HideInInspector] public int currentStep;

    [HideInInspector] public bool isPlayerA;
    [HideInInspector] public int attackProbability = 20;
    [HideInInspector] public bool allowedToClickPairingOK;

    private System.Random myRand;


    public void setServerLocation(Vector3 point)
    {
        this.serverLocation = point;
    }

    public void setClientLocation(Vector3 point)
    {
        this.clientLocation = point;
    }

    private void initializeMessages()
    {
        msgPublicKeyA = "";
        msgPublicKeyB = "";
        msgHashOfK = "";
        msgEncryptedK = "";
        msgFinalMessage = "";
        msgProtocolAborted = false;
        msgProtocolRestarted = false;
    }

    private void initializePrivateStateVariables()
    {
        myPrivateK = "";       // Player A needs to preserve this between states 3 and 5
        myHashOfK = "";        // Player B needs to preserve this between states 4 and 6
        myFinalSharedKey = ""; // Both players compute this independently from their PrivateK and exchanged public keys.
        userAConfirmedPairingSuccessful = false;
        userAConfirmedVisualACK = false;
        currentStep = 0;
        allowedToClickPairingOK = false;
    }

    public bool shouldAttackHappen()
    {
        int myRandomNumber = myRand.Next(1, 100);
        return (myRandomNumber <= attackProbability);
    }


    public void initializeNewPairing(bool isServer)
    {
        isPlayerA = isServer;
        initializeMessages();
        initializePrivateStateVariables();
        initializeFileStorage();
        //GenerateBasicInfoLogLine();
    }


    // --------- CODE RELATED TO STORING EXPERIMENT RESULTS!
    // TODO: move into a separate class eventually.
    public void initializeFileStorage()
    {
        if (resultsFilename == "")
            resultsFilename = "experiment_result_" + DateTime.Now.ToString("yy-MM-dd__hh_mm_ss") + ".csv";

        Debug.Log("Setting resultsFilename to: |" + resultsFilename + "|");
    }

    public string GenerateBasicInfoLogLine()
    {
        return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},",
            idPlayerA,
            idPlayerB,
            log_session_overall,
            log_session_for_user,
            didSwitchHappen ? 1 : 0,
            isAttackHappening ? 1 : 0,
            cnfgShouldUseSecretColors ? 1 : 0,
            cnfgNumberOfSecretElements);
    }

    public void AddTimeStampsToSteps(int stepNumber)
    {
        long timestampInMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        int logIndex = -1;
        switch(stepNumber)
        {
            case 1:
                logIndex = 0;
                break;
            case 3:
                logIndex = 1;
                break;
            case 5:
                logIndex = 2;
                break;
            case 7:
                logIndex = 3;
                break;
        }
        logTimestamps[logIndex] = timestampInMilliseconds.ToString();
    }

    public void SaveLogForPairingAttempt(string comment)
    {
        if (comment != "RESTART" && isPlayerA)
        {
            log_session_overall++;
            log_session_for_user++;
            string logStringToStore = GenerateBasicInfoLogLine();
            long timestampInMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            logTimestamps[logTimestamps.Length - 1] = timestampInMilliseconds.ToString();

            // Add all timestamps for all steps we need
            for (int i = 0; i < logTimestamps.Length; i++)
            {
                logStringToStore += logTimestamps[i] + ",";
                logTimestamps[i] = "-1";
            }

            logStringToStore += comment + "\n";
            AppendStringToFile(logStringToStore);
        }
    }

    public void SwitchRoles()
    {
        int tmpUserId = idPlayerA;
        idPlayerA = idPlayerB;
        idPlayerB = tmpUserId;
        log_session_for_user = 0;
        didSwitchHappen = !didSwitchHappen;
    }

    public void AppendStringToFile(string stringToStore)
    {
#if NETFX_CORE
        string folderPath = ApplicationData.Current.LocalFolder.Path;
        Debug.Log("Opening: |" + folderPath + "| filename: |" + resultsFilename + "|\\And storing: |" + stringToStore + "|");

        Task task = new Task(
            async () => {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                StorageFile file = await folder.CreateFileAsync(resultsFilename, CreationCollisionOption.OpenIfExists);
                await FileIO.AppendTextAsync(file, stringToStore);
            });
        task.Start();
        task.Wait();
#endif
    }

    // -----------------------------------------

    // Use this for initialization
    void Start()
    {
        myRand = new System.Random();
        idPlayerA = myRand.Next(1, 10000);
        idPlayerB = myRand.Next(1, 10000);
        // UpdateSharedHologramsLocation();
    }

    // Update is called once per frame
    void Update()
    {
//        UpdateSharedHologramsLocation();
    }

    public IEnumerator TranslateTo(Transform thisTransform, Vector3 endPos, float value)
    {
        yield return TranslationFromTo(thisTransform, thisTransform.position, endPos, value);
    }

    public IEnumerator TranslationRelativeBy(Transform thisTransform, Vector3 relPos, float value)
    {
        yield return TranslationFromTo(thisTransform, thisTransform.position, thisTransform.position + relPos, value);
    }

    public IEnumerator TranslationFromTo(Transform thisTransform, Vector3 startPos, Vector3 endPos, float value)
    {
        float rate = 1.0f / value;
        float t = 0.0f;
        while (t < 1.0)
        {
            t += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }
    }

    // TODO: we could have one update which is immediate and one which is with movement
    public void UpdateSharedHologramsLocation()
    {
        float startDistFromPlayerB = 1.5f;
        float endDistFromPlayerB = 0.85f; // in meters
        float timeForTranslation = 3.0f; // in seconds

        Vector3 dir = (this.serverLocation - this.clientLocation).normalized;
        // this.transform.position = this.clientLocation + dir * distFromPlayerB;    

        this.transform.position = this.clientLocation + dir * startDistFromPlayerB;
        Vector3 endPos = this.clientLocation + dir * endDistFromPlayerB;
        StartCoroutine(TranslateTo(this.transform, endPos, timeForTranslation));

        // Rotate the position of the shared elements if I am the player A
        if (isPlayerA)
        {
            this.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
        else
        {
            this.transform.LookAt(Camera.main.transform);
        }
    }
}
