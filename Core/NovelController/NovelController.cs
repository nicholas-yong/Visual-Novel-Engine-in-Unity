using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelController : MonoBehaviour
{
    public static NovelController instance;
    // The Lines of data coming from the Chapter File

    List<string> data = new List<string>();

    //The progress in the current data list ( Likely the index of the list that we are currently at)

    public int chapterProgress = 0;
    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        LoadChapterFile("chapter1.txt");
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( KeyCode.RightArrow ) )
        {
            Next();
        }
    }

    Coroutine handlingChapterFile = null;
    //trigger that advances progress through a chapter file
    bool _next = false;

    public void Next()
    {
        _next = true;
    }


    void LoadChapterFile( string filename)
    {
        data = FileManager.LoadFile( FileManager.savPath + "Resources/Story/Chapters/" + filename );
        cachedLastSpeaker = "";

        if(handlingChapterFile != null)
        {
            StopCoroutine(handlingChapterFile);
        }
        handlingChapterFile = StartCoroutine(HandlingChapterFile());

        Next(); //autostart the chapter
    }
    
    public bool isHandlingChapterFile
    {
        get
        {
            return handlingChapterFile != null;
        }
    }

    IEnumerator HandlingChapterFile()
    {
        //the progress through the lines inside the chapter
        chapterProgress = 0;

        while(chapterProgress < data.Count)
        {
            //need a way of knowing when the next trigger will appear.
            //there should be multiple types of triggers
            if(_next)
            {
                string line = data[chapterProgress];
                
                if(line.StartsWith("choice"))
                {
                    Debug.Log("test");
                    yield return HandlingChoiceLine(line);
                    chapterProgress++; //bypasses ending encapsulator(})
                }
                //normal line of dialogue/action
                else
                {
                    HandleLine(data[chapterProgress]);
                    chapterProgress++;
                    while(isHandlingLine())
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
    
    IEnumerator HandlingChoiceLine(string line)
    {
        string title = line.Split('"')[1];
        int choice_index = 0;
        List<string> choices = new List<string>();
        Dictionary<int, List<string>> actions = new Dictionary<int,List<string>>();
        string line_action = "";
        chapterProgress++;
        while( true )
        {
            line = data[chapterProgress];
            // start of a Choice Segment
            if( line == "    [")
            {
                List<string> individual_action = new List<string>();                           // reset list value
                line = data[chapterProgress-1];  // Populate the previous line data as that has the choice name
                line = line.Replace("    ", ""); // this eliminates the tabs in the choice line
                choice_index = choices.Count;
                choices.Add(line.Split('"')[1]);
                while( true )
                {
                    chapterProgress++;
                    line_action = data[chapterProgress];
                    if( line_action == "    ]")
                    {
                        break;
                    }
                    
                    else
                    {
                        individual_action.Add( line_action.Replace("        ", ""));
                    }
                }
                actions.Add( choice_index, individual_action );
            }
            else if( line == "}")
            {
                break;
            }
            chapterProgress++;
        }
        //display choices
        if( choices.Count > 0)
        {
            ChoiceScreen.Show(title, choices.ToArray());
            yield return new WaitForEndOfFrame();
            while( ChoiceScreen.isWaitingForChoiceToBeMade )
            {
                yield return new WaitForEndOfFrame();
            }
            List<string> list_of_actions = new List<string>();
            actions.TryGetValue( ChoiceScreen.lastChoiceMade.index, out list_of_actions );
            foreach( string action in list_of_actions )
            {
                HandleLine(action);
                while( isHandlingLine() )
                {
                    yield return new WaitForEndOfFrame();
                }
            }

        }
        else
        {
            Debug.Log("Invalid choice operation. No choices were created");
        }


    }


    void HandleLine( string rawLine )
    {
      CLM.LINE line = CLM.Interpret( rawLine );
      //now we need to handle the line, which requires a loop that handles each segment individually
      StopHandlingLine();
      handlingLine = StartCoroutine(HandlingLine(line));      
    }

    
    Coroutine handlingLine = null;
    public bool isHandlingLine()
    {
        return handlingLine != null;
    }

    IEnumerator HandlingLine( CLM.LINE line)
    {
        //since the next trigger controls the flow of a chapter by moving through lines and controls
        //progression through a line by its segments, it must be reset

        _next = false;
        int lineProgress = 0; //progress through the segments of a line

        while( lineProgress < line.segments.Count)
        {
            _next = false; //reset _next
            CLM.LINE.SEGMENT segment = line.segments[lineProgress];

            //always run the first segment automatically. But wait for trigger on all proceeding segments
            if( lineProgress > 0)
            {
                if(segment.trigger == CLM.LINE.SEGMENT.TRIGGER.autoDelay )
                {
                    for( float timer = segment.autoDelay ; timer >= 0; timer = timer - Time.deltaTime )
                    {
                        yield return new WaitForEndOfFrame();
                        if(_next)
                        {
                            break;  //termiante loop if player chooses to skip via trigger. Prevents unskippable wait timers.
                        }
                    }
                }

                else
                {
                    //wait until the player says to move to the next segment
                    while( !_next)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            _next = false; //next could have been triggered during an event above
            //the segment now needs to build and run.
            segment.Run();

            while( segment.isRunning )
            {
                yield return new WaitForEndOfFrame();
                //allow for auto-completion for skipping purposes, if that is decided
                //so _next is TRUE if the user is skipping
                if(_next)
                {
                    //if the architect is not already set to skip, set it to skip first
                    if(!segment.architect.skip)
                    {
                        segment.architect.skip = true;
                    }
                    //on the next call to skip, force the segment to finish
                    else
                    {
                        segment.forceFinish();
                    }
                    _next = false;
                }

            }
            lineProgress++;

            yield return new WaitForEndOfFrame();
        }
        //add actions to Line
        for(int i = 0; i < line.actions.Count; i++)
        {
            HandleAction(line.actions[i]);
        }
        handlingLine = null;

    }

    void StopHandlingLine()
    {
        if(isHandlingLine())
        {
            StopCoroutine(handlingLine);
        }

        handlingLine = null;
    }


    //Used as a fallback when no speaker is given.

   public string cachedLastSpeaker = "";

   //Action Handling from this point on
   public void HandleAction( string action )
    {
        string[] data = action.Split('(', ')');
        

        switch (data[0])
        {
            case "setBackground":
                Command_SetLayerImage( data[1], BCFC.instance.background );
                break;
            case "setForeground":
                Command_SetLayerImage( data[1], BCFC.instance.foreground);
                break;
            case "setCinematic":
                Command_SetLayerImage( data[1], BCFC.instance.cinematic );
                break;
            case "playSound":
                Command_PlaySound( data[1]);
                break;
            case "stopSound":
                Command_StopSound( data[1]);
                break;
            case "playMusic":
                Command_PlayMusic( data[1]);
                break;
            case "moveCharacter":
                Command_Move( data[1], false );
                break;
            case "setPosition":
                Command_Move( data[1], true );
                break;
            case "changeExpression":
                Command_ChangeExpression( data[1] );
                break;
            case "faceDirection":
                Command_FaceDirection( data[1] );
                break;
            case "Load":
                Command_Load(data[1]);
                break;
            default:
                break;

        }
    }

    void Command_Load( string chapterName )
    {
        NovelController.instance.LoadChapterFile( chapterName );
    }

    void Command_FaceDirection( string data )
    {
        string[] parameters = data.Split(',');
        if( parameters.Length < 2 )
        {
            Debug.LogError("Invalid Number of Parameters passed to Function Command_FaceDirection()");
        }
        string character = parameters[0];
             //replace all instances of underscores inside the charactername

        if( character.Contains("_"))
        {
           character = character.Replace("_", " ");
        }
        
        string direction = parameters[1];

        Character c = CharacterManager.instance.GetCharacter( character );

        if( direction == usrconst.DIRECTION_flip)
        {
            c.Flip();
        }
        else if( direction == usrconst.DIRECTION_left)
        {
            c.FaceLeft();
        }
        else if ( direction == usrconst.DIRECTION_right)
        {
            c.FaceRight();
        }
        else
        {
            Debug.LogError(" Invalid Direction Constant passed to Function Command_FaceDirection() ");
        }
    }

    void Command_SetLayerImage( string data, BCFC.LAYER layer )
    {
        string texName = "";
        if( data.Contains(","))
        {
            texName = data.Split(',')[0];
        }
        else
        {
            texName = data;
        }

        Texture2D tex = null;
        
        if( texName != "null")
        {
            tex = Resources.Load("Images/UI/Backdrops/Still/" + texName ) as Texture2D;
        }

        float spd = 2f;
        bool smooth = false;
        Texture2D transitionEffect = null;

        if( data.Contains(","))
        {
            string[] parameters = data.Split(',');
            // if only the texture and the speed was passed
            if( parameters.Length == 2)
            {
                float.TryParse(parameters[1], out spd);
            }

            else if ( parameters.Length == 3 )
            {
                float.TryParse(parameters[1], out spd);
                bool.TryParse(parameters[2], out smooth);  
            }

             else if ( parameters.Length == 4)
            {
                float.TryParse(parameters[1], out spd);
                bool.TryParse(parameters[2], out smooth);  
                transitionEffect = Resources.Load("Images/TransitionEffects/" + parameters[3] ) as Texture2D;
            }

            else
            {
                Debug.LogError( "Invalid Parameters passed to Function Command_SetLayerImage");
            }
          
        }
        if( transitionEffect != null)
        {
            TransitionMaster.TransitionLayer( layer, tex, transitionEffect, spd, smooth );
        }

        else
        {
            layer.TransitionToTexture(tex, spd, smooth );   
        }
        
    }

    void Command_PlaySound( string data )
    {
        string[] parameters = data.Split(',');

        AudioClip clip = Resources.Load("Audio/SFX/" + parameters[0] ) as AudioClip;

        float vol = 1f;
        float pitch = 1f;

        if(parameters.Length > 1 )
        {
            float.TryParse(parameters[1], out vol );
            float.TryParse(parameters[2], out pitch);
        }

        if( clip != null )
        {
            AudioManager.instance.PlaySFX( clip, vol, pitch );
        }
        else
        {
            Debug.LogError( "Clip does not exist - " + data);
        }
    }

    void Command_StopSound( string data )
    {
        string[] parameters = data.Split(',');

        string sfxName = parameters[0];

        float destroyTime = 0.1f;

        if( parameters.Length == 2 )
        {
            float.TryParse( parameters[1], out destroyTime );
        }

        AudioManager.instance.StopSFX( sfxName, destroyTime );
    }

    void Command_PlayMusic( string data )
    {
        string[] parameters = data.Split(',');
        
        AudioClip clip = Resources.Load("Audio/Music/" + parameters[0])  as AudioClip;

        float maxVol = 1f;
        float pitch = 1f;
        float startingVol = 0;
        bool playOnStart = true;
        bool loop = true;

        if( parameters.Length >= 2 )
        {
            float.TryParse( parameters[1], out maxVol);
        }

        if( parameters.Length >= 3 )
        {
            float.TryParse( parameters[2], out pitch);
        }

        if( parameters.Length >= 4 )
        {
            float.TryParse( parameters[3], out startingVol);
        }

        if( parameters.Length >= 5 )
        {
            bool.TryParse( parameters[4], out playOnStart );
        }

          if( parameters.Length == 6 )
        {
            bool.TryParse( parameters[5], out loop );
        }



        if( clip != null)
        {
            AudioManager.instance.PlaySong( clip, maxVol, pitch, startingVol, playOnStart, loop );
        }

        else
        {
             Debug.LogError( "Clip does not exist - " + data);
        }
    }
    
    /*void Command_StopMusic( string data )
    {
        string[] parameters = data.Split(',');
        AudioManager.instance.a
        
    }*/
    
    void Command_Move( string data, bool instant )
    {
        string[] parameters = data.Split( ',');
        string character = parameters[0];

        //replace all instances of underscores inside the charactername

        if( character.Contains("_"))
        {
           character = character.Replace("_", " ");
        }

        float locationX = float.Parse( parameters[1] );
        float locationY = float.Parse( parameters[2] );

        // default speed
        float speed = 1f;
        
        //Test if speed was passed
        if( parameters.Length == 4 && !instant )
        {
            speed = float.Parse( parameters[3] );
        }

        Character prim_character = CharacterManager.instance.GetCharacter( character );

        if( instant )
        {
            prim_character.SetPosition( new Vector2( locationX, locationY));
        }

        else
        {
             prim_character.MoveTo( new Vector2( locationX, locationY), speed, false );
        }
    }

    void Command_ChangeExpression( string data )
    {
        string[] parameters = data.Split( ',');
        string character = parameters[0];

        //replace all instances of underscores inside the charactername

        if( character.Contains("_"))
        {
           character = character.Replace("_", " ");
        }

        float speed = 1f;

        string expression = parameters[1] == "null" ? "" : character + "_" + parameters[1];
        
        // append the character's name to the expression so we don't have to include it inside the call to the sprite name

        if( parameters.Length == 3)
        {
            speed = float.Parse( parameters[2] );
        }

        Character c = CharacterManager.instance.GetCharacter(  character );

        if( expression == "")
        {
            c.enabled = false;
        }
        else
        {
            c.TransitionBody( c.GetSprite( expression ), speed, false );
        } 
    }
}
