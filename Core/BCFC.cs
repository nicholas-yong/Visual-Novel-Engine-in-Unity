using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BCFC : MonoBehaviour
{
    public static BCFC instance;
    public LAYER background = new LAYER();
    public LAYER cinematic = new LAYER();

    public LAYER foreground = new LAYER();

    
    void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class LAYER
    {
        public GameObject root;
        public GameObject newImageObjectReference; //Object Already Connected to Layer
        public RawImage activeImage;
        public List<RawImage> allImages = new List<RawImage>();

        public Coroutine specialTransitionCoroutine = null;

        public void SetTexture( Texture texture)
        {
            if( texture != null )
            {
                if( activeImage == null )
                {
                    CreateNewActiveImage();
                    activeImage.texture = texture;
                    activeImage.color = GlobalFunct.setAlpha( activeImage.color, 1f );

                    // VideoClips come here when the tutorial eventually comes out for it.
                    // For now, let's stick with normal textures
                }
            }
            else
            {
                if( activeImage != null )
                {
                    allImages.Remove( activeImage );
                    GameObject.DestroyImmediate( activeImage.gameObject );
                    activeImage = null;

                }
            }

        }

        public void TransitionToTexture( Texture texture, float speed = 1f, bool smooth = false,  bool ifMovieThenLoop = true )
        {
            if( activeImage != null && activeImage.texture == texture)
            {
                return;
            }

            StopTransitioning();
            transitioning = BCFC.instance.StartCoroutine( Transitioning( texture, speed, smooth, ifMovieThenLoop));
          
        }

        public bool isTransitoning
        {
            get
            {
                return transitioning != null;
            }
        }

        Coroutine transitioning = null;

        void StopTransitioning( )
        {
            if( isTransitoning )
            {
                BCFC.instance.StopCoroutine( transitioning );
            }

            transitioning = null;
        }

        IEnumerator Transitioning( Texture texture, float speed, bool smooth,  bool ifMovieThenLoop )
        {
            if( texture != null)
            {
                for ( int i = 0 ; i < allImages.Count; i++)
                {
                    RawImage image = allImages[i];
                    if( image.texture == texture)
                    {
                        activeImage = image;
                        break;
                    }
                }

                if( activeImage == null || activeImage.texture != texture )
                {
                    CreateNewActiveImage();
                    activeImage.texture = texture;
                    activeImage.color = GlobalFunct.setAlpha( activeImage.color, 0f);

                }
            }
            else
            {
                activeImage = null;
            }

            while( GlobalFunct.TransitionRawImages( ref activeImage, ref allImages, speed, smooth))
            {
                yield return new WaitForEndOfFrame();
            }

            StopTransitioning();
            
        }

        void CreateNewActiveImage()
        {
            GameObject ob = Instantiate( newImageObjectReference, root.transform ) as GameObject;
            ob.SetActive( true );
            RawImage raw = ob.GetComponent<RawImage>();
            activeImage = raw;
            allImages.Add( raw );
        }
    }
}
