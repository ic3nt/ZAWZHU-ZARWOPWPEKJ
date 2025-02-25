using DG.Tweening;
using EasyTransition;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuManager))]
public class ManualManagerEditor : Editor
{
    private string selectedName = ""; // Имя выбранного объекта

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MenuManager script = (MenuManager)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Manual 3D object position settings", EditorStyles.boldLabel);

        if (script.manualEntries.Count == 0)
        {
            EditorGUILayout.HelpBox("The manual list is empty!", MessageType.Warning);
            return;
        }

        // Составляем список имен объектов для отображения
        List<string> options = new List<string>();
        foreach (var entry in script.manualEntries)
        {
            if (entry.Target != null)
            {
                options.Add(entry.Target.name); // Добавляем имя объекта
            }
        }

        if (options.Count == 0)
        {
            EditorGUILayout.HelpBox("There are no objects with specified targets.", MessageType.Warning);
            return;
        }

        // Выбор объекта по имени
        int selectedIndex = options.IndexOf(selectedName); // Индекс выбранного объекта
        selectedIndex = EditorGUILayout.Popup("Select Object:", selectedIndex, options.ToArray());
        selectedName = selectedIndex >= 0 ? options[selectedIndex] : selectedName;

        // Находим выбранный объект по имени
        ManualEntry selectedEntry = script.manualEntries.Find(entry => entry.Target != null && entry.Target.name == selectedName);

        if (selectedEntry == null || selectedEntry.Target == null)
        {
            EditorGUILayout.HelpBox("The selected object is missing!", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Write the top coordinates of the object"))
        {
            selectedEntry.UpPosition = selectedEntry.Target.position;
            selectedEntry.UpRotation = selectedEntry.Target.rotation.eulerAngles;
            EditorUtility.SetDirty(script);
        }

        if (GUILayout.Button("Write the lower coordinates of the object"))
        {
            selectedEntry.BottomPosition = selectedEntry.Target.position;
            selectedEntry.BottomRotation = selectedEntry.Target.rotation.eulerAngles;
            EditorUtility.SetDirty(script);
        }
    }
}
