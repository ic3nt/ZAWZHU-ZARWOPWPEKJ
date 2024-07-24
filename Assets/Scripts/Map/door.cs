using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public float interactionDistance;
    public GameObject intText;
    public string doorOpenAnimName, doorCloseAnimName;
    public AudioClip doorOpen, doorClose;
    public float fill = 0f;
    float maxfill = 100f;
    public Image progressBar;

    public GameObject Bar;

    public bool canOpen = true;

    public GameObject LockText;

    private void Awake()
    {
        fill = progressBar.fillAmount;
    }

    public IEnumerator WaitSec()
    {
        yield return new WaitForSeconds(1); ;

        canOpen = true;
        yield return null;
    }


    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        

        if (fill < 0)
        {
            fill = 0;
        }


        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject.tag == "Door")
            {
                if (hit.collider.TryGetComponent<DoorSt>(out var ds))
                {
                    if (!ds.islocked)
                    {

                        Animator an = hit.collider.transform.GetComponent<Animator>();
                        GameObject Doorr = hit.collider.transform.gameObject;

                        AudioSource doorSound = hit.collider.gameObject.GetComponent<AudioSource>();

                        intText.SetActive(true);

                        if (Input.GetKey(KeyCode.E) && fill < maxfill && canOpen == true)
                        {
                            fill += Time.deltaTime * 160f; // Adjust fill rate if needed
                            progressBar.fillAmount = fill / maxfill;
                        }
                        else
                        {
                            fill -= Time.deltaTime * 160f; // Adjust fill rate if needed
                            progressBar.fillAmount = fill / maxfill;
                        }

                        if (fill >= maxfill)
                        {
                            canOpen = false;
                            StartCoroutine(WaitSec());

                            if (an.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName))
                            {
                                doorSound.clip = doorClose;
                                doorSound.Play();
                                an.ResetTrigger("Open");
                                an.SetTrigger("Close");


                            }
                            else if (an.GetCurrentAnimatorStateInfo(0).IsName(doorCloseAnimName))
                            {
                                doorSound.clip = doorOpen;
                                doorSound.Play();
                                an.ResetTrigger("Close");
                                an.SetTrigger("Open");

                            }
                            fill = 0;
                            progressBar.fillAmount = fill;
                        }

                    }
                    else
                    {
                        LockText.SetActive(true);
                        intText.SetActive(false);
                        fill = 0;
                        progressBar.fillAmount = fill;
                    }

                }
               

            }
                else
                {
                    LockText.SetActive(false);
                    intText.SetActive(false);
                    fill = 0;
                    progressBar.fillAmount = fill;
                }
            }
            else
            {
                LockText.SetActive(false);
                intText.SetActive(false);
                fill = 0;
                progressBar.fillAmount = fill;
            }
        }
}


