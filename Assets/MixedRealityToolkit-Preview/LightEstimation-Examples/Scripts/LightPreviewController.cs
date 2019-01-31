// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR || WINDOWS_UWP
using UnityEngine.Windows.Speech;
using UnityEngine.XR.WSA.Input;
#endif

using Microsoft.MixedReality.Toolkit.Preview.LightEstimation;

namespace Microsoft.MixedReality.Toolkit.Preview.Examples.LightEstimation
{
	public class LightPreviewController : MonoBehaviour
	{
		#region Fields
		#pragma warning disable 414, 649
		[Header("Scene/asset hooks")]
		[SerializeField] private GameObject   spheres      = null;
		[SerializeField] private GameObject   shaderBalls  = null;
		[SerializeField] private GameObject   cubes        = null;
		[SerializeField] private GameObject[] spawnPrefabs = null;

		[Header("Movement Values")]
		[SerializeField] private float velocityDecay = 4f;
		[SerializeField] private float moveScale     = 1.5f;
		[SerializeField] private float maxVelocity   = 1;
		[SerializeField] private float lerpSpeed     = 4;
	
		private Vector3 targetPos;
		private Vector3 handPos;
		private bool    pressed;
		private int     addIndex;
		private Vector3 handVelocity;
		private Vector3 lastPos;
		private float   lastTime;

		private int exposure   = -7;
		private int whitebalance = 6000;

		private LightCapture lightCapture;
		private Component       tts;
		private MethodInfo      speakMethod;

		#if UNITY_EDITOR || WINDOWS_UWP
		private KeywordRecognizer keywordRecognizer;
		#endif
		#pragma warning restore 414, 649
		#endregion

		private void OnEnable()
		{
			#if UNITY_EDITOR || WINDOWS_UWP
			// Setup hand interaction events
			InteractionManager.InteractionSourceLost     += SourceLost;
			InteractionManager.InteractionSourcePressed  += SourcePressed;
			InteractionManager.InteractionSourceReleased += SourceReleased;
			InteractionManager.InteractionSourceUpdated  += SourceUpdated;

			// Setup voice control events
			keywordRecognizer = new KeywordRecognizer(new string[] { "circle", "rabbit", "cube", "reset", "clear", "enable", "disable", "add", "up", "down", "white up", "white down", "save" });
			keywordRecognizer.OnPhraseRecognized += HeardKeyword;
			keywordRecognizer.Start();
			#endif

			// Setup scene objects
			spheres    .SetActive(false);
			shaderBalls.SetActive(true);
			cubes      .SetActive(false);

			transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;
			transform.LookAt(Camera.main.transform.position);
			transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
			targetPos = transform.position;
		
			// Hook up resources
			lightCapture = FindObjectOfType<LightCapture>();

			// No hard dependence on the MRTK, just reflect into the parts we want to use
			Type textToSpeechType = Type.GetType("HoloToolkit.Unity.TextToSpeech");
			if (textToSpeechType != null)
			{
				tts = (Component)FindObjectOfType(textToSpeechType);
				speakMethod = textToSpeechType.GetMethod("StartSpeaking");
			}
		}
		private void OnDisable()
		{
			#if UNITY_EDITOR || WINDOWS_UWP
			// Remove hand event hooks
			InteractionManager.InteractionSourceLost     -= SourceLost;
			InteractionManager.InteractionSourcePressed  -= SourcePressed;
			InteractionManager.InteractionSourceReleased -= SourceReleased;
			InteractionManager.InteractionSourceUpdated  -= SourceUpdated;

			keywordRecognizer.Stop();
			keywordRecognizer = null;
			#endif
		}

		#if UNITY_EDITOR || WINDOWS_UWP
		private void SourceLost(InteractionSourceLostEventArgs obj)
		{
			pressed = false;
		}
		private void SourceReleased(InteractionSourceReleasedEventArgs obj)
		{
			pressed = false;
		}
		private void SourcePressed(InteractionSourcePressedEventArgs obj)
		{
			pressed  = true;

			handVelocity = Vector3.zero;
			lastPos      = handPos;
			lastTime     = Time.time;
		}
		private void SourceUpdated(InteractionSourceUpdatedEventArgs obj)
		{
			if (obj.state.source.kind != InteractionSourceKind.Hand)
			{
				return;
			}

			Vector3 pos;
			if (Time.time - lastTime > 0 && obj.state.sourcePose.TryGetPosition(out pos))
			{
				if (pressed)
				{
					handVelocity = (pos - lastPos) / (Time.time - lastTime);
					if (handVelocity.sqrMagnitude > maxVelocity*maxVelocity)
					{
						handVelocity = handVelocity.normalized* maxVelocity;
					}
				}

				lastPos  = handPos;
				lastTime = Time.time;
				handPos  = pos;
				
				if (pressed)
				{
					targetPos += (pos - lastPos) * moveScale;
				}
			}
		}
	
