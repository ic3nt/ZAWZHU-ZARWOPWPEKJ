using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public GameObject Light;



    private void Start()
    {
        Light.SetActive(false);
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //an.SetBool("IsLighterOn", !an.GetBool("IsLighterOn"));

            Light.SetActive(!Light.active);
            AudioManager.Instance.Play("--");
        }

    }

}