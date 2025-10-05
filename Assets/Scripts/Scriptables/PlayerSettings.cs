using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Health Settings")]
    [Tooltip("Стартовое здоровье игрока")]
    public float playerHealth = 50f;

    [Header("Attack Settings")]
    [Tooltip("Урон Игрока")]
    public int damage = 10;
    [Tooltip("Время между атаками (откат атак)")]
    public float attackCoolDown = 1.2f;
    [Tooltip("Длительность самой атаки")]
    public float attackDuration = 0.5f;
    [Tooltip("Смещение зоны атаки от точки атаки")]
    public Vector2 offset = new Vector2(0.05f, 0.03f);
    [Tooltip("Радиус атаки")]
    public float attackRadius = 0.7f;
    [Tooltip("Сила рыка атаки")]
    public float attackDashForce = 13f;
    [Tooltip("Маска слоев попадающих под атаку")]
    public LayerMask attackMask;

    [Header("Movement Settings")]
    [Tooltip("Скорость движения")]
    public float moveSpeed = 8f;
    [Tooltip("Ускорение")]
    public float acceleration = 50f;
    [Tooltip("Потеря ускорения")]
    public float deceleration = 60f;
    [Tooltip("Множитель скорости управления в воздухе")]
    public float airControl = 0.8f;

    [Header("Jump Settings")]
    [Tooltip("Сила прыжка")]
    public float jumpForce = 14f;
    [Tooltip("Сила прыжка по горизонтали")]
    public float jumpHorizontalForce = 4f;
    [Tooltip("Кайот таймер")]
    public float coyoteTime = 0.12f;
    [Tooltip("Время на прыжок после схода с платформы")]
    public float jumpBufferTime = 0.12f;
    [Tooltip("Сила среза прыжка при отпускании пробела")]
    public float jumpCutMultiplier = 0.5f;
    [Tooltip("Ускорение падения")]
    public float fallMultiplier = 4f;
    [Tooltip("Ускорение падения при коротком прыжке")]
    public float lowJumpMultiplier = 3f;
    [Tooltip("Дополнительная длительность для удержания прыжка (чтобы он был чуть выше)")]
    public float maxJumpHoldTime = 0.1f;

    [Header("Wall Jump Settings")]
    [Tooltip("Сила прыжка от стены")]
    public float wallJumpForce = 14f;
    [Tooltip("Сила горизонтального прыжка от стены")]
    public float wallJumpHorizontalForce = 10f;
    [Tooltip("Кайот Таймер")]
    public float wallCoyoteTime = 0.15f;
    [Tooltip("Дистанция проверки стены")]
    public float wallCheckDistance = 0.3f;
    [Tooltip("Слой стен или поверхностей для прыжка")]
    public LayerMask wallLayer;

    [Header("Dodge Settings")]
    [Tooltip("Сила рыка")]
    public float dodgeForce = 15f;
    [Tooltip("Длительность Рыка")]
    public float dodgeDuration = 0.4f;
    [Tooltip("Откат между рыками")]
    public float dodgeCooldown = 1f;

    [Header("Damaged Settings")]
    [Tooltip("Длительность оглушения после урона")]
    public float damageDuration = 0.4f;
    [Tooltip("Сила откидывания после урона")]
    public float damagedKnockForce = 4f;

    [Header("Ground Check Settings")]
    [Tooltip("Радиус проверки пола")]
    public float groundCheckRadius = 0.12f;
    [Tooltip("Слои считающийся полом или землей")]
    public LayerMask groundLayer;

    [Header("Main Layer Settings")]
    public LayerMask playerLayer;
    public LayerMask diedLayer;
}
