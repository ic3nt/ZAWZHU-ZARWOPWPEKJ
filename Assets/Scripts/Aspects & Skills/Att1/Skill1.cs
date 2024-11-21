using UnityEngine;

public enum SkillType1
{
    Passive,         // Постоянно активен
    CooldownBased,   // Имеет КД
    Chargeable,      // Накапливает заряды
    Consumable       // Ограниченное число зарядов
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Skill")]
public class Skill1 : ScriptableObject
{
    public string SkillName; // Название скилла
    public SkillType1 Type;   // Тип скилла
    public KeyCode ActivationKey; // Клавиша для активации (например, E, R, F)
    public bool Damage;      // Наносит ли скилл урон
    public int DamageAmount; // Урон (если Damage = true)
    public float Cooldown;   // Перезарядка (только для CooldownBased и Chargeable)
    public int MaxCharges;   // Максимальное количество зарядов (для Chargeable и Consumable)
    public int Level = 1;    // Уровень скилла
    public int MaxLevel = 3; // Максимальный уровень
    public string Description; // Описание скилла

    // Улучшения
    public float RangeIncreasePerLevel;
    public int DamageIncreasePerLevel;
    public float CooldownReductionPerLevel;

    // Для отображения логики в инспекторе
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Type == SkillType1.Passive || Type == SkillType1.Consumable)
        {
            Cooldown = 0;
        }

        if (!Damage)
        {
            DamageAmount = 0;
        }

        if (Type == SkillType1.Consumable || Type == SkillType1.Chargeable)
        {
            MaxCharges = Mathf.Max(1, MaxCharges);
        }
        else
        {
            MaxCharges = 0;
        }
    }
#endif
}
