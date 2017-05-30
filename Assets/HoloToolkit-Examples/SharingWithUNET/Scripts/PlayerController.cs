// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

namespace HoloToolkit.Examples.SharingWithUNET
{
    /// <summary>
    /// Controls player behavior (local and remote).
    /// </summary>
    [NetworkSettings(sendInterval = 0.033f)]
    public class PlayerController : NetworkBehaviour, IInputClickHandler
    {

        KeywordRecognizer keywordRecognizer = null;
        Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

        private NetworkManager NM;
        private PairingProcess PP;
        private PairingCryptoManager CryptoManager;
        private GameObject HUD;

        private int maxNumSecretElements = 8;
        private int[] secretElementSizes = { 4, 6, 8 };

        // TODO: this should probably be an enum at some point, like this:
        //   public enum MoveType { Time, Speed }
        // MoveType moveType
        // if (moveType == MoveType.Time) {}
        private bool initialShouldUseSecretColors = false;
        private bool initialShouldUseArrows = false;              

        public GameObject secretPositionIndicator;
        public GameObject lineFromTo;
        public GameObject arrowDirectionPointer;
        /// <summary>
        /// The transform of the shared world anchor.
        /// </summary>
        private Transform sharedWorldAnchorTransform;

        public delegate void NoParamFunction();

        /// <summary>
        /// The position relative to the shared world anchor.
        /// </summary>
        [SyncVar]
        private Vector3 localPosition;

        /// <summary>
        /// The rotation relative to the shared world anchor.
        /// </summary>
        [SyncVar]
        private Quaternion localRotation;

        /// <summary>
        /// Sets the localPosition and localRotation on clients.
        /// </summary>
        /// <param name="postion">the localPosition to set</param>
        /// <param name="rotation">the localRotation to set</param>
        [Command]
        private void CmdTransform(Vector3 postion, Quaternion rotation)
        {
            if (!isLocalPlayer)
            {
                localPosition = postion;
                localRotation = rotation;
            }
        }

        [ClientRpc]
        private void RpcPlayerASendProtocolParameters(bool newShouldUseSecretColors, int numOfSecretElements, bool shouldAttackHappen)
        {
            PP.cnfgShouldUseSecretColors = newShouldUseSecretColors;
            PP.cnfgNumberOfSecretElements = numOfSecretElements;
            PP.isAttackHappening = shouldAttackHappen;
        }

        [ClientRpc]
        private void RpcPlayerASendPublicKey(string newPublicKeyA)
        {
            PP.msgPublicKeyA = newPublicKeyA;
        }

        [ClientRpc]
        private void RpcPlayerAAbort()
        {
            PP.msgProtocolAborted = true;
        }

        [Command]
        private void CmdMsgPlayerBAbort()
        {
            PP.msgProtocolAborted = true;
        }


        [Command]
        private void CmdMsgPlayerBRestarted()
        {
            PP.msgProtocolRestarted = true;
        }

        [Command]
        private void CmdPlayerBSendPublicKey(string newPublicKeyB)
        {
            PP.msgPublicKeyB = newPublicKeyB;
        }

        [ClientRpc]
        private void RpcPlayerASendFinalMessage(string newFinalMessage)
        {
            PP.msgFinalMessage = newFinalMessage;
        }

        [ClientRpc]
        private void RpcPlayerASendHashOfK(string newHashOfK)
        {
            PP.msgHashOfK = newHashOfK;
        }

        [ClientRpc]
        private void RpcPlayerASendEncryptedK(string newEncryptedK)
        {
            PP.msgEncryptedK = newEncryptedK;
        }

        [ClientRpc]
        private void RpcPlayerARestarted()
        {
            PP.msgProtocolRestarted = true;
        }

        [Command]
        private void CmdWriteLogLineToFile(string logLine)
        {
            PP.AppendStringToFile(logLine);
        }

        private void setAllBoxesVisibility(bool visibility)
        {
            for (int i = 0; i < maxNumSecretElements; i++)
            {
                setElementVisibility("SecretColor" + i.ToString(), visibility);
            }

            for (int i = 1; i <= 4; i++)
            {
                setElementVisibility("ButtonCube" + i.ToString(), visibility);
            }

            if (!visibility)
            {
                GameObject[] arrows = GameObject.FindGameObjectsWithTag("ArrowDirection");
                for (int i = 0; i < arrows.Length - 1; i++)
                {
                    Destroy(arrows[i]);
                }

            }
        }

