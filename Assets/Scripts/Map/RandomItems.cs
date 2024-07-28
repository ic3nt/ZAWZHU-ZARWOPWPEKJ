using UnityEngine;

public class RandomItems : MonoBehaviour
{
    public GameObject[] Items; // Массив объектов для спавна
    public Transform parentTransform; // Преобразование родительского объекта, куда будут спавниться предметы

    private Transform CurTr;
    
    void Start()
    {

        CurTr = GetComponent<Transform>();
        SpawnObject();
    }

    void SpawnObject()
    {
        if (Items.Length == 0) // Проверка на наличие предметов
        {
            Debug.LogError("Нет предметов для спавна!");
            return;
        }

        int randomIndex = Random.Range(0, Items.Length); // Генерация случайного индекса
        GameObject randomObject = Items[randomIndex]; // Выбор случайного объекта из массива

        GameObject spawnedObject = Instantiate(randomObject, CurTr.position, CurTr.rotation, parentTransform); // Спавн объекта как дочернего


       
    }
}