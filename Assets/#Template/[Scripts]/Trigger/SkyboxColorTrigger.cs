using UnityEngine;
using System.Collections;

public class SkyboxColorTrigger : MonoBehaviour
{
    [Header("目标设置")]
    public Color targetSkyColor = new Color(0.2f, 0.4f, 0.8f, 1f);
    public Color targetEquatorColor = new Color(0.8f, 0.5f, 0.2f, 1f);
    public Color targetGroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    [Range(0f, 1f)] public float targetEquatorHeight = 0.5f;

    [Header("过渡时间 (秒)")]
    public float transitionDuration = 3.0f;

    private Material skyboxInstance;
    private Coroutine transitionCoroutine;

    void Start()
    {
        if (RenderSettings.skybox != null)
        {
            skyboxInstance = Instantiate(RenderSettings.skybox);
            RenderSettings.skybox = skyboxInstance;
        }
        else
        {
            Debug.LogError("[SkyboxTrigger] 未检测到任何有效的天空盒材质！请检查 Lighting 面板。");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }

            transitionCoroutine = StartCoroutine(DoTransition());
        }
    }

    private IEnumerator DoTransition()
    {
        if (skyboxInstance == null) yield break;

        Color startSky = skyboxInstance.GetColor("_SkyColor");
        Color startEquator = skyboxInstance.GetColor("_EquatorColor");
        Color startGround = skyboxInstance.GetColor("_GroundColor");
        float startEquatorHeight = skyboxInstance.GetFloat("_EquatorHeight");

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            Color currentSky = Color.Lerp(startSky, targetSkyColor, t);
            Color currentEquator = Color.Lerp(startEquator, targetEquatorColor, t);
            Color currentGround = Color.Lerp(startGround, targetGroundColor, t);
            float currentHeight = Mathf.Lerp(startEquatorHeight, targetEquatorHeight, t);

            skyboxInstance.SetColor("_SkyColor", currentSky);
            skyboxInstance.SetColor("_EquatorColor", currentEquator);
            skyboxInstance.SetColor("_GroundColor", currentGround);
            skyboxInstance.SetFloat("_EquatorHeight", currentHeight);

            yield return null;
        }

        skyboxInstance.SetColor("_SkyColor", targetSkyColor);
        skyboxInstance.SetColor("_EquatorColor", targetEquatorColor);
        skyboxInstance.SetColor("_GroundColor", targetGroundColor);
        skyboxInstance.SetFloat("_EquatorHeight", targetEquatorHeight);
    }

    private void OnDestroy()
    {
        if (skyboxInstance != null)
        {
            Destroy(skyboxInstance);
        }
    }
}