        private void setElementVisibility(string element, bool visibility)
        {
            GameObject box = GameObject.Find(element);
            if (box != null)
            {
                box.GetComponent<Renderer>().enabled = visibility;
            }
        }

        private void userSaidUpdate()
        {
            if (isLocalPlayer)
            {
                PP.UpdateSharedHologramsLocation();
            }
        }

        private void userSaidSwitch()
        {
            if (isServer && isLocalPlayer)
            {
                PP.cnfgShouldUseSecretColors = !PP.cnfgShouldUseSecretColors;
                SetHUDMessage("Confirmation Method Changed!");
                RestartProtocol();
            }
        }

        // Executed when user decides to abort, or protocol is aborted due to missmatch in cryptography.
        // We make sure that other side is notified and then clean up the state.
        private void UserSaidAbortOrCryptoFailed()
        {
            if (!isLocalPlayer) return;

            PP.SaveLogForPairingAttempt("ABORT");
            protocolAborted();

            // Notify the other player
            if (PP.isPlayerA)
            {
                RpcPlayerAAbort();
            }
            else
            {
                CmdMsgPlayerBAbort();
            }
        }

        private void UserSaidRestart()
        {
            if (isLocalPlayer)
            {
                PP.SaveLogForPairingAttempt("RESTART");
                RestartProtocol();
            }
        }

        private void UserSaidChangeRoles()
        {
            if (isServer && isLocalPlayer)
            {
                SetHUDMessage("Changing roles successful \n say \"RESTART\" when ready");
                PP.SwitchRoles();
            }
        }

        private void userConfirmedColorsOK()
        {
            // if user A and in right step, change the variable
            if (isLocalPlayer && isServer && PP.currentStep == 7 && PP.allowedToClickPairingOK)
            {
                PP.userAConfirmedPairingSuccessful = true;
            }
        }

        private void userConfirmedStartPairing()
        {
            // if user A and in right step, then change the variable
            if (isLocalPlayer && isServer && PP.currentStep == 5)
            {
                PP.userAConfirmedVisualACK = true;

                PP.allowedToClickPairingOK = false;
                // After 2 seconds, allow that user clicks "pairing OK"
                StartCoroutine(ExecuteAfterDelay(2.0f, () => { PP.allowedToClickPairingOK = true; }));
            }
        }

        private void initializeNewPairing()
        {
            PP.initializeNewPairing(isServer);
            HideARChallengeVisualElements();
            SetHUDMessage("Waiting for others...");
        }

        private void setupKeywordRecognizer()
        {
            keywords.Add("Colors OK", () => { userConfirmedColorsOK(); });
            keywords.Add("Abort", () => { UserSaidAbortOrCryptoFailed(); });
            keywords.Add("Start Pairing", () => { userConfirmedStartPairing(); });
            keywords.Add("Restart", () => { UserSaidRestart(); });

            // Change Settings
            keywords.Add("Switch", () => { userSaidSwitch(); });
            keywords.Add("Update", () => { userSaidUpdate(); });
            keywords.Add("Change roles", () => { UserSaidChangeRoles(); });

            //keywords.Add("Ready", () => { });

            // Tell the KeywordRecognizer about our keywords.
            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

            // Register a callback for the KeywordRecognizer and start recognizing!
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Start();
        }

        private void SetOtherPlayerColor(Color newColor)
        {
            // TODO: this is not implemented properly!
            GetComponentInChildren<MeshRenderer>().material.color = newColor;
        }
        private void Start()
        {
            NM = GameObject.Find("UNETSharingStage").GetComponent<NetworkManager>();
            PP = GameObject.Find("NetKeyboard").GetComponent<PairingProcess>();
            CryptoManager = GameObject.Find("CryptoManager").GetComponent<PairingCryptoManager>();
            HUD = GameObject.Find("InstructionsHUD");

            PP.cnfgShouldUseSecretColors = initialShouldUseSecretColors;

            if (isLocalPlayer)
            {
                setupKeywordRecognizer();
                initializeNewPairing();
            }

            if (SharedCollection.Instance == null)
            {
                Debug.LogError("This script required a SharedCollection script attached to a gameobject in the scene");
                Destroy(this);
                return;
            }        

            if (isLocalPlayer)
            {
                // If we are the local player then we want to have airtaps 
                // sent to this object so that projeciles can be spawned.
                InputManager.Instance.AddGlobalListener(gameObject);
            }
            else
            {
                Debug.Log("remote player");
                Color baseColor = Color.yellow;
                Color otherPlayerColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.1f);
                SetOtherPlayerColor(otherPlayerColor);
            }

