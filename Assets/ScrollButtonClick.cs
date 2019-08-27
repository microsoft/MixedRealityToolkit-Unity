using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class ScrollButtonClick : MonoBehaviour
{
    public TextMeshPro clickTextbox;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DisplayClick(GameObject sender)
    {
        clickTextbox.text = "Click happened with" + sender;
    }


}
