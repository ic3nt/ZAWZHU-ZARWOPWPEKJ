using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHandler1 : MonoBehaviour
{
    public List<Skill1> Skills;

    private Dictionary<Skill1, float> skillCooldowns = new Dictionary<Skill1, float>();
    private Dictionary<Skill1, int> skillCharges = new Dictionary<Skill1, int>();

    private void Start()
    {
        foreach (var skill in Skills)
        {
            if (skill.Type == SkillType1.Chargeable || skill.Type == SkillType1.Consumable)
            {
                skillCharges[skill] = skill.MaxCharges;
            }

            skillCooldowns[skill] = 0f;
        }
    }

    private void Update()
    {
        foreach (var skill in Skills)
        {
            if (Input.GetKeyDown(skill.ActivationKey))
            {
                ActivateSkill(skill);
            }
        }

        // Обновление кулдаунов
        foreach (var skill in Skills)
        {
            if (skillCooldowns[skill] > 0)
            {
                skillCooldowns[skill] -= Time.deltaTime;
            }
        }
    }

    private void ActivateSkill(Skill1 skill)
    {
        if (skill.Type == SkillType1.Passive)
        {
            Debug.Log($"{skill.SkillName} is passive and always active.");
            return;
        }

        if (skillCooldowns[skill] > 0)
        {
            Debug.Log($"{skill.SkillName} is on cooldown.");
            return;
        }

        if (skill.Type == SkillType1.Chargeable || skill.Type == SkillType1.Consumable)
        {
            if (skillCharges[skill] <= 0)
            {
                Debug.Log($"{skill.SkillName} has no charges left.");
                return;
            }

            skillCharges[skill]--;
        }

        skillCooldowns[skill] = skill.Cooldown;
        Debug.Log($"{skill.SkillName} activated!");

        // Реализация работы скилла
        if (skill.SkillName == "Heal All")
        {
            HealAllPlayers(15);
        }
    }

    private void HealAllPlayers(int amount)
    {
        // Пример работы с хилом
        Debug.Log($"All players healed by {amount} HP.");
    }
}
