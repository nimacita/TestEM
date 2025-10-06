using UnityEngine;
using Utilities.EventManager;

public class FinalDoorController : MonoBehaviour, IInteractable
{

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D doorCollider;
    private bool isOpened = false;
    private bool isKyesCollected = false;

    private void OnEnable()
    {
        EventManager.Subscribe(eEventType.onKeysConfirmed, KeysCollected);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(eEventType.onKeysConfirmed, KeysCollected);
    }

    public void OnInteract()
    {
        if (isOpened) return;

        if (isKyesCollected)
        {
            DoorOpen();
            EventManager.InvokeEvent(eEventType.onDoorOpened);
        }
        else
        {
            EventManager.InvokeEvent(eEventType.onDoorCantOpened);
        }
    }

    private void DoorOpen()
    {
        if (isOpened) return;

        isOpened = true;
        animator.SetBool("IsOpened", true);
        doorCollider.enabled = false;
        EventManager.InvokeEvent(eEventType.onKeyTaked);
    }

    private void KeysCollected(object arg0)
    {
        isKyesCollected = true;
    }

}
