// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// SpeechInputSource allows you to specify keywords and methods in the Unity
    /// Inspector, instead of registering them explicitly in code.
    /// This also includes a setting to either automatically start the
    /// keyword recognizer or allow your code to start it.
    ///
    /// IMPORTANT: Please make sure to add the microphone capability in your app, in Unity under
    /// Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities
    /// or in your Visual Studio Package.appxmanifest capabilities.
    /// </summary>
    public class SpeechInputSource : BaseInputSource
    {
        /// <summary>
        /// Keywords are persistent across all scenes.  This Speech Input Source instance will not be destroyed when loading a new scene.
        /// </summary>
        [Tooltip("Keywords are persistent across all scenes.  This Speech Input Source instance will not be destroyed when loading a new scene.")]
        public bool PersistentKeywords;

        /// <summary>
        /// This enumeration gives the manager two different ways to handle the recognizer. Both will
        /// set up the recognizer and add all keywords. The first causes the recognizer to start
        /// immediately. The second allows the recognizer to be manually started at a later time.
        /// </summary>
        public enum RecognizerStartBehavior { AutoStart, ManualStart }

        [Tooltip("Whether the recognizer should be activated on start.")]
        public RecognizerStartBehavior RecognizerStart;

        [SerializeField]
        [Tooltip("The keywords to be recognized and optional keyboard shortcuts.")]
        private KeywordAndKeyCode[] keywords;
        public KeywordAndKeyCode[] Keywords { get { return keywords; } }

#if UNITY_WSA || UNITY_STANDALONE_WIN

        /// <summary>
        /// The confidence level at which the Keyword Recognizer will detect a correctly spoken keyword.
        /// <para><remarks>The serialized data of this field will be lost when switching between platforms and re-serializing this class.</remarks></para>
        /// </summary>
        [SerializeField]
        [Tooltip("The confidence level for the keyword recognizer.")]
        private ConfidenceLevel recognitionConfidenceLevel = ConfidenceLevel.Medium;

        private KeywordRecognizer keywordRecognizer;

        #region Unity Methods

        protected virtual void OnEnable()
        {
            if (keywordRecognizer != null && RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartKeywordRecognizer();
            }
        }

        protected virtual void Start()
        {
            if (PersistentKeywords)
            {
                gameObject.DontDestroyOnLoad();
            }

            UpdateKeywords(Keywords);
        }

        protected virtual void Update()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                for (int index = Keywords.Length; --index >= 0;)
                {
                    if (Input.GetKeyDown(Keywords[index].KeyCode))
                    {
                        OnPhraseRecognized(recognitionConfidenceLevel, TimeSpan.Zero, DateTime.Now, null, Keywords[index].Keyword);
                    }
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (keywordRecognizer != null)
            {
                StopKeywordRecognizer();
            }
        }

        protected virtual void OnDestroy()
        {
            if (keywordRecognizer != null)
            {
                StopKeywordRecognizer();
                keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
                keywordRecognizer.Dispose();
            }
        }

        #endregion Unity Methods

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            OnPhraseRecognized(args.confidence, args.phraseDuration, args.phraseStartTime, args.semanticMeanings, args.text);
        }

        protected void OnPhraseRecognized(ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string text)
        {
            InputManager.Instance.RaiseSpeechKeywordPhraseRecognized(this, 0, confidence, phraseDuration, phraseStartTime, semanticMeanings, text);
        }

        /// <summary>
        /// Make sure the keyword recognizer is off, then start it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StartKeywordRecognizer()
        {
            if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Start();
            }
        }

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StopKeywordRecognizer()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
        }

        /// <summary>
        /// Updates the list of keywords for the Keyword Recognizer to handle.
        /// </summary>
        /// <param name="newKeywordPairs"></param>
        public void UpdateKeywords(KeywordAndKeyCode[] newKeywordPairs)
        {
            if (keywordRecognizer != null)
            {
                StopKeywordRecognizer();
                keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
                keywordRecognizer.Dispose();
            }

            keywords = newKeywordPairs;

            int keywordCount = keywords.Length;
            if (keywordCount > 0)
            {
                var newKeywords = new string[keywordCount];

                for (int index = 0; index < keywordCount; index++)
                {
                    newKeywords[index] = keywords[index].Keyword;
                }

                keywordRecognizer = new KeywordRecognizer(newKeywords, recognitionConfidenceLevel);
                keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

                if (RecognizerStart == RecognizerStartBehavior.AutoStart)
                {
                    keywordRecognizer.Start();
                }
            }
            else
            {
                Debug.LogError("Must have at least one keyword specified in the Inspector on " + gameObject.name + ".");
            }
        }

#endif

        #region Base Input Source Methods

        public override bool TryGetSourceKind(uint sourceId, out InteractionSourceInfo sourceKind)
        {
            sourceKind = InteractionSourceInfo.Voice;
            return true;
        }

        public override bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        public override bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
            pointingRay = default(Ray);
            return false;
        }

        public override bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            return SupportedInputInfo.None;
        }

        public override bool TryGetThumbstick(uint sourceId, out bool isPressed, out Vector2 position)
        {
            isPressed = false;
            position = Vector2.zero;
            return false;
        }

        public override bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out Vector2 position)
        {
            isPressed = false;
            isTouched = false;
            position = Vector2.zero;
            return false;
        }

        public override bool TryGetSelect(uint sourceId, out bool isPressed, out double pressedAmount)
        {
            isPressed = false;
            pressedAmount = 0.0;
            return false;
        }

        public override bool TryGetGrasp(uint sourceId, out bool isPressed)
        {
            isPressed = false;
            return false;
        }

        public override bool TryGetMenu(uint sourceId, out bool isPressed)
        {
            isPressed = false;
            return false;
        }

        #endregion // Base Input Source Methods
    }
}