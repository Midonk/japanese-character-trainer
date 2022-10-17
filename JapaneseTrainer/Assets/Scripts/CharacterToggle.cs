using UnityEngine;

public class CharacterToggle : GridToggle
{
    [SerializeField] private CharacterGridItem _gridItem;

    public CharacterLibrary Library 
    {
        get => _library; 
        set
        {
            _library = value;
            SetupWithLibrary();
        } 
    }

    private void OnEnable() 
    {
        SetupWithLibrary();
    }

    private void SetupWithLibrary()
    {
        if(!Library) return;

        _toggle.SetIsOnWithoutNotify(Library.IsEnabled(Index));
        _gridItem.AssignCharacterSet(Library.Characters[Index]);
    }

    public override void ToggleItem(bool value)
    {
        base.ToggleItem(value);
        Library?.ToggleCharacter(Index, value);
    }

    private CharacterLibrary _library;
}