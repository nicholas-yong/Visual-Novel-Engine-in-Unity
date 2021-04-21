using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTesting : MonoBehaviour
{
    BCFC controller;

    public Texture texture;

    public float speed;
    public bool smooth;

    BCFC.LAYER layer;
    // Start is called before the first frame update
    void Start()
    {
        controller = BCFC.instance;
    }

    // Update is called once per frame
    void Update()
    {

        if( Input.GetKey( KeyCode.Q))
        {
            layer = controller.background;
        }
        if( Input.GetKey( KeyCode.W))
        {
            layer = controller.cinematic;
        }

        if( Input.GetKey( KeyCode.R))
        {
            layer = controller.foreground;
        }

        if( Input.GetKey( KeyCode.T ))
        {
            layer.TransitionToTexture(texture, speed, smooth);
            
        }

        else
        {
            if( Input.GetKeyDown( KeyCode.E))
            {
                layer.SetTexture( texture );
            }
        }
        
    }
}
