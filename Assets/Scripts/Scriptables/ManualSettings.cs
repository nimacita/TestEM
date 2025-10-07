using UnityEngine;

[CreateAssetMenu(fileName = "ManualSettings", menuName = "ScriptableObjects/ManualSettings")]
public class ManualSettings : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("�������� �����")]
    public KeyCode leftKey = KeyCode.A;
    [Tooltip("�������� ������")]
    public KeyCode rightKey = KeyCode.D;
    [Tooltip("�������")]
    public KeyCode dodgeKey = KeyCode.LeftShift;
    [Tooltip("������")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Interact")]
    [Tooltip("��������������")]
    public KeyCode interactKey = KeyCode.E;
}
