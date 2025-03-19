using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HitboxVisualizer : MonoBehaviour
{
    public enum HitboxType { Wall, Trigger, Hurt, Player }
    public HitboxType hitboxType = HitboxType.Wall;

    private Color GetColor()
    {
        return hitboxType switch
        {
            HitboxType.Wall => new Color(0, 1, 0, 0.3f),      // Зелёный
            HitboxType.Trigger => new Color(0, 0, 1, 0.2f),   // Глубокий синий (более полупрозрачный)
            HitboxType.Hurt => new Color(1, 0, 0, 0.3f),      // Красный (чуть менее прозрачный)
            HitboxType.Player => new Color(1, 1, 0, 0.3f),    // Жёлтый
            _ => Color.white,
        };
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GetColor();
        Gizmos.matrix = transform.localToWorldMatrix;

        if (TryGetComponent(out Collider collider))
        {
            if (collider is BoxCollider box) DrawBox(box);
            else if (collider is SphereCollider sphere) DrawSphere(sphere);
            else if (collider is CapsuleCollider capsule) DrawCapsule(capsule);
        }
    }

    private void DrawBox(BoxCollider box)
    {
        DrawSolidAndWire(box.center, box.size);

#if UNITY_EDITOR
        DrawStripesIfNeeded(box.center, box.size);
#endif
    }

    private void DrawSphere(SphereCollider sphere)
    {
        DrawSolidAndWire(sphere.center, Vector3.one * sphere.radius * 2);
    }

    private void DrawCapsule(CapsuleCollider capsule)
    {
        DrawSolidAndWire(capsule.center, Vector3.one * capsule.radius * 2);
    }

    private void DrawSolidAndWire(Vector3 center, Vector3 size)
    {
        Gizmos.DrawCube(center, size);
        Gizmos.color = GetColor() * 1.5f;
        Gizmos.DrawWireCube(center, size);
    }

#if UNITY_EDITOR
    private void DrawStripesIfNeeded(Vector3 center, Vector3 size)
    {
        if (hitboxType != HitboxType.Hurt && hitboxType != HitboxType.Trigger) return;

        Color stripeColor = hitboxType == HitboxType.Hurt ? new Color(1, 0, 0, 0.8f) : new Color(0, 0, 1, 0.8f);
        Handles.color = stripeColor;

        float step = 0.2f;
        for (float x = -size.x / 2; x < size.x / 2; x += step)
        {
            Vector3 start = center + new Vector3(x, -size.y / 2, 0);
            Vector3 end = center + new Vector3(x + size.y, size.y / 2, 0);
            Handles.DrawAAPolyLine(3, start, end);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(HitboxVisualizer))]
public class HitboxVisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HitboxVisualizer script = (HitboxVisualizer)target;
        script.hitboxType = (HitboxVisualizer.HitboxType)EditorGUILayout.EnumPopup("Hitbox Type", script.hitboxType);

        if (GUI.changed) EditorUtility.SetDirty(script);
    }
}
#endif
