using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public static List<SONG> allSongs = new List<SONG>();

    public static List<AudioSource> allSFX = new List<AudioSource>();

    public static SONG activeSong = null;

    public float songTransitionSpeed = 1f;

    public bool songSmoothTransitions = true;

    void Awake()
    {
        if( instance == null )
        {
            instance = this;
            DontDestroyOnLoad( gameObject );
        }

        else
        {
            DestroyImmediate( gameObject );
        }
    }

    public void PlaySFX( AudioClip effect, float volume = 1f, float pitch = 1f)
    {
        AudioSource source = CreateNewSource( string.Format( "SFX [{0}]", effect.name ));
        source.clip = effect;
        source.volume = volume;
        source.pitch = pitch;
        allSFX.Add(source);
        source.Play();

        Destroy( source.gameObject, effect.length );

    }

    Coroutine stoppingSFX = null;

    public bool isStoppingSFX
    {
        get
        {
            return stoppingSFX != null;
        }
    }

    public void stopStoppingSFX()
    {
        if( isStoppingSFX )
        {
            StopCoroutine(stoppingSFX);
        }

        stoppingSFX = null;
    }

    public void StopSFX( string sfxName, float time )
    {
        stopStoppingSFX();
        AudioSource sourceToBeDestroyed = null;
         // the first thing that we need to do here is to get the AudioSource
        foreach( AudioSource audio in allSFX)
        {     
            if( audio.name == "SFX [" + sfxName + "]")
            {   
                int index = allSFX.IndexOf( audio );
                sourceToBeDestroyed = allSFX[index];
                break;
            }
        }

        AudioManager.instance.StartCoroutine((DestroySFX( sourceToBeDestroyed, time ) ) );
    }

    public IEnumerator DestroySFX( AudioSource source, float time )
    {
        while( DestroyingSFX( source, time ) )
        {
            yield return new WaitForEndOfFrame();
        }

        stopStoppingSFX();
    }

    bool DestroyingSFX( AudioSource audio, float time )
    {
        bool stillDestroying = true;

        float spd = Time.deltaTime * time;

        if( audio.volume != 0 )
        {
            audio.volume = Mathf.MoveTowards( audio.volume, 0, spd );
        }

        stillDestroying = audio.volume != 0;

        if( !stillDestroying )
        {
             allSFX.Remove( audio );
             Destroy( audio.gameObject, 1f );
        }
        return stillDestroying;
    }

    public void PlaySong( AudioClip song, float maxVolume = 1f, float pitch = 1f, float startingVolume = 0f, bool playOnStart = true,  bool loop = true )
    {
        if( song != null)
        {
            for( int i = 0 ; i < allSongs.Count; i++ )
            {
                SONG s = allSongs[i];
                if( s.clip == song)
                {
                   activeSong = s;
                   break;
                }

            }

            if( activeSong == null || activeSong.clip != song)
            {
                activeSong = new SONG( song, maxVolume, pitch, startingVolume, playOnStart, loop);
            }
        }

        else
        {
            activeSong = null;
        }

        StopAllCoroutines();
        StartCoroutine(VolumeLevelling());
    }

    IEnumerator VolumeLevelling()
    {
        while( TransitionSongs() )
        {
            yield return new WaitForEndOfFrame();
        }
    }

    bool TransitionSongs()
    {
        bool anyValueChanged = false;

        float speed = songTransitionSpeed * Time.deltaTime;
        for( int i = allSongs.Count - 1 ; i >=0 ; i-- )
        {
          SONG song = allSongs[i];
          if( song == activeSong )
          {
              if( song.volume < song.maxVolume )
              {
                song.volume = songSmoothTransitions ? Mathf.Lerp( song.volume, song.maxVolume, speed) : Mathf.MoveTowards( song.volume, song.maxVolume, speed); 
                anyValueChanged = true;
              }
          }
          else
          {

             if( song.volume > 0 )
            {
                song.volume = songSmoothTransitions ? Mathf.Lerp( song.volume, 0f, speed) : Mathf.MoveTowards( song.volume, 0f, speed);
                anyValueChanged = true;

            }
            else
            {
                allSongs.RemoveAt( i );
                song.DestroySong();
                continue;
            }

          }
        }

        return anyValueChanged;
    }

    public static AudioSource CreateNewSource( string _name )
    {
        AudioSource newSource = new GameObject( _name ).AddComponent<AudioSource>();
        newSource.transform.SetParent( instance.transform );

        return newSource;
    }


    [System.Serializable]
    public class SONG
    {
        public AudioSource source;
        public AudioClip clip{
            get{
                return source.clip;
            }

            set
            {
                source.clip = value;
            }
        }
        public float maxVolume = 1f;

        public SONG( AudioClip clip, float _maxVolume, float pitch, float startingVolume, bool playOnStart , bool loop)
        {
            source = AudioManager.CreateNewSource( string.Format( "SONG [{0}])", clip.name));
            source.clip = clip;
            source.volume = startingVolume;
            maxVolume = _maxVolume;
            source.pitch = pitch;
            source.loop = loop;

            AudioManager.allSongs.Add( this );

            if( playOnStart )
            {
                source.Play();
            }

        }

        public float volume{
            get
            {
                return source.volume;
            }

            set
            {
               source.volume = value;
            }
        }

        public float pitch{
            get
            {
                return source.pitch;
            }
            set
            {
                source.pitch = value;
            }
        }

        public void Play()
        {
            source.Play();
        }

        public void Stop()
        {
            source.Stop();
        }

        public void Pause()
        {
            source.Pause();
        }

        public void UnPause()
        {
            source.UnPause();
        }

        public void DestroySong()
        {
            AudioManager.allSongs.Remove(this);
            DestroyImmediate( source.gameObject );
        }
    }

}
