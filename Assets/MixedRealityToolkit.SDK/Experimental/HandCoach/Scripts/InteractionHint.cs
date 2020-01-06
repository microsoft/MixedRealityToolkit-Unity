using System;
using System.Collections;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class InteractionHint : MonoBehaviour
    {
        private GameObject VisualsRoot;

        [Tooltip("React to hand tracking state to hide visuals when hands are being tracked.  If false, only the customShouldHideHands function will be evaluated.")]
        public bool checkHandTracking = true;

        [Tooltip("Min/max delay for showing the visuals.  If the user's hands are not in view, the visuals will appear after min seconds." +
            "  If the user's hands are in view, the min timer will reset to 0, but the visuals will appear after max seconds")]
        public Vector2 MinMaxDelay = new Vector2(5f, 10f);

        [Tooltip("Uncheck this if you don't want to use max timer, only show hint when hands are out of view")]
        public bool useMaxTimer = true;

        [Tooltip("Number of times to repeat the hint before fading out")]
        public int repeats = 2;

        [Tooltip("If true, logic runs whenever this component is active.  If false, you must manually start the logic with StartShowTimer.")]
        public bool autoActivate = true;

        [Tooltip("Name of animation to play during loop")]
        public string animationState;

        [Tooltip("Time to wait between repeats")]
        [SerializeField]
        public float repeatDelay = 1f;

        private string fadeInAnimationState = "Fade_In";

        private string fadeOutAnimationState = "Fade_Out";

        private float animationHideTime = 3f;

        private float animationHideDuration = 0.5f;

        private IMixedRealityHandJointService HandJointService => handJointService ?? (handJointService = MixedRealityToolkit.Instance.GetService<IMixedRealityHandJointService>());

        private IMixedRealityHandJointService handJointService = null;

        // Custom function to determine visibility of visuals.  Return true to hide visuals and reset min timer (max timer will still be in effect), return false when user is doing nothing and needs a hint.
        public Func<bool> customShouldHideVisuals = delegate { return false; };

        private Animator m_animator;

        private bool m_animatingOut = false;

        private bool m_loopRunning = false;

        private void Awake()
        {
            if (VisualsRoot == null && transform.childCount > 0)
            {
                VisualsRoot = transform.GetChild(0).gameObject;
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
            if (autoActivate)
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

        // Starts the hint loop logic
        public void StartHintLoop()
        {
            if (!m_loopRunning && VisualsRoot != null)
            {
                m_loopRunning = true;
                animationHideDuration = GetAnimationDuration(fadeOutAnimationState);
                animationHideTime = GetAnimationDuration(animationState) - animationHideDuration;
                if (animationHideTime < 0)
                {
                    animationHideTime = 0;
                }
                StartCoroutine(HintLoopSequence(animationState));
            }
        }

        // Fades out the hint and stops the hint loop logic
        public void StopHintLoop()
        {
            if (m_loopRunning && !m_animatingOut)
            {
                StopAllCoroutines();
                StartCoroutine(FadeOutHint());
            }
            m_loopRunning = false;
        }

        // stops the hint with appropriate fade
        IEnumerator FadeOutHint()
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

        // The main timer logic coroutine.  Pass the min/max delay to use and the function to evaluate the desired state
        IEnumerator HintLoopSequence(string stateToPlay)
        {
            // loop until the gameObject has been turned off
            while (VisualsRoot != null && m_loopRunning)
            {
                // First wait for the min timer, resetting it whenever ShouldHide is true.  Also
                // wait for the max timer, never resetting it.
                float maxTimer = 0;
                float timer = 0;
                while (timer < MinMaxDelay.x && maxTimer < MinMaxDelay.y)
                {
                    if (ShouldHideVisuals())
                    {
                        timer = 0;
                    }
                    else
                    {
                        timer += Time.deltaTime;
                    }

                    if (useMaxTimer)
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
                while (VisualsRoot.activeSelf && playCount < repeats)
                {
                    // hide if hand is present, but maxTimer was not hit
                    bool bShouldHide = ShouldHideVisuals();
                    bool bFadeOut = Time.time - visibleTime >= animationHideTime;
                    if (bShouldHide || bFadeOut)
                    {
                        // Yield while deactivate anim is playing (or instant deactivate if not animating)
                        yield return FadeOutHint();

                        // if fade out was caused by user interacting, we've reached the repeat limit, or we've stopped the loop, break out
                        if (bShouldHide || playCount == repeats - 1 || !m_loopRunning)
                        {
                            break;
                        }
                        // If we autohid, then reappear if hands are not tracked
                        else
                        {
                            yield return new WaitForSeconds(repeatDelay);
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

        void SetActive(GameObject root, bool bShow)
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

        // Check if the user is making an attempt to proceed, according to the hint.
        // Return true if the user is attempting to interact properly.  Visuals will hide until the max timer expires.
        // Return false if the user is doing nothing.  Visuals will show according to the min timer.
        private bool ShouldHideVisuals()
        {
            // Check hand trtacking, if configured to do so
            bool bShouldHide = checkHandTracking && IsHandTracked();

            // Check the custom show visuals function
            if (!bShouldHide)
            {
                bShouldHide |= customShouldHideVisuals();
            }
            return bShouldHide;
        }

        // Check if any hands are being tracked
        bool IsHandTracked()
        {
            return false; // return HandJointService.IsHandTracked(Handedness.Right) || HandJointService.IsHandTracked(Handedness.Left);
        }
    }
}
