using UnityEngine;
using Utilities.EventManager;

public class PlayerController : MonoBehaviour, IDamagable
{

    [Header("Settings")]
    [SerializeField] private PlayerSettings settings;

    [Header("Health Settings")]
    [SerializeField] private float playerHealth = 50f;
    [SerializeField] private float currHealth;

    [Header("Layer Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask diedLayer;

    [Header("Components")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerAttack attack;
    [SerializeField] private PlayerAnimation anim;

    [Header("Flags")]
    private bool isDied = false;

    private void Start()
    {
        InitPlayer();    
    }

    //�������������� ������
    private void InitPlayer()
    {
        currHealth = playerHealth;
        EventManager.InvokeEvent(eEventType.onMaxPlayerHealthUpdated, currHealth);

        movement.InitMovement(input, anim);
        attack.InitAttack(input, anim, movement);
    }

    //�������� ����
    public void TakeDamage(float damage, Vector2 damageSourcePosition)
    {
        if (isDied) return;

        //��������� ������ ����� ���� ����
        attack.OnTakedDamage();
        //��������� ������������ ��� �����
        movement.OnTakedDamage(damageSourcePosition);

        //������� ����
        currHealth -= damage;
        EventManager.InvokeEvent(eEventType.onPlayerHealthUpdated, currHealth);
        if (currHealth <= 0f) PlayerDied();
    }

    //����
    private void PlayerDied()
    {
        isDied = true;
        ChangeLayer(diedLayer);
        movement.OnPlayerDied();
        attack.OnPlayerDied();
        anim.SetDiedTrigger();
    }

    private void ChangeLayer(LayerMask currLayer)
    {
        gameObject.layer = (int)Mathf.Log(currLayer.value, 2);
    }
}
