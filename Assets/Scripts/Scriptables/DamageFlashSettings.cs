using UnityEngine;

[CreateAssetMenu(fileName = "DamageFlashSettings", menuName = "ScriptableObjects/DamageFlashSettings")]
public class DamageFlashSettings : ScriptableObject
{
    [Header("Damage Flash Settings")]
    public Color flashColor = new Color(1, 0, 0, 0.5f);
    public float flashDuration = 0.25f;
}
