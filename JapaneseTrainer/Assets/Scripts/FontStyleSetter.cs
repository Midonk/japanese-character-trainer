using UnityEngine;

public class FontStyleSetter : MonoBehaviour
{
    public void SetUnderline(TMPro.TMP_Text text)
    {
        text.fontStyle |= TMPro.FontStyles.Underline;
    }
    
    public void RemoveUnderline(TMPro.TMP_Text text)
    {
        text.fontStyle ^= TMPro.FontStyles.Underline;
    }
}
