using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    public Aspect PlayerAspect;

    private float[] skillCooldownTimers;

    private void Start()
    {
        if (PlayerAspect != null)
        {
            skillCooldownTimers = new float[PlayerAspect.Skills.Length];
        }
    }

    private void Update()
    {
        // Обновляем таймеры кд
        for (int i = 0; i < skillCooldownTimers.Length; i++)
        {
            if (skillCooldownTimers[i] > 0)
                skillCooldownTimers[i] -= Time.deltaTime;
        }

        // Проверяем нажатия клавиш
        for (int i = 0; i < PlayerAspect.Skills.Length; i++)
        {
            var skill = PlayerAspect.Skills[i];
            if (Input.GetKeyDown(skill.activationKey) && skillCooldownTimers[i] <= 0)
            {
                ActivateSkill(skill, i);
            }
        }
    }

    private void ActivateSkill(Skill skill, int skillIndex)
    {
        // Проверяем тип скилла и активируем
        if (skill.skillType == SkillType.Passive) return;

        // Пример активации
        if (skill is HackerHealSkill hackerHeal)
        {
            hackerHeal.ActivateSkill();
        }

        // Устанавливаем кд
        skillCooldownTimers[skillIndex] = skill.cooldownTime;
    }
}
