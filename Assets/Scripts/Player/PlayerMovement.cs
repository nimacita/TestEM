using UnityEngine;
using Utilities.EventManager;

public class PlayerMovement : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private PlayerSettings settings;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private GameObject mainCollider, dodgeCollider;
    private PlayerInput input;
    private PlayerAnimation anim;

    [Header("Flags")]
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isFacingRight = true;
    private bool isOnAir = false;
    private bool isDodged = false;
    private bool canDodge = true;
    private bool isAttacking = false;
    private bool isTouchingWall = false;
    private bool isDamaged = false;
    private bool isDied = false;
    private bool isCanWallJump = false;
    private bool isInited = false;

    [Header("Stats")]
    private int wallDir;
    private int lastWallJumpDir = 0;
    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    private float jumpHoldTimer = 0f;
    private float dodgeTimer = 0f;
    private float dodgeCooldownTimer = 0f;
    private float wallCoyoteTimer = 0f;
    private float damageTimer = 0f;
    private float targetVelX;

    public bool IsGrounded { get => isGrounded; private set => isGrounded = value; }
    public bool IsDodged { get => isDodged; private set => isDodged = value; }
    public bool IsFacingRight { get => isFacingRight; private set => isFacingRight = value; }
    public bool IsDamaged { get => isDamaged; private set => isDamaged = value; }

    private void OnDisable()
    {
        Unsubscribes();
    }

    public void InitMovement(PlayerInput playerInput, PlayerAnimation playerAnimation)
    {
        input = playerInput;
        anim = playerAnimation;

        Subscribes();
        isInited = true;
    }

    #region Subscribes

    private void Subscribes()
    {
        if (input != null) input.OnDodgePressed += HandleDodge;
    }

    private void Unsubscribes()
    {
        if (input != null) input.OnDodgePressed -= HandleDodge;
    }

    #endregion

    private void Update()
    {
        if (!isInited) return;
        HandleTimers();
        HandleDodgeTimers();
        HandleDamagedTimers();
        ReadInputForJumpBuffer();
        HandleFlip();
        SelectColliders();
    }

    private void FixedUpdate()
    {
        if (!isInited) return;
        CheckGround();
        CheckWall();
        HandleMovement();
        ApplyGravityModifiers();
        AnimationControlls();
    }

    #region Timers
    private void HandleTimers()
    {
        if ((isGrounded || isTouchingWall) && isJumping)
        {
            coyoteTimer = settings.coyoteTime;
            isJumping = false;
        }
        else if (coyoteTimer > 0f)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = 0f;
        }

        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;
        if (jumpHoldTimer > 0f) jumpHoldTimer -= Time.deltaTime;

        if (isTouchingWall)
        {
            wallCoyoteTimer = settings.wallCoyoteTime;
        }
        else if(wallCoyoteTimer > 0f)
        {
            wallCoyoteTimer -= Time.deltaTime;
        }
        else
        {
            wallCoyoteTimer = 0f;
        }
    }
    #endregion

    #region Input & Jump
    private void ReadInputForJumpBuffer()
    {
        if (isDodged || isAttacking 
            || isDamaged || isDied) return;

        if (input.JumpPressed && !isJumping)
        {
            jumpBufferTimer = settings.jumpBufferTime;
        }

        if (CanJumpGround())
            PerformGroundJump();
        else if (CanWallJump())
            PerformWallJump();

        if (input.JumpReleased)
            CutJump();
    }

    private bool CanJumpGround()
    {
        return jumpBufferTimer > 0f && (coyoteTimer > 0f || isGrounded) && !isJumping;
    }

    private bool CanWallJump()
    {
        if (!isTouchingWall || isGrounded) return false;

        bool oppositeInput =
        (wallDir < 0 && input.HorizontalInput > 0) ||
        (wallDir > 0 && input.HorizontalInput < 0);

        isCanWallJump = (jumpBufferTimer > 0f && wallCoyoteTimer > 0f && oppositeInput && wallDir != lastWallJumpDir);
        //Debug.Log($"Пытаемся прыгнуть (jumpBufferTimer {jumpBufferTimer}) (wallCoyoteTimer {wallCoyoteTimer}) (oppositeInput {oppositeInput}) (wallDir {wallDir}) (lastWallJumpDir {lastWallJumpDir})");
        return isCanWallJump;
    }

    private void PerformGroundJump()
    {
        jumpBufferTimer = 0f;
        coyoteTimer = 0f;

        isJumping = true;
        jumpHoldTimer = settings.maxJumpHoldTime;

        Vector2 lv = rb.linearVelocity;
        lv.y = 0f;
        rb.linearVelocity = lv;

        rb.AddForce(Vector2.up * settings.jumpForce, ForceMode2D.Impulse);
    }

    private void PerformWallJump()
    {
        jumpBufferTimer = 0f;
        wallCoyoteTimer = 0f;

        isJumping = true;
        jumpHoldTimer = settings.maxJumpHoldTime;

        Vector2 lv = rb.linearVelocity;
        lv.y = 0f;
        rb.linearVelocity = lv;

        Vector2 force = new Vector2(-wallDir * settings.wallJumpHorizontalForce, settings.wallJumpForce);
        rb.AddForce(force, ForceMode2D.Impulse);

        lastWallJumpDir = wallDir; 

        if ((wallDir == -1 && isFacingRight) || (wallDir == 1 && !isFacingRight))
            Flip();
    }

    private void CutJump()
    {
        Vector2 lv = rb.linearVelocity;
        if (lv.y > 0f)
        {
            lv.y *= settings.jumpCutMultiplier;
            rb.linearVelocity = lv;
        }
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        if (isDodged || isAttacking || 
            (isTouchingWall && isOnAir) 
            || isDamaged || isDied) return;

        float horiz = input.HorizontalInput;
        targetVelX = horiz * settings.moveSpeed;

        float accel = isGrounded ? settings.acceleration : settings.acceleration * settings.airControl;
        float decel = isGrounded ? settings.deceleration : settings.deceleration * settings.airControl;

        float newVelX;
        if (Mathf.Abs(targetVelX) > 0.01f)
            newVelX = Mathf.MoveTowards(rb.linearVelocity.x, targetVelX, accel * Time.fixedDeltaTime);
        else
            newVelX = Mathf.MoveTowards(rb.linearVelocity.x, 0f, decel * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);
    }
    #endregion

    #region Gravity Modifiers
    private void ApplyGravityModifiers()
    {
        Vector2 lv = rb.linearVelocity;

        if (lv.y < 0f)
        {
            // быстрее падать
            lv.y += Physics2D.gravity.y * (settings.fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (lv.y > 0f && (!input.JumpHeld || jumpHoldTimer <= 0f))
        {
            // короткий прыжок
            lv.y += Physics2D.gravity.y * (settings.lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = lv;
    }
    #endregion

    #region Dodge
    private void HandleDodge()
    {
        if (!canDodge || isDodged || 
            isAttacking || isDamaged || isDied
            || !isInited) return;

        isDodged = true;
        canDodge = false;
        dodgeTimer = settings.dodgeDuration;
        dodgeCooldownTimer = settings.dodgeCooldown;
        float dodgeDir = isFacingRight ? 1f : -1f;

        EventManager.InvokeEvent(eEventType.onPlaySound, eSoundType.dash);

        // сброс горизонтальной скорости
        Vector2 lv = rb.linearVelocity;
        lv.x = 0f;
        rb.linearVelocity = lv;

        // импульс доджа
        rb.AddForce(new Vector2(dodgeDir * settings.dodgeForce, 0f), ForceMode2D.Impulse);
        StartDodgeAnim();
    }

    private void HandleDodgeTimers()
    {
        if (isDodged)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0f)
            {
                isDodged = false;
                EndDodgeAnim();
            }
        }

        if (!canDodge)
        {
            dodgeCooldownTimer -= Time.deltaTime;
            if (dodgeCooldownTimer <= 0f)
                canDodge = true;
        }
    }

    private void SelectColliders()
    {
        mainCollider.SetActive(!isDodged);
        dodgeCollider.SetActive(isDodged);
    }

    #endregion

    #region Wall / Ground / Flip
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, settings.groundCheckRadius, settings.groundLayer);

        if (isGrounded && lastWallJumpDir != 0) 
            lastWallJumpDir = 0;
    }

    private void CheckWall()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(wallCheckPoint.position, Vector2.left, settings.wallCheckDistance, settings.wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(wallCheckPoint.position, Vector2.right, settings.wallCheckDistance, settings.wallLayer);

        if (hitLeft)
        {
            isTouchingWall = true;
            wallDir = -1;
        }
        else if (hitRight)
        {
            isTouchingWall = true;
            wallDir = 1;
        }
        else
        {
            isTouchingWall = false;
        }
    }

    private void HandleFlip()
    {
        if (isDodged || isAttacking || (isTouchingWall && isOnAir) || isDied) return;
        if (input.HorizontalInput > 0 && !isFacingRight) Flip();
        else if (input.HorizontalInput < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }
    #endregion

    #region Attacking

    public void DisableMovementForAttack()
    {
        isAttacking = true;
    }

    public void AttackMoveDash(float attackDashForce)
    {
        //Если стою
        if (Mathf.Abs(rb.linearVelocity.x) < 0.1f) return;

        Vector2 lv = rb.linearVelocity;
        lv.x = 0f;
        rb.linearVelocity = lv;

        float dashDir = isFacingRight ? 1f : -1f;

        // импульс доджа
        rb.AddForce(new Vector2(dashDir * attackDashForce, 0f), ForceMode2D.Impulse);
    }

    public void EnableMovementAfterAttack()
    {
        isAttacking = false;
    }

    #endregion

    #region Taked Damage

    //получили урон
    public void OnTakedDamage(Vector2 damageSourcePosition)
    {
        if (isDied) return;

        damageTimer = settings.damageDuration;
        isDamaged = true;

        //определеяем направления откидывания
        float knockDir = damageSourcePosition.x < transform.position.x ? 1f : -1f;

        // сброс горизонтальной скорости
        Vector2 lv = rb.linearVelocity;
        lv.x = 0f;
        rb.linearVelocity = lv;

        // импульс доджа
        rb.AddForce(new Vector2(knockDir * settings.damagedKnockForce, 0f), ForceMode2D.Impulse);
        StartDamageKnockAnim(damageSourcePosition);
    }

    private void HandleDamagedTimers()
    {
        if (isDamaged)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <=0f)
            {
                damageTimer = 0f;
                isDamaged = false;
            }
        }
    }

    //запускаем и настраиваем анимацию ткидывания
    private void StartDamageKnockAnim(Vector2 damageSourcePosition)
    {
        //определяем направление удара
        float knockDir = damageSourcePosition.x < transform.position.x ? 1f : -1f;
        float isFacingDir = isFacingRight ? 1f : -1f;

        //настраиваем длительность
        float damagedMulti = anim.GetDamagedDuration(isFacingDir != knockDir) / settings.dodgeDuration;
        anim.SetDamageMulti(damagedMulti);

        //запускаем
        anim.SetDamagedTrigger(isFacingDir != knockDir);
    }

    //игрок умер
    public void OnPlayerDied()
    {
        if (isDied) return;

        isDied = true;

        // сброс горизонтальной скорости
        Vector2 lv = rb.linearVelocity;
        lv.x = 0f;
        rb.linearVelocity = lv;
    }

    public void OnGameEnded()
    {
        if (!isInited) return;

        isInited = false;

        // сброс горизонтальной скорости
        Vector2 lv = rb.linearVelocity;
        lv.x = 0f;
        rb.linearVelocity = lv;
    }

    #endregion

    #region Animation Controlls
    private void AnimationControlls()
    {
        MoveOrRunAnim();
        JumpOrLandAnim();
        WallHoldAnim();
    }

    //ходьба - бег
    private void MoveOrRunAnim()
    {
        if (isDodged || isAttacking || isDied) return;

        if (!isJumping && isGrounded && input.HorizontalInput != 0f && isInited)
        {
            //если нынешняя скорость больше чем половина от заданной - бежим
            bool isWalking = Mathf.Abs(rb.linearVelocity.x) <= settings.moveSpeed / 3f;
            anim.SetRunState(!isWalking);
            anim.SetWalkState(isWalking);
        }
        else
        {
            anim.SetWalkState(false);
            anim.SetRunState(false);
        }
    }

    //если упал на землю когда был в воздухе - приземляемся
    private void JumpOrLandAnim()
    {
        if (isGrounded && isOnAir)
        {
            anim.SetJumpEnd();
            isOnAir = false;
        }
        //Если не на земле и не был в воздухе
        if (!isGrounded && !isOnAir)
        {
            anim.SetJumpStart();
            isOnAir = true;
        }

        anim.SetInAir(!isGrounded);
    }

    private void WallHoldAnim()
    {
        if (isDied) return;
        if (!isGrounded && !isDodged && !isAttacking && isTouchingWall)
        {
            int charDir = isFacingRight ? 1 : -1;
            if (charDir != wallDir) Flip();
            anim.SetHoldWall(isTouchingWall);
        }
        else
        {
            anim.SetHoldWall(false);
        }
    }

    //включаем и настраиваем анимацию доджа
    private void StartDodgeAnim()
    {
        float dodgeMulti =  anim.GetDodgeDuration() / settings.dodgeDuration;
        anim.SetDodgeMulti(dodgeMulti);
        anim.SetDodgeStart();
    }

    private void EndDodgeAnim()
    {
        anim.SetDodgeEnd();
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, settings.groundCheckRadius);
        }

        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.left * settings.wallCheckDistance);
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.right * settings.wallCheckDistance);
        }
    }
}
