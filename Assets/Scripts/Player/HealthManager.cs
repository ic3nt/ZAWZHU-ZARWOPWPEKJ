using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyTransition;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public DemoLoadScene loadScene;
    public Animator animatorDead;
    public FirstPersonMovement personMovement;
    public FirstPersonLook personLook;
    public FirstPersonAudio personAudio;
    public Image healthBar;
    public float healthAmount = 100f;

    public bool timerOn;

    void Start()
    {
        animatorDead.GetComponent<Animator>().enabled = false;
        personMovement.GetComponent<FirstPersonMovement>().enabled = true;
        personLook.GetComponent<FirstPersonLook>().enabled = true;
        personAudio.GetComponent<FirstPersonAudio>().enabled = true;
    }

    void Update()
    {
        if (healthAmount <= 0)
        {
            Dead();
        }

      
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        healthBar.fillAmount = healthAmount / 100f;
    }
    public void Dead()
    {
        timerOn = false;
        Cursor.lockState = CursorLockMode.None;
        animatorDead.GetComponent<Animator>().enabled = true;
        personMovement.GetComponent<FirstPersonMovement>().enabled = false;
        personLook.GetComponent<FirstPersonLook>().enabled = false;
        personAudio.GetComponent<FirstPersonAudio>().enabled = false;
       
    }
    public void GoToMenuButton()
    {
        loadScene.LoadScene("IsMenuScene");
    }
}
