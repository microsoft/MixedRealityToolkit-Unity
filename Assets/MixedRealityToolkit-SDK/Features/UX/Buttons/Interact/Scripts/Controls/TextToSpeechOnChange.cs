// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interact.Controls
{
    /// <summary>
    /// speak if TextMesh content has changed.
    /// </summary>
    public class TextToSpeechOnChange : MonoBehaviour
    {

        [Tooltip("The TextMesh object to look for changes, looks on self by default")]
        public TextMesh Label;

        [Tooltip("How fast speech should update if a Label is continually updating")]
        public float Interval = 0.5f;

        [Tooltip("if the speech should occur even if the Sound Source is busy")]
        public bool Interrupt = true;

        [Tooltip("Something to say in front of the changing text.")]
        public string ToolTip = "";

        // find the speechManager
        private Object mSpeechManager; // TEMP
        //timer
        private float counter = 0;
        // last Label value
        private string mCachedLabel;
        // there was an update
        private bool mChanged = false;
        // the component has started, so we do not get all the values spoken on start.
        private bool mInited = false;

        private void Awake()
        {
            if (Label == null)
            {
                Label = GetComponent<TextMesh>();
            }

            if (Label == null)
            {
                Debug.LogError("Textmesh:Label is not set in LabelSwapWidget!");
                Destroy(this);
            }

            mCachedLabel = Label.text;
            counter = Interval;
        }

        // Update is called once per frame
        void Update()
        {

            // the counter has expired
            if (counter >= Interval)
            {
                // there was a change
                if ((mCachedLabel != Label.text && mInited) || mChanged)
                {
                    // get the TextToSpeechManager in the parent
                    // TEMP mSpeechManager = GetComponentInParent<TextToSpeech>();

                    if (mSpeechManager == null)
                    {
                        // find the TextToSpeechManager somewhere else
                        // TEMP mSpeechManager = FindObjectOfType<TextToSpeech>();
                    }

                    if (mSpeechManager != null)
                    {
                        // should we speak?
                        bool speak;// TEMP = (!mSpeechManager.IsSpeaking() || Interrupt) && !mSpeechManager.AudioSource.mute;

                        // send text to the SpeechManager
                        if (mSpeechManager != null)// TEMP && speak)
                        {
                            if (ToolTip == "")
                            {
                                // TEMP mSpeechManager.StartSpeaking(Label.text);
                            }
                            else
                            {
                                // TEMP mSpeechManager.StartSpeaking(ToolTip + " " + Label.text);

                            }
                        }
                    }

                    counter = 0;
                    mChanged = false;
                }
            }
            else
            {
                // update the counter
                counter += Time.deltaTime;

                // record if a change has occured
                if (!mChanged)
                {
                    mChanged = mCachedLabel != Label.text;
                }
            }

            mCachedLabel = Label.text;
            mInited = true;
        }
    }
}
