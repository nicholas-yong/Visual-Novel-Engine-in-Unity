using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioTesting : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioClip[] music;

    public float volume, pitch;
    void Start()
    {

    }

    void Update()
    {

        if( Input.GetKeyDown ( KeyCode.Space ) )
        {
            AudioManager.instance.PlaySFX( clips[Random.Range( 0, clips.Length )], volume, pitch );
        }

        if( Input.GetKeyDown( KeyCode.M))
        {
            Debug.Log( "test");
            AudioManager.instance.PlaySong( music[Random.Range( 0, music.Length)]);
        }
    }



}