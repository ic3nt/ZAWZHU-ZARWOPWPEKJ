using UnityEngine;
using UnityEngine.SceneManagement;

public class AspectSelector : MonoBehaviour
{
    public Aspect1[] AvailableAspects;

    public void SelectAspect(int aspectIndex)
    {
        var aspect = AvailableAspects[aspectIndex];
        AspectManager1.Instance.AssignAspectToPlayer("Player1", aspect); // Для одного игрока

        SceneManager.LoadScene("TEST"); // Перейти на игровую сцену
    }
}
