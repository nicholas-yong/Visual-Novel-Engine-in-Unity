using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionMaster : MonoBehaviour
{
    public static TransitionMaster instance;

    public RawImage overlayImage;
    public Material transitionMaterialPrefab;

    void Awake()
    {
        instance = this;
        overlayImage.material = new Material( transitionMaterialPrefab );
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static bool sceneVisible = true;
    public static void ShowScene( bool show, float speed = 1, bool smooth = false, Texture2D transitionEffect = null )
    {
        Debug.Log("run");
        Debug.Log("Show:" + show);
        if( transitioningOverlay != null )
        {
            instance.StopCoroutine(transitioningOverlay);
        }
        sceneVisible = !show;

        if( transitionEffect != null)
        {
            instance.overlayImage.material.SetTexture("_AlphaTex", transitionEffect );
        }

        transitioningOverlay = instance.StartCoroutine(TransitioningOverlay( show, speed, smooth));
        Debug.Log( "sceneVisible:" + sceneVisible);
    }

    static Coroutine transitioningOverlay = null;

    static IEnumerator TransitioningOverlay( bool show, float speed, bool smooth)
    {
        float targval = show ? 1 : 0;
        float curVal = instance.overlayImage.material.GetFloat("_Cutoff");

        while( curVal != targval )
        {
            if( smooth )
            {
                curVal = Mathf.Lerp(curVal, targval, speed * Time.deltaTime );
            }
            else
            {
                curVal = Mathf.MoveTowards(curVal, targval, speed * Time.deltaTime );
            }
            instance.overlayImage.material.SetFloat("_Cutoff", curVal);

            yield return new WaitForEndOfFrame();
        }

        transitioningOverlay = null;
    }

    public static void TransitionLayer( BCFC.LAYER layer, Texture2D targetImage, Texture2D transitionImage, float speed = 1, bool smooth = false )
    {
        if( layer.specialTransitionCoroutine != null )
        {
            instance.StopCoroutine( layer.specialTransitionCoroutine );
        }

        if( targetImage != null )
        {
            layer.specialTransitionCoroutine = instance.StartCoroutine( TransitioningLayer( layer, targetImage, transitionImage, speed, smooth ));

        }
        else
        {
            layer.specialTransitionCoroutine = instance.StartCoroutine( TransitioningLayerToNull( layer, transitionImage, speed, smooth ));
        }
    }

    static IEnumerator TransitioningLayer( BCFC.LAYER layer, Texture2D targetTex, Texture2D transitionEffect, float speed, bool smooth)
    {
        GameObject ob = Instantiate( layer.newImageObjectReference, layer.newImageObjectReference.transform.parent ) as GameObject;
        ob.SetActive(true);

        RawImage im = ob.GetComponent<RawImage>();
        im.texture = targetTex;

        layer.activeImage = im;
        layer.allImages.Add(im);

        im.material = new Material( instance.transitionMaterialPrefab );
        im.material.SetTexture( "_AlphaTex", transitionEffect );
        im.material.SetFloat("_Cutoff", 1);

        float curVal = 1;

        while( curVal > 0 )
        {
            if( smooth )
            {
                curVal = Mathf.Lerp( curVal, 0, speed * Time.deltaTime );
            }

            else
            {
                curVal = Mathf.MoveTowards( curVal, 0, speed * Time.deltaTime );
            }

            im.material.SetFloat("_Cutoff", curVal );
            yield return new WaitForEndOfFrame();
        }

        //remove the material so we can use regular alpha
        //check for null if we rapidly progress through fading and transition overlaps

        if( im != null )
        {
            im.material = null;
            im.color = GlobalFunct.setAlpha(im.color, 1);
        }

        //remove all other images on layer
        for( int i = layer.allImages.Count - 1; i >= 0; i--)
        {
            if( layer.allImages[i] == layer.activeImage && layer.activeImage != null )
            {
                continue;
            }

            if( layer.allImages[i] != null )
            {
                Destroy( layer.allImages[i].gameObject, (float) 0.1 );
            }

            layer.allImages.RemoveAt(i);
        }

        //clear special transition field

        layer.specialTransitionCoroutine = null;

    }

    static IEnumerator TransitioningLayerToNull( BCFC.LAYER layer, Texture2D transitionEffect, float speed, bool smooth)
    {
        List<RawImage> currentImages = new List<RawImage>();
        foreach( RawImage r in layer.allImages )
        {
            r.material = new Material( instance.transitionMaterialPrefab);
            r.material.SetTexture("_AlphaTex", transitionEffect);
            r.material.SetFloat("_Cutoff", 0);
            currentImages.Add(r);
        }

        float curVal = 0;
        while ( curVal < 1)
        {
            if( smooth)
            {
               curVal = Mathf.Lerp( curVal, 1, speed * Time.deltaTime); 
            }

            else
            {
                curVal = Mathf.MoveTowards( curVal, 1, speed * Time.deltaTime);
            }

            for( int i = 0 ; i < layer.allImages.Count; i++)
            {
                layer.allImages[i].material.SetFloat("_Cutoff", curVal);
            }

            yield return new WaitForEndOfFrame();            
        }

        foreach( RawImage r in currentImages )
        {
            layer.allImages.Remove(r);
            if( r.material != null)
            {

            Destroy( r.gameObject, (float) 0.01 );

            }
        }

        layer.specialTransitionCoroutine = null;
    }


    
}
