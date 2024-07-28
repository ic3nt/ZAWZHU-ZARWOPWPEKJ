using UnityEngine;

public class RandomItems : MonoBehaviour
{
    public GameObject[] Items; // Массив объектов для спавна
    public Transform parentTransform; // Преобразование родительского объекта, куда будут спавниться предметы

    void Start()
    {
        // Убедитесь, что родительский трансформ задан в редакторе или инициализируйте его здесь
        if (parentTransform == null)
        {
            parentTransform = transform; // или вы можете назначить его на другой объект
        }

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

        GameObject spawnedObject = Instantiate(randomObject, parentTransform); // Спавн объекта как дочернего
        spawnedObject.transform.localPosition = Vector3.zero; // Установка локальной позиции на (0, 0, 0)
    }
}