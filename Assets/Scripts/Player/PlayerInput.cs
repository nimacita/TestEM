using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private ManualSettings settings;

    public float HorizontalInput { get; private set; }
    public float DodgeInput { get; private set; }
    public bool AttackInput {  get; private set; }
    public Action OnDodgePressed { get; set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool InteractInput {  get; private set; }

    private void Update()
    {
        HandleMoveInput();
        HandleJumpInput();
        HandleAttackInput();
        HandleInteractInput();
    }

    private void HandleMoveInput()
    {
        HorizontalInput = 0f;

        if (Input.GetKey(settings.leftKey))
            HorizontalInput = -1f;

        if (Input.GetKey(settings.rightKey))
            HorizontalInput = 1f;

        HandleDodgeInput();
    }

    //кувырок в определенную сторону
    private void HandleDodgeInput()
    {
        DodgeInput = 0f;
        if (Input.GetKeyDown(settings.dodgeKey))
        {
            OnDodgePressed?.Invoke();
        }
    }

    //инпут дл€ прыжков
    private void HandleJumpInput()
    {
        JumpPressed = Input.GetKeyDown(settings.jumpKey);
        JumpReleased = Input.GetKeyUp(settings.jumpKey);
        JumpHeld = Input.GetKey(settings.jumpKey);
    }

    //инпут дл€ атаки
    private void HandleAttackInput()
    {
        AttackInput = Input.GetMouseButton(0);
    }

    //инпут дл€ взаимодейств€и
    private void HandleInteractInput()
    {
        InteractInput = Input.GetKeyDown(settings.interactKey);
    }
}
