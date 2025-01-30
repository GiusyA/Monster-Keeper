
using UnityEngine;
using TMPro;

public static class MK_UI_Utils
{
    #region Login
    public static string SignUpSuccededText = "New account successfully registered!";
    #endregion

    #region Character
    public static string CharacterNameIsEmpty = "New character must have a name!";
    #endregion

    public static void Log(bool _useLog, string _message)
    {
        if (_useLog)
            Debug.Log(_message);
    }

    public static void UpdateText(TMP_Text _text, string _newText, Color _newColor, bool _isActive = true)
    {
        if (_text)
        {
            _text.text = _newText;
            _text.color = _newColor;

            if (!_text.IsActive())
                _text.gameObject.SetActive(_isActive);
        }
    }
}
