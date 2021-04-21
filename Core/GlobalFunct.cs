using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalFunct : MonoBehaviour
{
  public static bool TransitionImages( ref Image activeImage, ref List<Image> allImages, float speed, bool smooth )
    {
         bool anyValueChanged = false;

        speed *= Time.deltaTime;
        for( int i = allImages.Count - 1 ; i >=0 ; i-- )
        {
          Image image = allImages[i];
          if( image == activeImage )
          {
              if( image.color.a < 1f )
              {
                image.color = setAlpha( image.color, smooth ? Mathf.Lerp( image.color.a, 1f, speed) : Mathf.MoveTowards( image.color.a, 1f, speed)); 
                anyValueChanged = true;
              }
          }
          else
          {

             if( image.color.a > 0 )
            {
                image.color = setAlpha( image.color, smooth ? Mathf.Lerp( image.color.a, 0f, speed) : Mathf.MoveTowards( image.color.a, 0f, speed));
                anyValueChanged = true;

            }
            else
            {
                allImages.RemoveAt( i );
                DestroyImmediate( image.gameObject );
                continue;
            }

          }
        }

        return anyValueChanged;
    }

    public static bool TransitionRawImages( ref RawImage activeImage, ref List<RawImage> allImages, float speed, bool smooth )
    {
         bool anyValueChanged = false;

        speed *= Time.deltaTime;
        for( int i = allImages.Count - 1 ; i >=0 ; i-- )
        {
          RawImage image = allImages[i];
          if( image == activeImage )
          {
              if( image.color.a < 1f )
              {
                image.color = setAlpha( image.color, smooth ? Mathf.Lerp( image.color.a, 1f, speed) : Mathf.MoveTowards( image.color.a, 1f, speed)); 
                anyValueChanged = true;
              }
          }
          else
          {

             if( image.color.a > 0 )
            {
                image.color = setAlpha( image.color, smooth ? Mathf.Lerp( image.color.a, 0f, speed) : Mathf.MoveTowards( image.color.a, 0f, speed));
                anyValueChanged = true;

            }
            else
            {
                allImages.RemoveAt( i );
                DestroyImmediate( image.gameObject );
                continue;
            }

          }
        }

        return anyValueChanged;
    }

    public static Color setAlpha( Color color, float alpha )
    {
        return new Color( color.r, color.g, color.b, alpha);
    }

    public static bool fadeImage( ref Image image, float spd, bool smooth )
    {
      spd = spd * Time.deltaTime;
      if( image.color.a != 0 )
      {
        if( smooth )
        {
          image.color = setAlpha( image.color, Mathf.Lerp( image.color.a, 0f, spd ) );
        }
        else
        {
          image.color = setAlpha( image.color, Mathf.MoveTowards( image.color.a, 0f, spd) );
        }

        if( image.color.a == 0)
        {
          return false;
        }
      }
      return true;
    }

    public static void printArray( string[] array )
    {
       string holder = "";
       foreach( string a in array )
       {
         holder = holder + a + '\n';
       }

       Debug.Log( holder );
    }

}
