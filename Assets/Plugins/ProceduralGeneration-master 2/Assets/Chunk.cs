using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class Chunkk : NetworkBehaviour
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