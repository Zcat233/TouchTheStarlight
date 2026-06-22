using UnityEngine;
using System.Collections;

public class SkyboxStarTrigger : MonoBehaviour
{
    public Material skyboxMaterial;
    public float targetMaskValue = 0f;
    public float duration = 5f;

    private Material runtimeMaterial;
    private Coroutine animationCoroutine;

    void Start()
    {
        if (skyboxMaterial != null)
        {
            runtimeMaterial = new Material(skyboxMaterial);
            if (RenderSettings.skybox == skyboxMaterial)
            {
                RenderSettings.skybox = runtimeMaterial;
            }
            // 顺带保存一下初始值（虽然你不恢复，但万一调试需要）
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateStars());
        }
    }

    private IEnumerator AnimateStars()
    {
        if (runtimeMaterial == null) yield break;

        float elapsed = 0f;
        float startMaskValue = runtimeMaterial.GetFloat("_StarsBottomMask");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentMask = Mathf.Lerp(startMaskValue, targetMaskValue, t);

            runtimeMaterial.SetFloat("_StarsBottomMask", currentMask);
            // 【删除】RenderSettings.skybox = runtimeMaterial; 这一行是多余的，已删除

            yield return null;
        }

        runtimeMaterial.SetFloat("_StarsBottomMask", targetMaskValue);
        animationCoroutine = null;
    }

    // 同样删除 OnDisable 恢复逻辑
}