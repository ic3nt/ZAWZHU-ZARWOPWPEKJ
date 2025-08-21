using UnityEngine;

public interface IHittable
{
    void OnHit(Vector3 force, int damage, GameObject hitter);
}
