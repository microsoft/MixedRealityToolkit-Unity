using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR || WINDOWS_UWP
using UnityEngine.Windows.Speech;
using UnityEngine.XR.WSA.Input;
#endif

public class LightPreviewController : MonoBehaviour {

	#region Fields
	#pragma warning disable 414, 649
	[Header("Scene/asset hooks")]
	[SerializeField] GameObject   _spheres = null;
	[SerializeField] GameObject   _rabbits = null;
	[SerializeField] GameObject   _cubes = null;
	[SerializeField] GameObject[] _spawnPrefabs = null;

	[Header("Movement Values")]
	[SerializeField] float _velocityDecay = 4f;
	[SerializeField] float _moveScale     = 1.5f;
	[SerializeField] float _maxVelocity   = 1;
	[SerializeField] float _lerpSpeed     = 4;
	
	Vector3 _targetPos;
	Vector3 _handPos;
	bool    _pressed;
	int     _addIndex;
	Vector3 _handVelocity;
	Vector3 _lastPos;
	float   _lastTime;

	int _exposure   = -7;
	int _whitebalance = 6000;

	LightingCapture _light;
	Component       _tts;
	MethodInfo      _speakMethod;

	#if UNITY_EDITOR || WINDOWS_UWP
	KeywordRecognizer _keywordRecognizer;
	#endif
	#pragma warning restore 414, 649
	#endregion

	private void OnEnable() {
		#if UNITY_EDITOR || WINDOWS_UWP
		// Setup hand interaction events
		InteractionManager.InteractionSourceLost     += SourceLost;
		InteractionManager.InteractionSourcePressed  += SourcePressed;
		InteractionManager.InteractionSourceReleased += SourceReleased;
		InteractionManager.InteractionSourceUpdated  += SourceUpdated;

		// Setup voice control events
		_keywordRecognizer = new KeywordRecognizer(new string[] { "circle", "rabbit", "cube", "reset", "clear", "enable", "disable", "add", "up", "down", "white up", "white down", "save" });
		_keywordRecognizer.OnPhraseRecognized += HeardKeyword;
		_keywordRecognizer.Start();
		#endif

		// Setup scene objects
		_spheres.SetActive(false);
		_rabbits.SetActive(true);
		_cubes  .SetActive(false);

		transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;
		transform.LookAt(Camera.main.transform.position);
		transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
		_targetPos = transform.position;
		
		// Hook up resources
		_light = FindObjectOfType<LightingCapture>();

		// No hard dependence on the MRTK, just reflect into the parts we want to use
		Type textToSpeechType = Type.GetType("HoloToolkit.Unity.TextToSpeech");
		if (textToSpeechType != null) {
			_tts = (Component)FindObjectOfType(textToSpeechType);
			_speakMethod = textToSpeechType.GetMethod("StartSpeaking");
		}
	}
	private void OnDisable() {
		#if UNITY_EDITOR || WINDOWS_UWP
		// Remove hand event hooks
		InteractionManager.InteractionSourceLost     -= SourceLost;
		InteractionManager.InteractionSourcePressed  -= SourcePressed;
		InteractionManager.InteractionSourceReleased -= SourceReleased;
		InteractionManager.InteractionSourceUpdated  -= SourceUpdated;

		_keywordRecognizer.Stop();
		_keywordRecognizer = null;
		#endif
	}

	#if UNITY_EDITOR || WINDOWS_UWP
	private void SourceLost(InteractionSourceLostEventArgs obj) {
		_pressed = false;
	}
	private void SourceReleased(InteractionSourceReleasedEventArgs obj) {
		_pressed = false;
	}
	private void SourcePressed(InteractionSourcePressedEventArgs obj) {
		_pressed  = true;

		_handVelocity = Vector3.zero;
		_lastPos      = _handPos;
		_lastTime     = Time.time;
	}
	private void SourceUpdated(InteractionSourceUpdatedEventArgs obj){
		if (obj.state.source.kind != InteractionSourceKind.Hand)
			return;

		Vector3 pos;
		if (Time.time - _lastTime > 0 && obj.state.sourcePose.TryGetPosition(out pos)) {
			if (_pressed) {
				_handVelocity = (pos - _lastPos) / (Time.time - _lastTime);
				if (_handVelocity.sqrMagnitude > _maxVelocity*_maxVelocity)
					_handVelocity = _handVelocity.normalized* _maxVelocity;
			}

			_lastPos  = _handPos;
			_lastTime = Time.time;
			_handPos  = pos;
				
			if (_pressed) {
				_targetPos += (pos - _lastPos) * _moveScale;
			}
		}
	}
	
