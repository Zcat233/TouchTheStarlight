using UnityEngine;
using System.Collections;

public class SkyboxColorTrigger : MonoBehaviour
{
    public Material skyboxMaterial; // 仍拖拽原始材质
    public Color targetEquatorColor = Color.red;
    public Color targetGroundColor = Color.blue;
    public float transitionDuration = 2.0f;

    private Material runtimeMaterial; // 用于存放实例
    private Coroutine animationCoroutine;

    void Start()
    {
        if (skyboxMaterial != null)
        {
            // 【关键修复】创建独立实例，避免污染原始资源
            runtimeMaterial = new Material(skyboxMaterial);
            // 如果当前天空盒就是该材质，将实例赋给渲染设置
            if (RenderSettings.skybox == skyboxMaterial)
            {
                RenderSettings.skybox = runtimeMaterial;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateSkyboxColor());
        }
    }

    private IEnumerator AnimateSkyboxColor()
    {
        if (runtimeMaterial == null) yield break;

        float elapsed = 0f;
        Color startEquator = runtimeMaterial.GetColor("_EquatorColor");
        Color startGround = runtimeMaterial.GetColor("_GroundColor");

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            runtimeMaterial.SetColor("_EquatorColor", Color.Lerp(startEquator, targetEquatorColor, t));
            runtimeMaterial.SetColor("_GroundColor", Color.Lerp(startGround, targetGroundColor, t));

            yield return null;
        }

        // 确保最终值精准
        runtimeMaterial.SetColor("_EquatorColor", targetEquatorColor);
        runtimeMaterial.SetColor("_GroundColor", targetGroundColor);
        animationCoroutine = null;
    }

    // 【删除】不再需要 OnDisable 恢复，因为你说不恢复原值
}