using UnityEngine;
using System.Collections;
using UnityEngine.Windows;

public enum MovementMode { Patrol, Idle }

public class EnemyController : MonoBehaviour, IDamagable
{

    [Header("Settings")]
    [SerializeField] protected EnemySettings settings;

    [Header("Move Settings")]
    [Tooltip("“очки передвижени€")]
    [SerializeField] protected Vector2[] movePoints;

    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected BoxCollider2D boxCollider;
    [SerializeField] protected EnemyAnimation anim;
    [SerializeField] protected ParticleSystem splashAttackEffect;
    [SerializeField] protected DamageFlash flash;
    [SerializeField] private Transform attackPoint;

    [Header("Flags")]
    [SerializeField] protected bool isFacingRight = true;
    protected bool isAlive = false;
    protected bool isMoving = false;
    protected bool isAttacking = false;
    protected bool isDamaging = false;
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
        currentHealth = settings.maxHealth;

        if (boxCollider) boxCollider.isTrigger = false;

        //SetAnimSettings();

        isAlive = true;
    }

    protected virtual void SetAnimSettings()
    {
        //if (anim != null)
            //settings.attackDuration = anim.GetAttackDuration();
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

    protected void FixedUpdate()
    {
        TryAttack();
        HandleMovementState();
        AnimationControlls();
    }

    #region Movement

    //обрабатываем доступные состо€ни€
    protected void HandleMovementState()
    {
        if (isAttacking || isDamaging || !isAlive)
        {
            isMoving = false;
            return;
        }

        if (settings.canChasePlayer)
            CheckChase();

        if (isChasing && chasedPlayer != null)
        {
            HandleChase();
        }
        else if (isReturning)
        {
            HandleReturn();
        }
        else if (settings.movementMode == MovementMode.Patrol)
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

        //провер€ем на дистанцию или на нужный слой
        if (distX > GetMaxChaseDistance() || !IsNeededLayer())
        {
            chasedPlayer = null;
            isChasing = false;
            isReturning = true;
            returnTarget = settings.movementMode == MovementMode.Idle
                ? startPosition
                : startPosition + movePoints[FindClosestPointIndex(rb.position)];
            return;
        }

        isMoving = true;
        MoveTowards(chasedPlayer.position, settings.moveSpeed * settings.chaseSpeedMultiplier);
    }

    //возвращаемс€ до нужной точки
    protected void HandleReturn()
    {
        if (returnTarget == Vector2.zero)
            returnTarget = settings.movementMode == MovementMode.Idle
                ? startPosition
                : startPosition + movePoints[FindClosestPointIndex(rb.position)];

        MoveTowards(returnTarget, settings.moveSpeed * settings.returnSpeedMultiplier);
        isMoving = true;

        if (Vector2.Distance(rb.position, returnTarget) <= settings.pointReachThreshold)
        {
            isReturning = false;
            isChasing = false;
            chasedPlayer = null;

            if (settings.movementMode == MovementMode.Patrol)
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
        MoveTowards(targetPoint, settings.moveSpeed);

        if (Vector2.Distance(rb.position, targetPoint) <= settings.pointReachThreshold)
            currentPointIndex = (currentPointIndex + 1) % movePoints.Length;
    }

    //стоим на месте 
    protected void HandleIdle()
    {
        if (Vector2.Distance(rb.position, startPosition) > 0.01f)
        {
            isMoving = true;
            MoveTowards(startPosition, settings.moveSpeed);
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
        FacingToPoint(worldTarget);

        rb.MovePosition(newPosition);
    }

    //ищем ближайшую точку патрул€ по позиции мира
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
        if (!settings.canChasePlayer || isDamaging || isAttacking || !isAlive)
            return;

        Vector2 eyePos = (Vector2)transform.position + settings.eyeOffset;
        float frontDir = isFacingRight ? 1f : -1f;

        // ƒва луча Ч вперЄд и назад
        RaycastHit2D frontHit = Physics2D.Raycast(eyePos, new Vector2(frontDir, 0f), settings.frontRayDistance, settings.playerLayer);
        RaycastHit2D backHit = Physics2D.Raycast(eyePos, new Vector2(-frontDir, 0f), settings.backRayDistance, settings.playerLayer);

        // »грок замечен
        if (frontHit.collider != null || backHit.collider != null)
        {
            chasedPlayer = frontHit.collider ? frontHit.transform : backHit.transform;
            isChasing = true;
            isReturning = false;
        }
        else if (isChasing && chasedPlayer == null)
        {
            // ѕотер€ли игрока Ч начинаем возвращение
            isChasing = false;
            isReturning = true;
            returnTarget = settings.movementMode == MovementMode.Idle
                ? startPosition
                : startPosition + movePoints[FindClosestPointIndex(rb.position)];
        }
    }

    //получаем максимальную дистанцию чейза
    protected float GetMaxChaseDistance()
    {
        float maxRay = settings.frontRayDistance > settings.backRayDistance ?
            settings.frontRayDistance : settings.backRayDistance;

        if (maxRay == settings.maxChaseDistance)
        {
            return settings.maxChaseDistance + 1f;
        }

        float newMaxChaseDistance = settings.maxChaseDistance > maxRay ?
            settings.maxChaseDistance : maxRay;

        return newMaxChaseDistance;
    }

    protected bool IsNeededLayer()
    {
        if (chasedPlayer == null) return false;
        return chasedPlayer.gameObject.layer == (int)Mathf.Log(settings.playerLayer.value, 2);
    }

    #endregion

    #region Attack
    //можем ли атаковать
    protected virtual void TryAttack()
    {
        if (Time.time < lastAttackTime + settings.attackDuration || isAttacking || !isAlive) return;

        Collider2D playerCollider;
        if (isFacingRight)
        {
            playerCollider = Physics2D.OverlapCircle(attackPoint.position + (Vector3)settings.detectAtckRangeOffset, 
                settings.playerAtckDetectRange, settings.playerLayer);
        }
        else
        {
            playerCollider = Physics2D.OverlapCircle(attackPoint.position - (Vector3)settings.detectAtckRangeOffset,
                settings.playerAtckDetectRange, settings.playerLayer);
        }

        if (playerCollider)
        {
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine(AttackRoutine(playerCollider));
        }
    }

    //начинаем атаку и ждем ее завершени€
    protected virtual IEnumerator AttackRoutine(Collider2D target)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        StartAttackAnim();

        lastAttackTime = Time.time;
        yield return new WaitForSeconds(settings.attackDuration);

        isAttacking = false;
    }

    //наносим урон вызваемый из анимации
    protected virtual void AttackedTrigger()
    {
        //≈ффекст
        if (splashAttackEffect) splashAttackEffect.Play();

        // Ќаносим урон
        Collider2D[] hits;
        if (isFacingRight)
        {
             hits = Physics2D.OverlapCircleAll(attackPoint.position + (Vector3)settings.attackRangeOffset, 
                settings.attackRange, settings.playerLayer);
        }
        else
        {
            hits = Physics2D.OverlapCircleAll(attackPoint.position - (Vector3)settings.attackRangeOffset,
                settings.attackRange, settings.playerLayer);
        }

        foreach (Collider2D hit in hits)
        {
            IDamagable damagable = hit.transform.parent.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(settings.attackDamage, transform.position);
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
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (settings.isDamageKnocked)
        {
            StopAttack();
            if (damageCoroutine != null) StopCoroutine(damageCoroutine);
            damageCoroutine = StartCoroutine(DamageReaction(damageSourcePosition));
        }
    }

    protected virtual IEnumerator DamageReaction(Vector2 damageSourcePosition)
    {
        isDamaging = true;

        Vector2 startPos = rb.position;
        float knockDir = damageSourcePosition.x < startPos.x ? 1f : -1f;
        Vector2 finalPos = startPos + new Vector2(knockDir * settings.knockbackDistance, 0f);
        float elapsed = 0f;

        //запускаю аницию получени€ урона
        float isFacingDir = isFacingRight ? 1f : -1f;
        StartTakedDamageAnim(isFacingDir != knockDir);

        //отскок врага в случае удара (фиксированный шаг)
        while (elapsed < settings.takeDamageDuration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / settings.takeDamageDuration);

            //дл€ плавности
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
        isAlive = false;
        StartDieAnim();
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
            if (settings.isRunningToPlayerAnim)
            {
                anim.SetRunState(isChasing);
                anim.SetWalkState(!isChasing);
            }
            else
            {
                anim.SetRunState(false);
                anim.SetWalkState(true);
            }

        }
        else
        {
            anim.SetWalkState(false);
            anim.SetRunState(false);
        }
    }

    //включаем и настраиваем анимацию получени€ урона
    protected virtual void StartTakedDamageAnim(bool isfront)
    {
        float damagedMulti = anim.GetDamagedDuration(isfront) / settings.takeDamageDuration;
        anim?.SetTakeDamageMulti(damagedMulti);
        anim?.SetTakeDamageTrigger(isfront);
    }

    protected virtual void StartAttackAnim()
    {
        float attackMulti = anim.GetAttackDuration() / settings.attackDuration;
        anim?.SetAttackMulti(attackMulti);
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
            if(isFacingRight) Gizmos.DrawWireSphere(attackPoint.position + (Vector3)settings.attackRangeOffset, settings.attackRange);
            else Gizmos.DrawWireSphere(attackPoint.position - (Vector3)settings.attackRangeOffset, settings.attackRange);

            Gizmos.color = Color.yellow;
            if(isFacingRight) Gizmos.DrawWireSphere(attackPoint.position + (Vector3)settings.detectAtckRangeOffset, settings.playerAtckDetectRange);
            else Gizmos.DrawWireSphere(attackPoint.position - (Vector3)settings.detectAtckRangeOffset, settings.playerAtckDetectRange);
        }

        if (movePoints.Length != 0 && settings.movementMode == MovementMode.Patrol)
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
            Vector2 eyePos = Application.isPlaying ? (Vector2)transform.position + settings.eyeOffset : (Vector2)transform.position + settings.eyeOffset;
            float frontDir = isFacingRight ? 1f : -1f;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(eyePos, eyePos + new Vector2(frontDir, 0f) * settings.frontRayDistance);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(eyePos, eyePos + new Vector2(-frontDir, 0f) * settings.backRayDistance);
        }
    }

    #endregion

}
