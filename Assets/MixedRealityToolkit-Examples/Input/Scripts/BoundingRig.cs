using System.Collections;
using System.Collections.Generic;
using MixedRealityToolkit.UX.BoundingBoxes;
using UnityEngine;

public class BoundingRig : MonoBehaviour {

    public GameObject ObjectToBound;
    public BoundingBox Box;

    private GameObject[] rotateHandles;
    private GameObject[] cornerHandles;
    private List<Vector3> handleCentroids;
    private bool isActive = false;
    private System.Int64 updateCount = 0;

    private GameObject transformRig;


    // Use this for initialization
    void Start () {
    }
	// Update is called once per frame
	void Update () {
        if (isActive == true)
        {
            if (updateCount == 2)
            {
                CreateHandles();
            }
            else if (updateCount > 2)
            {
                //UpdateHandles();
            }
            updateCount++;
        }
	}

    public void Activate()
    {
        isActive = true;
        updateCount = 1;
    }
    public void Deactivate()
    {
        updateCount = 0;
        isActive = false;
        ClearHandles();
    }

    private void CreateHandles()
    {
        ClearHandles();

        CreateCornerPositions();
        CreateCornerHandles();
        CreateRotateHandles();
        ParentHandles();
    }
    private void CreateCornerHandles()
    {
        cornerHandles = new GameObject[handleCentroids.Count];
        for (int i = 0; i < handleCentroids.Count; ++i)
        {
            cornerHandles[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cornerHandles[i].GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1);
            cornerHandles[i].GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
            cornerHandles[i].transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            cornerHandles[i].transform.localPosition = handleCentroids[i];
        }
    }
    private void CreateRotateHandles()
    {
        rotateHandles = new GameObject[4];

        for (int i = 0; i < rotateHandles.Length; ++i)
        {
            rotateHandles[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rotateHandles[i].GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1);
            rotateHandles[i].GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
            rotateHandles[i].transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        }

        rotateHandles[0].transform.localPosition = (handleCentroids[2] + handleCentroids[0]) * 0.5f;
        rotateHandles[1].transform.localPosition = (handleCentroids[3] + handleCentroids[1]) * 0.5f;
        rotateHandles[2].transform.localPosition = (handleCentroids[6] + handleCentroids[4]) * 0.5f;
        rotateHandles[3].transform.localPosition = (handleCentroids[7] + handleCentroids[5]) * 0.5f;
    }
    private void ParentHandles()
    {
        Transform parent = Box.gameObject.transform;

        for (int i = 0; i < rotateHandles.Length; ++i)
        {
            rotateHandles[i].transform.SetParent(parent, true);
        }

        for (int i = 0; i < cornerHandles.Length; ++i)
        {
            cornerHandles[i].transform.SetParent(parent, true);
        }
    }

    private void CreateCornerPositions()
    {
        handleCentroids = new List<Vector3>();
        LayerMask mask = new LayerMask();
        GameObject clone = GameObject.Instantiate(Box.gameObject);
        clone.transform.localRotation = Quaternion.identity;
        BoundingBox.GetRenderBoundsPoints(clone, handleCentroids, mask);
        GameObject.Destroy(clone);

        Quaternion q = Box.gameObject.transform.rotation;
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(new Vector3(0,0,0), q, new Vector3(1, 1, 1));
        for (int i = 0; i < handleCentroids.Count; ++i)
        {
            handleCentroids[i] = m.MultiplyVector(handleCentroids[i]);
           // handleCentroids[i] += ObjectToBound.gameObject.transform.localPosition;
        }
    }

    private void UpdateHandles()
    {
        for (int i = 0; i < handleCentroids.Count; ++i)
        {
           cornerHandles[i].transform.localRotation = ObjectToBound.gameObject.transform.rotation;
           cornerHandles[i].transform.localPosition = ObjectToBound.transform.TransformPoint(handleCentroids[i]);
        }
    }

