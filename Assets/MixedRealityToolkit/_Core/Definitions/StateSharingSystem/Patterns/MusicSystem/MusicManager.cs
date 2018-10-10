using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Initialization;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Patterns.MusicSystem
{
    public class MusicManager : MonoBehaviour, IMusicManager
    {
        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource music1;
        [SerializeField]
        private AudioSource music2;
        [SerializeField]
        private AudioSource ambient;

        [Header("Music settings")]
        [SerializeField]
        private float masterVolume = 1f;
        [SerializeField]
        private float musicCrossFadeSpeed = 2.5f;
        [SerializeField]
        private float updateMusicInterval = 0.5f;

        private AudioSource fadingInAudio;
        private AudioSource fadingOutAudio;
        private bool updatingMusicFade = false;
        private bool updatingAmbientFade = false;
        private bool musicStarted = false;

        private AudioClip currentMusic;

        private ISessionManager session;

        public float MasterVolume
        {
            get { return masterVolume; }
            set
            {
                masterVolume = value;
                if (music1.volume != 0f)
                {
                    music1.volume = masterVolume;
                }
                if (music2.volume != 0f)
                {
                    music2.volume = masterVolume;
                }
            }
        }

        public AudioSource ActiveMusic
        {
            get
            {
                if (music1.clip != null && music1.isPlaying && music1.volume > 0f)
                {
                    return music1;
                }
                else if (music2.clip != null && music2.isPlaying)
                {
                    return music2;
                }
                return null;
            }
        }

        private void OnEnable()
        {
            fadingInAudio = null;
            fadingOutAudio = null;

            music1.loop = true;
            music1.spatialBlend = 0f;
            music2.loop = true;
            music2.spatialBlend = 0f;
            ambient.loop = true;
            ambient.spatialBlend = 0f;
        }

        private void PlayTransition(AudioClip transitionClip, float oneShotVolume)
        {
            if (transitionClip == null)
                return;

            oneShotVolume *= masterVolume;

            ambient.PlayOneShot(transitionClip, oneShotVolume);
        }

        private IEnumerator PlayAmbient(AudioClip newAmbient, float ambientVolume)
        {
            while (updatingAmbientFade)
            {
                yield return null;
            }

            updatingAmbientFade = true;

            if (ambient.clip == newAmbient)
            {
                // No need to do anything
                yield break;
            }

            ambientVolume *= masterVolume;

            if (ambient.clip != null)
            {
                while (ambient.volume > 0.001f)
                {
                    ambient.volume = Mathf.Lerp(ambient.volume, 0f, Time.deltaTime * musicCrossFadeSpeed);
                    yield return null;
                }
            }

            ambient.volume = 0f;
            ambient.Stop();
            ambient.clip = newAmbient;
            ambient.Play();

            while (ambient.volume < ambientVolume * 0.99f)
            {
                ambient.volume = Mathf.Lerp(ambient.volume, ambientVolume, Time.deltaTime * musicCrossFadeSpeed);
                yield return null;
            }

            ambient.volume = ambientVolume;
            updatingAmbientFade = false;

            yield break;
        }

        private IEnumerator PlayMusic(AudioClip newMusic, float musicVolume)
        {
            currentMusic = newMusic;

            while (updatingMusicFade)
            {
                yield return null;
            }

            //start updating!
            updatingMusicFade = true;

            musicVolume *= masterVolume;

            //this is the audio source we're fading out
            fadingOutAudio = ActiveMusic;
            if (fadingOutAudio == music2)
            {
                fadingInAudio = music1;
            }
            else
            {
                fadingInAudio = music2;
            }
            yield return null;

            //reset fade in audio
            fadingInAudio.volume = 0f;
            //if it has a clip remove it
            if (fadingInAudio.clip != null && fadingInAudio.clip != newMusic)
            {
                fadingInAudio.Stop();
                fadingInAudio.clip = null;
            }

            if (fadingInAudio.clip == null)
            {
                fadingInAudio.clip = newMusic;
            }
            if (!fadingInAudio.isPlaying)
            {
                fadingInAudio.Play();
            }

            bool doneFadingIn = false;
            bool doneFadingOut = false;
            float fadeInTargetAudioVolume = musicVolume;
            while (!(doneFadingIn && doneFadingOut))
            {
                //fade in new audio over time
                fadingInAudio.volume = Mathf.Clamp01(Mathf.Lerp(fadingInAudio.volume, fadeInTargetAudioVolume, Time.unscaledDeltaTime * musicCrossFadeSpeed));
                doneFadingIn = Mathf.Abs(fadingInAudio.volume - fadeInTargetAudioVolume) < 0.01f;
                //fade out existing audio over time
                if (fadingOutAudio != null)
                {
                    fadingOutAudio.volume = Mathf.Clamp01(Mathf.Lerp(fadingOutAudio.volume, 0f, Time.unscaledDeltaTime * musicCrossFadeSpeed));
                    doneFadingOut = (fadingOutAudio.volume < 0.01f);
                }
                else
                {
                    doneFadingOut = true;
                }
                yield return null;
            }
            //before we leave set everything one last time
            //and destroy the fading out clip
            fadingInAudio.volume = fadeInTargetAudioVolume;
            if (fadingOutAudio != null)
            {
                fadingOutAudio.volume = 0f;
            }
            fadingOutAudio = null;
            updatingMusicFade = false;
            yield break;
        }

        #region ISharingAppObject implementation

        public void OnSessionStart()
        {
            SceneScraper.FindInScenes<ISessionManager>(out session);
        }

        public void OnSessionUpdate(SessionState sessionState) { }

        public void OnSessionStageBegin()
        {
            PlayTransition(session.CurrentStage.OneShotClip, session.CurrentStage.OneShotVolume);
            StartCoroutine(PlayMusic(session.CurrentStage.MusicClip, session.CurrentStage.MusicVolume));
            StartCoroutine(PlayAmbient(session.CurrentStage.AmbientClip, session.CurrentStage.AmbientVolume));
        }

        public void OnSessionStageEnd() { }

        public void OnSessionEnd() { }

        #endregion
    }
}