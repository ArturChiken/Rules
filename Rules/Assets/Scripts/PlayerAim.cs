using UnityEngine;
using UnityEngine.UI;

public class PlayerAim : MonoBehaviour
{
        [Header("Aim Settings")]
        [SerializeField] private float maxAimDistance = 100f;
        [SerializeField] private LayerMask aimLayers = -1; // Все слои по умолчанию
        [SerializeField] private Color defaultAimColor = Color.white;
        [SerializeField] private Color hitAimColor = Color.red;

        [Header("UI References")]
        [SerializeField] private Image aimImage; // Ссылка на UI Image прицела
        [SerializeField] private Sprite defaultAimSprite;
        [SerializeField] private Sprite hitAimSprite;

        private Camera playerCamera;
        private GameObject currentHitObject;

        private void Start()
        {
            playerCamera = GetComponent<Camera>();

            // Если камера не найдена, пытаемся найти её на этом же объекте
            if (playerCamera == null)
                playerCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            UpdateAim();
        }

        private void UpdateAim()
        {
            if (playerCamera == null) return;

            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxAimDistance, aimLayers))
            {
                currentHitObject = hit.collider.gameObject;

                // Меняем цвет/спрайт прицела при наведении на объект
                if (aimImage != null)
                {
                    aimImage.color = hitAimColor;
                    if (hitAimSprite != null)
                        aimImage.sprite = hitAimSprite;
                }

                // Здесь можно добавить подсветку объекта
                // HighlightObject(hit.collider.gameObject);
            }
            else
            {
                currentHitObject = null;

                // Возвращаем обычный вид прицела
                if (aimImage != null)
                {
                    aimImage.color = defaultAimColor;
                    if (defaultAimSprite != null)
                        aimImage.sprite = defaultAimSprite;
                }
            }
        }

        // Метод для получения объекта, на который сейчас наведен прицел
        public GameObject GetCurrentAimTarget()
        {
            return currentHitObject;
        }

        // Метод для проверки, наведен ли прицел на объект
        public bool IsAimingAtObject()
        {
            return currentHitObject != null;
        }

        // Визуализация луча прицела в редакторе
        private void OnDrawGizmosSelected()
        {
            if (playerCamera != null)
            {
                Gizmos.color = Color.yellow;
                Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                Gizmos.DrawRay(ray.origin, ray.direction * maxAimDistance);
            }
        }
}
