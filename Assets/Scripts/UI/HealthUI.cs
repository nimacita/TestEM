using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities.EventManager;

public class HealthUI : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private Image healthFill;
    private float startHealth = 0f;

    private void OnEnable()
    {
        Subscribes();
    }

    private void OnDisable()
    {
        Unsubscribes();
    }

    #region Subscribed
    private void Subscribes()
    {
        EventManager.Subscribe(eEventType.onMaxPlayerHealthUpdated, SetStartHealth);
        EventManager.Subscribe(eEventType.onPlayerHealthUpdated, UpdateCurrHealth);
    }


    private void Unsubscribes()
    {
        EventManager.Unsubscribe(eEventType.onMaxPlayerHealthUpdated, SetStartHealth);
        EventManager.Unsubscribe(eEventType.onPlayerHealthUpdated, UpdateCurrHealth);
    }
    #endregion

    //обновляем здоровье
    private void UpdateCurrHealth(object currHealth)
    {
        float health = (float) currHealth;
        if(health < 0f) health = 0f;
        healthFill.fillAmount = health / startHealth;
    }

    //обновляем стартовое здоровье
    private void SetStartHealth(object health)
    {
        startHealth = (float) health;
        healthFill.fillAmount = 1f;
    }
}
