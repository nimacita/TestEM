using UnityEngine;

public enum eEventType
{
    //Ui
    onPlayerHealthUpdated,
    onMaxPlayerHealthUpdated,
    onKeysUpdated,
    onVictoryPanelEnabled,
    onDefeatPanelEnabled,

    //Main
    onKeyTaked,
    onKeysConfirmed,
    onGameEnded,
    onDoorCantOpened,
    onDoorOpened,
    onPlayerDied,
    onPlayerFault,

    //Audio
    onPlaySound,
}
