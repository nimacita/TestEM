using UnityEngine;
using Utilities.EventManager;

public class PlayerController : MonoBehaviour, IDamagable
{

    [Header("Settings")]
    [SerializeField] private PlayerSettings settings;

    [Header("Health Settings")]
    private float currHealth;

    [Header("Layer Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask diedLayer;

    [Header("Components")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerAttack attack;
    [SerializeField] private PlayerAnimation anim;
    [SerializeField] private DamageFlash flash;

    [Header("Flags")]
    private bool isDied = false;

    private void Start()
    {
        InitPlayer();    
    }

    //инициализируем игрока
    private void InitPlayer()
    {
        currHealth = settings.playerHealth;
        EventManager.InvokeEvent(eEventType.onMaxPlayerHealthUpdated, currHealth);

        movement.InitMovement(input, anim);
        attack.InitAttack(input, anim, movement);
    }

    //получаем урон
    public void TakeDamage(float damage, Vector2 damageSourcePosition)
    {
        if (isDied) return;

        flash.Flash();

        //запускаем отмену атаки если идет
        attack.OnTakedDamage();
        //запускаем откидываение при атаке
        movement.OnTakedDamage(damageSourcePosition);

        //наносим урон
        currHealth -= damage;
        EventManager.InvokeEvent(eEventType.onPlayerHealthUpdated, currHealth);
        if (currHealth <= 0f) PlayerDied();
    }

    //умер
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
        int newLayerInd = (int)Mathf.Log(currLayer.value, 2);
        gameObject.layer = newLayerInd;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = newLayerInd;
        }
    }
}
