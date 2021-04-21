using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceButton : MonoBehaviour
{
    public TextMeshProUGUI tmpro;
    public string text
    {
        get
        {
            return tmpro.text;
        }

        set
        {
            tmpro.text = value;
        }
    }

    [HideInInspector]
    public int choiceIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
