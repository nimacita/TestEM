using UnityEngine;
using Utilities.EventManager;

public class ChestController : MonoBehaviour, IInteractable
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D chestCollider;
    private bool isOpened = false;

    public void OnInteract()
    {
        if(isOpened) return;

        EventManager.InvokeEvent(eEventType.onPlaySound, eSoundType.chestOpen);
        isOpened = true;
        OpenChestAnim();
        chestCollider.enabled = false;
        EventManager.InvokeEvent(eEventType.onKeyTaked);
    }

    private void OpenChestAnim()
    {
        animator.SetTrigger("Open");
    }
}
