using UnityEngine;
using UnityEngine.UI;

public class RenderTextureInteraction : MonoBehaviour
{
    public RawImage rawImage;
    public Camera sceneCamera; // Камера, которая рендерит в RenderTexture
    public RenderTexture renderTexture; // Ваш RenderTexture

    private void Update()
    {
        // Проверка на клик мыши по UI-элементу (например, RawImage)
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform, Input.mousePosition, null, out localPos);

            // Преобразование местоположения в координаты мира
            Vector3 worldPos = sceneCamera.ScreenToWorldPoint(new Vector3(localPos.x, localPos.y, sceneCamera.nearClipPlane));

            Ray ray = sceneCamera.ScreenPointToRay(worldPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    Debug.Log("Hit Object: " + hit.collider.gameObject.name);
                    // Здесь можно вызвать логику для перетаскивания или другой логики взаимодействия
                }
            }
        }
    }
}
