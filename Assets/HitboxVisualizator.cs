using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HitboxVisualizer : MonoBehaviour
{
    public enum HitboxType { Wall, Trigger, DeathZone, Player }
    public HitboxType hitboxType = HitboxType.Wall;

    private Color GetColor()
    {
        switch (hitboxType)
        {
            case HitboxType.Wall: return Color.green;
            case HitboxType.Trigger: return Color.blue;
            case HitboxType.DeathZone: return Color.red;
            case HitboxType.Player: return Color.yellow;
            default: return Color.white;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GetColor();
        Gizmos.matrix = transform.localToWorldMatrix;

        Collider collider = GetComponent<Collider>();
        if (collider is BoxCollider box)
        {
            Gizmos.DrawWireCube(box.center, box.size);
        }
        else if (collider is SphereCollider sphere)
        {
            Gizmos.DrawWireSphere(sphere.center, sphere.radius);
        }
        else if (collider is CapsuleCollider capsule)
        {
            Gizmos.DrawWireSphere(capsule.center, capsule.radius);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HitboxVisualizer))]
public class HitboxVisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HitboxVisualizer script = (HitboxVisualizer)target;

        script.hitboxType = (HitboxVisualizer.HitboxType)EditorGUILayout.EnumPopup("Hitbox Type", script.hitboxType);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }
    }
}
#endif
