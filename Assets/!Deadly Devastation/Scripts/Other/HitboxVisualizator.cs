using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HitboxVisualizer : MonoBehaviour
{
    public enum HitboxType
    {
        Wall,
        Trigger,
        Hurt,
        Player,
        Arrow
    }

    public HitboxType hitboxType = HitboxType.Wall;

    [Header("Arrow Settings")]
    public float arrowLength = 1.5f;
    public float arrowHeadSize = 0.3f;

    private Color GetMainColor()
    {
        return hitboxType switch
        {
            HitboxType.Wall => new Color(0.2f, 1f, 0.2f, 0.25f),
            HitboxType.Trigger => new Color(0.3f, 0.6f, 1f, 0.2f),
            HitboxType.Hurt => new Color(1f, 0.25f, 0.25f, 0.3f),
            HitboxType.Player => new Color(1f, 0.9f, 0.2f, 0.3f),
            HitboxType.Arrow => new Color(1f, 0.5f, 0f, 0.9f),
            _ => Color.white
        };
    }

    private Color GetWireColor()
    {
        Color c = GetMainColor();
        c.a = 1f;
        return c;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

#if UNITY_EDITOR
        Handles.matrix = transform.localToWorldMatrix;
#endif

        if (hitboxType == HitboxType.Arrow)
        {
            DrawArrow();
            return;
        }

        if (!TryGetComponent(out Collider collider)) return;

        Gizmos.color = GetMainColor();

        if (collider is BoxCollider box)
            DrawBox(box);
        else if (collider is SphereCollider sphere)
            DrawSphere(sphere);
        else if (collider is CapsuleCollider capsule)
            DrawCapsule(capsule);
    }


    private void DrawBox(BoxCollider box)
    {
        Gizmos.DrawCube(box.center, box.size);
        Gizmos.color = GetWireColor();
        Gizmos.DrawWireCube(box.center, box.size);
    }

    private void DrawSphere(SphereCollider sphere)
    {
        Gizmos.DrawSphere(sphere.center, sphere.radius);
        Gizmos.color = GetWireColor();
        Gizmos.DrawWireSphere(sphere.center, sphere.radius);
    }

    private void DrawCapsule(CapsuleCollider capsule)
    {
#if UNITY_EDITOR
        Gizmos.color = GetWireColor();
        Handles.color = Gizmos.color;

        Vector3 center = capsule.center;
        float radius = capsule.radius;
        float height = Mathf.Max(capsule.height, radius * 2f);

        Vector3 up = Vector3.up * (height / 2f - radius);

        Handles.DrawWireDisc(center + up, Vector3.up, radius);
        Handles.DrawWireDisc(center - up, Vector3.up, radius);

        Handles.DrawLine(center + up + Vector3.forward * radius, center - up + Vector3.forward * radius);
        Handles.DrawLine(center + up - Vector3.forward * radius, center - up - Vector3.forward * radius);
        Handles.DrawLine(center + up + Vector3.right * radius, center - up + Vector3.right * radius);
        Handles.DrawLine(center + up - Vector3.right * radius, center - up - Vector3.right * radius);
#endif
    }

    private void DrawArrow()
    {
#if UNITY_EDITOR
        Vector3 dir = Vector3.forward;

        float bodyLength = arrowLength * 0.7f;
        float headLength = arrowLength * 0.3f;

        float bodyRadius = arrowHeadSize * 0.25f;
        float headRadius = arrowHeadSize;

        Handles.color = GetWireColor();
        Gizmos.color = GetMainColor();

        // ===== BODY (CYLINDER) =====
        Vector3 bodyCenter = dir * (bodyLength * 0.5f);

        DrawWireCylinder(bodyCenter, dir, bodyLength, bodyRadius);

        // ===== HEAD (CONE) =====
        Vector3 headPos = dir * bodyLength;

        Handles.ConeHandleCap(
            0,
            headPos,
            Quaternion.LookRotation(dir),
            headLength,
            EventType.Repaint
        );
#endif
    }

#if UNITY_EDITOR
    private void DrawWireCylinder(Vector3 center, Vector3 dir, float height, float radius)
    {
        Vector3 up = dir.normalized;
        Vector3 right = Vector3.Cross(up, Vector3.up).normalized;
        if (right == Vector3.zero)
            right = Vector3.Cross(up, Vector3.forward).normalized;

        Vector3 forward = Vector3.Cross(right, up).normalized;

        Vector3 top = center + up * (height / 2f);
        Vector3 bottom = center - up * (height / 2f);

        Handles.DrawWireDisc(top, up, radius);
        Handles.DrawWireDisc(bottom, up, radius);

        Handles.DrawLine(top + right * radius, bottom + right * radius);
        Handles.DrawLine(top - right * radius, bottom - right * radius);
        Handles.DrawLine(top + forward * radius, bottom + forward * radius);
        Handles.DrawLine(top - forward * radius, bottom - forward * radius);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(HitboxVisualizer))]
public class HitboxVisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
#endif