		private void HeardKeyword(PhraseRecognizedEventArgs args)
		{
			string reply = "ok";

			// Execute the command we just heard
			if (args.text == "circle")
			{
				spheres.SetActive(true);
				shaderBalls.SetActive(false);
				cubes.SetActive(false);
			}
			else if (args.text == "ball")
			{
				spheres.SetActive(false);
				shaderBalls.SetActive(true);
				cubes.SetActive(false);
			}
			else if (args.text == "cube")
			{
				spheres.SetActive(false);
				shaderBalls.SetActive(false);
				cubes.SetActive(true);
			}
			else if (args.text == "reset")
			{
				transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;
				transform.LookAt(Camera.main.transform.position);
				transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
				targetPos = transform.position;
			}
			else if (args.text == "clear")
			{
				lightCapture.Clear();
			}
			else if (args.text == "disable")
			{
				lightCapture.enabled = false;
			}
			else if (args.text == "enable")
			{
				lightCapture.enabled = true;
			}
			else if (args.text == "add")
			{
				RaycastHit hit;
				if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
				{
					Instantiate(spawnPrefabs[addIndex%spawnPrefabs.Length], hit.point, Quaternion.identity);
					addIndex++;
				}
			}
			else if (args.text == "up")
			{
				exposure += 1;
				lightCapture.SetExposure(exposure);
				reply = ""+exposure;
			}
			else if (args.text == "down")
			{
				exposure -= 1;
				lightCapture.SetExposure(exposure);
				reply = ""+exposure;
			}
			else if (args.text == "white up")
			{
				whitebalance += 250;
				lightCapture.SetWhitebalance(whitebalance);
				Debug.Log("WB: " + whitebalance);
				reply = ""+whitebalance;
			}
			else if (args.text == "white down")
			{
				whitebalance -= 250;
				lightCapture.SetWhitebalance(whitebalance);
				Debug.Log("WB: " + whitebalance);
				reply = ""+whitebalance;
			}
			else if (args.text == "save")
			{
				#if WINDOWS_UWP
				SaveMap();
				reply = "Saving environment map to photo roll!";
				#else
				reply = "Saving to photo roll not supported on this platform.";
				#endif
			}

			// Output results of the command
			Debug.Log("Heard command: " + args.text);
			if (tts != null)
			{
				speakMethod.Invoke(tts, new object[] { reply });
			}
		}
		#endif

		#if WINDOWS_UWP
		private async void SaveMap()
		{
			Texture2D tex     = CubeMapper.CreateCubemapTex(lightCapture.CubeMapper.Map);
			byte[]    texData = tex.EncodeToPNG();

			var picturesLibrary = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
			// Fall back to the local app storage if the Pictures Library is not available
			var captureFolder = picturesLibrary.SaveFolder ?? Windows.Storage.ApplicationData.Current.LocalFolder;
			var file = await captureFolder.CreateFileAsync("EnvironmentMap.png", Windows.Storage.CreationCollisionOption.GenerateUniqueName);
			await Windows.Storage.FileIO.WriteBytesAsync(file, texData);
		}
		#endif

		private void Update()
		{
			if (!pressed)
			{
				handVelocity = Vector3.Lerp(handVelocity, Vector3.zero, velocityDecay*Time.deltaTime);
				targetPos   += handVelocity * Time.deltaTime;
			}
			else
			{
				// Blend rotation towards the player
				Quaternion dest = Quaternion.LookRotation( Camera.main.transform.position-transform.position );
				Vector3    rot  = dest.eulerAngles;
				rot.x = rot.z = 0;
				dest = Quaternion.Euler(rot);

				transform.rotation = Quaternion.Slerp(transform.rotation, dest, 4 * Time.deltaTime);
			}
			transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);
		}
	}
}