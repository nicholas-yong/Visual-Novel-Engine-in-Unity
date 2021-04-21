using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceScreenTesting : MonoBehaviour
{
   public string title = "I like...";

   void Update()
   {
       if(Input.GetKeyDown(KeyCode.Alpha1))
       {
           ChoiceScreen.Show(title, "one", "two");
       }
   }
}
