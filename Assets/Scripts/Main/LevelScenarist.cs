using UnityEngine;

public class LevelScenarist : MonoBehaviour, IInitializable
{

    [Header("Components")]
    [SerializeField] private EnemyScenarist enemyScenarist;
    [SerializeField] private ItemScenarist itemScenarist;

    public void Initialized()
    {
        enemyScenarist.Initialized();
        itemScenarist.Initialized();
    }
}
