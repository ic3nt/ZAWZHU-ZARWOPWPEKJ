using PlayFab;
using PlayFab.ServerModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tag : MonoBehaviour
{
    private void Start()
    {
        AddPlayerTags("E058D8D081DDAF28", new List<string> { "Developer" });
    }

    public void AddPlayerTags(string playFabId, List<string> tags)
    {
        var request = new AddPlayerTagRequest
        {
            PlayFabId = playFabId,
            TagName = tags[0]
        };

        PlayFabServerAPI.AddPlayerTag(request, OnAddTagsSuccess, OnAddTagsError);
    }

    private void OnAddTagsSuccess(AddPlayerTagResult result)
    {
        Debug.Log("Теги успешно добавлены игроку.");
    }

    private void OnAddTagsError(PlayFabError error)
    {
        Debug.LogError("Ошибка при добавлении тегов: " + error.GenerateErrorReport());
    }
}