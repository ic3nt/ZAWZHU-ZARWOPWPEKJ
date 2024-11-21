using UnityEngine;
using Unity.Netcode;

public class AspectSelection : MonoBehaviour
{
    public Aspect[] AvailableAspects; // Доступные аспекты

    public void SelectAspect(int aspectIndex)
    {
        var selectedAspect = AvailableAspects[aspectIndex];
        AspectManager.Instance.SetPlayerAspect(NetworkManager.Singleton.LocalClientId, selectedAspect);

        // Перенос на следующую сцену
        UnityEngine.SceneManagement.SceneManager.LoadScene("TEST");
    }
}
