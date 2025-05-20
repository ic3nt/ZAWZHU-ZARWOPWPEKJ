using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Настройки количества этажей")]
    public int totalUpperFloors = 30;
    public int totalLowerFloors = 20;

    [Header("Обычные этажи")]
    public List<GameObject> upperNormalFloors;
    public List<GameObject> lowerNormalFloors;

    [Header("Боссы")]
    public List<GameObject> bossFloors;

    public Transform spawnPoint;
    public GameObject currentFloor;

    public int currentFloorNumber = 0;     // Счётчик всех сгенерированных этажей (начиная с 1)
    public int currentFloorIndex = 0;     // Номер текущего этажа по логике (может быть отрицательным)

    private void Start()
    {
        currentFloorIndex = totalUpperFloors;
    }

    private void OnEnable()
    {
        RoundEvents.OnGenerationUpdated += HandleGenerationUpdate;
    }

    private void OnDisable()
    {
        RoundEvents.OnGenerationUpdated -= HandleGenerationUpdate;
    }

    private void HandleGenerationUpdate(GenerationInfo info)
    {
        if (info.Stage == RoundEvents.GenerationStage.Started)
        {
            currentFloorIndex = info.Floor;
            currentFloorNumber++;
            SpawnFloor(currentFloorIndex);
        }
    }


    public void SpawnFloor(int floorIndex)
    {
        if (currentFloor != null)
            Destroy(currentFloor);

        bool isBoss = (floorIndex % 10 == 0);
        bool isUpper = floorIndex >= 0;

        GameObject prefabToSpawn = null;

        if (isBoss && bossFloors.Count > 0)
        {
            prefabToSpawn = bossFloors[Random.Range(0, bossFloors.Count)];
        }
        else
        {
            var normalList = isUpper ? upperNormalFloors : lowerNormalFloors;
            if (normalList.Count > 0)
            {
                prefabToSpawn = normalList[Random.Range(0, normalList.Count)];
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"Нет {(isBoss ? "босс" : "обычных")} {(isUpper ? "верхних" : "нижних")} этажей для спавна!");
            return;
        }

        currentFloor = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);

        Transform spawnOrigin = currentFloor.transform.Find("SpawnOrigin");
        if (spawnOrigin != null)
        {
            Vector3 offset = currentFloor.transform.position - spawnOrigin.position;
            currentFloor.transform.position = spawnPoint.position + offset;
        }
        else
        {
            Debug.LogWarning($"[ChunkManager] У префаба {prefabToSpawn.name} нет SpawnOrigin!");
            currentFloor.transform.position = spawnPoint.position;
        }

        Debug.Log($"[ChunkManager] Счёт этажей: {currentFloorNumber}, Этаж: {floorIndex} — {(isBoss ? "БОСС" : "обычный")} ({(isUpper ? "верхний" : "нижний")})");
    }

    public int GetCurrentFloorIndex()
    {
        return currentFloorIndex;
    }

    public int GetGeneratedFloorCount()
    {
        return currentFloorNumber;
    }
}
