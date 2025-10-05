using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private ParticleSystem splashEffect;
    private PlayerInput input;
    private PlayerAnimation anim;
    private PlayerMovement movement;

    [Header("Stats")]
    [SerializeField] private float attackCoolDown = 1f; // ����� ����� �������
    [SerializeField] private float attackDuration = 0.4f; // ������������ ����� �����
    [SerializeField] private Vector2 offset = Vector2.zero; // �������� �� attackPoint
    [SerializeField] private float attackRadius = 0.5f; // ������ ����� Overlap
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackDashForce = 2f; //���� ����� �����
    [SerializeField] private LayerMask enemyMask;

    //Stats
    private float lastAttackTime = -999f;
    private bool isAttacking = false;
    private bool isDied = false;

    private void OnDisable()
    {
        Unsubscribes();    
    }

    public void InitAttack(PlayerInput playerInput, 
        PlayerAnimation playerAnimation, 
        PlayerMovement playerMovement)
    {
        input = playerInput; 
        anim = playerAnimation; 
        movement = playerMovement;

        Subscribes();
    }

    #region Subscribes

    private void Subscribes()
    {
        anim.OnAttacked += OnAttackTrigger;
        anim.OnAttackEnded += OnAttackEnded;
    }

    private void Unsubscribes()
    {
        anim.OnAttacked -= OnAttackTrigger;
        anim.OnAttackEnded -= OnAttackEnded;
    }

    #endregion

    private void FixedUpdate()
    {
        AttackHandle();
    }

    private void AttackHandle()
    {
        if (input == null) return;
        if (movement.IsDodged || movement.IsDamaged || isDied) return;

        if (input.AttackInput)
        {
            TryStartAttack();
        }
    }

    private void TryStartAttack()
    {
        if (Time.time < lastAttackTime + attackCoolDown) return; // ������� �� ������
        if (isAttacking) return; // ��� � �������� �����

        //�������� �����: ������������� ���������, ��������� �������� � ���������� ������
        StartAttack();
    }

    private void StartAttack()
    {
        isAttacking = true;

        //��������� ������� �� ������� �����
        lastAttackTime = Time.time;

        //����� �� ����� �����
        movement.DisableMovementForAttack();
        movement.AttackMoveDash(attackDashForce);

        //��������� �������� �����
        StartAttackAnim();
    }

    private void OnAttackTrigger()
    {
        if (!isAttacking || isDied) return;

        StartSplashEffect();

        //������� �������� � ������ ��������
        Vector2 center = (attackPoint != null) ? (Vector2)attackPoint.position + offset : (Vector2)transform.position + offset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, enemyMask);

        if (hits != null && hits.Length > 0)
        {
            foreach (var c in hits)
            {
                IDamagable iDamagable = c.GetComponent<IDamagable>();
                if (iDamagable != null)
                {
                    iDamagable.TakeDamage(damage, transform.position);
                }
            }
        }
    }

    //��������� �����
    private void OnAttackEnded()
    {
        if (isAttacking)
        {
            movement.EnableMovementAfterAttack();
            isAttacking = false;
        }
    }

    //������ ������
    private void StartSplashEffect()
    {
        splashEffect.Play();
    }

    public void OnTakedDamage()
    {
        OnAttackEnded();
    }

    public void OnPlayerDied()
    {
        if (isDied) return;
        isDied = true;
    }

    #region Animation Settings

    private void StartAttackAnim()
    {
        float attackMulti = anim.GetAttackDuration() / attackDuration;
        anim.SetAttackMulti(attackMulti);
        anim.SetAttackTrigger();
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Vector3 pos = attackPoint.position + (Vector3)offset;
        Gizmos.DrawWireSphere(pos, attackRadius);

        if (isAttacking)
        {
            Gizmos.DrawSphere(pos, 0.05f);
        }
    }
}

