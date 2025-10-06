using TMPro;
using UnityEngine;
using Utilities.EventManager;
using Utilities.Objects;

public class KeyCollecctorUI : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private TMP_Text keyCounter;
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        EventManager.Subscribe(eEventType.onKeysUpdated, UpdateKeys);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(eEventType.onKeysUpdated, UpdateKeys);
    }

    //обновляем ui ключей
    private void UpdateKeys(object arg0)
    {
        GameKeys gameKeys = (GameKeys)arg0;
        if (gameKeys != null)
        {
            keyCounter.text = $"{gameKeys.currKeys}/{gameKeys.neededKeys}";
            animator?.SetTrigger("Pop");
        }
    }

}
