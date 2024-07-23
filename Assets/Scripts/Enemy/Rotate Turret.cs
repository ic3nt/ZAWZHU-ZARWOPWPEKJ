using UnityEngine;

public class RotateTurret : MonoBehaviour
{
    public GameObject Target;
    private GameObject Player;

    public float rotationSpeed;

    private void Start()
    {

   Player = GameObject.FindGameObjectWithTag("FovPlr");

    }

    void Update()
    {
        Vector3 direction = Player.transform.position - Target.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Target.transform.rotation = Quaternion.Slerp(Target.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }
}