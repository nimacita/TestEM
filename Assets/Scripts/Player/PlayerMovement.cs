using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Settings")]
    private PlayerSettings settings;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private float airControl = 0.8f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float jumpHorizontalForce = 4f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [Tooltip("Ускорение падения")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [Tooltip("Ускорение падения при коротком прыжке")]
    [SerializeField] private float lowJumpMultiplier = 2f;
    [Tooltip("Дополнительная длительность для удержания прыжка (чтобы он был чуть выше)")]
    [SerializeField] private float maxJumpHoldTime = 0.15f;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpForce = 14f;
    [SerializeField] private float wallJumpHorizontalForce = 10f;
    [SerializeField] private float wallCoyoteTime = 0.15f;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Dodge")]
    [SerializeField] private float dodgeForce = 20f;
    [SerializeField] private float dodgeDuration = 0.25f;
    [SerializeField] private float dodgeCooldown = 1f;

    [Header("Damaged")]
    [SerializeField] private float damageDuration = 1f;
    [SerializeField] private float damagedKnockForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    private PlayerInput input;
    private PlayerAnimation anim;

    [Header("Flags")]
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private bool isOnAir = false;
    [SerializeField] private bool isDodged = false;
    [SerializeField] private bool canDodge = true;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isTouchingWall = false;
    [SerializeField] private bool isDamaged = false;
    [SerializeField] private bool isDied = false;

    [Header("Stats")]
    private int wallDir;
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
        HandleTimers();
        HandleDodgeTimers();
        HandleDamagedTimers();
        ReadInputForJumpBuffer();
        HandleFlip();
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();
        HandleMovement();
        ApplyGravityModifiers();
        AnimationControlls();
    }

    #region Timers
    private void HandleTimers()
    {
        if (isGrounded && isJumping)
        {
            coyoteTimer = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;
        if (jumpHoldTimer > 0f) jumpHoldTimer -= Time.deltaTime;

        if (isTouchingWall)
        {
            wallCoyoteTimer = wallCoyoteTime;
        }
        else
        {
            wallCoyoteTimer -= Time.deltaTime;
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
            jumpBufferTimer = jumpBufferTime;
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
        return (jumpBufferTimer > 0f && wallCoyoteTimer > 0f) && 
            ((wallDir < 0 && input.HorizontalInput > 0) ||
            (wallDir > 0 && input.HorizontalInput < 0));
    }

    private void PerformGroundJump()
    {
        jumpBufferTimer = 0f;
        coyoteTimer = 0f;

        isJumping = true;
        jumpHoldTimer = maxJumpHoldTime;

        Vector2 lv = rb.linearVelocity;
        lv.y = 0f;
        rb.linearVelocity = lv;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void PerformWallJump()
    {
        jumpBufferTimer = 0f;
        wallCoyoteTimer = 0f;

        isJumping = true;
        jumpHoldTimer = maxJumpHoldTime;

        Vector2 lv = rb.linearVelocity;
        lv.y = 0f;
        rb.linearVelocity = lv;

        Vector2 force = new Vector2(-wallDir * wallJumpHorizontalForce, wallJumpForce);
        rb.AddForce(force, ForceMode2D.Impulse);

        if ((wallDir == -1 && isFacingRight) || (wallDir == 1 && !isFacingRight))
            Flip();
    }

    private void CutJump()
    {
        Vector2 lv = rb.linearVelocity;
        if (lv.y > 0f)
        {
            lv.y *= jumpCutMultiplier;
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
        targetVelX = horiz * moveSpeed;

        float accel = isGrounded ? acceleration : acceleration * airControl;
        float decel = isGrounded ? deceleration : deceleration * airControl;

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
            lv.y += Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (lv.y > 0f && (!input.JumpHeld || jumpHoldTimer <= 0f))
        {
            // короткий прыжок
            lv.y += Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = lv;
    }
    #endregion

    #region Dodge
    private void HandleDodge()
    {
        if (!canDodge || isDodged || 
            isAttacking || isDamaged || isDied) return;

        isDodged = true;
        canDodge = false;
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;
        float dodgeDir = isFacingRight ? 1f : -1f;

        // сброс горизонтальной скорости
        Vector2 lv = rb.linearVelocity;
        lv.x = 0f;
        rb.linearVelocity = lv;

        // импульс доджа
        rb.AddForce(new Vector2(dodgeDir * dodgeForce, 0f), ForceMode2D.Impulse);
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
    #endregion

    #region Wall / Ground / Flip
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    private void CheckWall()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(wallCheckPoint.position, Vector2.left, wallCheckDistance, wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(wallCheckPoint.position, Vector2.right, wallCheckDistance, wallLayer);

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

        damageTimer = damageDuration;
        isDamaged = true;

        //определеяем направления откидывания
        float knockDir = damageSourcePosition.x < transform.position.x ? 1f : -1f;

        // сброс горизонтальной скорости
        Vector2 lv = rb.linearVelocity;
        lv.x = 0f;
        rb.linearVelocity = lv;

        // импульс доджа
        rb.AddForce(new Vector2(knockDir * damagedKnockForce, 0f), ForceMode2D.Impulse);
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
        float damagedMulti = anim.GetDamagedDuration(isFacingDir != knockDir) / dodgeDuration;
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

        if (!isJumping && isGrounded && input.HorizontalInput != 0f)
        {
            //если нынешняя скорость больше чем половина от заданной - бежим
            bool isWalking = Mathf.Abs(rb.linearVelocity.x) <= moveSpeed / 3f;
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
        float dodgeMulti =  anim.GetDodgeDuration() / dodgeDuration;
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
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }

        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.left * wallCheckDistance);
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.right * wallCheckDistance);
        }
    }
}
