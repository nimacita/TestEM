using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "ScriptableObjects/EnemySettings")]
public class EnemySettings : ScriptableObject
{

    [Header("Name Settings")]
    [Tooltip("��� �����")]
    public string enemyName;

    [Header("Health Settings")]
    [Tooltip("��������� ��������")]
    public float maxHealth = 20f;

    [Header("Movement Settings")]
    [Tooltip("������� �������� ��������")]
    public float moveSpeed = 2f;

    [Header("Move Type Settings")]
    [Tooltip("���������� �� ����� �����")]
    public float pointReachThreshold = 0.1f;
    [Tooltip("��� ��������")]
    public MovementMode movementMode = MovementMode.Patrol;

    [Header("Chase Settings")]
    [Tooltip("���������� �� ������")]
    public bool canChasePlayer = true;
    [Tooltip("�������� �� ����")]
    public Vector2 eyeOffset = new Vector2(0f, 1.55f);
    [Tooltip("��������� ��������� ��������")]
    public float frontRayDistance = 6f;
    [Tooltip("��������� ������� ��������")]
    public float backRayDistance = 3f;
    [Tooltip("������������ ���������� ������������� ������")]
    public float maxChaseDistance = 7f;
    [Tooltip("�������� �� ����� �������������")]
    public float chaseSpeedMultiplier = 1.5f;
    [Tooltip("�������� �����������")]
    public float returnSpeedMultiplier = 1f;
    [Tooltip("���� �� �������� ���� �� ����������")]
    public bool isRunningToPlayerAnim = true;

    [Header("Attack Settings")]
    [Tooltip("����")]
    public float attackDamage = 10f;
    [Tooltip("������������ �����")]
    public float attackDuration = 1.2f;
    [Tooltip("������ ����������� ���������")]
    public float playerAtckDetectRange = 0.55f;
    [Tooltip("�������� ������� �����������")]
    public Vector2 detectAtckRangeOffset = Vector2.zero;
    [Tooltip("������� �����")]
    public float attackRange = 0.57f;
    [Tooltip("�������� ������� �����")]
    public Vector2 attackRangeOffset = Vector2.zero; 
    [Tooltip("���� ������")]
    public LayerMask playerLayer;

    [Header("Damage Reaction Settings")]
    [Tooltip("������������ �� ���� " +
        "(����������� ��� ��������� �����)")]
    public bool isDamageKnocked = true;
    [Tooltip("���������� �������")]
    public float knockbackDistance = 2f; 
    [Tooltip("������������ �������")]
    public float takeDamageDuration = 0.5f;
    [Tooltip("��� ����� �����")]
    public eSoundType damagedSoundType;

    [Header("Wall Check Settings")]
    [Tooltip("����� ���� ��� �������� �����")]
    public float wallCheckRayDistance = 0.5f;
    public LayerMask wallLayer;

    [Header("Ground Check Settings")]
    [Tooltip("������� ������� �������� �����")]
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;
}
