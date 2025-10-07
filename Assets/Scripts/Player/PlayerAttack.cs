using UnityEngine;
using Utilities.EventManager;

public class PlayerAttack : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private PlayerSettings settings;

    [Header("Components")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private ParticleSystem splashEffect;
    private PlayerInput input;
    private PlayerAnimation anim;
    private PlayerMovement movement;

    //Stats
    private float lastAttackTime = -999f;
    private bool isAttacking = false;
    private bool isDied = false;
    private bool isInited = false;

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
        isInited = true;
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
        if (!isInited) return;
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
        if (Time.time < lastAttackTime + settings.attackCoolDown) return; // ������� �� ������
        if (isAttacking) return; // ��� � �������� �����

        //�������� �����: ������������� ���������, ��������� �������� � ���������� ������
        StartAttack();
    }

    private void StartAttack()
    {
        isAttacking = true;

        EventManager.InvokeEvent(eEventType.onPlaySound, eSoundType.swordSwish);

        //��������� ������� �� ������� �����
        lastAttackTime = Time.time;

        //����� �� ����� �����
        movement.DisableMovementForAttack();
        movement.AttackMoveDash(settings.attackDashForce);

        //��������� �������� �����
        StartAttackAnim();
    }

    private void OnAttackTrigger()
    {
        if (!isAttacking || isDied) return;

        StartSplashEffect();

        //������� �������� � ������ ��������
        Vector2 center = (attackPoint != null) ? (Vector2)attackPoint.position + settings.AttackZoneOffset : (Vector2)transform.position + settings.AttackZoneOffset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, settings.attackRadius, settings.attackMask);

        if (hits != null && hits.Length > 0)
        {
            foreach (var c in hits)
            {
                IDamagable iDamagable = c.GetComponent<IDamagable>();
                if (iDamagable != null)
                {
                    iDamagable.TakeDamage(settings.damage, transform.position, "Player");
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

    public void OnGameEnded()
    {
        if (!isInited) return;
        isInited = false;
    }

    #region Animation Settings

    private void StartAttackAnim()
    {
        float attackMulti = anim.GetAttackDuration() / settings.attackDuration;
        anim.SetAttackMulti(attackMulti);
        anim.SetAttackTrigger();
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Vector3 pos = attackPoint.position + (Vector3)settings.AttackZoneOffset;
        Gizmos.DrawWireSphere(pos, settings.attackRadius);

        if (isAttacking)
        {
            Gizmos.DrawSphere(pos, 0.05f);
        }
    }
}

