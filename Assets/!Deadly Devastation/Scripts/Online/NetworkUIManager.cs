using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NetworkUIManager : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;
    public GameObject connectionUI;
    public GameObject gameUI;
    public TMP_Text debugText;
    public TMP_Text ipLobbyText;

    public GameObject playerPrefab;  // Префаб для игрока или другого объекта
    public List<Transform> spawnPoints;  // Список точек спавна

    private static List<int> occupiedSpawnIndexes = new List<int>();  // Индексы занятых точек спавна

    void Start()
    {
        connectionUI.gameObject.SetActive(true);
        gameUI.gameObject.SetActive(false);
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
        serverButton.onClick.AddListener(StartServer);

        debugText.text = "Network UI Manager initialized. Ready to start.";
        StartHost();
    }

    void StartHost()
    {
        debugText.text = "Attempting to start Host...";

        if (NetworkManager.Singleton.StartHost())
        {
            ipLobbyText.text = "CONNECTED TO LOBBY | HOST";
            debugText.text = "Host started successfully. Listening for clients...";
            connectionUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(true);
            ResetOccupiedSpawnPoints(); // Очищаем занятые точки при старте хоста
            SpawnPlayer(); // Спавним игрока для хоста
        }
        else
        {
            debugText.text = "Failed to start Host!";
        }
    }

    void StartClient()
    {
        string ipAddress = ipInputField.text;
        debugText.text = $"Attempting to start Client. Connecting to IP: {ipAddress}";

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ipAddress, 7777);

        if (NetworkManager.Singleton.StartClient())
        {
            ipLobbyText.text = $"CONNECTED TO LOBBY: {ipAddress} | CLIENT";
            debugText.text = $"Client started successfully. Connecting to {ipAddress}...";
            connectionUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(true);
            SpawnPlayer(); // Спавним игрока для клиента
        }
        else
        {
            debugText.text = "Failed to start Client!";
        }
    }

    void StartServer()
    {
        debugText.text = "Attempting to start Server...";
        if (NetworkManager.Singleton.StartServer())
        {
            debugText.text = "Server started successfully. Waiting for clients to connect...";
            connectionUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(true);
            ResetOccupiedSpawnPoints(); // Очищаем занятые точки при старте сервера
        }
        else
        {
            debugText.text = "Failed to start Server!";
        }
    }

    // Метод для спавна игрока
    void SpawnPlayer()
    {
        // Проверяем, есть ли доступные точки спавна
        if (spawnPoints.Count == 0)
        {
            debugText.text = "No spawn points available.";
            return;
        }

        // Выбираем случайную точку спавна
        int spawnIndex = GetAvailableSpawnIndex();

        debugText.text = $"Available spawn point index: {spawnIndex}";  // Логирование индекса

        if (spawnIndex == -1)
        {
            debugText.text = "No available spawn points.";
            return;
        }

        debugText.text = $"Spawning player at spawn point {spawnIndex + 1}";

        // Получаем префаб игрока из NetworkManager
        GameObject player = Instantiate(playerPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
        NetworkObject networkObject = player.GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            debugText.text = "Player prefab does not have a NetworkObject!";
            return;
        }

        // Спавним игрока
        networkObject.Spawn();

        // Отмечаем точку как занятую
        occupiedSpawnIndexes.Add(spawnIndex);

        debugText.text = $"Player spawned at spawn point {spawnIndex + 1}";
    }

    // Получаем индекс доступной точки спавна
    int GetAvailableSpawnIndex()
    {
        List<int> availableSpawnIndexes = new List<int>();

        // Ищем все свободные точки спавна
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (!occupiedSpawnIndexes.Contains(i))
            {
                availableSpawnIndexes.Add(i);
            }
        }

        // Если свободных точек нет
        if (availableSpawnIndexes.Count == 0)
        {
            return -1;
        }

        // Возвращаем случайную доступную точку
        return availableSpawnIndexes[Random.Range(0, availableSpawnIndexes.Count)];
    }

    // Метод для освобождения точки спавна
    public void FreeSpawnPoint(int spawnIndex)
    {
        if (occupiedSpawnIndexes.Contains(spawnIndex))
        {
            occupiedSpawnIndexes.Remove(spawnIndex);
            debugText.text = $"Spawn point {spawnIndex + 1} is now free.";
        }
        else
        {
            debugText.text = $"Spawn point {spawnIndex + 1} was already free.";
        }
    }

    // Метод для сброса занятых точек спавна
    void ResetOccupiedSpawnPoints()
    {
        occupiedSpawnIndexes.Clear();
        debugText.text = "Occupied spawn points have been reset.";
    }

    // Пример: Освобождение точек спавна при отключении игрока
    void OnPlayerDisconnected(ulong playerId)
    {
        // Найдите игрока по его ID и освободите его точку спавна
        int spawnIndex = GetSpawnIndexForPlayer(playerId);
        FreeSpawnPoint(spawnIndex);
    }

    // Пример функции, которая может возвращать индекс точки спавна игрока по его ID
    int GetSpawnIndexForPlayer(ulong playerId)
    {
        // Ваш код для поиска индекса точки спавна игрока по его ID (например, по его позиции или через объект игрока)
        // В качестве примера вернем первый индекс.
        return 0;  // Это нужно заменить на ваш реальный механизм поиска индекса
    }
}
