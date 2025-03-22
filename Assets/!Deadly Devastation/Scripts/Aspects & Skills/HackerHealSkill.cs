using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "HackerHealSkill", menuName = "Game/Skills/HackerHeal")]
public class HackerHealSkill : Skill
{
    public int HealAmount;

    public void ActivateSkill()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // ѕример: восстановить здоровье всем игрокам
            foreach (var player in FindObjectsOfType<HealthManager>())
            {
                player.Heal(HealAmount);
            }
        }
    }
}
