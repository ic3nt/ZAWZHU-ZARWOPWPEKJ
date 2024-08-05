using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EasyTransition;

public class HealthManager : MonoBehaviour
{
    public DemoLoadScene loadScene;
    public Animator animatorDead;
    public FirstPersonMovement personMovement;
    public FirstPersonLook personLook;
    public FirstPersonAudio personAudio;
    public Image healthBar;
    public TextMeshProUGUI healthAmountText;
    public Animator animatorHealth;
    public float healthAmount = 100f;
    public bool timerOn;

    private Coroutine healthCoroutine;
    private float damageCooldown = 0.3f;
    private float lastDamageTime;

    void Start()
    {
        animatorDead.GetComponent<Animator>().enabled = false;
        personMovement.GetComponent<FirstPersonMovement>().enabled = true;
        personLook.GetComponent<FirstPersonLook>().enabled = true;
        personAudio.GetComponent<FirstPersonAudio>().enabled = true;
        lastDamageTime = Time.time;
    }

    void Update()
    {
        healthAmountText.text = healthAmount.ToString("F0") + "%";
        if (healthAmount <= 0)
        {
            Dead();
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift) && ((personMovement.GetComponent<Rigidbody>().velocity.x != 0) && (personMovement.GetComponent<Rigidbody>().velocity.z != 0)))
            {
                if (!timerOn)
                {
                    timerOn = true;
                    healthCoroutine = StartCoroutine(DeductHealthOverTime());
                }
            }
            else
            {
                if (timerOn)
                {
                    timerOn = false;
                    StopCoroutine(healthCoroutine);
                }
            }

            if (Time.time - lastDamageTime >= damageCooldown)
            {
                animatorHealth.SetTrigger("Normal");
            }
        }
    }

    private IEnumerator DeductHealthOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            TakeDamage(0.25f);
        }
    }

    public void TakeDamage(float damage)
    {
        animatorHealth.SetTrigger("Damage");
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
        lastDamageTime = Time.time;
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
