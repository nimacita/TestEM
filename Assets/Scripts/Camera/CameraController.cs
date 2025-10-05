 using UnityEngine;

public class CameraController : MonoBehaviour
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
    private Camera cam;
    private Vector3 velocity = Vector2.zero;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
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

    public void EnableFollowing()
    {
        isFollowing = true;
    }

    public void DisableFollowing()
    {
        isFollowing = false;
    }

    public void ToggleFollowing()
    {
        isFollowing = !isFollowing;
    }

    public void SetCameraBounds(Vector2 newMinBounds, Vector2 newMaxBounds)
    {
        minBounds = newMinBounds;
        maxBounds = newMaxBounds;
    }

    public void SetUseBounds(bool useBounds)
    {
        useCameraBounds = useBounds;
    }

    public void SetSmoothSpeed(float speed)
    {
        smoothSpeed = Mathf.Max(0.1f, speed);
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
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
