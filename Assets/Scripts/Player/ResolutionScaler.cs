#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ResolutionScaler : MonoBehaviour
{
    [Range(2, 16)] public float Scale = 2;

    private Camera cameraComponent;
    private RenderTexture texture;
    private int mainTexID;

    private void Start()
    {
        CreateTexture();
    }

    private void CreateTexture()
    {
        int width = Mathf.RoundToInt(Screen.width / Scale);
        int height = Mathf.RoundToInt(Screen.height / Scale);
        texture = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
        texture.antiAliasing = 1;

        cameraComponent = GetComponent<Camera>();

        mainTexID = Shader.PropertyToID("_MainTex");
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (EditorApplication.isPlaying) return;
        CreateTexture();
    }
#endif

    private void OnPreRender()
    {
        cameraComponent.targetTexture = texture;
    }

    private void OnPostRender()
    {
        cameraComponent.targetTexture = null;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        src.filterMode = FilterMode.Point;

        Material blitMaterial = new Material(Shader.Find("Universal Render Pipeline/2D/Unlit"));
        blitMaterial.SetTexture(mainTexID, src);

        Graphics.Blit(src, dest, blitMaterial);
    }
}
