using UnityEngine;

[CreateAssetMenu(fileName = "HealAllSkill", menuName = "Game/Skills/Heal All")]
public class HealAllSkill1 : Skill1
{
    public int HealAmount = 15;

    public void Execute()
    {
        Debug.Log($"All players healed by {HealAmount} HP.");
    }
}
