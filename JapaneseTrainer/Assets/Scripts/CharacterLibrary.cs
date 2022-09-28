using UnityEngine;

[CreateAssetMenu(menuName = "Character Library", fileName = "new Character Library")]
public class CharacterLibrary : ScriptableObject
{
    public CharacterSet[] Characters;
}

[System.Serializable]
public class CharacterSet
{
    public string Latin;
    public string Hiragana;
    //public Sprite Katakana;
    public bool Enable;
}