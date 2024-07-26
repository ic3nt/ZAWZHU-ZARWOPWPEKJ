using UnityEngine;

public class ButtonSwitchLang: MonoBehaviour
{
    [SerializeField]
    private LocalizationManager localizationManager;

    void OnButtonClick()
    {
		localizationManager.CurrentLanguage = name;
        Debug.Log("Change");
    }
}