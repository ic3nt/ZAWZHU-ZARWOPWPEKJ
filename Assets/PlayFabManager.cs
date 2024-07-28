using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using System.Collections;

public class PlayFabManager : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public GameObject ErrorLoginWindow;
    public GameObject BanWindow;
    public GameObject Buttons;
    public GameObject DevelopersSegmentObject;

    private string playFabId;

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
        playFabId = result.PlayFabId;
        NameText.text = playFabId;
        Debug.Log("Successful login: " + playFabId);

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnBanned);
        GetPlayerSegments();
    }

    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        if (result.AccountInfo.TitleInfo.isBanned == true)
        {
            Debug.Log("The player is banned.");
        }
        else
        {
            Debug.Log("The player is successfully authorized and not banned.");
        }
    }

    private void OnBanned(PlayFabError fabError)
    {
        Debug.Log("The player is banned.");
        StartCoroutine(BanWindowWaitForSecondCoroutine());
        Buttons.SetActive(false);
    }

    private void OnLoginError(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.ErrorMessage);

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

    private void GetPlayerSegments()
    {
        var request = new GetPlayerSegmentsRequest
        {
            PlayFabId = playFabId
        };

        PlayFabClientAPI.GetPlayerSegments(request, OnGetPlayerSegmentsSuccess, OnGetPlayerSegmentsFailure);
    }

    private void OnGetPlayerSegmentsSuccess(GetPlayerSegmentsResult result)
    {
        Debug.Log("Segments for the player: " + playFabId);

        if (result.Segments != null && result.Segments.Count > 0)
        {
            foreach (var segment in result.Segments)
            {
                Debug.Log("Segment name: " + segment.Name);
                if (segment.Name == "Developers")
                {
                    DevelopersSegmentObject.SetActive(true);
                }
            }
        }
        else
        {
            Debug.Log("This player does not belong to any segment.");
        }
    }

    private void OnGetPlayerSegmentsFailure(PlayFabError error)
    {
        Debug.LogError("Error receiving segments: " + error.GenerateErrorReport());
    }
}
