using UnityEngine;

public class PlayerRoundVariables : MonoBehaviour
{
    [Header("Main Variables")]
    [SerializeField] private bool playerInElevator;
    [SerializeField] private bool isSpectator;
    [SerializeField] private bool isAFK;
    [SerializeField] private bool isSpeaking;

    public bool PlayerInElevator { get => playerInElevator; set => playerInElevator = value; }
    public bool IsSpectator { get => isSpectator; set => isSpectator = value; }
    public bool IsAFK { get => isAFK; set => isAFK = value; }
    public bool IsSpeaking { get => isSpeaking; set => isSpeaking = value; }
}
