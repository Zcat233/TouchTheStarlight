using UnityEngine;
using System.Collections;

public class SkyboxStarTrigger : MonoBehaviour
{
    public Material skyboxMaterial;
    public float targetMaskValue = 0f;
    public float duration = 5f;

    private float originalMaskValue;
    private Coroutine animationCoroutine;

    void Start()
    {
        if (skyboxMaterial != null)
        {
            originalMaskValue = skyboxMaterial.GetFloat("_StarsBottomMask");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            animationCoroutine = StartCoroutine(AnimateStars());
        }
    }

    private IEnumerator AnimateStars()
    {
        if (skyboxMaterial == null) yield break;

        float elapsed = 0f;
        float startMaskValue = skyboxMaterial.GetFloat("_StarsBottomMask");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentMask = Mathf.Lerp(startMaskValue, targetMaskValue, t);

            skyboxMaterial.SetFloat("_StarsBottomMask", currentMask);
            RenderSettings.skybox = skyboxMaterial;

            yield return null;
        }

        skyboxMaterial.SetFloat("_StarsBottomMask", targetMaskValue);
        RenderSettings.skybox = skyboxMaterial;
        animationCoroutine = null;
    }

    private void OnDisable()
    {
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_StarsBottomMask", originalMaskValue);
            RenderSettings.skybox = skyboxMaterial;
        }
    }
}