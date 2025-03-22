using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualMonsterCheck : MonoBehaviour
{
    public float interactionDistance;
    public GameObject catalogManager;
    public bool IsRobotToySaw;

    void Start()
    {
        IsRobotToySaw = PlayerPrefs.GetInt("IsRobotToySaw") != 0;
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.gameObject.tag == "Enemy" )
            {


                MonsterType mt = hit.collider.gameObject.GetComponent<MonsterType>();

                if (mt != null && mt._type == "ToyRobot" && IsRobotToySaw == false)
                {
                    Debug.Log("TOYROBOT FOUND");
                    PlayerPrefs.SetInt("IsRobotToySaw", (IsRobotToySaw ? 0 : 1));
                    IsRobotToySaw = true;
                    catalogManager.GetComponent<CatalogManager>().ToyRobotBool = true;
                }
 
            }

        }
    }
}