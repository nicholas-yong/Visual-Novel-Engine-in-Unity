using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleHeader : MonoBehaviour
{
    public Image banner;
    public TextMeshProUGUI titleText;
    public string title{
        get
        {
            return titleText.text;
        }
        set
        {
            titleText.text = value;
        }
    }

    public enum DISPLAY_METHOD
    {
        instant,
        slowFade,
        typeWriter
    }

    public DISPLAY_METHOD displayMethod = DISPLAY_METHOD.instant;
    public float fadeSpeed = 0.5f;

    public void Show( string displayTitle )
    {
        title = displayTitle;

        if(isRevealing)
        {
            StopCoroutine(revealing);
        }

        revealing = StartCoroutine(Revealing());

    }

    public void Hide()
    {
        if(isRevealing)
        {
           StopCoroutine(revealing); 
        }
        revealing = null;

        banner.enabled = false;
        titleText.enabled = false;
    }

    public bool isRevealing{
        get
        {
            return revealing != null;
        }
    }

    Coroutine revealing = null;

    //Couroutine to gradually show header
    IEnumerator Revealing()
    {
        banner.enabled = true;
        titleText.enabled = true;

        //yield for the current display method
        switch(displayMethod)
        {
            case DISPLAY_METHOD.instant:
                // makes it fully non-transparent(opqaue)
                banner.color = GlobalFunct.setAlpha(banner.color, 1);
                titleText.color = GlobalFunct.setAlpha(titleText.color, 1);
                break;
            case DISPLAY_METHOD.slowFade:   
                // initally set them to be hidden
                banner.color = GlobalFunct.setAlpha(banner.color, 0);
                titleText.color = GlobalFunct.setAlpha(titleText.color, 0);
                while( banner.color.a < 1 )
                {
                    banner.color = GlobalFunct.setAlpha(banner.color, Mathf.MoveTowards(banner.color.a, 1, fadeSpeed * Time.unscaledDeltaTime));
                    titleText.color = GlobalFunct.setAlpha(banner.color, banner.color.a );
                    yield return new WaitForEndOfFrame();
                }
                break;
            case DISPLAY_METHOD.typeWriter:
                banner.color = GlobalFunct.setAlpha(banner.color, 1);
                titleText.color = GlobalFunct.setAlpha(titleText.color, 1);
                //make a new TextArchitect
                TextArchitect architect = new TextArchitect( titleText, title );
                while( architect.isConstructing )
                {
                    yield return new WaitForEndOfFrame();
                }
                break;
        }

        //title is displayed now.
        revealing = null;
    }
}