            sharedWorldAnchorTransform = SharedCollection.Instance.gameObject.transform;
            transform.SetParent(sharedWorldAnchorTransform);
        }

        void setupSecretColors(int numOfSecretElements, string myFinalSharedSecret)
        {
            // secretColoring will be twice the size: element 2*i is the color, element 2*i+1 is orientation
            int[] secretColoring = CryptoManager.generateSecretColoringFromString(myFinalSharedSecret, numOfSecretElements);            

            Color[] availableColors = new Color[4] {
                Color.red,
                Color.green,
                Color.blue,
                Color.white
            };

            // First hide all of them, but then show those which we don't need.
            setAllBoxesVisibility(false);
            for (int i = 0; i < numOfSecretElements; i++)
            {
                string currentBoxName = "SecretColor" + i;
                setElementVisibility(currentBoxName, true);

                int secretColorIndex = isServer ? i : numOfSecretElements - i - 1; 
                setObjectColor(currentBoxName, availableColors[secretColoring[secretColorIndex]]);
                if (initialShouldUseArrows && initialShouldUseSecretColors) { 
                    AddArrow(secretColoring[secretColorIndex + numOfSecretElements], i);
                }
            }

            for (int i = 1; i <= 4; i++)
            {
                setElementVisibility("ButtonCube" + i.ToString(), true);
                setObjectColor("ButtonCube" + i.ToString(), availableColors[i - 1]);
            }
        }

        private void AddArrow(int rotationInNumber, int arrowIdx)
        {
            string[] directions = new string[4]
            {
                "↑", "→", "↓", "←"
            };

            GameObject secretColorObject = GameObject.Find("SecretColor" + arrowIdx);
            GameObject newArrowObject = (GameObject)Instantiate(arrowDirectionPointer);
            newArrowObject.transform.parent = secretColorObject.transform;
            newArrowObject.transform.localRotation = Quaternion.identity; // make the object aligned with the parent.
            newArrowObject.transform.localPosition = new Vector3(0f, 0f, 0f); // change 0f to something negative if we want to move it a bit farther from userB



            //arrowDirectionGO.transform.position = secColGO.transform.position;
            //arrowDirectionGO.transform.rotation = secColGO.transform.rotation;

            TextMesh text = newArrowObject.GetComponent<TextMesh>();
            if (isServer)
            {
                if (rotationInNumber == 1)
                {
                    rotationInNumber = 3;
                }
                else if (rotationInNumber == 3)
                {
                    rotationInNumber = 1;
                }
            }
            text.text = directions[rotationInNumber];
        } 

        private void SetLineDimensionsFromTo(GameObject objectToTransform, Transform posStart, Transform posEnd)
        {
            objectToTransform.transform.position = (posEnd.position + posStart.position) / 2.0f;  // it should be positioned at half
            objectToTransform.transform.localScale = new Vector3(objectToTransform.transform.localScale.x, 0.95f * (posEnd.localPosition - posStart.localPosition).magnitude / 2.0f, objectToTransform.transform.localScale.z); // set the correct local scale
            objectToTransform.transform.localRotation = Quaternion.FromToRotation(Vector3.up, posEnd.position - posStart.position);  // rotate properly
        }

