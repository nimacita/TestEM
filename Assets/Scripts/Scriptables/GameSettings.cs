using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Main Game Settings")]
    [Tooltip("����������� ���������� ������ ��� �������� �����")]
    public int neededKeys = 3;
}
