using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Experimental;
using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class HideTapToPlaceLabel : MonoBehaviour
{
    // Start is called before the first frame update

    private TapToPlace tapToPlace;
    void Start()
    {
        tapToPlace = gameObject.GetComponent<TapToPlace>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tapToPlace.IsBeingPlaced)
        {
            var label = gameObject.transform.GetChild(0).gameObject;
            label.SetActive(false);
        }
        else
        {
            var label = gameObject.transform.GetChild(0).gameObject;
            label.SetActive(true);

        }
        
    }
}