        void setupSecretPositions(int numOfSecretPositions, string myFinalSharedSecret)
        {
            float[] secretPositions = CryptoManager.generateSecretPositionsFromString(myFinalSharedSecret, numOfSecretPositions);

            Transform previousSecretTransform = null;
            for (int i = 0; i < numOfSecretPositions; ++i)
            {
                // Generate numbers
                GameObject newSecretPositionIndicator = (GameObject) Instantiate(secretPositionIndicator);
                Transform indicatorTransform = newSecretPositionIndicator.transform;
                indicatorTransform.parent = GameObject.Find("NetKeyboard").transform;
                indicatorTransform.localRotation = Quaternion.identity; // make the object aligned with the parent.
                newSecretPositionIndicator.GetComponent<TextMesh>().text = (i + 1).ToString();
                if (i == 0) { newSecretPositionIndicator.GetComponent<TextMesh>().fontSize *= 2; } // make number 1 more obvious
                indicatorTransform.localPosition = new Vector3(secretPositions[2*i], secretPositions[2*i+1], 0f); // change 0f to something negative if we want to move it a bit farther from userB

                if (!isServer)
                {
                    // Rotate again around y-axis if I am player B (because the whole NetKeyboard will be rotated once again
                    indicatorTransform.RotateAround(indicatorTransform.position, indicatorTransform.up, 180f);
                }

                // Generate lines between the previous and the current secret position?
                if (i > 0)
                {
                    GameObject newSecretLine = (GameObject)Instantiate(lineFromTo);
                    // , new Vector3(0f, 0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                    SetLineDimensionsFromTo(newSecretLine, previousSecretTransform, newSecretPositionIndicator.transform);
                    newSecretLine.transform.parent = GameObject.Find("NetKeyboard").transform;
                }

                previousSecretTransform = newSecretPositionIndicator.transform;
            }
        }
        void setupSharedSecretVisualisation(bool shouldUseSecretColors, int numElements, string myFinalSecretKey)
        {
            if (shouldUseSecretColors)
            {
                setupSecretColors(numElements, myFinalSecretKey);
            }
            else
            {
                setupSecretPositions(numElements, myFinalSecretKey);

            }
            // so that the location is properly set between two players.
            PP.UpdateSharedHologramsLocation();
        }

