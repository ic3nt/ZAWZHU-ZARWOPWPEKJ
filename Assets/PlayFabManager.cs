using UnityEngine;
using PlayFab;
using PlayFab.ProfilesModels;
using PlayFab.AdminModels;
using PlayFab.AuthenticationModels;
using PlayFab.ClientModels;
using System.Collections;

public class PlayFabManager : MonoBehaviour
{
    public GameObject ErrorLoginWindow;
    public GameObject BanWindow;
    public GameObject Buttons;

    private void Start()
    {
        LoginWithAnonymous();
    }

    private void LoginWithAnonymous()
    {
        string customId = SystemInfo.deviceUniqueIdentifier;
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Успешный вход: " + result.PlayFabId);
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnBanned);
    }

    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        if (result.AccountInfo.TitleInfo.isBanned == true)
        {
            Debug.Log("Игрок забанен.");
        }
        else
        {
            Debug.Log("Игрок успешно авторизован и не забанен.");
        }
    }

    private void OnBanned(PlayFabError fabError)
    {
        Debug.Log("Игрок забанен.");
        StartCoroutine(BanWindowWaitForSecondCoroutine());
        Buttons.SetActive(false);
    }

    private void OnLoginError(PlayFabError error)
    {
        Debug.LogError("Ошибка входа: " + error.ErrorMessage);


        if (error.Error == PlayFabErrorCode.AccountBanned)
        {
            OnBanned(error);
        }
        else
        {
            StartCoroutine(ErrorWindowWaitForSecondCoroutine());
            Buttons.SetActive(false);
        }
    }


    private IEnumerator ErrorWindowWaitForSecondCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        ErrorLoginWindow.SetActive(true);
    }

    private IEnumerator BanWindowWaitForSecondCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        BanWindow.SetActive(true);
    }
}