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
    public GameObject DevelopersSegmentObject; // Объект, который активируем, если игрок в сегменте Developers

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
        playFabId = result.PlayFabId; // сохранить PlayFabID
        NameText.text = playFabId;
        Debug.Log("Успешный вход: " + playFabId);

        // Получаем информацию о пользователе и проверяем сегменты
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnBanned);
        GetPlayerSegments(); // получаем сегменты после успешного входа
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

    private void GetPlayerSegments()
    {
        // Запрос информации о пользователе
        var request = new GetPlayerSegmentsRequest
        {
            PlayFabId = playFabId // используем сохраненный PlayFabID
        };

        PlayFabClientAPI.GetPlayerSegments(request, OnGetPlayerSegmentsSuccess, OnGetPlayerSegmentsFailure);
    }

    private void OnGetPlayerSegmentsSuccess(GetPlayerSegmentsResult result)
    {
        Debug.Log("Сегменты для игрока: " + playFabId);

        // Проверка и вывод сегментов
        if (result.Segments != null && result.Segments.Count > 0)
        {
            foreach (var segment in result.Segments)
            {
                Debug.Log("Имя сегмента: " + segment.Name);
                if (segment.Name == "Developers")
                {
                    DevelopersSegmentObject.SetActive(true); // Включаем объект, если игрок в сегменте Developers
                }
            }
        }
        else
        {
            Debug.Log("Этот игрок не принадлежит ни к одному сегменту.");
        }
    }

    private void OnGetPlayerSegmentsFailure(PlayFabError error)
    {
        Debug.LogError("Ошибка получения сегментов: " + error.GenerateErrorReport());
    }
}