    private void ClearCornerHandles()
    {
        if (cornerHandles != null)
        {
            for (int i = 0; i < cornerHandles.Length; ++i)
            {
                GameObject.Destroy(cornerHandles[i]);
            }
            cornerHandles = null;
            handleCentroids = null;
        }

        cornerHandles = null;
        handleCentroids = null;
    }
    private void ClearRotateHandles()
    {
        if (rotateHandles != null && rotateHandles.Length > 0 && rotateHandles[0] != null)
        {
            for (int i = 0; i < rotateHandles.Length; ++i)
            {
                if (rotateHandles[i] != null)
                {
                    Destroy(rotateHandles[i]);
                    rotateHandles[i] = null;
                }
            }
        }

        rotateHandles = null;
    }
    private void ClearHandles()
    {
        ClearCornerHandles();
        ClearRotateHandles();
    }

    private GameObject BuildRig()
    {
        GameObject transformRig = new GameObject();
        transformRig.name = "center";
        transformRig.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
        transformRig.transform.localScale = new Vector3(1, 1, 1);


        GameObject upperLeftFront = new GameObject();
        upperLeftFront.name = "upperleftfront";
        upperLeftFront.transform.SetPositionAndRotation(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity);
        upperLeftFront.transform.localScale = new Vector3(1, 1, 1);
        upperLeftFront.transform.parent = transformRig.transform;

        GameObject upperLeftBack = new GameObject();
        upperLeftBack.name = "upperleftback";
        upperLeftBack.transform.SetPositionAndRotation(new Vector3(0.5f, 0.5f, -0.5f), Quaternion.identity);
        upperLeftBack.transform.localScale = new Vector3(1, 1, 1);
        upperLeftBack.transform.parent = transformRig.transform;

        GameObject lowerLeftFront = new GameObject();
        lowerLeftFront.name = "lowerleftfront";
        lowerLeftFront.transform.SetPositionAndRotation(new Vector3(0.5f, -0.5f, 0.5f), Quaternion.identity);
        lowerLeftFront.transform.localScale = new Vector3(1, 1, 1);
        lowerLeftFront.transform.parent = transformRig.transform;

        GameObject lowerLeftBack = new GameObject();
        lowerLeftBack.name = "lowerleftback";
        lowerLeftBack.transform.SetPositionAndRotation(new Vector3(0.5f, -0.5f, -0.5f), Quaternion.identity);
        lowerLeftBack.transform.localScale = new Vector3(1, 1, 1);
        lowerLeftBack.transform.parent = transformRig.transform;


        GameObject upperRightFront = new GameObject();
        upperRightFront.name = "upperrightfront";
        upperRightFront.transform.SetPositionAndRotation(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity);
        upperRightFront.transform.localScale = new Vector3(1, 1, 1);
        upperRightFront.transform.parent = transformRig.transform;

        GameObject upperRightBack = new GameObject();
        upperRightBack.name = "upperrightback";
        upperRightBack.transform.SetPositionAndRotation(new Vector3(0.5f, 0.5f, -0.5f), Quaternion.identity);
        upperRightBack.transform.localScale = new Vector3(1, 1, 1);
        upperRightBack.transform.parent = transformRig.transform;

        GameObject lowerRightFront = new GameObject();
        lowerRightFront.name = "lowerrightfront";
        lowerRightFront.transform.SetPositionAndRotation(new Vector3(0.5f, -0.5f, 0.5f), Quaternion.identity);
        lowerRightFront.transform.localScale = new Vector3(1, 1, 1);
        lowerRightFront.transform.parent = transformRig.transform;

        GameObject lowerRightBack = new GameObject();
        lowerRightBack.name = "lowerrightback";
        lowerRightBack.transform.SetPositionAndRotation(new Vector3(0.5f, -0.5f, -0.5f), Quaternion.identity);
        lowerRightBack.transform.localScale = new Vector3(1, 1, 1);
        lowerRightBack.transform.parent = transformRig.transform;

        return transformRig;
    }

    private List<Vector3> GetRigCorners(GameObject rig)
    {
        List<Vector3> corners = new List<Vector3>();

        for (int i = 0; i < rig.transform.childCount; ++i)
        {
            corners.Add(rig.transform.GetChild(i).position);
        }

        return corners;
    }
}
