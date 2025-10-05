using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerSettings))]
public class PlayerSettingsEditor : Editor
{
    private SerializedProperty playerHealth;

    private SerializedProperty damage, attackCoolDown, attackDuration, AttackZoneOffset, attackRadius, attackDashForce, attackMask;
    private SerializedProperty moveSpeed, acceleration, deceleration, airControl;
    private SerializedProperty jumpForce, jumpHorizontalForce, coyoteTime, jumpBufferTime, jumpCutMultiplier, fallMultiplier, lowJumpMultiplier, maxJumpHoldTime;
    private SerializedProperty wallJumpForce, wallJumpHorizontalForce, wallCoyoteTime, wallCheckDistance, wallLayer;
    private SerializedProperty dodgeForce, dodgeDuration, dodgeCooldown;
    private SerializedProperty damageDuration, damagedKnockForce;
    private SerializedProperty groundCheckRadius, groundLayer;
    private SerializedProperty playerLayer, diedLayer;

    // Флаги для разворачивания секций
    private bool showHealth = true;
    private bool showAttack = false;
    private bool showMovement = false;
    private bool showJump = false;
    private bool showWallJump = false;
    private bool showDodge = false;
    private bool showDamaged = false;
    private bool showGroundCheck = false;
    private bool showLayers = false;

    private void OnEnable()
    {
        playerHealth = serializedObject.FindProperty("playerHealth");

        damage = serializedObject.FindProperty("damage");
        attackCoolDown = serializedObject.FindProperty("attackCoolDown");
        attackDuration = serializedObject.FindProperty("attackDuration");
        AttackZoneOffset = serializedObject.FindProperty("AttackZoneOffset");
        attackRadius = serializedObject.FindProperty("attackRadius");
        attackDashForce = serializedObject.FindProperty("attackDashForce");
        attackMask = serializedObject.FindProperty("attackMask");

        moveSpeed = serializedObject.FindProperty("moveSpeed");
        acceleration = serializedObject.FindProperty("acceleration");
        deceleration = serializedObject.FindProperty("deceleration");
        airControl = serializedObject.FindProperty("airControl");

        jumpForce = serializedObject.FindProperty("jumpForce");
        jumpHorizontalForce = serializedObject.FindProperty("jumpHorizontalForce");
        coyoteTime = serializedObject.FindProperty("coyoteTime");
        jumpBufferTime = serializedObject.FindProperty("jumpBufferTime");
        jumpCutMultiplier = serializedObject.FindProperty("jumpCutMultiplier");
        fallMultiplier = serializedObject.FindProperty("fallMultiplier");
        lowJumpMultiplier = serializedObject.FindProperty("lowJumpMultiplier");
        maxJumpHoldTime = serializedObject.FindProperty("maxJumpHoldTime");

        wallJumpForce = serializedObject.FindProperty("wallJumpForce");
        wallJumpHorizontalForce = serializedObject.FindProperty("wallJumpHorizontalForce");
        wallCoyoteTime = serializedObject.FindProperty("wallCoyoteTime");
        wallCheckDistance = serializedObject.FindProperty("wallCheckDistance");
        wallLayer = serializedObject.FindProperty("wallLayer");

        dodgeForce = serializedObject.FindProperty("dodgeForce");
        dodgeDuration = serializedObject.FindProperty("dodgeDuration");
        dodgeCooldown = serializedObject.FindProperty("dodgeCooldown");

        damageDuration = serializedObject.FindProperty("damageDuration");
        damagedKnockForce = serializedObject.FindProperty("damagedKnockForce");

        groundCheckRadius = serializedObject.FindProperty("groundCheckRadius");
        groundLayer = serializedObject.FindProperty("groundLayer");

        playerLayer = serializedObject.FindProperty("playerLayer");
        diedLayer = serializedObject.FindProperty("diedLayer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawFoldout(ref showHealth, "Health Settings", () =>
        {
            EditorGUILayout.PropertyField(playerHealth);
        });

        DrawFoldout(ref showAttack, "Attack Settings", () =>
        {
            EditorGUILayout.PropertyField(damage);
            EditorGUILayout.PropertyField(attackCoolDown);
            EditorGUILayout.PropertyField(attackDuration);
            EditorGUILayout.PropertyField(AttackZoneOffset);
            EditorGUILayout.PropertyField(attackRadius);
            EditorGUILayout.PropertyField(attackDashForce);
            EditorGUILayout.PropertyField(attackMask);
        });

        DrawFoldout(ref showMovement, "Movement Settings", () =>
        {
            EditorGUILayout.PropertyField(moveSpeed);
            EditorGUILayout.PropertyField(acceleration);
            EditorGUILayout.PropertyField(deceleration);
            EditorGUILayout.PropertyField(airControl);
        });

        DrawFoldout(ref showJump, "Jump Settings", () =>
        {
            EditorGUILayout.PropertyField(jumpForce);
            EditorGUILayout.PropertyField(jumpHorizontalForce);
            EditorGUILayout.PropertyField(coyoteTime);
            EditorGUILayout.PropertyField(jumpBufferTime);
            EditorGUILayout.PropertyField(jumpCutMultiplier);
            EditorGUILayout.PropertyField(fallMultiplier);
            EditorGUILayout.PropertyField(lowJumpMultiplier);
            EditorGUILayout.PropertyField(maxJumpHoldTime);
        });

        DrawFoldout(ref showWallJump, "Wall Jump Settings", () =>
        {
            EditorGUILayout.PropertyField(wallJumpForce);
            EditorGUILayout.PropertyField(wallJumpHorizontalForce);
            EditorGUILayout.PropertyField(wallCoyoteTime);
            EditorGUILayout.PropertyField(wallCheckDistance);
            EditorGUILayout.PropertyField(wallLayer);
        });

        DrawFoldout(ref showDodge, "Dodge Settings", () =>
        {
            EditorGUILayout.PropertyField(dodgeForce);
            EditorGUILayout.PropertyField(dodgeDuration);
            EditorGUILayout.PropertyField(dodgeCooldown);
        });

        DrawFoldout(ref showDamaged, "Damaged Settings", () =>
        {
            EditorGUILayout.PropertyField(damageDuration);
            EditorGUILayout.PropertyField(damagedKnockForce);
        });

        DrawFoldout(ref showGroundCheck, "Ground Check Settings", () =>
        {
            EditorGUILayout.PropertyField(groundCheckRadius);
            EditorGUILayout.PropertyField(groundLayer);
        });

        DrawFoldout(ref showLayers, "Main Layer Settings", () =>
        {
            EditorGUILayout.PropertyField(playerLayer);
            EditorGUILayout.PropertyField(diedLayer);
        });

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFoldout(ref bool state, string title, System.Action drawContent)
    {
        state = EditorGUILayout.BeginFoldoutHeaderGroup(state, title);
        if (state)
        {
            EditorGUI.indentLevel++;
            drawContent.Invoke();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space(5);
    }
}
