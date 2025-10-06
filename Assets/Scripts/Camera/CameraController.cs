 using UnityEngine;
using Utilities.EventManager;

public class CameraController : MonoBehaviour, IInitializable
{
    [Header("Target Settings")]
    [SerializeField] private Transform playerTransform;

    [Header("Camera Settings")]
    [SerializeField] private Vector2 offset = new Vector3(0f, 0f);
    [SerializeField] private float camZPos = -10f;
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Camera Bounds")]
    [SerializeField] private bool useCameraBounds = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f);

    private bool isFollowing = true;
    private bool isInit = false;
    private bool isPlayerDied = false;
    private Camera cam;
    private Vector3 velocity = Vector2.zero;

    #region Subscribes

    private void OnEnable()
    {
        EventManager.Subscribe(eEventType.onPlayerDied, OnPlayerDied);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(eEventType.onPlayerDied, OnPlayerDied);
    }

    #endregion

    public void Initialized()
    {
        cam = GetComponent<Camera>();
        isInit = true;
    }

    private void LateUpdate()
    {
        if (!isInit || isPlayerDied) return;
        if (isFollowing && playerTransform != null)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = playerTransform.position + (Vector3) offset;
        targetPosition.z = camZPos;

        if (useCameraBounds)
        {
            targetPosition = ClampCameraPosition(targetPosition);
        }

        //плавное перемещение камеры
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / smoothSpeed);
    }

    private Vector3 ClampCameraPosition(Vector3 targetPosition)
    {
        if (cam == null) return targetPosition;

        // Рассчитываем половину размера видимой области камеры
        float cameraHeight = 2f * cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        Vector2 halfSize = new Vector2(cameraWidth / 2f, cameraHeight / 2f);

        // Ограничиваем позицию камеры в пределах границ
        float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x + halfSize.x, maxBounds.x - halfSize.x);
        float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y + halfSize.y, maxBounds.y - halfSize.y);

        return new Vector3(clampedX, clampedY, targetPosition.z);
    }

    public void OnPlayerDied(object arg0)
    {
        isPlayerDied = true;
    }

    // Методы для отладки в редакторе
    private void OnDrawGizmosSelected()
    {
        if (useCameraBounds)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector2((minBounds.x + maxBounds.x) * 0.5f, (minBounds.y + maxBounds.y) * 0.5f),
                               new Vector2(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y));
        }
    }
}
