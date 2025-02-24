using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuManager))]
public class MenuManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MenuManager script = (MenuManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Записать верхние координаты объекта"))
        {
            script.upPosition = script.target3D.position;
            script.upRotation = script.target3D.rotation.eulerAngles;
        }

        if (GUILayout.Button("Записать нижние координаты объекта"))
        {
            script.bottomPosition = script.target3D.position;
            script.bottomRotation = script.target3D.rotation.eulerAngles;
        }
    }
}
