using System;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine;

public class UAudioManagerTest : MonoBehaviour
{
	void Start ()
	{
	    StartCoroutine(ContinouslyPlaySounds());
	}

    private IEnumerator ContinouslyPlaySounds()
    {
        while (true)
        {
            UAudioManager.Instance.PlayEvent("Laser");

            yield return new WaitForSeconds(1.0f);

            UAudioManager.Instance.PlayEvent("Vocals");

            yield return new WaitForSeconds(10.0f);
        }
    }

}
