using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody))]
public class BreakableBox : MonoBehaviour, IHittable
{
    private Health health;
    private Rigidbody rb;

    private void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();

        health.OnDied += Break;
    }

    public void OnHit(Vector3 force, int damage, GameObject hitter)
    {
        health.TakeDamage(damage);

        if (rb != null)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    private void Break()
    {
        Debug.Log("ящик сломан!");
        Destroy(gameObject);
    }
}
