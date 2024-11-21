using UnityEngine;

public enum SkillType
{
    Cooldown,
    Passive,
    Accumulated,
    Charging
}

public enum UpgradeType
{
    Damage,
    Range,
    Duration,
}

[System.Serializable]
public class SkillUpgrade
{
    public UpgradeType upgradeType; // Тип улучшения
    public float newValue;
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "ScriptableObjects/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public SkillType skillType;
    public KeyCode activationKey; // Клавиша для активации

    public Sprite skillIcon; // Иконка

    [Header("Cooldown Settings")]
    public bool has_A_Cooldown;
    public float cooldownTime;

    [Header("Charges Settings")]
    public int maxCharges;
    public int chargesAmount;

    [Header("Damage Settings")]
    public bool dealsDamage;
    public float damage;

    [Header("Leveling Up")]
    public SkillUpgrade[] upgrades; // Массив для улучшений
    public int SkillLevel = 0;

    public void UpgradeSkill()
    {
        if (SkillLevel < 3)
        {
            SkillLevel++;
        }
    }
}
