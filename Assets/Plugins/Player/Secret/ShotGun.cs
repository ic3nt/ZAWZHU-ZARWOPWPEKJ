using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : MonoBehaviour
{
    public AudioSource aus;
    public Animator an;

    private bool canSh = true;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canSh == true)
        {
            an.SetTrigger("Pum");
            aus.Play();
            StartCoroutine(Cooldown());
        }
    }


    public IEnumerator Cooldown()
    {
        canSh = false;
        yield return new WaitForSeconds(1);
        canSh = true;

    }
}