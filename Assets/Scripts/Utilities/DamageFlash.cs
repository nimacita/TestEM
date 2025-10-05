using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private DamageFlashSettings settings;
    private string overlayProperty = "_OverlayColor";

    private List<SkinnedMeshRenderer> renderers = new List<SkinnedMeshRenderer>();
    private MaterialPropertyBlock block;
    private Coroutine flashRoutine;

    void Awake()
    {
        renderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
        block = new MaterialPropertyBlock();
    }

    public void Flash()
    {
        Flash(settings.flashColor, settings.flashDuration);
    }

    public void Flash(Color color, float duration)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(color, duration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = 1f - (timer / duration);
            ApplyOverlay(Color.Lerp(Color.clear, color, t));
            yield return null;
        }

        ApplyOverlay(Color.clear);
        flashRoutine = null;
    }

    private void ApplyOverlay(Color overlay)
    {
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetColor(overlayProperty, overlay);
            r.SetPropertyBlock(block);
        }
    }
}
