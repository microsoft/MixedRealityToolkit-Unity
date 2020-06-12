using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FFITest : MonoBehaviour
{
    public Text TimeText;
    public Text DisplacementText;
    public Text OrientationText;

    public Transform testObject;
    public Transform testGoal;

    public float displacementTolerance;
    public float orientationTolerance;

    private bool testHasStarted = false;
    private bool testHasCompleted = false;

    private float testTime = 0.0f;

    private Vector3 initialPosition = Vector3.zero;
    private Quaternion initialRotation = Quaternion.identity;


    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartTest()
    {
        testHasStarted = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (testHasStarted)
        {
            var displacementError = testObject.position - testGoal.position;
            var orientationError = Quaternion.Angle(testObject.rotation, testGoal.rotation);

            TimeText.text = "Elapsed time: " + testTime.ToString("F2") + " sec";
            DisplacementText.text = $"Displacement error: {(displacementError.magnitude * 100.0f):F2} cm (required: <{(displacementTolerance * 100.0f):F2} cm)";
            OrientationText.text = $"Orientation error: {orientationError:F2} deg (required: <{orientationTolerance:F2} deg)";

            if (displacementError.magnitude < displacementTolerance && orientationError < orientationTolerance)
            {
                testHasCompleted = true;
            } else
            {
                if (testHasCompleted)
                {
                    TimeText.color = Color.white;
                    DisplacementText.color = Color.white;
                    OrientationText.color = Color.white;
                }
                testHasCompleted = false;
                testTime += Time.deltaTime;
            }
        }

        if (testHasCompleted)
        {
            TimeText.text = "Test complete! " + testTime.ToString("F2") + " sec";
            TimeText.color = Color.green;
            DisplacementText.color = Color.green;
            OrientationText.color = Color.green;
        }


    }
}
