using UnityEngine;
using System.Collections;

public class EmissionFlash : MonoBehaviour
{
    public Renderer[] renderers;
    public float startIntensity = 1f;
    public float peakIntensity = 5f;
    public float duration = 0.5f;

    private MaterialPropertyBlock mpb;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(mpb);
            mpb.SetFloat("_EmissionIntensity", startIntensity);
            r.SetPropertyBlock(mpb);
        }
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        mpb.SetFloat("_EmissionIntensity", peakIntensity);
        foreach (var r in renderers)
            r.SetPropertyBlock(mpb);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float val = Mathf.Lerp(peakIntensity, startIntensity, t);
            mpb.SetFloat("_EmissionIntensity", val);
            foreach (var r in renderers)
                r.SetPropertyBlock(mpb);
            yield return null;
        }

        mpb.SetFloat("_EmissionIntensity", startIntensity);
        foreach (var r in renderers)
            r.SetPropertyBlock(mpb);
    }
}