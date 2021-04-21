using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Responsible for adding and maintaining characters in the scene.
public class CharacterManager : MonoBehaviour
{

    public static CharacterManager instance;

    //Panel References
    public RectTransform characterPanel;

    // List of All Characters in the scene.
    public List<Character> characters = new List<Character>();

    public Dictionary<string, int> characterDictionary = new Dictionary<string, int>();

    void Awake()
    {
      instance = this;
    }

    //Used to search charcterDictionary and return the specified character from characters
    public Character GetCharacter( string characterName, bool createCharacterIfDoesNotExist = true, bool enabledOnStart = true )
    {
      int index = -1;
      if( characterDictionary.TryGetValue(characterName, out index) )
      {
        return characters[index];
      }

      else if ( createCharacterIfDoesNotExist )
      {
        return CreateCharacter( characterName, enabledOnStart );
      }

      return null;
    }

    public Character CreateCharacter( Vector2 initialPosition, string characterName, bool enabledOnStart = true )
    {
        Character newCharacter = new Character( initialPosition, characterName, enabledOnStart );
        characterDictionary.Add( characterName, characters.Count);
        characters.Add( newCharacter );

        return newCharacter;
    }

      public Character CreateCharacter( string characterName, bool enabledOnStart = true )
    {
        return CreateCharacter( new Vector2(0,0), characterName, enabledOnStart );
    }

}
