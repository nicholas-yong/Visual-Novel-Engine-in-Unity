using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextArchitect 
{
    // Dictionary prevents multiple architects from working on the same line of text
    private static Dictionary<TextMeshProUGUI, TextArchitect> activeArchitect = new Dictionary<TextMeshProUGUI, TextArchitect>();

   // What is trying to be built ( IE: The text being inputted in this case)
   public string targetText;

   // Any text that is already inside the textbox prior to the TextArchitect being created
   private string preText;

   // How many characters can be built per Frame ( 1 by Default )
   public int charactersPerFrame = 1;

   [Range(1f, 60f)]
   // The speed at which characters are built
   public float speed = 1f;
   // Whether a frame should be skipped. Basically speeds up text building
   public bool skip = false;

   public bool isConstructing
   {
       get
       {
           return buildProcess != null;
       }
   }

   Coroutine buildProcess = null;

   public TextMeshProUGUI tmpro;

   public TextArchitect( TextMeshProUGUI tmpro, string targetText, string preText = "", int charactersPerFrame = 1, float speed = 1f )
   {
       this.targetText = targetText;
       this.preText = preText;
       this.charactersPerFrame = charactersPerFrame;
       this.speed = speed;
       this.tmpro = tmpro;
       
       buildProcess = DialogSystem.instance.StartCoroutine(Construction());

   }

   public void Stop()
   {
       if( isConstructing)
       {
           DialogSystem.instance.StopCoroutine(buildProcess);
       }
       buildProcess = null;
   }

   IEnumerator Construction()
   {
     int runsthisFrame = 0;
     
      tmpro.text = "";
      tmpro.text += preText;

      tmpro.ForceMeshUpdate();
      TMP_TextInfo txt_info = tmpro.textInfo;
      int vis = txt_info.characterCount;

      tmpro.text += targetText;

      tmpro.ForceMeshUpdate();
      txt_info = tmpro.textInfo;
      int max = txt_info.characterCount;

      tmpro.maxVisibleCharacters = vis;

      //temporary cache of cpf per construction sequence.
      int cpf = charactersPerFrame;

      while( vis < max )
      {
          if( skip )
          {
              speed = 1;
              cpf = charactersPerFrame < 5 ? 5 : charactersPerFrame + 3;
          }
          while( runsthisFrame < cpf )
          {
              vis++;
              tmpro.maxVisibleCharacters = vis;
              runsthisFrame++;
          }

          runsthisFrame = 0;
          yield return new WaitForSeconds( 0.01f * speed );
      }

      buildProcess = null;

   }

   void Initate()
   {
       //check if an architect for this text object is already running. 
       //If it is, terminate it.
       //Do not allow more than one architect to affect the same text.

       TextArchitect existingArchitect = null;

       if( activeArchitect.TryGetValue( tmpro, out existingArchitect ) )
       {
           existingArchitect.Terminate();
       }

       buildProcess = DialogSystem.instance.StartCoroutine(Construction());
       activeArchitect.Add(tmpro, this);


   }

    //Terminates the current architect. Stops text generation and removes from it the active architect cache
    void Terminate()
    {
        activeArchitect.Remove(tmpro);
        if( isConstructing )
        {
            DialogSystem.instance.StopCoroutine(buildProcess);
        }

        buildProcess = null;
    }

    //set max visible characters to max and terminate the Architext afterwards
   public void forceFinish()
    {
        tmpro.maxVisibleCharacters = tmpro.text.Length;
        Terminate();
    }

    //Function expects a new targetText to build, as well as a potential preText field for additive use
    public void Renew( string targ, string pre )
    {
        targetText = targ;
        preText = pre;
        //reset skip
        skip = false;

        if( isConstructing )
        {
            DialogSystem.instance.StopCoroutine(buildProcess);
        }

        //start the coroutine again after stopping it

        buildProcess = DialogSystem.instance.StartCoroutine(Construction());

    }

    public void ShowText( string text )
    {
        if(isConstructing)
        {
            DialogSystem.instance.StopCoroutine(buildProcess);
        }

        targetText = text;
        tmpro.text = text;

        tmpro.maxVisibleCharacters = tmpro.text.Length;

        //update the target of the dialogue system only if this architect is being used directly by the dialogue system

        if(tmpro == DialogSystem.instance.speechText)
        {
            DialogSystem.instance.targetSpeech = text;
        }

    }
}
