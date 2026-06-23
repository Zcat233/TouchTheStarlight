using UnityEngine;
using System.Collections;

public class SkyboxColorTrigger : MonoBehaviour
{
    [Header("目标颜色设置")]
    public Color targetSkyColor = new Color(0.2f, 0.4f, 0.8f, 1f);
    public Color targetEquatorColor = new Color(0.8f, 0.5f, 0.2f, 1f);
    public Color targetGroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    [Header("过渡时间 (秒)")]
    public float transitionDuration = 3.0f;

    [Header("触发设置")]
    [Tooltip("是否只允许触发一次？如果为 false，每次 Player 进入都会重新向目标颜色过渡")]
    public bool triggerOnlyOnce = true;

    [Header("可选：自定义网格天空盒")]
    [Tooltip("如果你是将天空盒材质挂在场景的球体/网格上，请把它的 Renderer 拖到这里。如果留空，则默认操控全局天空盒 (RenderSettings.skybox)")]
    public Renderer customSkyboxRenderer;

    private Material skyboxInstance;
    private Coroutine transitionCoroutine;
    private bool hasTriggered = false;

    void Start()
    {
        // 【核心设计：创建材质实例，防止污染项目资产】
        if (customSkyboxRenderer != null)
        {
            // 如果使用的是场景中的天空球/网格，访问 .material 会在内存中自动克隆一份实例
            skyboxInstance = customSkyboxRenderer.material;
        }
        else if (RenderSettings.skybox != null)
        {
            // 如果使用的是 Unity 全局天空盒，需要手动 Instantiate 克隆一份，并重新赋值给全局天空盒
            skyboxInstance = Instantiate(RenderSettings.skybox);
            RenderSettings.skybox = skyboxInstance;
        }
        else
        {
            Debug.LogError("[SkyboxTrigger] 未检测到任何有效的天空盒材质！请检查 Lighting 面板或指定 customSkyboxRenderer。");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. 检查进入触发器的对象是否带有 "Player" 标签
        if (other.CompareTag("Player"))
        {
            // 如果设置了只触发一次且已经触发过，则直接跳过
            if (triggerOnlyOnce && hasTriggered) return;

            hasTriggered = true;

            // 如果当前已经有正在运行的过渡协程，先将其停止以防止颜色冲突
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }

            // 2. 开启协程开始渐变过渡
            transitionCoroutine = StartCoroutine(DoColorTransition());
        }
    }

    private IEnumerator DoColorTransition()
    {
        if (skyboxInstance == null) yield break;

        // 3. 获取开始过渡时，实例化材质的当前颜色作为起始值（支持从任意当前状态平滑过渡）
        Color startSky = skyboxInstance.GetColor("_SkyColor");
        Color startEquator = skyboxInstance.GetColor("_EquatorColor");
        Color startGround = skyboxInstance.GetColor("_GroundColor");

        float elapsed = 0f;

        // 4. 在设定的时间内进行渐变。
        // 由于我们没有在 OnTriggerExit 中添加任何停止代码，即使 Player 离开触发器，这个循环也会执意运行到结束。
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            // 使用 Color.Lerp 计算当前帧的插值颜色
            Color currentSky = Color.Lerp(startSky, targetSkyColor, t);
            Color currentEquator = Color.Lerp(startEquator, targetEquatorColor, t);
            Color currentGround = Color.Lerp(startGround, targetGroundColor, t);

            // 将新颜色应用到克隆出来的材质实例中
            skyboxInstance.SetColor("_SkyColor", currentSky);
            skyboxInstance.SetColor("_EquatorColor", currentEquator);
            skyboxInstance.SetColor("_GroundColor", currentGround);

            yield return null; // 等待下一帧
        }

        // 5. 确保过渡结束后颜色完全精准达到目标值
        skyboxInstance.SetColor("_SkyColor", targetSkyColor);
        skyboxInstance.SetColor("_EquatorColor", targetEquatorColor);
        skyboxInstance.SetColor("_GroundColor", targetGroundColor);
    }

    private void OnDestroy()
    {
        // 6. 良好的内存管理：当场景切换或脚本销毁时，释放动态生成的材质实例，防止内存泄漏
        if (skyboxInstance != null)
        {
            Destroy(skyboxInstance);
        }
    }
}