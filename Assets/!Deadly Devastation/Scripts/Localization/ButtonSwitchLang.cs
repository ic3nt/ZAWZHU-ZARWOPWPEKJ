using UnityEngine;

public class ButtonSwitchLang : MonoBehaviour
{
    [SerializeField]
    private LocalizationManager localizationManager;

    private void Awake()
    {
        if (localizationManager == null)
        {
            GameObject managerObject = GameObject.FindWithTag("LocalizationManager");
            if (managerObject != null)
            {
                localizationManager = managerObject.GetComponent<LocalizationManager>();
            }
            else
            {
                Debug.LogError("No object with tag 'LocalizationManager' found in the scene!");
            }
        }
    }

    public void OnButtonClick()
    {
        if (localizationManager != null)
        {
            localizationManager.CurrentLanguage = name;
            Debug.Log("Language changed to: " + name);
        }
        else
        {
            Debug.LogError("LocalizationManager is not assigned!");
        }
    }
}
