using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum DoorState { Opened, Closed }

public class DoorInteraction : NetworkBehaviour
{
    public float interactionDistance;
    public GameObject intText;
    public string doorOpenAnimName, doorCloseAnimName;
    public AudioClip doorOpen, doorClose;
    public float fill = 0f;
    float maxFill = 100f;
    public Image progressBar;
    public GameObject Bar;
    public bool canOpen = true;
    public GameObject LockText;

    private Animator an;
    private AudioSource doorSound;

    private NetworkVariable<DoorState> doorState = new NetworkVariable<DoorState>(DoorState.Closed);

    private void Awake()
    {
        fill = progressBar.fillAmount;
    }

    public IEnumerator WaitSec()
    {
        yield return new WaitForSeconds(1);
        canOpen = true;
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
            if (hit.collider.CompareTag("Door"))
            {
                if (hit.collider.TryGetComponent<Door>(out var ds) && !ds.IsLocked)
                {
                    an = hit.collider.transform.GetComponent<Animator>();
                    doorSound = hit.collider.gameObject.GetComponent<AudioSource>();
                    ShowUI(intText);

                    if (Input.GetKey(KeyCode.E) && fill < maxFill && canOpen)
                    {
                        fill += Time.deltaTime * 60f;
                        progressBar.fillAmount = fill / maxFill;
                    }
                    else
                    {
                        fill -= Time.deltaTime * 60f;
                        progressBar.fillAmount = fill / maxFill;
                    }

                    if (fill >= maxFill)
                    {
                        canOpen = false;
                        StartCoroutine(WaitSec());

                        if (an.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName))
                        {
                            CloseDoorServerRpc();
                        }
                        else if (an.GetCurrentAnimatorStateInfo(0).IsName(doorCloseAnimName))
                        {
                            OpenDoorServerRpc();
                        }

                        fill = 0;
                        progressBar.fillAmount = fill;
                    }
                }
                else
                {
                    ShowUI(LockText);
                    HideUI(intText);
                    fill = 0;
                    progressBar.fillAmount = fill;
                }
            }
            else
            {
                HideUI(intText);
                HideUI(LockText);
                fill = 0;
                progressBar.fillAmount = fill;
            }
        }
        else
        {
            HideUI(intText);
            HideUI(LockText);
            fill = 0;
            progressBar.fillAmount = fill;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OpenDoorServerRpc()
    {
        OpenDoor();
        NotifyDoorStateChange(DoorState.Opened);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CloseDoorServerRpc()
    {
        CloseDoor();
        NotifyDoorStateChange(DoorState.Closed);
    }

    private void OpenDoor()
    {
        doorSound.clip = doorOpen;
        doorSound.Play();
        an.ResetTrigger("Close");
        an.SetTrigger("Open");
        doorState.Value = DoorState.Opened;
    }

    private void CloseDoor()
    {
        doorSound.clip = doorClose;
        doorSound.Play();
        an.ResetTrigger("Open");
        an.SetTrigger("Close");
        doorState.Value = DoorState.Closed;
    }

    private void NotifyDoorStateChange(DoorState newState)
    {
        doorState.Value = newState;
    }

    private void ShowUI(GameObject uiElement)
    {
        if (!uiElement.activeSelf)
        {
            uiElement.SetActive(true);
            uiElement.transform.DOKill();
            uiElement.transform.localScale = Vector3.one * 0.8f;
            uiElement.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }

    private void HideUI(GameObject uiElement)
    {
        if (uiElement.activeSelf)
        {
            uiElement.transform.DOKill();
            uiElement.transform.DOScale(Vector3.one * 0.0f, 0.3f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    uiElement.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack)
                        .OnComplete(() => uiElement.SetActive(false));
                });
        }
    }
}
