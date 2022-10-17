using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Character Library", fileName = "new Character Library")]
public class CharacterLibrary : ScriptableObject
{
    public CharacterSet[] Characters;
    
    public void ToggleCharacter(string latin, bool value)
    {
        var setIndex = Array.FindIndex(Characters, set => set.Latin.Equals(latin));
        Characters[setIndex].Enable = value;
    }
    
    public void ToggleCharacter(int index, bool value)
    {
        Characters[index].Enable = value;
    }

    public bool IsEnabled(string latin)
    {
        var setIndex = Array.FindIndex(Characters, set => set.Latin.Equals(latin));
        return Characters[setIndex].Enable;
    }
    
    public bool IsEnabled(int index) => Characters[index].Enable;

    public int EnabledCharacters => Characters.Count(set => set.Enable);
}

[System.Serializable]
public class CharacterSet
{
    public string Latin;
    [FormerlySerializedAs("Hiragana")]
    public string Native;
    //public string Katakana;
    public bool Enable;
}