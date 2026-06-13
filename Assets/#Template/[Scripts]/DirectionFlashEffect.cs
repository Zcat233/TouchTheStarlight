using UnityEngine;
using System.Collections;

public class DirectionFlashEffect : MonoBehaviour
{
    [Header("直接把 Project 文件夹里的星星材质球资产拖进来")]
    public Material starMaterial;

    [Header("特效数值设置")]
    public float normalIntensity = 2f;   // 平时的常规亮度
    public float flashIntensity = 10f;  // 转向瞬间的爆炸亮度
    public float fadeDuration = 0.3f;   // 恢复常规亮度的消退时间（秒）

    [Header("如果你的星星是在游戏运行中动态克隆生成的，请勾选此项")]
    public bool isDynamicStars = true;

    // 内存中的唯一临时材质
    private static Material runtimeSharedMaterial;
    private Coroutine currentFlashCoroutine;

    void Awake()
    {
        // 1. 游戏启动时，在内存中把 Project 里的材质球克隆一份（不污染原文件）
        if (starMaterial != null && runtimeSharedMaterial == null)
        {
            runtimeSharedMaterial = Instantiate(starMaterial);
            runtimeSharedMaterial.EnableKeyword("_EMISSION");
            runtimeSharedMaterial.SetFloat("_EmissionIntensity", normalIntensity);
        }

        // 2. 如果星星是一开始就完全摆在场景里的，游戏启动时立刻给它们批量换上这件新衣服
        if (!isDynamicStars)
            ApplyRuntimeMaterialToAllStars();
    }

    // 核心：当事件面板把 enabled 设为 true（打勾）时自动触发
    void OnEnable()
    {
        // 刚进游戏初始化那一帧，先不闪烁
        if (Time.timeSinceLevelLoad < 0.1f) return;
        if (runtimeSharedMaterial == null) return;

        // 如果星星是随着线前进动态生成的，在转弯的这一帧瞬间去搜寻一次，帮新诞生的星星也换上材质
        if (isDynamicStars)
            ApplyRuntimeMaterialToAllStars();

        // 触发爆闪协程
        if (currentFlashCoroutine != null)
            StopCoroutine(currentFlashCoroutine);

        currentFlashCoroutine = StartCoroutine(AnimateFlash());
    }

    /// <summary>
    /// 全局搜寻：把场景里所有穿着“Project旧材质”的星星，统一强行换上“内存新材质”
    /// </summary>
    private void ApplyRuntimeMaterialToAllStars()
    {
        if (runtimeSharedMaterial == null || starMaterial == null) return;

        // 【版本兼容修复】：换成了所有老版本 Unity 都 100% 支持的经典全局搜寻写法
        Renderer[] allRenderers = Object.FindObjectsOfType<Renderer>();
        foreach (Renderer ren in allRenderers)
        {
            // 如果发现这个物体的材质指向的是你 Project 里的那个原始文件
            if (ren.sharedMaterial == starMaterial)
            {
                // 强制让它换上我们在内存中克隆出来的、安全的运行时材质
                ren.sharedMaterial = runtimeSharedMaterial;
            }
        }
    }

    IEnumerator AnimateFlash()
    {
        // 1. 瞬间将亮度拉满到 10
        runtimeSharedMaterial.SetFloat("_EmissionIntensity", flashIntensity);

        // 2. 0.3秒内平滑衰减回 2
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float currentIntensity = Mathf.Lerp(flashIntensity, normalIntensity, t);

            runtimeSharedMaterial.SetFloat("_EmissionIntensity", currentIntensity);
            yield return null;
        }

        // 3. 确保最终稳定回到 2
        runtimeSharedMaterial.SetFloat("_EmissionIntensity", normalIntensity);
        currentFlashCoroutine = null;

        // 闪烁结束后自动关闭自己（相当于在面板上自动取消打勾），等待下一次被事件唤醒激活
        this.enabled = false;
    }

    void OnDestroy()
    {
        runtimeSharedMaterial = null;
    }
}