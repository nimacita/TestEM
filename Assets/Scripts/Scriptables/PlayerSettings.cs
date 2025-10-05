using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Health Settings")]
    [Tooltip("��������� �������� ������")]
    public float playerHealth = 50f;

    [Header("Attack Settings")]
    [Tooltip("���� ������")]
    public int damage = 10;
    [Tooltip("����� ����� ������� (����� ����)")]
    public float attackCoolDown = 1.2f;
    [Tooltip("������������ ����� �����")]
    public float attackDuration = 0.5f;
    [Tooltip("�������� ���� ����� �� ����� �����")]
    public Vector2 offset = new Vector2(0.05f, 0.03f);
    [Tooltip("������ �����")]
    public float attackRadius = 0.7f;
    [Tooltip("���� ���� �����")]
    public float attackDashForce = 13f;
    [Tooltip("����� ����� ���������� ��� �����")]
    public LayerMask attackMask;

    [Header("Movement Settings")]
    [Tooltip("�������� ��������")]
    public float moveSpeed = 8f;
    [Tooltip("���������")]
    public float acceleration = 50f;
    [Tooltip("������ ���������")]
    public float deceleration = 60f;
    [Tooltip("��������� �������� ���������� � �������")]
    public float airControl = 0.8f;

    [Header("Jump Settings")]
    [Tooltip("���� ������")]
    public float jumpForce = 14f;
    [Tooltip("���� ������ �� �����������")]
    public float jumpHorizontalForce = 4f;
    [Tooltip("����� ������")]
    public float coyoteTime = 0.12f;
    [Tooltip("����� �� ������ ����� ����� � ���������")]
    public float jumpBufferTime = 0.12f;
    [Tooltip("���� ����� ������ ��� ���������� �������")]
    public float jumpCutMultiplier = 0.5f;
    [Tooltip("��������� �������")]
    public float fallMultiplier = 4f;
    [Tooltip("��������� ������� ��� �������� ������")]
    public float lowJumpMultiplier = 3f;
    [Tooltip("�������������� ������������ ��� ��������� ������ (����� �� ��� ���� ����)")]
    public float maxJumpHoldTime = 0.1f;

    [Header("Wall Jump Settings")]
    [Tooltip("���� ������ �� �����")]
    public float wallJumpForce = 14f;
    [Tooltip("���� ��������������� ������ �� �����")]
    public float wallJumpHorizontalForce = 10f;
    [Tooltip("����� ������")]
    public float wallCoyoteTime = 0.15f;
    [Tooltip("��������� �������� �����")]
    public float wallCheckDistance = 0.3f;
    [Tooltip("���� ���� ��� ������������ ��� ������")]
    public LayerMask wallLayer;

    [Header("Dodge Settings")]
    [Tooltip("���� ����")]
    public float dodgeForce = 15f;
    [Tooltip("������������ ����")]
    public float dodgeDuration = 0.4f;
    [Tooltip("����� ����� ������")]
    public float dodgeCooldown = 1f;

    [Header("Damaged Settings")]
    [Tooltip("������������ ��������� ����� �����")]
    public float damageDuration = 0.4f;
    [Tooltip("���� ����������� ����� �����")]
    public float damagedKnockForce = 4f;

    [Header("Ground Check Settings")]
    [Tooltip("������ �������� ����")]
    public float groundCheckRadius = 0.12f;
    [Tooltip("���� ����������� ����� ��� ������")]
    public LayerMask groundLayer;

    [Header("Main Layer Settings")]
    public LayerMask playerLayer;
    public LayerMask diedLayer;
}
