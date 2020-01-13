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
    public class HandInteractionHint : MonoBehaviour
    {
        public GameObject VisualsRoot { get; set; }

        [Tooltip("React to hand tracking state to hide visuals when hands are being tracked. If false, only the customShouldHideHands function will be evaluated.")]
        [SerializeField]
        private bool hideIfHandTracked = false;

        /// <summary>
        /// React to hand tracking state to hide visuals when hands are being tracked.
        /// If false, only the customShouldHideHands function will be evaluated.
        /// </summary>
        public bool HideIfHandTracked
        {
            get
            {
                return hideIfHandTracked;
            }
            set
            {
                hideIfHandTracked = value;
            }
        }

        [Tooltip("Min delay for showing the visuals.  If the user's hands are not in view, the visuals will appear after min seconds.")]
        [SerializeField]
        private float minDelay = 5f;

        /// <summary>
        /// Min delay for showing the visuals.  If the user's hands are not in view, the visuals will appear after min seconds.
        /// </summary>
        public float MinDelay
        {
            get
            {
                return minDelay;
            }
            set
            {
                minDelay = value;
            }
        }

        [Tooltip("Max delay for showing the visuals.  If the user's hands are in view, the min timer will reset to 0, but the visuals will appear after max seconds.")]
        [SerializeField]
        private float maxDelay = 10f;

        /// <summary>
        /// Max delay for showing the visuals.  If the user's hands are in view, the min timer will reset to 0, but the visuals will appear after max seconds.
        /// </summary>
        public float MaxDelay
        {
            get
            {
                return maxDelay;
            }
            set
            {
                maxDelay = value;
            }
        }

        [Tooltip("Set to false if you don't want to use a max timer and only want to show the hint when user's hands are not tracked.")]
        [SerializeField]
        private bool useMaxDelay = true;

        /// <summary>
        /// Set to false if you don't want to use a max timer and only want to show the hint when user's hands are not tracked.
        /// </summary>
        public bool UseMaxDelay
        {
            get
            {
                return useMaxDelay;
            }
            set
            {
                useMaxDelay = value;
            }
        }

        [Tooltip("Number of times to repeat the hint before fading out and waiting for timer again.")]
        [SerializeField]
        private int repeats = 2;

        /// <summary>
        /// Number of times to repeat the hint before fading out and waiting for timer again.
        /// </summary>
        public int Repeats
        {
            get
            {
                return repeats;
            }
            set
            {
                repeats = value;
            }
        }

        [Tooltip("If true, logic runs whenever this component is active.  If false, you must manually start the logic with StartShowTimer.")]
        [SerializeField]
        private bool autoActivate = true;

        /// <summary>
        /// If true, logic runs whenever this component is active.  If false, you must manually start the logic with StartShowTimer.
        /// </summary>
        public bool AutoActivate
        {
            get
            {
                return autoActivate;
            }
            set
            {
                autoActivate = value;
            }
        }

        [Tooltip("Name of animation to play during loop.")]
        [SerializeField]
        private string animationState = "";

        /// <summary>
        /// Name of animation to play during loop.
        /// </summary>
        public string AnimationState
        {
            get
            {
                return animationState;
            }
            set
            {
                animationState = value;
            }
        }

        [Tooltip("Time to wait between repeats in seconds.")]
        [SerializeField]
        private float repeatDelay = 1f;

        /// <summary>
        /// Time to wait between repeats in seconds.
        /// </summary>
        public float RepeatDelay
        {
            get
            {
                return repeatDelay;
            }
            set
            {
                repeatDelay = value;
            }
        }

        private string fadeInAnimationState = "Fade_In";

        private string fadeOutAnimationState = "Fade_Out";

        private float animationHideTime = 3f;

        private float animationHideDuration = 0.5f;

        /// <summary>
        /// Custom function to determine visibility of visuals.
        /// Return true to hide visuals and reset min timer (max timer will still be in effect), return false when user is doing nothing and needs a hint.
        /// </summary>
        public Func<bool> CustomShouldHideVisuals = delegate { return false; };

        private Animator animator;

        private bool animatingOut = false;

        private bool loopRunning = false;

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
            animator = VisualsRoot.GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError("Hand rig does not have an animator. Disabling gameObject");
                gameObject.SetActive(false);
            }

            // hide visuals by default
            if (VisualsRoot != null)
            {
                VisualsRoot.SetActive(false);
            }
        }

        private void OnEnable()
        {
            // When component is enabled, start up the timer logic if auto activate is specified
            if (AutoActivate)
            {
                StartHintLoop();
            }
        }

        private void OnDisable()
        {
            // Stop all logic when the component is disabled, even if not using auto activate
            if (loopRunning)
            {
                StopAllCoroutines();
                loopRunning = false;
            }

            // Also turn off the visuals
            SetActive(VisualsRoot, false);
        }

        /// <summary>
        /// Starts the hint loop logic.
        /// </summary>
        public void StartHintLoop()
        {
            if (!loopRunning && VisualsRoot != null)
            {
                loopRunning = true;
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
            if (loopRunning && !animatingOut)
            {
                StopAllCoroutines();
                StartCoroutine(FadeOutHint());
            }
            loopRunning = false;
        }

        /// <summary>
        /// Stops the hint with appropriate fade.
        /// </summary>
        private IEnumerator FadeOutHint()
        {
            animatingOut = true;
            if (animationHideDuration > 0)
            {
                // Tell the animator to play the animation
                if (animator != null)
                {
                    animator.Play(fadeOutAnimationState);
                }

                yield return new WaitForSeconds(animationHideDuration);
            }
            SetActive(VisualsRoot, false);
            animatingOut = false;
        }

        /// <summary>
        /// The main timer logic coroutine. Pass the min/max delay to use and the function to evaluate the desired state.
        /// </summary>
        private IEnumerator HintLoopSequence(string stateToPlay)
        {
            // loop until the gameObject has been turned off
            while (VisualsRoot != null && loopRunning)
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
                if (animator != null)
                {
                    animator.Play(stateToPlay);
                }

                float visibleTime = Time.time;
                int playCount = 0;

                // loop as long as visuals are active and we haven't repeated the number of times desired
                while (VisualsRoot.activeSelf && playCount < Repeats)
                {
                    // hide if hand is present, but maxTimer was not hit
                    bool shouldHide = ShouldHideVisuals();
                    bool fadeOut = Time.time - visibleTime >= animationHideTime;
                    if (shouldHide || fadeOut)
                    {
                        // Yield while deactivate anim is playing (or instant deactivate if not animating)
                        yield return FadeOutHint();

                        // if fade out was caused by user interacting, we've reached the repeat limit, or we've stopped the loop, break out
                        if (shouldHide || playCount == Repeats - 1 || !loopRunning)
                        {
                            break;
                        }
                        // If we autohid, then reappear if hands are not tracked
                        else
                        {
                            yield return new WaitForSeconds(RepeatDelay);
                            SetActive(VisualsRoot, true);
                            if (animator != null)
                            {
                                animator.Play(stateToPlay);
                            }
                            visibleTime = Time.time;
                            playCount++;
                        }
                    }
                    yield return null;
                }
            }
        }

        private void SetActive(GameObject root, bool show)
        {
            if (root != null)
            {
                root.SetActive(show);

                if (show && animator != null)
                {
                    animator.Play(fadeInAnimationState);
                }
            }
        }

        /// <summary>
        /// Gets the duration of the animation name passed in, or 0 if the state name is not found.
        /// </summary>
        public float GetAnimationDuration(string animationStateName)
        {
            if (animator != null)
            {
                RuntimeAnimatorController ac = animator.runtimeAnimatorController;
                for (int i = 0; i < ac.animationClips.Length; i++)
                {
                    if (ac.animationClips[i].name.StartsWith(animationStateName))
                    {
                        return ac.animationClips[i].length;
                    }
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
            bool shouldHide = HideIfHandTracked && IsHandTracked();

            // Check the custom show visuals function
            if (!shouldHide)
            {
                shouldHide |= CustomShouldHideVisuals();
            }
            return shouldHide;
        }

        /// <summary>
        /// Return true if either of the user's hands are being tracked.
        /// Return false if neither of the user's hands are being tracked.
        /// </summary>
        private bool IsHandTracked()
        {
            return HandJointUtils.FindHand(Handedness.Right) != null || HandJointUtils.FindHand(Handedness.Left) != null;
        }
    }
}
