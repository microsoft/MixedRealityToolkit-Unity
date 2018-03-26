using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColorChanger : NetworkBehaviour

{
    [SyncVar]
    Color color;

    Renderer objectRenderer;

    float timer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        ChangeColor();
    }

    void Update()
    {
        if (isServer)
        {
            if (timer > 3.0f)
            {
                ChangeColor();
                timer = 0f;
            }
            timer += Time.deltaTime;
        }
        objectRenderer.material.color = color;
    }

    void ChangeColor()
    {
        color = UnityEngine.Random.ColorHSV();
        objectRenderer.material.color = color; 
    }
}
