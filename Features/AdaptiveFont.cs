using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveFont : MonoBehaviour
{

    Text txt;        //Text component we will be controlling

    public bool update = true;
    // Start is called before the first frame update

    public int defaultFontResolution = 24;

    public static float defaultResolution = 2200f;
    void Start()
    {
        txt = GetComponent<Text>();

        if( update )
        {
            InvokeRepeating("Adjust", 0f, Random.Range( 0.5f, 2f) );
        }
        else
        {
            Adjust();
            enabled = false;
        }
    }

    void Adjust()
    {
        if( !enabled || !gameObject.activeInHierarchy)
        {
            return;
        }

        float totalCurrentRes = Screen.height + Screen.width;

        float percentage = totalCurrentRes / defaultResolution;
        int fontsize = Mathf.RoundToInt ((float) defaultFontResolution * percentage);
        txt.fontSize = fontsize;

    }
}
