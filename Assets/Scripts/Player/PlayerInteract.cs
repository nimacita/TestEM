using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private PlayerSettings settings;

    [Header("Components")]
    private IInteractable currInteractItem;
    private PlayerInput input;
    private bool isInited = false;

    public void InitInteract(PlayerInput playerInput)
    {
        currInteractItem = null;
        input = playerInput;

        isInited = true;
    }

    private void Update()
    {
        if (!isInited) return;

        HandleInteractDetect();
        HandleTouchDetect();
        HandleInteractInput();
    }

    private void HandleInteractInput()
    {
        if (currInteractItem != null 
            && input.InteractInput)
        {
            currInteractItem.OnInteract();
        }
    }

    private void HandleInteractDetect()
    {
        Collider2D interactHit = Physics2D.OverlapCircle((Vector2)transform.position + settings.detectOffset, 
            settings.detectRadius, settings.interactItemsLayer);
        if (interactHit != null)
        {
            IInteractable interactItem = interactHit.GetComponent<IInteractable>();
            if (interactItem != null)
            {
                currInteractItem = interactItem;
            }
            else
            {
                currInteractItem = null;
            }
        }
        else
        {
            currInteractItem = null;
        }
    }

    private void HandleTouchDetect()
    {
        Collider2D interactHit = Physics2D.OverlapCircle((Vector2)transform.position + settings.detectOffset,
            settings.detectRadius, settings.touchItemsLayer);
        if (interactHit != null)
        {
            ITouchable touchItem = interactHit.GetComponent<ITouchable>();
            if (touchItem != null)
            {
                touchItem.OnTouch();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 pos = (Vector2)transform.position + settings.detectOffset;
        Gizmos.DrawWireSphere(pos, settings.detectRadius);
    }
}
