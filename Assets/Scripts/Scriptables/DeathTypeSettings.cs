using UnityEngine;

[CreateAssetMenu(fileName = "DeathTypeSettings", menuName = "ScriptableObjects/DeathTypeSettings")]
public class DeathTypeSettings : ScriptableObject
{
    [Header("Death Types")]
    public string killedDethTxt;
    public string fallingDeathTxt;
}
