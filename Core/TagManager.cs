using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    public static void Inject( ref string s)
    {
        if(!s.Contains("["))
        {
            return;
        }

        s = s.Replace("[mainCharName]", "Yong Yong");
        s = s.Replace("[murderWeapon]", "Knife" );
    }
    public static string[] SplitByTags( string targetText )
    {
        return targetText.Split( new char[2]{'<', '>'});
    }
}
