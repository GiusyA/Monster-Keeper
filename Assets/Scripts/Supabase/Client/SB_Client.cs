
using System;
using System.Collections.Generic;
using UnityEngine;
using Postgrest.Responses;
using Supabase;
using Supabase.Gotrue.Exceptions;
using Client = Supabase.Client;

public class SB_Client : MonoBehaviour
{
    [SerializeField]
    bool useLog = true;

    Client client = null;


    #region SignUp
    public Action<string, string> OnPlayerSignUpSucceded { get; set; } = null;

    public Action<string> OnPlayerSignUpFailed { get; set; } = null;
    #endregion


    #region LogInOut
    public Action<string> OnPlayerLogInSucceded { get; set; } = null;

    public Action<string> OnPlayerLogInFailed { get; set; } = null;

    public Action OnPlayerLogOut { get; set; } = null;
    #endregion


    #region Character
    public Action<Dictionary<int, SB_CharacterModel>> OnGetPlayerCharactersFromDB { get; set; } = null;

    public Action<int, string> OnAddNewCharacterInDBSucceded { get; set; } = null;

    public Action<string> OnAddNewCharacterInDBFailed { get; set; } = null;

    public Action<int> OnCharacterDeletedFromDB { get; set; } = null;
    #endregion


    async void Start()
    {
        SupabaseOptions _options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        client = new Client(SB_DB_Informations.DB_Url, SB_DB_Informations.DB_Key, _options);
        await client.InitializeAsync();
    }

    void OnApplicationQuit()
    {
        if (client != null)
        {
            client.Auth.Shutdown();
            client = null;
        }
    }


    public async void SignUp(string _email, string _password)
    {
        if (client != null)
        {
            try
            {
                Supabase.Gotrue.Session _session = await client.Auth.SignUp(_email, _password);
                CheckIfPlayerRegisteredInDB(_session.User.Id);
                OnPlayerSignUpSucceded.Invoke(_email, _password);
            }
            catch (GotrueException _goException)
            {
                string _showedMessage = SB_Utils.GetCleanedGotrueExceptionMessage(_goException);
                OnPlayerSignUpFailed.Invoke(_showedMessage);
            }
        }
    }


    #region LogInOut
    public async void LogIn(string _email, string _password)
    {
        if (client != null)
        {
            try
            {
                await client.Auth.SignInWithPassword(_email, _password);
                OnPlayerLogInSucceded.Invoke(client.Auth.CurrentUser.Id);
            }
            catch (GotrueException _goException)
            {
                string _showedMessage = SB_Utils.GetCleanedGotrueExceptionMessage(_goException);
                OnPlayerLogInFailed.Invoke(_showedMessage);
            }
        }
    }

    public async void LogOut()
    {
        if (client != null)
        {
            try
            {
                await client.Auth.SignOut();
                OnPlayerLogOut.Invoke();
            }
            catch (GotrueException _goException)
            {
                string _showedMessage = SB_Utils.GetCleanedGotrueExceptionMessage(_goException);
                MK_UI_Utils.Log(useLog, _showedMessage);
            }
        }
    }
    #endregion


    #region Player
    async void CheckIfPlayerRegisteredInDB(string _id)
    {
        if (client != null)
        {
            try
            {
                ModeledResponse<SB_PlayerModel> _response = await client.From<SB_PlayerModel>().Get();

                foreach (SB_PlayerModel _playerModel in _response.Models)
                {
                    if (_playerModel.Id == _id)
                        return;
                }

                SB_PlayerModel _currentPlayerModel = new SB_PlayerModel { Id = _id };
                AddNewPlayerInDB(_currentPlayerModel);
            }
            catch (GotrueException _goException)
            {
                string _showedMessage = SB_Utils.GetCleanedGotrueExceptionMessage(_goException);
                MK_UI_Utils.Log(useLog, _showedMessage);
            }
        }
    }

    async void AddNewPlayerInDB(SB_PlayerModel _newPlayerModel)
    {
        if (client != null)
        {
            try
            {
                await client.From<SB_PlayerModel>().Insert(_newPlayerModel);
            }
            catch (GotrueException _goException)
            {
                string _showedMessage = SB_Utils.GetCleanedGotrueExceptionMessage(_goException);
                MK_UI_Utils.Log(useLog, _showedMessage);
            }
        }
    }
    #endregion


    #region Character
    public async void GetPlayerCharactersFromDB(string _playerId)
    {
        if (client != null)
        {
            try
            {
                ModeledResponse<SB_CharacterModel> _response = await client.From<SB_CharacterModel>().Get();
                Dictionary<int, SB_CharacterModel> _playerCharacters = new();

                foreach (SB_CharacterModel _characterModel in _response.Models)
                {
                    if (_characterModel.PlayerID == _playerId)
                        _playerCharacters.Add(_characterModel.Id, _characterModel);
                }

                OnGetPlayerCharactersFromDB.Invoke(_playerCharacters);
            }
            catch (GotrueException _goException)
            {
                string _showedMessage = SB_Utils.GetCleanedGotrueExceptionMessage(_goException);
                MK_UI_Utils.Log(useLog, _showedMessage);
            }
        }
    }

    public async void AddNewCharacterInDB(string _characterName)
    {
        if (client != null)
        {
            if (_characterName != string.Empty)
            {
                try
                {
                    SB_CharacterModel _newCharacterModel = new SB_CharacterModel
                    {
                        Name = _characterName,
                        PlayerID = client.Auth.CurrentUser.Id
                    };

                    ModeledResponse<SB_CharacterModel> _response = await client.From<SB_CharacterModel>().Insert(_newCharacterModel);
                    OnAddNewCharacterInDBSucceded.Invoke(_response.Model.Id, _characterName);
                }
                catch (GotrueException _goException)
                {
                    string _showedMessage = SB_Utils.GetCleanedGotrueExceptionMessage(_goException);
                    OnAddNewCharacterInDBFailed.Invoke(_showedMessage);
                }
            }
            else
                OnAddNewCharacterInDBFailed.Invoke(MK_UI_Utils.CharacterNameIsEmpty);
        }
    }

    public async void DeleteCharacterFromDB(int _characterId)
    {
        if (client != null)
        {
            try
            {
                await client.From<SB_CharacterModel>().Where(_character => _character.Id == _characterId).Delete();
                OnCharacterDeletedFromDB.Invoke(_characterId);
            }
            catch (GotrueException _goException)
            {
                string _showedMessage = SB_Utils.GetCleanedGotrueExceptionMessage(_goException);
                MK_UI_Utils.Log(useLog, _showedMessage);
            }
        }
    }
    #endregion
}

