using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Color flashColor = Color.red;

    private List<SkinnedMeshRenderer> renderers = new List<SkinnedMeshRenderer>();
    private List<Color[]> originalColors = new List<Color[]>();
    private Coroutine flashRoutine;

    void Awake()
    {
        // Находим все рендереры в потомках
        renderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());

        // Сохраняем оригинальные цвета материалов
        foreach (var renderer in renderers)
        {
            var colors = new Color[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                colors[i] = renderer.materials[i].color;
            }
            originalColors.Add(colors);
        }
    }

    public void Flash()
    {
        Flash(flashColor, flashDuration);
    }

    public void Flash(Color color, float duration)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(color, duration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        // Меняем цвет
        for (int r = 0; r < renderers.Count; r++)
        {
            var renderer = renderers[r];
            foreach (var mat in renderer.materials)
            {
                mat.color = color;
            }
        }

        yield return new WaitForSeconds(duration);

        // Возвращаем оригинальные цвета
        for (int r = 0; r < renderers.Count; r++)
        {
            var renderer = renderers[r];
            var colors = originalColors[r];

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].color = colors[i];
            }
        }

        flashRoutine = null;
    }
}
