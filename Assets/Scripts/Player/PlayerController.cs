using UnityEngine;
using Utilities.EventManager;

public class PlayerController : MonoBehaviour, IDamagable, IInitializable
{

    [Header("Settings")]
    [SerializeField] private PlayerSettings settings;
    [SerializeField] private DeathTypeSettings deathTypeSettings;

    [Header("Health Settings")]
    private float currHealth;

    [Header("Components")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerAttack attack;
    [SerializeField] private PlayerAnimation anim;
    [SerializeField] private DamageFlash flash;
    [SerializeField] private PlayerInteract interact;

    [Header("Flags")]
    private bool isDied = false;
    private bool isGameEnded = false;

    private void OnEnable()
    {
        EventManager.Subscribe(eEventType.onGameEnded, OnGameEnded);
        EventManager.Subscribe(eEventType.onPlayerFault, OnPlayerFault);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(eEventType.onGameEnded, OnGameEnded);
        EventManager.Unsubscribe(eEventType.onPlayerFault, OnPlayerFault);
    }

    public void Initialized()
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
        interact.InitInteract(input);
    }

    //получаем урон
    public void TakeDamage(float damage, Vector2 damageSourcePosition, string damagerName)
    {
        if (isDied || isGameEnded) return;

        flash.Flash();

        EventManager.InvokeEvent(eEventType.onPlaySound, eSoundType.damaged);

        //запускаем отмену атаки если идет
        attack.OnTakedDamage();
        //запускаем откидываение при атаке
        movement.OnTakedDamage(damageSourcePosition);

        //наносим урон
        currHealth -= damage;
        EventManager.InvokeEvent(eEventType.onPlayerHealthUpdated, currHealth);

        if (currHealth <= 0f)
        {
            PlayerDied();
            string deathTypeTxt = $"{deathTypeSettings.killedDethTxt} {damagerName}";
            EventManager.InvokeEvent(eEventType.onDefeatPanelEnabled, deathTypeTxt);
        }
    }

    //умер
    private void PlayerDied(bool isAnim = true)
    {
        if (isDied) return;

        isDied = true;
        ChangeLayer(settings.diedLayer);
        movement.OnPlayerDied();
        attack.OnPlayerDied();
        if(isAnim) anim.SetDiedTrigger();

        EventManager.InvokeEvent(eEventType.onPlayerDied);
    }

    //игрок упал
    private void OnPlayerFault(object arg0)
    {
        EventManager.InvokeEvent(eEventType.onDefeatPanelEnabled, deathTypeSettings.fallingDeathTxt);
        PlayerDied(false);
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

    private void OnGameEnded(object arg0)
    {
        isGameEnded = true;
        movement.OnGameEnded();
        attack.OnGameEnded();
    }
}
