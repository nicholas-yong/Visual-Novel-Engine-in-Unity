using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTesting : MonoBehaviour
{
  public Character YongYong;
    // Start is called before the first frame update
    void Start()
    {
        YongYong  = CharacterManager.instance.GetCharacter("Yong Yong", true, true);
        YongYong.GetSprite(2);
    }

    public string[] speech;
    int i = 0;

    public Vector2 moveTarget;
    public float moveSpeed;
    public bool smooth;

    public int bodyIndex;
    public float transitionSpeed;

    public Sprite testSprite;

    public bool transitionSmooth;


    // Update is called once per frame
    void Update()
    {
       if ( Input.GetKeyDown( KeyCode.Space))
        {
        if( i < speech.Length)
          {
            YongYong.Say(speech[i]);

          }
          else
          {
            DialogSystem.instance.Close();
          }
          i++;
        }

        if ( Input.GetKey( KeyCode.M) )
        {
          YongYong.MoveTo( moveTarget, moveSpeed/2, smooth);
        }

        if( Input.GetKey( KeyCode.S))
        {
          YongYong.StopMoving( true );
        }

     if( Input.GetKey( KeyCode.B))
      {     
          if( Input.GetKey( KeyCode.T ))
          {
            testSprite = YongYong.GetSprite ( bodyIndex );
            YongYong.TransitionBody( testSprite, transitionSpeed, transitionSmooth );
          }
          else
          {
            YongYong.SetBody( bodyIndex );
          }
        }
    }
}
