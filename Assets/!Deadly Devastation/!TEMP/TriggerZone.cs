using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class TriggerZone : NetworkBehaviour
{
    public bool requireTag = true;
    [Tooltip("Tags separated by commas: Player,Enemy,NPC")]
    public string requiredTags = "Player";
    public LayerMask requiredLayer = ~0;
    public bool requireNetworkComponent = true;
    public bool requireLocalPlayer = true;
    public bool checkForNetworkObject = true;

    public bool triggerOnEnter = true;
    public bool triggerOnExit = true;
    public bool triggerOnStay = false;
    public float stayRepeatInterval = 1f;
    public bool onlyOnce = false;

    public float delayOnEnter = 0f;
    public float delayOnExit = 0f;
    public float autoDeactivateAfter = 0f;

    public UnityEvent onEnter;
    public UnityEvent onExit;
    public UnityEvent onStay;

    private bool isPlayerInside = false;
    private Coroutine stayCoroutine;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnEnter || !IsValidTarget(other)) return;
        if (onlyOnce && hasTriggered) return;

        isPlayerInside = true;
        if (delayOnEnter > 0)
            StartCoroutine(DelayedAction(delayOnEnter, onEnter));
        else
            onEnter.Invoke();

        if (autoDeactivateAfter > 0)
            StartCoroutine(AutoDeactivate(autoDeactivateAfter));

        if (triggerOnStay)
            stayCoroutine = StartCoroutine(StayLoop());

        hasTriggered = true;

        // ServerRpc для обработки события на сервере
        OnPlayerEnterServerRpc();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!triggerOnExit || !IsValidTarget(other)) return;
        if (onlyOnce && hasTriggered) return;

        isPlayerInside = false;

        if (delayOnExit > 0)
            StartCoroutine(DelayedAction(delayOnExit, onExit));
        else
            onExit.Invoke();

        if (stayCoroutine != null)
        {
            StopCoroutine(stayCoroutine);
            stayCoroutine = null;
        }

        // ServerRpc для обработки события на сервере
        OnPlayerExitServerRpc();
    }

    private IEnumerator StayLoop()
    {
        while (isPlayerInside)
        {
            onStay.Invoke();
            yield return new WaitForSeconds(stayRepeatInterval);
        }
    }

    private IEnumerator DelayedAction(float delay, UnityEvent action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    private IEnumerator AutoDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private bool IsValidTarget(Collider other)
    {
        if (requireTag)
        {
            string[] tags = requiredTags.Split(',');
            bool found = false;
            foreach (string tag in tags)
            {
                if (other.CompareTag(tag.Trim()))
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }

        if (((1 << other.gameObject.layer) & requiredLayer) == 0) return false;

        if (requireNetworkComponent && checkForNetworkObject)
        {
            NetworkObject netObj = other.GetComponent<NetworkObject>();
            if (netObj == null) return false;
            if (requireLocalPlayer && !netObj.IsOwner) return false;
        }

        return true;
    }

    // ServerRpc для обработки события входа игрока
    [ServerRpc]
    private void OnPlayerEnterServerRpc(ServerRpcParams rpcParams = default)
    {
        // Когда игрок входит, вызываем ClientRpc для всех клиентов
        OnPlayerEnterClientRpc();
    }

    // ServerRpc для обработки события выхода игрока
    [ServerRpc]
    private void OnPlayerExitServerRpc(ServerRpcParams rpcParams = default)
    {
        // Когда игрок выходит, вызываем ClientRpc для всех клиентов
        OnPlayerExitClientRpc();
    }

    // ClientRpc для уведомления всех клиентов о входе игрока
    [ClientRpc]
    private void OnPlayerEnterClientRpc(ClientRpcParams rpcParams = default)
    {
        // Событие для всех клиентов, когда игрок входит
        onEnter.Invoke();
    }

    // ClientRpc для уведомления всех клиентов о выходе игрока
    [ClientRpc]
    private void OnPlayerExitClientRpc(ClientRpcParams rpcParams = default)
    {
        // Событие для всех клиентов, когда игрок выходит
        onExit.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (TryGetComponent<Collider>(out var col))
        {
            if (col is BoxCollider box)
                Gizmos.DrawWireCube(transform.position + box.center, box.size);
            else if (col is SphereCollider sphere)
                Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TriggerZone))]
public class TriggerZoneEditor : Editor
{
    private bool showConditions, showBehavior, showDelays, showEvents;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle foldStyle = new GUIStyle(EditorStyles.foldout);
        foldStyle.fontStyle = FontStyle.Bold;

        showConditions = EditorGUILayout.Foldout(showConditions, "Conditions", true, foldStyle);
        if (showConditions)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("requireTag"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredTags"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredLayer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("requireNetworkComponent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("checkForNetworkObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("requireLocalPlayer"));
        }

        showBehavior = EditorGUILayout.Foldout(showBehavior, "Behavior", true, foldStyle);
        if (showBehavior)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerOnEnter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerOnExit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerOnStay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stayRepeatInterval"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onlyOnce"));
        }

        showDelays = EditorGUILayout.Foldout(showDelays, "Delays", true, foldStyle);
        if (showDelays)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("delayOnEnter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("delayOnExit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoDeactivateAfter"));
        }

        showEvents = EditorGUILayout.Foldout(showEvents, "Events", true, foldStyle);
        if (showEvents)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onEnter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onExit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onStay"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
