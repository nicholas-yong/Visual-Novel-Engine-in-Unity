using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueEvents : MonoBehaviour
{
    //Handle an event on a line segment
    public static void HandleEvent( string _event, CLM.LINE.SEGMENT segment )
    {
        if( _event.Contains( "(" ) )
        {
            //get all actions delimited by a comma
            string[] actions = _event.Split(' ');

            for( int i = 0 ; i < actions.Length ; i++)
            {
                NovelController.instance.HandleAction( actions[i] );
            }

            //if this event was an action, return afterwards so we don't bother using it as an instruction
            return;

        }

        string[] eventData = _event.Split(' ');
        
        switch( eventData[0])
        {
            case "txtSpd":
                EVENT_TxtSpd( eventData[1], segment );
                break;
            case "/txtSpd":
                segment.architect.charactersPerFrame = 1;
                segment.architect.speed = 1;
                break;
        }
    }

    static void EVENT_TxtSpd( string data, CLM.LINE.SEGMENT segment)
    {
        string[] parts = data.Split(',');
        
        float delay = float.Parse( parts[0]);
        int numChar = int.Parse( parts[1] );
        segment.architect.charactersPerFrame = numChar;
        segment.architect.speed = delay;
    }
}
