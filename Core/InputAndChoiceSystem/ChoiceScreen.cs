using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceScreen : MonoBehaviour
{
    public static ChoiceScreen instance;

    public GameObject root;

    public TitleHeader _header;

    public static TitleHeader header
    {
        get
        {
            return instance._header;
        }
    }
    public ChoiceButton chociePrefab;

    public static List<ChoiceButton> choices = new List<ChoiceButton>();

    public VerticalLayoutGroup layoutGroup;
    
    void Awake()
    {
        instance = this;
        Hide();
    }

    public static void Hide()
    {
        if( isShowingChoices )
        {
            instance.StopCoroutine( showingChoices );
        }
        showingChoices = null;

        header.Hide();

        ClearAllCurrentChoices();

        instance.root.SetActive(false);

    }

    public static void Show(string title, params string[] choices)
    {
        instance.root.SetActive(true);

        // we essentially reset the header and the choice menu here
        if( title != "")
        {
            header.Show( title );
        }
        else
        {
            header.Hide();
        }

        if( isShowingChoices )
        {
            instance.StopCoroutine(showingChoices);
        }

        ClearAllCurrentChoices();

        showingChoices = instance.StartCoroutine(ShowingChoices( choices ));

    }

    public static bool isWaitingForChoiceToBeMade
    {
        get
        {
            return isShowingChoices && !lastChoiceMade.hasBeenMade;
        }
    }

    static void ClearAllCurrentChoices()
    {
        foreach( ChoiceButton b in choices)
        {
            DestroyImmediate(b.gameObject);
        }

        choices.Clear();

    }

    public static bool isShowingChoices
    {
        get
        {
            return showingChoices != null;
        }
    }

    public static Coroutine showingChoices = null;

    public static IEnumerator ShowingChoices( string[] choices )
    {
        yield return new WaitForEndOfFrame();       //allow the header to begin appearing if it will be present

        lastChoiceMade.Reset();

        while(header.isRevealing)
        {
            yield return new WaitForEndOfFrame();
        }

        for( int i = 0 ; i < choices.Length; i++ )
        {
            CreateChoice(choices[i]);
        }

        SetLayoutSpacing();

        while( isWaitingForChoiceToBeMade )
        {
            yield return new WaitForEndOfFrame();
        }

        Hide();
    }

    public static void SetLayoutSpacing()
    {
        int i = choices.Count;

        if( i <= 3 )
        {
            instance.layoutGroup.spacing = 7;
        }
        else
        {
            instance.layoutGroup.spacing = 3;
        }
    }

    public static void CreateChoice( string choice )
    {
        GameObject gb = Instantiate( instance.chociePrefab.gameObject, instance.chociePrefab.transform.parent );
        gb.SetActive(true);
        ChoiceButton b = gb.GetComponent<ChoiceButton>();

        b.text = choice;
        b.choiceIndex = choices.Count;

        choices.Add(b);
    }


    [System.Serializable]
    public class CHOICE
    {
        public int index = -1;      //index of the choice button that we clicked
        public string title = "";   //string of the choice button that we ticked

        public bool hasBeenMade
        {
            get
            {
                return title != "" && index != -1;
            }
            
        }

        public void Reset()
        {
            title = "";
            index = -1;
        }
    }

    public CHOICE choice = new CHOICE();
    public static CHOICE lastChoiceMade
    {
            
        get
        {
            return instance.choice;
        }
    }

    public void MakeChoice( ChoiceButton button )
    {
        choice.index = button.choiceIndex;
        choice.title = button.text;

    }


    
}
