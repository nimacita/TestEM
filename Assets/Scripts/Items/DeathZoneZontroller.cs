using UnityEngine;
using Utilities.EventManager;

public class DeathZoneZontroller : MonoBehaviour, ITouchable
{

    private bool isTouched = false;

    public void OnTouch()
    {
        if(isTouched) return;
        EventManager.InvokeEvent(eEventType.onPlayerFault);
        isTouched = true;
    }
}
