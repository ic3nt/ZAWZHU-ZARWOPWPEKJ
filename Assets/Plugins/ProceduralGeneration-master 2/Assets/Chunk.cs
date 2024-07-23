using TMPro;
using UnityEngine;

public class Chunkk : MonoBehaviour
{
    public TextMeshPro floorCounterText;

    public Transform Begin;
    public Transform End;
    public int currentenflr;

    public int floor;

    void Start()
    {
        floorCounterText.text = (floor - 1) + " ↓";
    }
}