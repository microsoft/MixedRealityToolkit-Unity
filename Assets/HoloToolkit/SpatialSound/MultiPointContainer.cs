using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity
{
	public class MultiPointContainer : MonoBehaviour
	{
		[SerializeField]
		private ContainerTypes containerType;
		[SerializeField]
		private float crossfadeTime;
		[SerializeField]
		private Emitter[] emitters;
		private bool playing = false;
		private bool playAlt = false;
		private int currentClip = 0;
		private int totalClips = 0;
		private float sourceOneVolDest = 0f;
		private float sourceTwoVolDest = 0f;
		private float sourceOneCurrentVol = 0f;
		private float sourceTwoCurrentVol = 0f;
		private float remainingFadeTime = 0f;

		/// <summary>
		/// An emitter contains an array of clips and two AudioSource components to crossfade between
		/// </summary>
		[System.Serializable]
		public class Emitter
		{
			public GameObject emitter;
			public UnityEngine.AudioClip[] sounds;
			public AudioSource sourceOne;
			public AudioSource sourceTwo;
		}

		/// <summary>
		/// Describes whether audioclips should be selected sequentially or randomly
		/// </summary>
		public enum ContainerTypes
		{
			Random,
			Sequence
		}

		private void Start()
		{
			//Get total number of clips that will be used, so we never choose a clip outside of the clip array in an emitter
			SetTotalClips();
			//get two AudioSource components ready for each emitter, setting them to be 3D Spatialized
			for (int i = 0; i < emitters.Length; i++)
			{
				this.emitters[i].sourceOne = GetUnusedAudioSource(this.emitters[i].emitter);
				this.emitters[i].sourceTwo = GetUnusedAudioSource(this.emitters[i].emitter, this.emitters[i].sourceOne);
				this.emitters[i].sourceOne.spatialBlend = 1f;
				this.emitters[i].sourceTwo.spatialBlend = 1f;
			}
		}

		private void Update()
		{
			if (this.playing)
			{
				//Calculate volumes for the AudioSources (fade in/out)
				if (this.remainingFadeTime > Time.deltaTime)
				{
					this.sourceOneCurrentVol += (this.sourceOneVolDest - this.sourceOneCurrentVol) * Time.deltaTime / this.remainingFadeTime;
					this.sourceTwoCurrentVol += (this.sourceTwoVolDest - this.sourceTwoCurrentVol) * Time.deltaTime / this.remainingFadeTime;
					this.remainingFadeTime -= Time.deltaTime;
				}
				else
				{
					this.sourceOneCurrentVol = this.sourceOneVolDest;
					this.sourceTwoCurrentVol = this.sourceTwoVolDest;
				}
				//Once current volume for sources is set, apply to each emitter, so all emitters are in sync
				for (int i = 0; i < this.emitters.Length; i++)
				{
					this.emitters[i].sourceOne.volume = this.sourceOneCurrentVol;
					this.emitters[i].sourceTwo.volume = this.sourceTwoCurrentVol;
				}
			}
		}

		//Find the number of clips that will be played back on each emitter, using the smallest count if emitter have different numbers of clips
		private void SetTotalClips()
		{
			this.totalClips = 0;
			bool inconsistentClips = false;
			for (int i = 0; i < this.emitters.Length; i++)
			{
				if (i == 0)
				{
					this.totalClips = this.emitters[i].sounds.Length;
				}
				else
				{
					if (this.emitters[i].sounds.Length != this.totalClips)
					{
						inconsistentClips = true;
						if (this.emitters[i].sounds.Length < this.totalClips)
						{
							this.totalClips = this.emitters[i].sounds.Length;
						}
					}
				}
			}

			if (inconsistentClips)
			{
				Debug.LogWarning("Inconsistent number of clips in multipoint emitter: " + gameObject.name, this);
			}
		}

		//Return an unused AudioSource component, creating one if no free source is found
		private AudioSource GetUnusedAudioSource(GameObject emitter, AudioSource currentSource = null)
		{
			AudioSource[] sources = emitter.GetComponents<AudioSource>();
			for (int s = 0; s < sources.Length; s++)
			{
				if (!sources[s].isPlaying)
				{
					if (currentSource == null)
					{
						return sources[s];
					}
					else if (sources[s] != currentSource)
					{
						return sources[s];
					}
				}
			}
			return emitter.AddComponent<AudioSource>();
		}

		/// <summary>
		/// Play all emitters in emitter array
		/// </summary>
		public void Play()
		{
			if (this.playing)
			{
				Debug.LogWarning("Continuous audio container is already playing.", this);
				return;
			}
			this.playing = true;
			SetNextClip(true);
			float waitTime = 0f;
			for (int i = 0; i < this.emitters.Length; i++)
			{
				waitTime = PlayEmitterAndGetTime(this.emitters[i]);
			}
			this.playAlt = !this.playAlt;
			waitTime -= this.crossfadeTime;
            StartCoroutine(ContinueContainerCoroutine(waitTime));
			//CoroutineEx.Run(ContinueContainerCoroutine(waitTime), this.cancelSource.Token);
		}

		//Assign the next clip to play based on container type (random or sequence)
		private void SetNextClip(bool firstClip = false)
		{
			if (this.containerType == ContainerTypes.Sequence)
			{
				if (firstClip)
				{
					this.currentClip = 0;
				}
				else
				{
					this.currentClip++;
					if (this.currentClip >= this.totalClips)
					{
						this.currentClip = 0;
					}
				}
			}
			else
			{
				this.currentClip = Random.Range(0, this.totalClips);
			}
		}

		//Play a single emitter, returning the time it will be active (sound clip length)
		private float PlayEmitterAndGetTime(Emitter emitter)
		{
			if (this.playAlt)
			{
                emitter.sourceTwo.PlayClip(emitter.sounds[this.currentClip]);
				this.sourceOneVolDest = 0f;
				this.sourceTwoVolDest = 1f;
				this.remainingFadeTime = this.crossfadeTime;
			}
			else
			{
                emitter.sourceOne.PlayClip(emitter.sounds[this.currentClip]);
				this.sourceOneVolDest = 1f;
				this.sourceTwoVolDest = 0f;
				this.remainingFadeTime = this.crossfadeTime;
			}
			return emitter.sounds[this.currentClip].length;
		}

		//Continuously choose new clips to play when current clip ends, and crossfade into new clip
		private IEnumerator ContinueContainerCoroutine(float waitTime)
		{
			while (this.playing)
			{
                yield return new WaitForSeconds(waitTime);
				SetNextClip();
				for (int i = 0; i < this.emitters.Length; i++)
				{
					waitTime = PlayEmitterAndGetTime(this.emitters[i]);
				}
				this.playAlt = !this.playAlt;
				waitTime -= this.crossfadeTime;
			}
		}

		/// <summary>
		/// Stop all emitters in emitter array
		/// </summary>
		public void Stop()
		{
			this.playing = false;
            //TODO: Stop event's coroutine - ie. StopCoroutine(activeEvent);
			for (int i = 0; i < this.emitters.Length; i++)
			{
				this.emitters[i].sourceOne.Stop();
				this.emitters[i].sourceTwo.Stop();
			}
		}
	}
}