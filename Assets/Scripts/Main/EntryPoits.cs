using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntryPoits : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private List<Object> initPoints;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initPoints.Count == 0) return;
        foreach (Object point in initPoints)
        {
            IInitializable initPoint = point.GetComponent<IInitializable>();
            if (initPoint == null)
            {
                Debug.Log($"Объект {point} не содержит IInitialized");
                continue;
            }
            initPoint.Initialized();
        }
    }
}
