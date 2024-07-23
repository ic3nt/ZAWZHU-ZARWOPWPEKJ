using UnityEngine;

public class RandomItems : MonoBehaviour
{
    public GameObject[] Items;

    void Start()
    {
        SpawnObject();
    }

    void SpawnObject()
    {
        int randomIndex = Random.Range(0, Items.Length);
        GameObject randomObject = Items[randomIndex];
        Instantiate(randomObject, transform.position, transform.rotation);
    }
}