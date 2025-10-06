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

    // ���������� ��� ��������
    private float targetUnderFillAmount;
    private float currentUnderFillAmount;
    private float animationTimer;
    private float delayTimer;
    private bool isAnimating;

    private void OnEnable()
    {
        Subscribes();
        // ���������� ��������� ��������
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

    // ��������� ��������
    private void UpdateCurrHealth(object currHealth)
    {
        float health = (float)currHealth;
        if (health < 0f) health = 0f;

        float targetFillAmount = health / startHealth;

        // ������ �������� fill �����
        healthFill.fillAmount = targetFillAmount;

        // ����������� �������� ��� ��������
        StartUnderFillAnimation(targetFillAmount);
    }

    // ��������� ��������� ��������
    private void SetStartHealth(object health)
    {
        startHealth = (float)health;
        healthFill.fillAmount = 1f;
        healthUnderFill.fillAmount = 1f;
        ResetAnimationState();
    }

    // ��������� �������� ��������
    private void StartUnderFillAnimation(float targetAmount)
    {
        targetUnderFillAmount = targetAmount;
        currentUnderFillAmount = healthUnderFill.fillAmount;
        delayTimer = underFillDelay;
        animationTimer = 0f;
        isAnimating = true;
    }

    // ������������ �������� �������� � Update
    private void HandleUnderFillAnimation()
    {
        if (!isAnimating) return;

        // ������������ ��������
        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        // ��������� ��������
        animationTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(animationTimer / underFillSpeed);
        float curvedProgress = underFillCurve.Evaluate(progress);

        healthUnderFill.fillAmount = Mathf.Lerp(currentUnderFillAmount, targetUnderFillAmount, curvedProgress);

        // ��������� ���������� ��������
        if (progress >= 1f || Mathf.Abs(healthUnderFill.fillAmount - targetUnderFillAmount) < 0.001f)
        {
            healthUnderFill.fillAmount = targetUnderFillAmount;
            isAnimating = false;
        }
    }

    // ���������� ��������� ��������
    private void ResetAnimationState()
    {
        isAnimating = false;
        animationTimer = 0f;
        delayTimer = 0f;
        currentUnderFillAmount = healthUnderFill.fillAmount;
        targetUnderFillAmount = healthUnderFill.fillAmount;
    }

    // ����� ��� �������������� ��������� ��������
    public void StopAnimation()
    {
        if (isAnimating)
        {
            healthUnderFill.fillAmount = targetUnderFillAmount;
            ResetAnimationState();
        }
    }

    // ����� ��� ��������, ���� �� ��������
    public bool IsAnimating => isAnimating;
}
