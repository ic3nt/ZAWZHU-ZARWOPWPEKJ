using UnityEngine;

public class DestroyWithChance : MonoBehaviour
{
    [Range(0f, 100f)]
    public float ChanceOfStaying = 50f;

    private void Start()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue > ChanceOfStaying)
        {
            Destroy(gameObject);
        }
    }
}
