using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Main Game Settings")]
    [Tooltip("Необходимое количество ключей для открытия двери")]
    public int neededKeys = 3;
}
