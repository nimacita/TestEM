using UnityEngine;

public class EnemyScenarist : MonoBehaviour, IInitializable
{

    [Header("Components")]
    private Transform enemysParent;

    public void Initialized()
    {
        enemysParent = transform;
        InitAllEnemys();
    }

    private void InitAllEnemys()
    {
        for (int i = 0; i < enemysParent.childCount; i++)
        {
            IInitializable enemyInit = enemysParent.GetChild(i).GetComponent<IInitializable>();
            enemyInit?.Initialized();
        }
    }
}
