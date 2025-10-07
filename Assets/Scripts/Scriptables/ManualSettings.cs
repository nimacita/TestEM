using UnityEngine;

[CreateAssetMenu(fileName = "ManualSettings", menuName = "ScriptableObjects/ManualSettings")]
public class ManualSettings : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("Движение влево")]
    public KeyCode leftKey = KeyCode.A;
    [Tooltip("Движение вправо")]
    public KeyCode rightKey = KeyCode.D;
    [Tooltip("Кувырок")]
    public KeyCode dodgeKey = KeyCode.LeftShift;
    [Tooltip("Прыжок")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Interact")]
    [Tooltip("Взаимодействие")]
    public KeyCode interactKey = KeyCode.E;
}
