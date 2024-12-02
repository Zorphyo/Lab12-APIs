using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEditor.PackageManager;

public interface ILogin
{
    void Login(System.Action<LoginResult> onSuccess, System.Action<PlayFabError> onFailure);
}

public class DeviceLogin : ILogin
{
    private string deviceId;

    public DeviceLogin (string deviceId)
    {
        this.deviceId = deviceId;
    }

    public void Login(System.Action<LoginResult> onSuccess, System.Action<PlayFabError> onFailure)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = deviceId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, onSuccess, onFailure);
    }
}

public class LoginManager
{
    private ILogin loginMethod;

    public void SetLoginMethod(ILogin method)
    {
        loginMethod = method;
    }

    public void Login(System.Action<LoginResult> onSuccess, System.Action<PlayFabError> onFailure)
    {
        if (loginMethod != null)
        {
            loginMethod.Login(onSuccess, onFailure);
        }

        else 
        {
            Debug.LogError("No login method set!");
        }
    }
}

public class PlayFabManager
{
    private LoginManager loginManager;
    private string savedEmailKey = "SavedEmail";
    private string userEmail;

    private void Start()
    {
        loginManager = new LoginManager();

        if (PlayerPrefs.HasKey(savedEmailKey))
        {
            string savedEmail = PlayerPrefs.GetString(savedEmailKey);
            EmailLoginButtonClicked(savedEmail, "SavedPassword");
        }
    }

    public void EmailLoginButtonClicked(string email, string password)
    {
        userEmail = email;
        //loginManager.SetLoginMethod(new EmailLogin(email, password));
        loginManager.Login(OnLoginSuccess, OnLoginFailure);
    }

    public void DeviceIDLoginButtonClicked(string deviceId)
    {
        loginManager.SetLoginMethod(new DeviceLogin(deviceId));
        loginManager.Login(OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");

        if (!string.IsNullOrEmpty(userEmail))
        {
            PlayerPrefs.SetString(savedEmailKey, userEmail);
        }

        LoadPlayerData(result.PlayFabId);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("Login failed: " + error.ErrorMessage);
    }

    private void LoadPlayerData(string playFabId)
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = playFabId
        };

        PlayFabClientAPI.GetUserData(request, OnDataSuccess, OnDataFailure);
    }

    private void OnDataSuccess(GetUserDataResult result)
    {
        Debug.Log("Player data loaded successfully");
    }

    private void OnDataFailure(PlayFabError error)
    {
        Debug.LogError("Failed to load player data: " + error.ErrorMessage);
    }
}
