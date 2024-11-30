using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenSceneOnFirstRun : MonoBehaviour
{
    private void Start()
    {
        if(PlayerPrefs.GetInt("isFirstRun", 0) == 0) // Проверяем, была ли игра запущена ранее
        {
            PlayerPrefs.SetInt("isFirstRun", 1); // Устанавливаем флаг о том, что игра уже была запущена
            SceneManager.LoadScene("YourSceneName"); // Загружаем сцену
        }
    }
}