        void setObjectColor(string boxName, Color color)
        {
            GameObject box = GameObject.Find(boxName);
            MeshRenderer mr = box.GetComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("Diffuse"));
            mr.material.color = color;
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            System.Action keywordAction;
            if (keywords.TryGetValue(args.text, out keywordAction))
            {
                keywordAction.Invoke();
            }
        }

        private void OnDestroy()
        {
            if (isLocalPlayer)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        private void SetHUDMessage(string text)
        {
            var hud = GameObject.Find("StepsText").GetComponent<Text>();
            hud.text = text;

        }

        public void RemoveAllPositionIndicators()
        {
            foreach (var currentObject in GameObject.FindGameObjectsWithTag("secret-position-indicator"))
            {
                Destroy(currentObject);
            }
        }

        private void HideARChallengeVisualElements()
        {
            setAllBoxesVisibility(false);
            RemoveAllPositionIndicators();
        }

        private void UpdateLocationsIfNeeded()
        {
            if (isServer)
            {
                if (isLocalPlayer)
                    PP.setServerLocation(Camera.main.transform.position);
                else
                    PP.setClientLocation(transform.localPosition);
            }
            else
            {
                if (isLocalPlayer)
                    PP.setClientLocation(Camera.main.transform.position);
                else
                    PP.setServerLocation(transform.position);
            }
        }

        private void runProtocolAsPlayerA()         // Player A is Server
        {
            if (PP.msgProtocolAborted) protocolAborted();
            if (PP.msgProtocolRestarted) initializeNewPairing();

            // Step 0: once player B has connected, send the shared protocol parameters.
            //    For now, we send:
            //         -- which type of confirmation step are we using: secretColors or secretNumbers
            //         -- how many secret elements should there be
            if (PP.currentStep == 0 && NM.numPlayers == 2)
            {
                // Choose the number of secret elements
                System.Random rand = new System.Random();
                PP.cnfgNumberOfSecretElements = secretElementSizes[rand.Next(0, secretElementSizes.Length)];



                RpcPlayerASendProtocolParameters(PP.cnfgShouldUseSecretColors, PP.cnfgNumberOfSecretElements, PP.shouldAttackHappen());

                PP.currentStep = 1;
            }

            // Step 1: User A sends his public key
            if (PP.currentStep == 1 && PP.msgPublicKeyA == "")
            {
                PP.AddTimeStampsToSteps(PP.currentStep);
                Debug.Log("Entering Step 1");

                // generate userA's key pair
                CryptoManager.generateNewKeyPair();
                // send it to the server who will broadcast it to everyone else

                Debug.Log("moj kljuc: " + CryptoManager.getPublicKey());
                RpcPlayerASendPublicKey(CryptoManager.getPublicKey());
                Debug.Log("Sending PublicKeyA: " + CryptoManager.getPublicKey());
                PP.currentStep = 3; // go to the next step in the protocol
            }

            // Step 3: After receiving B's public key, user A makes a commitment on some value K
            if (PP.currentStep == 3 && PP.msgPublicKeyB != "")
            {
                PP.AddTimeStampsToSteps(PP.currentStep);
                // Player A generates those himself
                PP.myPrivateK = CryptoManager.generateRandomNonce();
                PP.myHashOfK = CryptoManager.generateHashFromString(PP.myPrivateK);
                RpcPlayerASendHashOfK(PP.myHashOfK);
                SetHUDMessage("1. When other user waves, click on their cube.");

                PP.currentStep = 5;
            }

            // Step 5: After user A confirms that he has received an ACK from user B on the visual channel, we proceed
            if (PP.currentStep == 5 && PP.userAConfirmedVisualACK)
            {
                PP.AddTimeStampsToSteps(PP.currentStep);
                // Encrypt the plaintext K using B's public key
                string myEncryptedK = CryptoManager.encrypt(PP.myPrivateK, PP.msgPublicKeyB);

                RpcPlayerASendEncryptedK(myEncryptedK);
                PP.msgEncryptedK = myEncryptedK;

                // I can now also set all the colors 
                PP.myFinalSharedKey = PP.msgPublicKeyA + PP.msgPublicKeyB + PP.myPrivateK;
                setupSharedSecretVisualisation(PP.cnfgShouldUseSecretColors, PP.cnfgNumberOfSecretElements, PP.myFinalSharedKey);

                SetHUDMessage("2. If gestures are correct, click on their cube.\nIf they make a mistake, say \"Abort\"");

                PP.currentStep = 7;
            }

            if (PP.currentStep == 7 && PP.userAConfirmedPairingSuccessful)
            {
                PP.AddTimeStampsToSteps(PP.currentStep);

                // Send a message in which a hash of our shared key is encrypted with B's private key
                string hashedSharedKey = CryptoManager.generateHashFromString(PP.myFinalSharedKey);
                RpcPlayerASendFinalMessage(hashedSharedKey);

                protocolSuccessful();
            }
        }

        private void runProtocolAsPlayerB()
        {
            if (PP.msgProtocolAborted) protocolAborted();
            if (PP.msgProtocolRestarted) initializeNewPairing();
            if (PP.currentStep == 0) { PP.currentStep = 2; }


            // Step 2: Generate and broadcast userB's public keypr
            if (PP.currentStep == 2 && PP.msgPublicKeyA != "")
            {
                // generate userB's public key
                CryptoManager.generateNewKeyPair();
                // send it to the server who will broadcast it to everyone else

                PP.msgPublicKeyB = CryptoManager.getPublicKey();
                CmdPlayerBSendPublicKey(PP.msgPublicKeyB);
                
                PP.currentStep = 4;
            }

            // Step 4: Acknowledge receipt of hashed value of K
            if (PP.currentStep == 4 && PP.msgHashOfK != "")
            {
                // player B stores the values that he receives
                PP.myHashOfK = PP.msgHashOfK;

                // We should tell the user to wave here! Or somehow differently acknowledge that he has received what he was supposed to.
                SetHUDMessage("1. Wave to the other user!");
                PP.currentStep = 6;
            }

            // Step 6: Decrypt the plaintext value of K and confirm that it's OK.
            if (PP.currentStep == 6 && PP.msgEncryptedK != "")
            {
                PP.myPrivateK = CryptoManager.decryptWithMyKeypair(PP.msgEncryptedK);

                // Check if the value K matches the commitment.
                if (CryptoManager.generateHashFromString(PP.myPrivateK) != PP.myHashOfK)
                {
                    UserSaidAbortOrCryptoFailed(); // Crypto does not match!
                }

                if (PP.isAttackHappening)
                {
                    // If attack is happening, we simulate it by generating the sharedKeyK
                    //    in way as if received data was wrong.
                    PP.myFinalSharedKey = PP.msgPublicKeyB + PP.myPrivateK + PP.msgPublicKeyA;
                }
                else
                {
                    // If there is no attack, do it right.
                    PP.myFinalSharedKey = PP.msgPublicKeyA + PP.msgPublicKeyB + PP.myPrivateK;
                }
                
                setupSharedSecretVisualisation(PP.cnfgShouldUseSecretColors, PP.cnfgNumberOfSecretElements, PP.myFinalSharedKey);
                if (PP.cnfgShouldUseSecretColors)
                    SetHUDMessage("2. Point to the cubes in the right order.");
                else
                    SetHUDMessage("2. Follow the path with your finger.");

                PP.currentStep = 8;
            }

            if (PP.currentStep == 8 && PP.msgFinalMessage != "")
            {
                // Check if the hash of the sharedKey is the same as the received msgFinalMessage
                // If we are simulating an attack, this won't be detected since the attacker would supposedly be smart enough here?
                // TODO: is this correct?!
                if (PP.isAttackHappening || CryptoManager.generateHashFromString(PP.myFinalSharedKey) == PP.msgFinalMessage)
                    protocolSuccessful();
                else
                    UserSaidAbortOrCryptoFailed(); // CryptoFailed
            }
        }

        private void protocolSuccessful()
        {
            PP.SaveLogForPairingAttempt("SUCCESS"); // for now, step "10" is success // TODO: implement smarter.
            Debug.Log("Pairing Successful!");
            SetHUDMessage("3. Pairing Successful!");
            // Make the other user's cube green to indicate that pairing was successful!
            // TODO: this does not work because we are actually only changing our own color (here isLocalPlayer == true)
            SetOtherPlayerColor(Color.green);

            PP.currentStep = -1;
            HideARChallengeVisualElements();
        }

        private void RestartProtocol()
        {
            if (PP.isPlayerA)
            {
                RpcPlayerARestarted();
            }
            else
            {
                CmdMsgPlayerBRestarted();
            }

            initializeNewPairing();
        }

        private void protocolAborted()
        {
            Debug.Log("Pairing Failed!");
            SetHUDMessage("3. Pairing Failed!");

            PP.currentStep = -1;
            HideARChallengeVisualElements();

            RestartProtocol();
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                if (isServer)
                {
                    runProtocolAsPlayerA();
                }
                else if (!isServer)
                {
                    runProtocolAsPlayerB();
                }


                // ------------- Handle positioning and different player's locations

                // if we are the remote player then we need to update our worldPosition and then set our 
                // local (to the shared world anchor) position for other clients to update our position in their world.
                transform.position = Camera.main.transform.position;
                transform.rotation = Camera.main.transform.rotation;

                // Depending on if you are host or client, either setting the SyncVar (client) 
                // or calling the Cmd (host) will update the other users in the session.
                // So we have to do both.
                localPosition = transform.localPosition;
                localRotation = transform.localRotation;

                CmdTransform(localPosition, localRotation);
            }
            else
            {

                // If we aren't the local player, we only need to make sure that the position of this object is set properly
                // so that we properly render their avatar in our world.           

                transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, 0.3f);
                transform.localRotation = localRotation;
            }

            UpdateLocationsIfNeeded();
            UpdateHUDPosition();
        }

        public IEnumerator ExecuteAfterDelay(float totalTime, NoParamFunction ActionToCall)
        {
            float rate = 1.0f / totalTime;
            float t = 0.0f;
            while (t < 1.0)
            {
                t += Time.deltaTime * rate;
                yield return null;
            }
            ActionToCall();
        }

        /// <summary>
        /// Called when the local player starts.  In general the side effect should not be noticed
        /// as the players' avatar is always rendered on top of their head.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            SetOtherPlayerColor(Color.blue);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (isLocalPlayer)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    if (hit.transform.gameObject.name == "Cube")    // TODO: maybe player?
                    {
                        // TODO: ovo NIJE DOBRO!
                        userConfirmedStartPairing();
                        userConfirmedColorsOK();
                    }
                }
            }
        }

        private void UpdateHUDPosition()
        {
            // Choose where to place it
            Vector3 otherPos = (isServer) ? PP.clientLocation : PP.serverLocation;
            Vector3 myPos = (isServer) ? PP.serverLocation : PP.clientLocation;
         
            // Set position slightly above the other player's head
            HUD.transform.position = new Vector3(otherPos.x, otherPos.y + 0.3f, otherPos.z);

			// Make the text face me
			HUD.transform.rotation = Quaternion.LookRotation(otherPos - myPos);
        }
    }
}
