using System.Collections;
using UnityEngine;

public class HitManager : MonoBehaviour
{
    public Animator animator; // Ссылка на аниматор
    public float kickForce = 500f; // Сила удара
    public float delay = 0.2f; // Задержка между ударами
    public PlayerMovement PM;

    private bool isKicking = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isKicking)
        {
            StartCoroutine(Kick());
        }
    }

    private IEnumerator Kick()
    {
        PM.CanMove = false;
        isKicking = true;

        // Вызов анимации
        animator.SetTrigger("Kick"); // Предполагается, что вы создали триггер "Kick" в аниматоре

        // Найти предметы рядом
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f); // Радиус поиска

        foreach (var hitCollider in hitColliders)
        {
            // Проверка, является ли объект "предметом" для пинания
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Применить силу к предмету
                rb.AddForce(transform.forward * kickForce);
            }
        }

        // Задержка
        yield return new WaitForSeconds(delay);

        isKicking = false;
        PM.CanMove = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация зоны пинания в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}