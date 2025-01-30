
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MK_UI_CharacterSelection : MonoBehaviour
{
    [SerializeField]
    SB_Client client = null;


    [SerializeField]
    Canvas characterSelectionCanvas = null;


    [SerializeField]
    RectTransform charactersNamesContent = null;

    [SerializeField]
    RectTransform deleteCharactersContent = null;


    [SerializeField]
    TMP_InputField characterNameField = null;

    [SerializeField]
    TMP_Text communicationText = null;

    [SerializeField]
    Button createCharacterButton = null;


    [SerializeField]
    Button logOutButton = null;


    [SerializeField]
    MK_UI_CharacterButton characterNameButtonType = null;

    [SerializeField]
    MK_UI_CharacterButton deleteCharacterButtonType = null;


    Color communicationTextBaseColor = Color.white;
    Dictionary<int, SB_CharacterModel> currentPlayerCharacters = new();


    void Start()
    {
        if (communicationText)
            communicationTextBaseColor = communicationText.color;

        AssignClientLogInOutEvents();
        AssignClientCharacterEvents();
        AssignButtonsOnClickEvents();
    }


    void AssignClientLogInOutEvents()
    {
        if (client)
        {
            client.OnPlayerLogInSucceded += (_playerId) =>
            {
                SetCharacterSelectionCanvasIsActive(true);
                client.GetPlayerCharactersFromDB(_playerId);
            };

            client.OnPlayerLogOut += () =>
            {
                if (communicationText)
                {
                    if (communicationText.IsActive())
                        communicationText.gameObject.SetActive(false);
                }

                SetCharacterSelectionCanvasIsActive(false);
            };
        }
    }

    void AssignClientCharacterEvents()
    {
        if (client)
        {
            client.OnGetPlayerCharactersFromDB += (_playerCharacters) =>
            {
                currentPlayerCharacters = _playerCharacters;
                UpdateCharactersListContents();
            };

            client.OnAddNewCharacterInDBSucceded += (_characterId, _characterName) =>
            {
                CreateCharacterNameButton(_characterId, _characterName);
                CreateDeleteCharacterButton(_characterId);
                currentPlayerCharacters.Add(_characterId, new SB_CharacterModel
                {
                    Id = _characterId,
                    Name = _characterName
                });

                if (communicationText)
                {
                    if (communicationText.IsActive())
                        communicationText.gameObject.SetActive(false);
                }
            };

            client.OnAddNewCharacterInDBFailed += (_error) =>
            {
                MK_UI_Utils.UpdateText(communicationText, _error, Color.red);
            };

            client.OnCharacterDeletedFromDB += (_characterId) =>
            {
                DestroyCharacterButtons(_characterId, charactersNamesContent);
                DestroyCharacterButtons(_characterId, deleteCharactersContent);
                currentPlayerCharacters.Remove(_characterId);
            };
        }
    }

    void AssignButtonsOnClickEvents()
    {
        if (logOutButton)
        {
            logOutButton.onClick.AddListener(() =>
            {
                if (client)
                    client.LogOut();
            });
        }

        if (createCharacterButton)
        {
            createCharacterButton.onClick.AddListener(() =>
            {
                if (client && characterNameField)
                    client.AddNewCharacterInDB(characterNameField.text);
            });
        }
    }


    void SetCharacterSelectionCanvasIsActive(bool _isActive)
    {
        if (characterSelectionCanvas)
            characterSelectionCanvas.gameObject.SetActive(_isActive);
    }


    void ClearContent(RectTransform _content)
    {
        if (_content)
        {
            foreach (Transform _child in _content)
                Destroy(_child.gameObject);
        }
    }

    void ClearCharactersListContents()
    {
        ClearContent(charactersNamesContent);
        ClearContent(deleteCharactersContent);
    }

    void UpdateCharactersListContents()
    {
        ClearCharactersListContents();

        int _charactersCount = currentPlayerCharacters.Count;
        int _characterId = 0;

        foreach (KeyValuePair<int, SB_CharacterModel> _pair in currentPlayerCharacters)
        {
            _characterId = _pair.Key;
            CreateCharacterNameButton(_characterId, _pair.Value?.Name);
            CreateDeleteCharacterButton(_characterId);
        }
    }


    void CreateCharacterNameButton(int _characterId, string _characterName)
    {
        if (characterNameButtonType && charactersNamesContent)
        {
            MK_UI_CharacterButton _characterNameButton = Instantiate(characterNameButtonType, charactersNamesContent);

            if (_characterNameButton)
            {
                _characterNameButton.CharacterId = _characterId;
                TMP_Text _nameText = _characterNameButton.GetComponentInChildren<TMP_Text>();

                if (_nameText)
                    _nameText.text = _characterName;

                _characterNameButton.onClick.AddListener(() =>
                {
                    MK_UI_Utils.UpdateText(communicationText, $"Character Id: {_characterId}", communicationTextBaseColor);
                });
            }
        }
    }

    void CreateDeleteCharacterButton(int _characterId)
    {
        if (deleteCharacterButtonType && deleteCharactersContent)
        {
            MK_UI_CharacterButton _deleteCharacterButton = Instantiate(deleteCharacterButtonType, deleteCharactersContent);

            if (_deleteCharacterButton)
            {
                _deleteCharacterButton.CharacterId = _characterId;

                _deleteCharacterButton.onClick.AddListener(() =>
                {
                    if (client)
                        client.DeleteCharacterFromDB(_characterId);
                });
            }
        }
    }


    void DestroyCharacterButtons(int _characterId, RectTransform _content)
    {
        MK_UI_CharacterButton _button = null;

        if (_content)
        {
            foreach (Transform _child in _content)
            {
                _button = _child.GetComponent<MK_UI_CharacterButton>();

                if (_button)
                {
                    if (_button.CharacterId == _characterId)
                    {
                        Destroy(_child.gameObject);
                        break;
                    }
                }
            }
        }
    }
}
