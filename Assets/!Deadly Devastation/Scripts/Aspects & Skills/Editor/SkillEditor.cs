using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Skill))]
public class SkillEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Skill skill = (Skill)target;

        // Заголовок для Skill
        EditorGUILayout.LabelField("Skill Configuration", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Отображаем поле skillName
        skill.skillName = EditorGUILayout.TextField("Skill Name", skill.skillName);

        // Отображаем поле для выбора иконки
        skill.skillIcon = (Sprite)EditorGUILayout.ObjectField("Skill Icon", skill.skillIcon, typeof(Sprite), false);

        // Отображаем skillType и проверяем его значение
        skill.skillType = (SkillType)EditorGUILayout.EnumPopup("Skill Type", skill.skillType);
        EditorGUILayout.Space();

        // Отображаем activationKey и проверяем его значение
        skill.activationKey = (KeyCode)EditorGUILayout.EnumPopup("Activation Key", skill.activationKey);
        EditorGUILayout.Space();

        // Если skillType равно Passive или Cooldown, убираем раздел с Charges Settings
        if (skill.skillType == SkillType.Cooldown || skill.skillType == SkillType.Passive)
        {
            // Не отображаем Charges Settings
        }
        else
        {
            // HEADER Charges Settings
            EditorGUILayout.LabelField("Charges Settings", EditorStyles.boldLabel);

            // Если skillType равно Charging, скрываем maxCharges
            if (skill.skillType != SkillType.Charging)
            {
                skill.maxCharges = EditorGUILayout.IntField("Max Charges", skill.maxCharges);
            }

            skill.chargesAmount = EditorGUILayout.IntField("Charges Amount", skill.chargesAmount);
            EditorGUILayout.Space();
        }

        // HEADER Cooldown Settings
        if (skill.skillType != SkillType.Passive && skill.skillType != SkillType.Charging) // Добавлено условие для Charging
        {
            EditorGUILayout.LabelField("Cooldown Settings", EditorStyles.boldLabel);

            // Если skillType равен Cooldown, всегда устанавливаем has_A_Cooldown в true и отключаем поле
            if (skill.skillType == SkillType.Cooldown)
            {
                skill.has_A_Cooldown = true; // Устанавливаем has_A_Cooldown в true
                EditorGUI.BeginDisabledGroup(true); // Отключаем поле
                EditorGUILayout.Toggle("Has A Cooldown", skill.has_A_Cooldown);
                EditorGUI.EndDisabledGroup(); // Включаем его обратно
            }
            else
            {
                skill.has_A_Cooldown = EditorGUILayout.Toggle("Has A Cooldown", skill.has_A_Cooldown);
            }

            if (skill.has_A_Cooldown)
            {
                skill.cooldownTime = EditorGUILayout.FloatField("Cooldown Time", skill.cooldownTime);
            }

            EditorGUILayout.Space();
        }

        // HEADER Damage Settings
        if (skill.skillType != SkillType.Passive)
        {
            EditorGUILayout.LabelField("Damage Settings", EditorStyles.boldLabel);
            skill.dealsDamage = EditorGUILayout.Toggle("Deals Damage", skill.dealsDamage);

            if (skill.dealsDamage)
            {
                skill.damage = EditorGUILayout.FloatField("Damage", skill.damage);
            }
            EditorGUILayout.Space();
        }

        // Header Leveling Up
        EditorGUILayout.LabelField("Leveling Up", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Ограничиваем количество улучшений до 3
        if (skill.upgrades.Length > 3)
        {
            Array.Resize(ref skill.upgrades, 3);
        }

        for (int i = 0; i < skill.upgrades.Length; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Проверка типа улучшения в зависимости от skillType
            if (skill.skillType == SkillType.Passive)
            {
                // Ограничение выбора для UpgradeType
                UpgradeType selectedUpgradeType = (UpgradeType)EditorGUILayout.EnumPopup($"Upgrade Type {i + 1}", skill.upgrades[i].upgradeType);
                if (selectedUpgradeType == UpgradeType.Damage)
                {
                    EditorGUILayout.HelpBox("Cannot select Damage upgrade type for Passive skills.", MessageType.Warning);
                }
                else
                {
                    skill.upgrades[i].upgradeType = selectedUpgradeType;
                }
            }
            else
            {
                // Для всех типов, кроме Passive
                skill.upgrades[i].upgradeType = (UpgradeType)EditorGUILayout.EnumPopup($"Upgrade Type {i + 1}", skill.upgrades[i].upgradeType);
            }

            // Теперь добавим возможность для Damage upgrade, если dealsDamage true и не Passive
            if (skill.skillType != SkillType.Passive && skill.dealsDamage)
            {
                // Проверка на уже добавленный "Damage upgrade"
                if (skill.upgrades[i].upgradeType == UpgradeType.Damage)
                {
                    skill.upgrades[i].newValue = EditorGUILayout.FloatField("New Value", skill.upgrades[i].newValue);
                }
                else
                {
                    // Если тип улучшения не "Damage", показываем поле для ввода New Value
                    skill.upgrades[i].newValue = EditorGUILayout.FloatField("New Value", skill.upgrades[i].newValue);
                }
            }
            else
            {
                // Если не разрешено улучшение Damage, просто показываем N/A
                EditorGUILayout.LabelField("New Value", "N/A", EditorStyles.label);
            }

            // Кнопка для удаления улучшения
            if (GUILayout.Button($"Remove Upgrade {i + 1}"))
            {
                Array.Copy(skill.upgrades, i + 1, skill.upgrades, i, skill.upgrades.Length - i - 1);
                Array.Resize(ref skill.upgrades, skill.upgrades.Length - 1);
                break; // Прерываем цикл, чтобы избежать изменения размера массива при переборе
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // Если длина массива меньше 3, показывать кнопку для добавления нового улучшения
        if (skill.upgrades.Length < 3)
        {
            if (GUILayout.Button("Add Upgrade"))
            {
                Array.Resize(ref skill.upgrades, skill.upgrades.Length + 1);
                skill.upgrades[skill.upgrades.Length - 1] = new SkillUpgrade(); // Инициализируем новое улучшение
            }
        }

        // Сохраняем изменения в объекте
        if (GUI.changed)
        {
            EditorUtility.SetDirty(skill);
        }
    }
}