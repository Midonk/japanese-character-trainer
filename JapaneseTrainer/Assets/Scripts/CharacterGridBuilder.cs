#if UNITY_EDITOR
using System;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

public class CharacterGridBuilder : MonoBehaviour
{
    [SerializeField] private CharacterGridItem _itemPrefab;
    [SerializeField] private GameObject _rangeSelector;
    [SerializeField] private ResponsiveGridLayout _grid;
    [SerializeField] private CharacterLibrary _library;
    [SerializeField] private UnityEvent _onGridRebuilt;

    [ContextMenu("Rebuild Grid")]
    public void RebuildGrid()
    {
        var gridTransform = _grid.transform;
        for (int i = gridTransform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(gridTransform.GetChild(i).gameObject);
#else
            Destroy(gridTransform.GetChild(i).gameObject);
#endif
        }

        for (int i = -1; i < _consonants.Length; i++)
        {
            CreateSelector(_consonants, i);
        }

        var totalGridLength = _library.Characters.Length + _vowels.Length;
        var rowLength = _consonants.Length - 1;
        var columnLength = _vowels.Length;
        for (int j = 0; j < columnLength; j++)
        {
            for (int i = 0; i < totalGridLength; i += columnLength)
            {
                if(i == 0)
                {
                    var vowelIndex = j;
                    CreateSelector(_vowels ,vowelIndex);
                }

                else
                {
                    var characterIndex = (i - columnLength) + j;
                    CreateCharacter(characterIndex);
                }
            }
        }

        _onGridRebuilt?.Invoke();
    }

    private void CreateSelector(string characters, int index)
    {
        GameObject selector;
#if UNITY_EDITOR
        selector = PrefabUtility.InstantiatePrefab(_rangeSelector, _grid.transform) as GameObject;
#else
        selector = Instantiate(_itemPrefab, _grid.transform);
#endif
        var characterDisplay = selector.GetComponentInChildren<TMPro.TMP_Text>();
        characterDisplay.text = index >= 0 ? characters[index].ToString() : string.Empty;
        if(!string.IsNullOrEmpty(characterDisplay.text))
        {
            selector.name = $"{characterDisplay.text} selector";
        }
        
        if(!selector.TryGetComponent<GridToggle>(out var toggle)) return;
        
        toggle.Index = index;
        if(index >= 0) return;

        toggle.Interactable = false;
    }

    private void CreateCharacter(int index)
    {
        CharacterGridItem item;
#if UNITY_EDITOR
        item = PrefabUtility.InstantiatePrefab(_itemPrefab, _grid.transform) as CharacterGridItem;
#else
        item = Instantiate(_itemPrefab, _grid.transform);
#endif
        var latinCharacter = _library.Characters[index].Latin;
        item.AssignCharacterSet(_library.Characters[index]);

        if(!item.TryGetComponent<CharacterToggle>(out var toggle)) return;
            
        toggle.Index = index;
        if(string.IsNullOrEmpty(latinCharacter))
        {
            toggle.Interactable = false;
            return;
        }

        item.name = latinCharacter;
    }

    private const string _vowels = "aiueo";
    private const string _consonants = "/kstnhmyrwgzdbp";
}