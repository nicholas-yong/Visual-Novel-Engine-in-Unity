using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Character
{
  public string characterName;

  [HideInInspector] public RectTransform root;
  [HideInInspector] public RectTransform characterImage_root;

  public bool enable;

  GameObject ob;


DialogSystem dialogue;

public bool enabled{
  get
  {
    return root.gameObject.activeInHierarchy;
  }
  set
  {
    enable = value;
    if(value)
    {
      renderers.bodyRenderer.color = GlobalFunct.setAlpha( renderers.bodyRenderer.color, 1 );
      root.gameObject.SetActive(value);
    }
    else
    {
      HideBody();
    }
    
  }
}

public Vector2 anchorPadding{
  get{
    return root.anchorMax - root.anchorMin;
  }
}


public void Say( string speech, bool add = false)
{
  if( !enabled)
  {
    enabled = true;
  }

  dialogue.Say( speech, characterName, add);
}

Vector2 targetPosition;

Coroutine moving;

bool IsMoving{
  get{
    return moving != null;
  }
}

public void MoveTo( Vector2 target, float speed, bool smooth = true)
{
  StopMoving();
  moving = CharacterManager.instance.StartCoroutine(Moving(target, speed, smooth));
}

public void StopMoving( bool Immediate = false )
{

  if( IsMoving )
  {
     CharacterManager.instance.StopCoroutine( moving );
     if( Immediate)
     {
       SetPosition(targetPosition);
     }
  }

  moving = null;

}

public void SetPosition( Vector2 target)
{
  targetPosition = target;
  Vector2 padding = anchorPadding;
  // get Anchors of the Character
  float maxX = 1f - padding.x;
  float maxY = 1f - padding.y;


  //now get the minimum anchors of the target position ( i.e: how much we are actually moving )

  Vector2 minAnchorTarget = new Vector2( maxX * targetPosition.x, maxY * targetPosition.y);
  root.anchorMin = minAnchorTarget;
  root.anchorMax = root.anchorMin + padding;

}

IEnumerator Moving(Vector2 target, float speed, bool smooth)
{
  targetPosition = target;
  Vector2 padding = anchorPadding;
  // get Anchors of the Character
  float maxX = 1f - padding.x;
  float maxY = 1f - padding.y;

  //now get the minimum anchors of the target position ( i.e: how much we are actually moving )

  Vector2 minAnchorTarget = new Vector2( maxX * targetPosition.x, maxY * targetPosition.y);

  speed *= Time.deltaTime;

  while( root.anchorMin != minAnchorTarget )
  {
    if ( !smooth )
    {
      root.anchorMin = Vector2.MoveTowards( root.anchorMin, minAnchorTarget, speed);
    }
    else
    {
      root.anchorMin = Vector2.Lerp( root.anchorMin, minAnchorTarget, speed);
    }

    root.anchorMax = root.anchorMin + padding;
    yield return new WaitForEndOfFrame();
  }

  StopMoving();
}

////    START TRANSITION CODE
public Sprite GetSprite( int index = 0 )
{
  Sprite[] sprites = Resources.LoadAll<Sprite> ("Images/Characters/" + characterName);
  
  return sprites[index];
}

// Returns a specific sprite via sprite name instead of using an index
public Sprite GetSprite( string sprite_name = " " )
{
  Sprite[] sprites = Resources.LoadAll<Sprite>( "Images/Characters/" + characterName );

  foreach( Sprite s in sprites)
  {
    if( s.name == sprite_name)
    {
      return s;
    }
  }

  return null;
}

public void SetBody( int index)
{
 renderers.bodyRenderer.sprite = GetSprite( index );
}

public void SetBody( Sprite sprite)
{
  renderers.bodyRenderer.sprite = sprite;
}

public void SetBody( string spriteName )
{
  renderers.bodyRenderer.sprite = GetSprite( spriteName );
}

Coroutine hidingBody = null;
bool isHidingBody
{
  get
  {
    return hidingBody != null;
  } 
}

public void stopHidingBody()
{
  if(isHidingBody)
  {
    hidingBody = null;
  }

  CharacterManager.instance.StopCoroutine(HidingBody());
}

public void HideBody()
{
  stopHidingBody();
  hidingBody = CharacterManager.instance.StartCoroutine(HidingBody());
}

IEnumerator HidingBody()
{
  while(GlobalFunct.fadeImage( ref renderers.bodyRenderer, 0.4f, false ) )
  {
    yield return new WaitForEndOfFrame();
  }

  stopHidingBody();
  root.gameObject.SetActive(false);
}

Coroutine transitioningBody = null;

bool isTransitioningBody{
  get
  {
    return transitioningBody != null;
  }
}

public void TransitionBody( Sprite sprite, float speed, bool smooth )
{
   if( renderers.bodyRenderer.sprite == sprite )
   {
     return;
   }
   StopTransitioningBody();

   transitioningBody = CharacterManager.instance.StartCoroutine( TransitioningBody( sprite, speed, smooth) ) ;
}

void StopTransitioningBody()
{
  if( isTransitioningBody )
  {
    CharacterManager.instance.StopCoroutine( transitioningBody );
    transitioningBody = null;
  }

}

public IEnumerator TransitioningBody( Sprite sprite, float speed, bool smooth)
{
  for ( int i = 0 ; i < renderers.allBodyRenderers.Count; i++ )
  {
    Image image = renderers.allBodyRenderers[i];
    if( image.sprite == sprite )
    {
      renderers.bodyRenderer = image;
      break;
    }
  }

  if ( renderers.bodyRenderer.sprite != sprite )
  {
    Image image = GameObject.Instantiate( renderers.bodyRenderer.gameObject, renderers.bodyRenderer.transform.parent ).GetComponent<Image>();
    renderers.allBodyRenderers.Add( image );
    renderers.bodyRenderer = image;
    image.color = GlobalFunct.setAlpha( image.color, 0f );
    image.sprite = sprite;
  }

  while( GlobalFunct.TransitionImages( ref renderers.bodyRenderer, ref renderers.allBodyRenderers, speed, smooth) )
  {
    yield return new WaitForEndOfFrame();
  }

  StopTransitioningBody();
}



///// STOP TRANSITION CODE

public void Flip()
{
  characterImage_root.localScale = new Vector3( characterImage_root.localScale.x * -1, 1, 1);
}

public void FaceLeft()
{
  characterImage_root.localScale = Vector3.one;
}

public void FaceRight()
{
  characterImage_root.localScale = new Vector3( -1, 1, 1);
}



  //load prefab set other panel values here
  public Character ( Vector2 initialPosition, string _name, bool enable = true )
  {
    CharacterManager cm = CharacterManager.instance;
    GameObject prefab = Resources.Load("Characters/Character["+_name+"]") as GameObject;
    ob = GameObject.Instantiate( prefab, cm.characterPanel );
    root = ob.transform.GetComponent<RectTransform>();
    characterName = _name;

    //get the renderer(s)
    renderers.allBodyRenderers = new List<Image>();
    renderers.bodyRenderer = ob.GetComponentInChildren<Image>();
    characterImage_root = renderers.bodyRenderer.GetComponent<RectTransform>();
    renderers.allBodyRenderers.Add( renderers.bodyRenderer );
    dialogue = DialogSystem.instance;
    enabled = enable;
    SetPosition( initialPosition );

  }

 [System.Serializable]
 public class Renderers
  {
    public Image bodyRenderer;
    public List<Image> allBodyRenderers;
  }

  public Renderers renderers = new Renderers();
  
}
