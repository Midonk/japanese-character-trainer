using UnityEngine;

public class CharacterGridItem : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _characterDisplay;
    [SerializeField] private TMPro.TMP_Text _traductionDisplay;

    private string _nativeCharacter;

    public void AssignCharacterSet(CharacterSet characterSet)
    {
        _nativeCharacter = characterSet.Native;
        _characterDisplay.SetText(_nativeCharacter);
        _traductionDisplay.SetText(characterSet.Latin);
    }
}