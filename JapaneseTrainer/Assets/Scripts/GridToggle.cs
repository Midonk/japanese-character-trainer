using UnityEngine;
using UnityEngine.UI;

public class GridToggle : MonoBehaviour 
{
    [SerializeField] protected Toggle _toggle;
    
    public Toggle Toggle => _toggle;
    public int Index;
    public bool Interactable 
    { 
        get => _toggle.interactable;
        set => _toggle.interactable = value;
    }

    public virtual void ToggleItem(bool value)
    {
        if(!Interactable) return;
        
        _toggle.SetIsOnWithoutNotify(value);
    }
}