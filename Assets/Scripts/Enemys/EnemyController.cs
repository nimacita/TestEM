using UnityEngine;
using System.Collections;
using UnityEngine.Windows;

public enum MovementMode { Patrol, Idle }

public class EnemyController : MonoBehaviour, IDamagable
{

    [Header("Settings")]
    [SerializeField] protected EnemySettings settings;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector2[] movePoints; // Локальные смещения от стартовой позиции
    [SerializeField] private float pointReachThreshold = 0.1f;

    [Header("Behavior")]
    [SerializeField] private MovementMode movementMode = MovementMode.Patrol;
    [SerializeField] private bool canChasePlayer = true;
    [SerializeField] private Vector2 eyeOffset = new Vector2(0f, 1f);
    [SerializeField] private float frontRayDistance = 4f;
    [SerializeField] private float backRayDistance = 3f;
    [SerializeField] private float maxChaseDistance = 6f;
    [SerializeField] private float chaseSpeedMultiplier = 1.2f;
    [SerializeField] private float returnSpeedMultiplier = 1f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float playerDetectRange = 1f;
    [SerializeField] private Vector2 detectRangeOffset = Vector2.zero;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private Vector2 attackRangeOffset = Vector2.zero; // смещение от attackPoint
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform attackPoint;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Damage Reaction")]
    [SerializeField] private float knockbackDistance = 2f; // расстояние отскока по X
    [SerializeField] private float takeDamageDuration = 0.15f; // Время стана при получении урона

    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected BoxCollider2D boxCollider;
    [SerializeField] protected EnemyAnimation anim;
    [SerializeField] protected ParticleSystem splashAttackEffect;
    [SerializeField] protected DamageFlash flash;

    [Header("Flags")]
    protected bool isAlive = false;
    protected bool isMoving = false;
    protected bool isAttacking = false;
    protected bool isDamaging = false;
    protected bool isFacingRight = true;
    protected bool isChasing = false;
    protected bool isReturning = false;

    [Header("Stats")]
    protected int currentPointIndex = 0;
    protected float currentHealth;
    protected float lastAttackTime = -999f;
    protected Vector2 startPosition;
    protected Vector2 returnTarget;
    protected Coroutine damageCoroutine;
    protected Coroutine attackCoroutine;
    protected Transform chasedPlayer = null;

    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        startPosition = transform.position;
        currentHealth = maxHealth;

        if (boxCollider) boxCollider.isTrigger = false;

        SetAnimSettings();

