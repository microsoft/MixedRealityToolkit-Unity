using System;
using System.Collections;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This class provides wrapper functionality for triggering animations and fades for the hand rig.
    /// </summary>
    public class InteractionHint : MonoBehaviour
    {
        private GameObject VisualsRoot;

        /// <summary>
        /// React to hand tracking state to hide visuals when hands are being tracked.
        /// If false, only the customShouldHideHands function will be evaluated.
        /// </summary>
        public bool HideIfHandTracked = false;

        /// <summary>
        /// Min delay for showing the visuals.  If the user's hands are not in view, the visuals will appear after min seconds.
        /// </summary>
        public float MinDelay = 5f;

        /// <summary>
        /// Max delay for showing the visuals.  If the user's hands are in view, the min timer will reset to 0, but the visuals will appear after max seconds.
        /// <summary>
        public float MaxDelay = 10f;

        /// <summary>
        /// Set to false if you don't want to use a max timer and only want to show the hint when user's hands are not tracked.
        /// </summary>
        public bool UseMaxDelay = true;

        /// <summary>
        /// Number of times to repeat the hint before fading out and waiting for timer again.
        /// </summary>
        public int Repeats = 2;

        /// <summary>
        /// If true, logic runs whenever this component is active.  If false, you must manually start the logic with StartShowTimer.
        /// </summary>
        public bool AutoActivate = true;

        /// <summary>
        /// Name of animation to play during loop.
        /// </summary>
        public string AnimationState;

        /// <summary>
        /// Time to wait between repeats in seconds.
        /// </summary>
        public float RepeatDelay = 1f;

        private string fadeInAnimationState = "Fade_In";

        private string fadeOutAnimationState = "Fade_Out";

        private float animationHideTime = 3f;

        private float animationHideDuration = 0.5f;

        private IMixedRealityHandJointService HandJointService => handJointService ?? (handJointService = MixedRealityToolkit.Instance.GetService<IMixedRealityHandJointService>());

        private IMixedRealityHandJointService handJointService = null;

        /// <summary>
        /// Custom function to determine visibility of visuals.
        /// Return true to hide visuals and reset min timer (max timer will still be in effect), return false when user is doing nothing and needs a hint.
        /// </summary>
        public Func<bool> CustomShouldHideVisuals = delegate { return false; };

        private Animator m_animator;

        private bool m_animatingOut = false;

        private bool m_loopRunning = false;

        private void Awake()
        {
            if (VisualsRoot == null)
            {
                if (transform.childCount > 0)
                {
                    VisualsRoot = transform.GetChild(0).gameObject;
                }
                else
                {
                    Debug.LogError("Incorrect hand rig setup. Disabling gameObject");
                    gameObject.SetActive(false);
                }
            }

            // store the root's animator
            m_animator = VisualsRoot.GetComponent<Animator>();

            // hide visuals by default
            if (VisualsRoot != null)
            {
                VisualsRoot.SetActive(false);
            }
        }

        public void OnEnable()
        {
            // When component is enabled, start up the timer logic if auto activate is specified
            if (AutoActivate)
            {
                StartHintLoop();
            }
        }

        public void OnDisable()
        {
            // Stop all logic when the component is disabled, even if not using auto activate
            if (m_loopRunning)
            {
                StopAllCoroutines();
                m_loopRunning = false;
            }

            // Also turn off the visuals
            SetActive(VisualsRoot, false);
        }

        /// <summary>
        /// Starts the hint loop logic.
        /// </summary>
        public void StartHintLoop()
        {
            if (!m_loopRunning && VisualsRoot != null)
            {
                m_loopRunning = true;
                animationHideDuration = GetAnimationDuration(fadeOutAnimationState);
                animationHideTime = GetAnimationDuration(AnimationState) - animationHideDuration;
                if (animationHideTime < 0)
                {
                    animationHideTime = 0;
                }
                StartCoroutine(HintLoopSequence(AnimationState));
            }
        }

        /// <summary>
        /// Fades out the hint and stops the hint loop logic
        /// </summary>
        public void StopHintLoop()
        {
            if (m_loopRunning && !m_animatingOut)
            {
                StopAllCoroutines();
                StartCoroutine(FadeOutHint());
            }
            m_loopRunning = false;
        }

        /// <summary>
        /// Stops the hint with appropriate fade.
        /// </summary>
        private IEnumerator FadeOutHint()
        {
            m_animatingOut = true;
            if (animationHideDuration > 0)
            {
                // Tell the animator to play the animation
                if (m_animator != null)
                {
                    m_animator.Play(fadeOutAnimationState);
                }

                yield return new WaitForSeconds(animationHideDuration);
            }
            SetActive(VisualsRoot, false);
            m_animatingOut = false;
        }

        /// <summary>
        /// The main timer logic coroutine. Pass the min/max delay to use and the function to evaluate the desired state.
        /// </summary>
        private IEnumerator HintLoopSequence(string stateToPlay)
        {
            // loop until the gameObject has been turned off
            while (VisualsRoot != null && m_loopRunning)
            {
                // First wait for the min timer, resetting it whenever ShouldHide is true.  Also
                // wait for the max timer, never resetting it.
                float maxTimer = 0;
                float timer = 0;
                while (timer < MinDelay && maxTimer < MaxDelay)
                {
                    if (ShouldHideVisuals())
                    {
                        timer = 0;
                    }
                    else
                    {
                        timer += Time.deltaTime;
                    }

                    if (UseMaxDelay)
                    {
                        maxTimer += Time.deltaTime;
                    }

                    yield return null;
                }

                // show the root
                SetActive(VisualsRoot, true);
                m_animator.Play(stateToPlay);

                float visibleTime = Time.time;
                int playCount = 0;

                // loop as long as visuals are active and we haven't repeated the number of times desired
                while (VisualsRoot.activeSelf && playCount < Repeats)
                {
                    // hide if hand is present, but maxTimer was not hit
                    bool bShouldHide = ShouldHideVisuals();
                    bool bFadeOut = Time.time - visibleTime >= animationHideTime;
                    if (bShouldHide || bFadeOut)
                    {
                        // Yield while deactivate anim is playing (or instant deactivate if not animating)
                        yield return FadeOutHint();

                        // if fade out was caused by user interacting, we've reached the repeat limit, or we've stopped the loop, break out
                        if (bShouldHide || playCount == Repeats - 1 || !m_loopRunning)
                        {
                            break;
                        }
                        // If we autohid, then reappear if hands are not tracked
                        else
                        {
                            yield return new WaitForSeconds(RepeatDelay);
                            SetActive(VisualsRoot, true);
                            m_animator.Play(stateToPlay);
                            visibleTime = Time.time;
                            playCount++;
                        }
                    }
                    yield return null;
                }
            }
        }

        private void SetActive(GameObject root, bool bShow)
        {
            if (root != null)
            {
                root.SetActive(bShow);

                if (bShow)
                {
                    m_animator.Play(fadeInAnimationState);
                }
            }
        }

        /// <summary>
        /// Gets the duration of the animation name passed in, or 0 if the state name is not found.
        /// </summary>
        public float GetAnimationDuration(string animationStateName)
        {
            RuntimeAnimatorController ac = m_animator.runtimeAnimatorController;
            for (int i = 0; i < ac.animationClips.Length; i++)
            {
                if (ac.animationClips[i].name.StartsWith(animationStateName))
                {
                    return ac.animationClips[i].length;
                }
            }

            // the specified state is not found
            return 0;
        }

        /// <summary>
        /// Check if the user is making an attempt to proceed, according to the hint.
        /// Return true if the user is attempting to interact properly.  Visuals will hide until the max timer expires.
        /// Return false if the user is doing nothing.  Visuals will show according to the min timer.
        /// </summary>
        private bool ShouldHideVisuals()
        {
            // Check hand tracking, if configured to do so
            bool bShouldHide = HideIfHandTracked && IsHandTracked();

            // Check the custom show visuals function
            if (!bShouldHide)
            {
                bShouldHide |= CustomShouldHideVisuals();
            }
            return bShouldHide;
        }

        /// <summary>
        /// Return true if either of the user's hands are being tracked.
        /// Return false if neither of the user's hands are being tracked.
        /// </summary>
        bool IsHandTracked()
        {
            return HandJointService.IsHandTracked(Handedness.Right) || HandJointService.IsHandTracked(Handedness.Left);
        }
    }
}
