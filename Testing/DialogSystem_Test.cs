using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSystem_Test : MonoBehaviour
{

    DialogSystem dialogue;
    // Start is called before the first frame update
    void Start()
    {
        dialogue = DialogSystem.instance;
    }

    public string[] s = new string[]
    {
      "test:YongYong",
      "Hello",
      "There"
    };
    int index = 0;
    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.Space))
      {
        if ( !dialogue.isSpeaking || dialogue.isWaitingForUserInput)
        {
          if (index >= s.Length)
          {
            return;
          }
          Say(s[index]);
          index++;
        }
      }

    }


    void Say(string s)
    {
      string[] parts = s.Split(':');
      string speech = parts[0];
      string speaker = ( parts.Length >= 2) ? parts[1]: "";
      speaker = ( speaker != "" ) ? speaker + ":": speaker;
      dialogue.Say( speech, speaker);
    }
}
