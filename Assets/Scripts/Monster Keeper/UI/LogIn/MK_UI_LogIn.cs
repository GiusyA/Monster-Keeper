
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MK_UI_Login : MonoBehaviour
{
    [SerializeField]
    SB_Client client = null;


    [SerializeField]
    Canvas logInCanvas = null;

    [SerializeField]
    TMP_InputField emailField = null;

    [SerializeField]
    TMP_InputField passwordField = null;

    [SerializeField]
    TMP_Text communicationText = null;

    [SerializeField]
    Button signUpButton = null;

    [SerializeField]
    Button logInButton = null;

    Color communicationTextBaseColor = Color.white;


    void Start()
    {
        if (communicationText)
            communicationTextBaseColor = communicationText.color;

        AssignClientSignUpEvents();
        AssignClientLogInOutEvents();
        AssignButtonsOnClickEvent();
    }


    void AssignClientSignUpEvents()
    {
        if (client)
        {
            client.OnPlayerSignUpSucceded += (_email, _password) =>
            {
                MK_UI_Utils.UpdateText(communicationText, MK_UI_Utils.SignUpSuccededText, communicationTextBaseColor);
            };

            client.OnPlayerSignUpFailed += (_error) =>
            {
                MK_UI_Utils.UpdateText(communicationText, _error, Color.red);
            };
        }
    }

    void AssignClientLogInOutEvents()
    {
        if (client)
        {
            client.OnPlayerLogInSucceded += (_playerId) =>
            {
                if (communicationText)
                {
                    if (communicationText.IsActive())
                        communicationText.gameObject.SetActive(false);
                }

                SetLogInCanvasIsActive(false);
            };

            client.OnPlayerLogInFailed += (_error) =>
            {
                MK_UI_Utils.UpdateText(communicationText, _error, Color.red);
            };

            client.OnPlayerLogOut += () =>
            {
                SetLogInCanvasIsActive(true);
            };
        }
    }

    void AssignButtonsOnClickEvent()
    {
        if (signUpButton)
        {
            signUpButton.onClick.AddListener(() =>
            {
                if (client && emailField && passwordField)
                    client.SignUp(emailField.text, passwordField.text);
            });
        }

        if (logInButton)
        {
            logInButton.onClick.AddListener(() =>
            {
                if (client && emailField && passwordField)
                    client.LogIn(emailField.text, passwordField.text);
            });
        }
    }


    void SetLogInCanvasIsActive(bool _isActive)
    {
        if (logInCanvas)
            logInCanvas.gameObject.SetActive(_isActive);
    }
}