	private void HeardKeyword(PhraseRecognizedEventArgs args) {
		string reply = "ok";

		// Execute the command we just heard
		if (args.text == "circle") {
			_spheres.SetActive(true);
			_rabbits.SetActive(false);
			_cubes.SetActive(false);
		} else if (args.text == "rabbit") {
			_spheres.SetActive(false);
			_rabbits.SetActive(true);
			_cubes.SetActive(false);
		} else if (args.text == "cube") {
			_spheres.SetActive(false);
			_rabbits.SetActive(false);
			_cubes.SetActive(true);
		} else if (args.text == "reset") {
			transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;
			transform.LookAt(Camera.main.transform.position);
			transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
			_targetPos = transform.position;
		} else if (args.text == "clear") {
			_light.Clear();
		} else if (args.text == "disable") {
			_light.enabled = false;
		} else if (args.text == "enable") {
			_light.enabled = true;
		} else if (args.text == "add") {
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit)) {
				Instantiate(_spawnPrefabs[_addIndex%_spawnPrefabs.Length], hit.point, Quaternion.identity);
				_addIndex++;
			}
		} else if (args.text == "up") {
			_exposure += 1;
			_light.SetExposure(_exposure);
			reply = ""+_exposure;
		} else if (args.text == "down") {
			_exposure -= 1;
			_light.SetExposure(_exposure);
			reply = ""+_exposure;
		} else if (args.text == "white up") {
			_whitebalance += 250;
			_light.SetWhitebalance(_whitebalance);
			Debug.Log("WB: " + _whitebalance);
			reply = ""+_whitebalance;
		} else if (args.text == "white down") {
			_whitebalance -= 250;
			_light.SetWhitebalance(_whitebalance);
			Debug.Log("WB: " + _whitebalance);
			reply = ""+_whitebalance;
		} else if (args.text == "save") {
			#if WINDOWS_UWP
			SaveMap();
			reply = "Saving environment map to photo roll!";
			#else
			reply = "Saving to photo roll not supported on this platform.";
			#endif
		}

		// Output results of the command
		Debug.Log("Heard command: " + args.text);
		if (_tts != null) {
			_speakMethod.Invoke(_tts, new object[] { reply });
		}
	}
	#endif

	#if WINDOWS_UWP
	private async void SaveMap() {
		Texture2D tex     = CubeMapper.CreateCubemapTex(_light.CubeMapper.Map);
		byte[]    texData = tex.EncodeToPNG();

		var picturesLibrary = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
        // Fall back to the local app storage if the Pictures Library is not available
        var captureFolder = picturesLibrary.SaveFolder ?? Windows.Storage.ApplicationData.Current.LocalFolder;
		var file = await captureFolder.CreateFileAsync("EnvironmentMap.png", Windows.Storage.CreationCollisionOption.GenerateUniqueName);
		await Windows.Storage.FileIO.WriteBytesAsync(file, texData);
	}
	#endif

	private void Update() {
		if (!_pressed) {
			_handVelocity = Vector3.Lerp(_handVelocity, Vector3.zero, _velocityDecay*Time.deltaTime);
			_targetPos   += _handVelocity * Time.deltaTime;
		} else {
			// Blend rotation towards the player
			Quaternion dest = Quaternion.LookRotation( Camera.main.transform.position );
			Vector3    rot  = dest.eulerAngles;
			rot.x = rot.z = 0;
			dest = Quaternion.Euler(rot);

			transform.rotation = Quaternion.Slerp(transform.rotation, dest, 4 * Time.deltaTime);
		}

		transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * _lerpSpeed);
	}
}
