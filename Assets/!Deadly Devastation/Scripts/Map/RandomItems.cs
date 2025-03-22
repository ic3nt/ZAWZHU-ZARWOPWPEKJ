using Unity.Netcode;
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

    private void SpawnObject()
    {
        int randomIndex = Random.Range(0, Items.Length);
        GameObject randomObject = Items[randomIndex];

        GameObject spawnedObject = Instantiate(randomObject, CurTr.position, CurTr.rotation, parentTransform);

        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.Spawn();
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }

}