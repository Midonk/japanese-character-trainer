using UnityEngine;

public class KeepActiveUGUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField _inputField;

    private void Start() 
    {
        SetActive();
    }

    public void SetActive()
    {
        _inputField.ActivateInputField();
        _inputField.Select();
    }
}