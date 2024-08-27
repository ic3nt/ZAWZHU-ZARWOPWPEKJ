using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Burst.CompilerServices;

public class OutlineRender : NetworkBehaviour
{
    public float rayDistance = 3f; // Длина луча

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Проверяем, попадает ли луч в объект
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // Проверяем тег объекта
            if (hit.collider.CompareTag("canPickUp"))
            {
                // Если объект с тегом canPickUp, включаем Outline
                Outline sdvg = hit.collider.gameObject.GetComponent<Outline>();
                if (sdvg != null)
                {
                    sdvg.enabled = true;
                }
            }
            else
            {
                // Если объект не с тегом canPickUp, отключаем Outline
                Outline sdvg = hit.collider.gameObject.GetComponent<Outline>();
                if (sdvg != null)
                {
                    sdvg.enabled = false;
                }
            }
        }
        else
        {
            // Если луч не попадает в объекты, отключаем Outline у всех объектов
            DisableOutlines();
        }

      
        
    }

   
    private void DisableOutlines()
    {
        // Можно добавить логику для отключения Outline на всех объектах, если необходимо
        Outline[] outlines = FindObjectsOfType<Outline>();
        foreach (Outline outline in outlines)
        {
            outline.enabled = false;
        }
    }
}