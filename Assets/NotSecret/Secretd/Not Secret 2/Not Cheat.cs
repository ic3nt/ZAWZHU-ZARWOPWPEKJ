using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.TextCore.Text;

public class NotCheat : NetworkBehaviour
{
    public Animator an;

    public float slowSpeed;
    public float normalSpeed;
    public float sprintSpeed;
    float currentSpeed;

    public Transform character;
   
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    Vector2 velocity;
    Vector2 frameVelocity;

   
    public AudioSource aus;
   
    
 

    private bool canSh;

  

    public GameObject bullet;
    public Camera mainCamera;
    public Transform spawnBullet;

    public float shootForce;
    public float spread;

  


    private void Start()
    {
        an.SetBool("Smod", true);
        Cursor.lockState = CursorLockMode.Locked;
        canSh = false;

    }

    private void Shoot()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 dirWithoutSpread = targetPoint - spawnBullet.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 dirWithSpread = dirWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, spawnBullet.position, Quaternion.identity);

        currentBullet.transform.forward = dirWithSpread.normalized;

      

    }

    private void SpawnEffect(GameObject effectPrefab, Vector3 position)
    {
        GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
        Destroy(effect, 3f); // Удаляем эффект через 3 секунды
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canSh == true)
        {
            an.SetTrigger("Shoot");
            Shoot();
            aus.Play();
           
           
            StartCoroutine(Cooldown());

        }

        Movement();
        Rotation();

        if (Input.GetKeyDown(KeyCode.E))
        {
            an.SetBool("Smod", true);
            canSh = false;
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            an.SetBool("Smod", false);
            canSh = true;

        }


    }


    public IEnumerator Cooldown()
    {
        canSh = false;
        yield return new WaitForSeconds(0.1f);
        canSh = true;
     
    }



    

    

    public void Rotation()
    {

        PlayerPrefs.SetFloat("currentSensitivity", sensitivity);
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -70, 80);

        // Теперь комбинируем вращения
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up) * Quaternion.AngleAxis(-velocity.y, Vector3.right);
    }

    public void Movement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            currentSpeed = slowSpeed;
        }
        else
        {
            currentSpeed = normalSpeed;
        }
        transform.Translate(input * currentSpeed * Time.deltaTime);
    }

}
