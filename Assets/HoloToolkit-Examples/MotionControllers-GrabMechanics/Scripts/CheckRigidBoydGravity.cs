using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRigidBoydGravity : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("ARE WE USING GRAVITY BEFORE " +GetComponent<Rigidbody>().useGravity);
       // GetComponent<Rigidbody>().useGravity = false;
        Debug.Log("ARE WE USING GRAVITY AFTER " + GetComponent<Rigidbody>().useGravity);


    }

    // Update is called once per frame
    void Update () {
        if (GetComponent<MRTK.Grabbables.ThrowableObject>().gravUpdate)
        {
            GetComponent<Rigidbody>().useGravity = false;
            Debug.Log("We are turning gravity off");
        }
	}
}