        isAlive = true;
    }

    protected virtual void SetAnimSettings()
    {
        if (anim != null)
            attackDuration = anim.GetAttackDuration();
    }

    #region Subscribes
    protected void OnEnable()
    {
        if (anim != null) anim.onAttacked += AttackedTrigger;
    }

    protected void OnDisable()
    {
        if (anim != null) anim.onAttacked -= AttackedTrigger;
    }
    #endregion

    protected virtual void Update()
    {
        TryAttack();
    }

    protected void FixedUpdate()
    {
        HandleMovementState();
        AnimationControlls();
    }

    #region Movement

    //обрабатываем доступные состояния
    protected void HandleMovementState()
    {
        if (isAttacking || isDamaging || !isAlive)
        {
            isMoving = false;
            return;
        }

        if (canChasePlayer)
            CheckChase();

        if (isChasing && chasedPlayer != null)
        {
            HandleChase();
        }
        else if (isReturning)
        {
            HandleReturn();
        }
        else if (movementMode == MovementMode.Patrol)
        {
            HandlePatrol();
        }
        else
        {
            HandleIdle();
        }
    }

    #region Movement States

    //прселедуем
    protected void HandleChase()
    {           
        float distX = Mathf.Abs(chasedPlayer.position.x - transform.position.x);

        //проверяем на дистанцию или на нужный слой
        if (distX > GetMaxChaseDistance() || !IsNeededLayer())
        {
            chasedPlayer = null;
            isChasing = false;
            isReturning = true;
            returnTarget = movementMode == MovementMode.Idle
                ? startPosition
                : startPosition + movePoints[FindClosestPointIndex(rb.position)];
            return;
        }

        isMoving = true;
        MoveTowards(chasedPlayer.position, moveSpeed * chaseSpeedMultiplier);
    }

    //возвращаемся до нужной точки
    protected void HandleReturn()
    {
        if (returnTarget == Vector2.zero)
            returnTarget = movementMode == MovementMode.Idle
                ? startPosition
                : startPosition + movePoints[FindClosestPointIndex(rb.position)];

        MoveTowards(returnTarget, moveSpeed * returnSpeedMultiplier);
        isMoving = true;

        if (Vector2.Distance(rb.position, returnTarget) <= pointReachThreshold)
        {
            isReturning = false;
            isChasing = false;
            chasedPlayer = null;

            if (movementMode == MovementMode.Patrol)
                currentPointIndex = FindClosestPointIndex(rb.position);
        }
    }

    //патрулируем
    protected void HandlePatrol()
    {
        if (movePoints.Length == 0)
        {
            isMoving = false;
            return;
        }

        isMoving = true;
        Vector2 targetPoint = startPosition + movePoints[currentPointIndex];
        MoveTowards(targetPoint, moveSpeed);

        if (Vector2.Distance(rb.position, targetPoint) <= pointReachThreshold)
            currentPointIndex = (currentPointIndex + 1) % movePoints.Length;
    }

    //стоим на месте 
    protected void HandleIdle()
    {
        if (Vector2.Distance(rb.position, startPosition) > 0.01f)
        {
            isMoving = true;
            MoveTowards(startPosition, moveSpeed);
        }
        else
        {
            isMoving = false;
        }
    }
    #endregion

    //идем до точки
    protected void MoveTowards(Vector2 worldTarget, float speed)
    {
        Vector2 newPosition = Vector2.MoveTowards(rb.position, worldTarget, speed * Time.fixedDeltaTime);
        newPosition.y = transform.position.y;
        FacingToPoint(newPosition);

        rb.MovePosition(newPosition);
    }

    //ищем ближайшую точку патруля по позиции мира
    protected int FindClosestPointIndex(Vector2 worldPos)
    {
        if (movePoints.Length == 0) return 0;
        float bestDist = float.MaxValue;
        int bestIndex = 0;
        for (int i = 0; i < movePoints.Length; i++)
        {
            Vector2 wp = startPosition + movePoints[i];
            float d = Vector2.SqrMagnitude(wp - worldPos);
            if (d < bestDist)
            {
                bestDist = d;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    protected void FacingToPoint(Vector2 targetPoint)
    {
        isFacingRight = targetPoint.x >= transform.position.x;
        Flip();
    }

    protected void Flip()
    {
        Vector3 newScale = transform.localScale;
        newScale.x = isFacingRight ? 1f : -1f;
        transform.localScale = newScale;
    }
    #endregion

    #region Chase Logic

    protected void CheckChase()
    {
        if (!canChasePlayer || isDamaging || isAttacking || !isAlive)
            return;

        Vector2 eyePos = (Vector2)transform.position + eyeOffset;
        float frontDir = isFacingRight ? 1f : -1f;

        // Два луча — вперёд и назад
        RaycastHit2D frontHit = Physics2D.Raycast(eyePos, new Vector2(frontDir, 0f), frontRayDistance, playerLayer);
        RaycastHit2D backHit = Physics2D.Raycast(eyePos, new Vector2(-frontDir, 0f), backRayDistance, playerLayer);

        // Игрок замечен
        if (frontHit.collider != null || backHit.collider != null)
        {
            chasedPlayer = frontHit.collider ? frontHit.transform : backHit.transform;
            isChasing = true;
            isReturning = false;
        }
        else if (isChasing && chasedPlayer == null)
        {
            // Потеряли игрока — начинаем возвращение
            isChasing = false;
            isReturning = true;
            returnTarget = movementMode == MovementMode.Idle
                ? startPosition
                : startPosition + movePoints[FindClosestPointIndex(rb.position)];
        }
    }

    //получаем максимальную дистанцию чейза
    protected float GetMaxChaseDistance()
    {
        float maxRay = frontRayDistance > backRayDistance ? 
            frontRayDistance : backRayDistance;

        if (maxRay == maxChaseDistance)
        {
            return maxChaseDistance + 1f;
        }

        float newMaxChaseDistance = maxChaseDistance > maxRay ? 
            maxChaseDistance : maxRay;

        return newMaxChaseDistance;
    }

    protected bool IsNeededLayer()
    {
        if (chasedPlayer == null) return false;
        return chasedPlayer.gameObject.layer == (int)Mathf.Log(playerLayer.value, 2);
    }

    #endregion

    #region Attack
    //можем ли атаковать
    protected virtual void TryAttack()
    {
        if (Time.time < lastAttackTime + attackDuration || isAttacking || !isAlive) return;

        Collider2D playerCollider = Physics2D.OverlapCircle(attackPoint.position + (Vector3)detectRangeOffset,
            playerDetectRange, playerLayer);

        if (playerCollider)
        {
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(AttackRoutine(playerCollider));
        }
    }

    //начинаем атаку и ждем ее завершения
    protected virtual IEnumerator AttackRoutine(Collider2D target)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        StartAttackAnim();

        lastAttackTime = Time.time;
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
    }

    //наносим урон вызваемый из анимации
    protected virtual void AttackedTrigger()
    {
        //Еффекст
        if (splashAttackEffect) splashAttackEffect.Play();
        // Наносим урон
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position + (Vector3)attackRangeOffset, attackRange, playerLayer);
        foreach (Collider2D hit in hits)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(attackDamage, transform.position);
            }
        }
    }

    //останавливаем атаку если она идет
    protected virtual void StopAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        isAttacking = false;
    }
    #endregion

    #region Damage/Health

    public void TakeDamage(float damage, Vector2 damageSourcePosition)
    {
        OnTakedDamage(damage, damageSourcePosition);
    }

    protected virtual void OnTakedDamage(float damage, Vector2 damageSourcePosition)
    {
        if (!isAlive) return;

        flash.Flash();
        StopAttack();
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (damageCoroutine != null) StopCoroutine(damageCoroutine);
        damageCoroutine = StartCoroutine(DamageReaction(damageSourcePosition));
    }

    protected virtual IEnumerator DamageReaction(Vector2 damageSourcePosition)
    {
        isDamaging = true;

        Vector2 startPos = rb.position;
        float knockDir = damageSourcePosition.x < startPos.x ? 1f : -1f;
        Vector2 finalPos = startPos + new Vector2(knockDir * knockbackDistance, 0f);
        float elapsed = 0f;

        //запускаю аницию получения урона
        float isFacingDir = isFacingRight ? 1f : -1f;
        StartTakedDamageAnim(isFacingDir != knockDir);

        //отскок врага в случае удара (фиксированный шаг)
        while (elapsed < takeDamageDuration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / takeDamageDuration);

            //для плавности
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            Vector2 newPos = Vector2.Lerp(startPos, finalPos, smoothT);

            rb.MovePosition(newPos);

            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(finalPos);

        isDamaging = false;
    }

    protected virtual void Die()
    {
        if (!isAlive) return;
        if (boxCollider) boxCollider.isTrigger = true;
        StartDieAnim();

        isAlive = false;
    }

    #endregion

    #region Animation Settings

    protected virtual void AnimationControlls()
    {
        MoveOrRunAnim();
    }

    //ходьба - бег
    protected virtual void MoveOrRunAnim()
    {
        if (isDamaging || isAttacking) return;

        if (isMoving)
        {
            anim.SetRunState(isChasing);
            anim.SetWalkState(!isChasing);
        }
        else
        {
            anim.SetWalkState(false);
            anim.SetRunState(false);
        }
    }

    //включаем и настраиваем анимацию получения урона
    protected virtual void StartTakedDamageAnim(bool isfront)
    {
        float damagedMulti = anim != null ? anim.GetDamagedDuration(isfront) / takeDamageDuration : 1f;
        if (anim != null) anim.SetTakeDamageMulti(damagedMulti);
        if (anim != null) anim.SetTakeDamageTrigger(isfront);
    }

    protected virtual void StartAttackAnim()
    {
        anim?.SetAttackTrigger();
    }

    protected virtual void StartDieAnim()
    {
        anim?.SetDieTrigger();
    }

    #endregion

    #region Gizmo
    private void OnDrawGizmosSelected()
    {
        if (attackPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position + (Vector3)attackRangeOffset, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position + (Vector3)detectRangeOffset, playerDetectRange);
        }

        if (movePoints.Length != 0 && movementMode == MovementMode.Patrol)
        {
            Gizmos.color = Color.cyan;
            Vector2 basePos = Application.isPlaying ? (Vector2)startPosition : (Vector2)transform.position;
            foreach (var p in movePoints)
            {
                Gizmos.DrawSphere(basePos + p, 0.1f);
            }
        }

        if (isChasing && chasedPlayer != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, chasedPlayer.position);
        }
        else
        {
            Vector2 eyePos = Application.isPlaying ? (Vector2)transform.position + eyeOffset : (Vector2)transform.position + eyeOffset;
            float frontDir = isFacingRight ? 1f : -1f;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(eyePos, eyePos + new Vector2(frontDir, 0f) * frontRayDistance);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(eyePos, eyePos + new Vector2(-frontDir, 0f) * backRayDistance);
        }
    }

    #endregion

}
