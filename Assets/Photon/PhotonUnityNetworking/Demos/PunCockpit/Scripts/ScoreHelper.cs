using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class ScoreHelper : MonoBehaviour {


	public int Score;

	int _currentScore;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	

		if (PhotonNetwork.LocalPlayer !=null && Score != _currentScore)
		{
			_currentScore = Score;
			PhotonNetwork.LocalPlayer.SetScore(Score);
		}

	}
}
