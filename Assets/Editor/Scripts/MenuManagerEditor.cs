using DG.Tweening;
using EasyTransition;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuManager))]
public class ManualManagerEditor : Editor
{
    private string selectedName = "";
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

        List<string> options = new List<string>();
        foreach (var entry in script.manualEntries)
        {
            if (entry.Target != null)
            {
                options.Add(entry.Target.name);
            }
        }

        if (options.Count == 0)
        {
            EditorGUILayout.HelpBox("There are no objects with specified targets.", MessageType.Warning);
            return;
        }

        int selectedIndex = options.IndexOf(selectedName);
        selectedIndex = EditorGUILayout.Popup("Select Object:", selectedIndex, options.ToArray());
        selectedName = selectedIndex >= 0 ? options[selectedIndex] : selectedName;

        ManualEntry selectedEntry = script.manualEntries.Find(entry => entry.Target != null && entry.Target.name == selectedName);

        if (selectedEntry == null || selectedEntry.Target == null)
        {
            EditorGUILayout.HelpBox("The selected object is missing!", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Record coordinates in the screen area"))
        {
            selectedEntry.onScreenPosition = selectedEntry.Target.position;
            selectedEntry.onScreenRotation = selectedEntry.Target.rotation.eulerAngles;
            EditorUtility.SetDirty(script);
        }

        if (GUILayout.Button("Record coordinates in outside the screen area"))
        {
            selectedEntry.offScreenPosition = selectedEntry.Target.position;
            selectedEntry.offScreenRotation = selectedEntry.Target.rotation.eulerAngles;
            EditorUtility.SetDirty(script);
        }
    }
}
