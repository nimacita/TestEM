using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    [Header("Input Settings")]
    private KeyCode leftKey = KeyCode.A;
    private KeyCode rightKey = KeyCode.D;
    private KeyCode dodgeKey = KeyCode.LeftShift;
    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode attackKey = KeyCode.Mouse0;

    public float HorizontalInput { get; private set; }
    public float DodgeInput { get; private set; }
    public bool AttackInput {  get; private set; }
    public Action OnDodgePressed { get; set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool JumpHeld { get; private set; }

    private void Update()
    {
        HandleMoveInput();
        HandleJumpInput();
        HandleAttackInput();
    }

    private void HandleMoveInput()
    {
        HorizontalInput = 0f;

        if (Input.GetKey(leftKey))
            HorizontalInput = -1f;

        if (Input.GetKey(rightKey))
            HorizontalInput = 1f;

        HandleDodgeInput();
    }

    //кувырок в определенную сторону
    private void HandleDodgeInput()
    {
        DodgeInput = 0f;
        if (Input.GetKeyDown(dodgeKey))
        {
            OnDodgePressed?.Invoke();
        }
    }

    //инпут для прыжков
    private void HandleJumpInput()
    {
        JumpPressed = Input.GetKeyDown(jumpKey);
        JumpReleased = Input.GetKeyUp(jumpKey);
        JumpHeld = Input.GetKey(jumpKey);
    }

    //инпут для атаки
    private void HandleAttackInput()
    {
        AttackInput = Input.GetMouseButton(0);
    }
}
