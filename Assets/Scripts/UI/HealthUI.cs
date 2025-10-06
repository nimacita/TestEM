using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities.EventManager;

public class HealthUI : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image healthUnderFill;

    [Header("Animation Settings")]
    [SerializeField] private float underFillDelay = 0.3f;
    [SerializeField] private float underFillSpeed = 1f;
    [SerializeField] private AnimationCurve underFillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float startHealth = 0f;

    // Переменные для анимации
    private float targetUnderFillAmount;
    private float currentUnderFillAmount;
    private float animationTimer;
    private float delayTimer;
    private bool isAnimating;

    private void OnEnable()
    {
        Subscribes();
        // Сбрасываем состояние анимации
        ResetAnimationState();
    }

    private void OnDisable()
    {
        Unsubscribes();
        ResetAnimationState();
    }

    private void Update()
    {
        HandleUnderFillAnimation();
    }

    #region Subscription
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

    // Обновляем здоровье
    private void UpdateCurrHealth(object currHealth)
    {
        float health = (float)currHealth;
        if (health < 0f) health = 0f;

        float targetFillAmount = health / startHealth;

        // Меняем основной fill сразу
        healthFill.fillAmount = targetFillAmount;

        // Настраиваем анимацию для подложки
        StartUnderFillAnimation(targetFillAmount);
    }

    // Обновляем стартовое здоровье
    private void SetStartHealth(object health)
    {
        startHealth = (float)health;
        healthFill.fillAmount = 1f;
        healthUnderFill.fillAmount = 1f;
        ResetAnimationState();
    }

    // Запускаем анимацию подложки
    private void StartUnderFillAnimation(float targetAmount)
    {
        targetUnderFillAmount = targetAmount;
        currentUnderFillAmount = healthUnderFill.fillAmount;
        delayTimer = underFillDelay;
        animationTimer = 0f;
        isAnimating = true;
    }

    // Обрабатываем анимацию подложки в Update
    private void HandleUnderFillAnimation()
    {
        if (!isAnimating) return;

        // Обрабатываем задержку
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        // Анимируем подложку
        animationTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(animationTimer / underFillSpeed);
        float curvedProgress = underFillCurve.Evaluate(progress);

        healthUnderFill.fillAmount = Mathf.Lerp(currentUnderFillAmount, targetUnderFillAmount, curvedProgress);

        // Проверяем завершение анимации
        if (progress >= 1f || Mathf.Abs(healthUnderFill.fillAmount - targetUnderFillAmount) < 0.001f)
        {
            healthUnderFill.fillAmount = targetUnderFillAmount;
            isAnimating = false;
        }
    }

    // Сбрасываем состояние анимации
    private void ResetAnimationState()
    {
        isAnimating = false;
        animationTimer = 0f;
        delayTimer = 0f;
        currentUnderFillAmount = healthUnderFill.fillAmount;
        targetUnderFillAmount = healthUnderFill.fillAmount;
    }

    // Метод для принудительной остановки анимации
    public void StopAnimation()
    {
        if (isAnimating)
        {
            healthUnderFill.fillAmount = targetUnderFillAmount;
            ResetAnimationState();
        }
    }

    // Метод для проверки, идет ли анимация
    public bool IsAnimating => isAnimating;
}
