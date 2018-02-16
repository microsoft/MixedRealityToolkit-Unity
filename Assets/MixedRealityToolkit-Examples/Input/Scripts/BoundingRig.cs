using System.Collections;
using System.Collections.Generic;
using MixedRealityToolkit.UX.BoundingBoxes;
using UnityEngine;

public class BoundingRig : MonoBehaviour {

    public GameObject ObjectToBound;

    private GameObject[] rotateXHandles;

    public BoundingBox Box;
	// Use this for initialization
	void Start () {
        rotateXHandles = new GameObject[4];

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        //scalars
        Vector3 currentScale = Box.gameObject.transform.localScale;
        Vector3 offset = currentScale;

        //create object
        SetUpHandles(rotateXHandles, PrimitiveType.Sphere);
   
        offset.Set(currentScale.x * 0.25f, currentScale.y * 0.25f, currentScale.z * 0.25f);
        rotateXHandles[0].transform.localPosition = Box.gameObject.transform.localPosition + currentScale;

        offset.Set(currentScale.x * 0.25f, currentScale.y * 0.25f, currentScale.z * -0.25f);
        rotateXHandles[1].transform.localPosition = Box.gameObject.transform.localPosition + currentScale;

        offset.Set(currentScale.x * -0.25f, currentScale.y * 0.25f, currentScale.z * 0.25f);
        rotateXHandles[2].transform.localPosition = Box.gameObject.transform.localPosition + currentScale;

        offset.Set(currentScale.x * -0.25f, currentScale.y * 0.25f, currentScale.z * -0.25f);
        rotateXHandles[3].transform.localPosition = Box.gameObject.transform.localPosition + currentScale;
    }

    public void Deactivate()
    {


    }

    private void SetUpHandles(GameObject[] handles, PrimitiveType type)
    {
        for (int i = 0; i < handles.Length; ++i)
        {
            //create
            handles[i] = GameObject.CreatePrimitive(type);

            //set material
            handles[i].GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1);
            handles[i].GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");

            //affines
            handles[i].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            handles[i].transform.localRotation = Box.gameObject.transform.localRotation;

            //parent
            handles[i].transform.parent = Box.gameObject.transform;
        }
    }
}
