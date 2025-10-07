using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "ScriptableObjects/EnemySettings")]
public class EnemySettings : ScriptableObject
{

    [Header("Name Settings")]
    [Tooltip("Имя врага")]
    public string enemyName;

    [Header("Health Settings")]
    [Tooltip("Стартовое здоровье")]
    public float maxHealth = 20f;

    [Header("Movement Settings")]
    [Tooltip("Базовая скорость движения")]
    public float moveSpeed = 2f;

    [Header("Move Type Settings")]
    [Tooltip("Расстояния до смены точки")]
    public float pointReachThreshold = 0.1f;
    [Tooltip("Тип Движения")]
    public MovementMode movementMode = MovementMode.Patrol;

    [Header("Chase Settings")]
    [Tooltip("Преследует ли игрока")]
    public bool canChasePlayer = true;
    [Tooltip("Смещения до глаз")]
    public Vector2 eyeOffset = new Vector2(0f, 1.55f);
    [Tooltip("Дистанция переднего рейкаста")]
    public float frontRayDistance = 6f;
    [Tooltip("Дистанция заднего рейкаста")]
    public float backRayDistance = 3f;
    [Tooltip("Максимальное расстояние преследования игрока")]
    public float maxChaseDistance = 7f;
    [Tooltip("Скорость во время преследования")]
    public float chaseSpeedMultiplier = 1.5f;
    [Tooltip("Скорость возвращения")]
    public float returnSpeedMultiplier = 1f;
    [Tooltip("Есть ли анимация бега за персонажем")]
    public bool isRunningToPlayerAnim = true;

    [Header("Attack Settings")]
    [Tooltip("Урон")]
    public float attackDamage = 10f;
    [Tooltip("Длительность атаки")]
    public float attackDuration = 1.2f;
    [Tooltip("Радиус обнаружения персонажа")]
    public float playerAtckDetectRange = 0.55f;
    [Tooltip("Смещение области обнаружения")]
    public Vector2 detectAtckRangeOffset = Vector2.zero;
    [Tooltip("Область атаки")]
    public float attackRange = 0.57f;
    [Tooltip("Смещение области атаки")]
    public Vector2 attackRangeOffset = Vector2.zero; 
    [Tooltip("Слой игрока")]
    public LayerMask playerLayer;

    [Header("Damage Reaction Settings")]
    [Tooltip("Откидывается ли враг " +
        "(прерывается при получении урона)")]
    public bool isDamageKnocked = true;
    [Tooltip("Расстояние отскока")]
    public float knockbackDistance = 2f; 
    [Tooltip("Длительность отскока")]
    public float takeDamageDuration = 0.5f;
    [Tooltip("Тип звука атаки")]
    public eSoundType damagedSoundType;

    [Header("Wall Check Settings")]
    [Tooltip("Длина луча для проверки стены")]
    public float wallCheckRayDistance = 0.5f;
    public LayerMask wallLayer;

    [Header("Ground Check Settings")]
    [Tooltip("Радитус области проверки земли")]
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;
}
