using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogSystem: MonoBehaviour
{

    public static DialogSystem instance;
    public ELEMENTS elements;
    public bool isWaitingForUserInput;

    void Awake()
    {
      instance = this;
    }
  

    public void Say( string speech, string speaker = "" , bool additive = false)
    {
      StopSpeaking();

      if( additive )
      {
        speechText.text = targetSpeech;
      }
      speaking = StartCoroutine(Speaking( speech, additive, speaker));
    }


    public void StopSpeaking()
    {
      if(  isSpeaking )
      {
        StopCoroutine( speaking );
      }

      if( architect != null && architect.isConstructing )
      {
        architect.Stop();
      }

      speaking = null;
    }

    public bool isSpeaking
    {
      get
      {
        return speaking != null;
      }
    }
    public string targetSpeech = "";
    Coroutine speaking = null;

    TextArchitect architect = null;

    public TextArchitect currentArchitect()
    {
      return architect;
    }
    IEnumerator Speaking( string speech, bool additive  = false , string speaker = "")
    {
      speechPanel.SetActive( true );

      // Create a new Text Architect Here

      string additiveSpeech = additive ? speechText.text : "";
      targetSpeech = additiveSpeech + speech;
      if( architect == null)
      {
          architect = new TextArchitect( speechText, speech, additiveSpeech );
      }
      else
      {
        architect.Renew( speech, additiveSpeech );
      }
  
      speakerNameText.text = DetermineSpeaker( speaker );

      isWaitingForUserInput = false;

      while ( architect.isConstructing )
      {
        if( Input.GetKey( KeyCode.Space))
        {
          architect.skip = true;
        }
        yield return new WaitForEndOfFrame();
      }

      //skipping will not finish string
      isWaitingForUserInput = true;
      while( isWaitingForUserInput )
      {
        yield return new WaitForEndOfFrame();
      }

      StopSpeaking();
    }
   string DetermineSpeaker(string s)
	{
		string retVal = speakerNameText.text;//default return is the current name
		if (s != speakerNameText.text && s != "")
			retVal = (s.ToLower().Contains("narrator")) ? "" : s;

		return retVal;
	}
    public void Close()
    {
      StopSpeaking();
      speechPanel.SetActive ( false );
    }


    [System.Serializable]
    // This is a reference to all Dialog related elements in the UI
    public class ELEMENTS
    {
      public GameObject speechPanel;
      public TextMeshProUGUI speakerNameText;
      public TextMeshProUGUI speechText;
    }
      public GameObject speechPanel
      {
          get
          {
              return elements.speechPanel;
          }
      }

      public TextMeshProUGUI speakerNameText
      {
          get
          {
              return elements.speakerNameText;
          }
      }

      public TextMeshProUGUI speechText
      {
          get
          {
              return elements.speechText;
          }
      }

  }
