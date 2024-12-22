using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Burst.CompilerServices;

public class OutlineRender : NetworkBehaviour
{
    public float rayDistance = 3f;
 
    public bool IsObjSaw;

    void Start()
    {
        IsObjSaw = PlayerPrefs.GetInt("IsObjSaw") != 0;
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // проверяем, попадает ли луч в объект
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // проверяем тег объекта
            if (hit.collider.CompareTag("canPickUp"))
            {
                // если объект с тегом canPickUp, включаем обводку
                OutlineScript outline = hit.collider.gameObject.GetComponent<OutlineScript>();
                if (outline != null)
                {

                    outline.enabled = true;

                    if (IsObjSaw == false)
                    {
                        Debug.Log("Is Object Saw");
                        PlayerPrefs.SetInt("IsObjSaw", (IsObjSaw ? 0 : 1));
                        IsObjSaw = true;
                    }
                }
            }
            else
            {
                // если объект не с тегом canPickUp, отключаем обводку
                OutlineScript outline = hit.collider.gameObject.GetComponent<OutlineScript>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }
        }
        else
        {
            // вот тута вот вызываем метод отключения обводки

            DisableOutlines();
        }

      
        
    }

    private void DisableOutlines()
    {
        // метод отключения обводки

        OutlineScript[] outlines = FindObjectsOfType<OutlineScript>();
        foreach (OutlineScript outline in outlines)
        {
            outline.enabled = false;
        }
    }


  

   

    
   

}