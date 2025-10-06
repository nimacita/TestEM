using UnityEngine;
using Utilities.EventManager;
using Utilities.Objects;

public class GameScenarist : MonoBehaviour, IInitializable
{
    [Header("Game Settings")]
    [SerializeField] private GameSettings settings;

    [Header("Stats")]
    private bool isGameEnded = false;
    private int currKeys = 0;
    private GameKeys gameKeys;

    private void OnEnable()
    {
        Subscribes();
    }

    private void OnDisable()
    {
        Unsubscribes();
    }

    public void Initialized()
    {
        currKeys = 0;
        isGameEnded = false;
        gameKeys = new GameKeys(currKeys, settings.neededKeys);
        UpdateGameKeys();
    }

    #region Subscribes

    private void Subscribes()
    {
        EventManager.Subscribe(eEventType.onKeyTaked, OnKeyTaked);
        EventManager.Subscribe(eEventType.onDoorOpened, OnGameEnded);
        EventManager.Subscribe(eEventType.onDoorCantOpened, TryOpenedDoor);
    }

    private void Unsubscribes()
    {
        EventManager.Unsubscribe(eEventType.onKeyTaked, OnKeyTaked);
        EventManager.Unsubscribe(eEventType.onDoorOpened, OnGameEnded);
        EventManager.Unsubscribe(eEventType.onDoorCantOpened, TryOpenedDoor);
    }

    #endregion

    //собрали ключ
    private void OnKeyTaked(object arg0)
    {
        if (isGameEnded) return;

        currKeys++;
        UpdateGameKeys();

        if (currKeys >= settings.neededKeys)
        {
            //все ключи собраны
            EventManager.InvokeEvent(eEventType.onKeysConfirmed);
        }
    }

    //обновляем ключи в UI
    private void UpdateGameKeys()
    {
        gameKeys.currKeys = currKeys;
        EventManager.InvokeEvent(eEventType.onKeysUpdated, gameKeys);
    }

    //попытались открыть дверь без нужного колва ключей
    private void TryOpenedDoor(object arg0)
    {
        Debug.Log($"Нехватает {settings.neededKeys - currKeys} ключей");
    }

    //закончили игру
    private void OnGameEnded(object arg0)
    {
        isGameEnded = true;
        EventManager.InvokeEvent(eEventType.onGameEnded);
        EventManager.InvokeEvent(eEventType.onVictoryPanelEnabled);
    }
}
