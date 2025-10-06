using UnityEngine;

public class ItemScenarist : MonoBehaviour, IInitializable
{
    [Header("Components")]
    private Transform itemsParent;

    public void Initialized()
    {
        itemsParent = transform;
        InitAllEnemys();
    }

    private void InitAllEnemys()
    {
        for (int i = 0; i < itemsParent.childCount; i++)
        {
            IInitializable itemInit = itemsParent.GetChild(i).GetComponent<IInitializable>();
            itemInit?.Initialized();
        }
    }
}
