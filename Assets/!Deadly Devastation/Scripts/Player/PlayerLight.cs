using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] private GameObject lightObj;
    [SerializeField] private KeyCode toggleKey = KeyCode.F;
    [SerializeField] private string sfxKey = "--";

    private void Start()
    {
        if (lightObj) lightObj.SetActive(false);
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(toggleKey) && lightObj)
        {
            lightObj.SetActive(!lightObj.activeSelf);
            if (!string.IsNullOrEmpty(sfxKey) && AudioManager.Instance)
                AudioManager.Instance.Play(sfxKey);
        }
    }
}
