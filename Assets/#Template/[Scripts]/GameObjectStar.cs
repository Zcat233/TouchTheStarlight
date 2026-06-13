using UnityEngine;

public class GameObjectStar : MonoBehaviour
{
    [Header("自转与闪烁设置")]
    public float rotateSpeed = 30f;
    public float blinkSpeed = 2f;
    public float minBrightnessPercent = 0.4f;

    private Renderer myRenderer;
    private MaterialPropertyBlock mpb;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();

        // 核心逻辑：让场景里所有的星星，全部强制换上触发器在 Awake 里创建好的运行时新衣服！
        if (myRenderer != null && DirectionFlashEffect.runtimeSharedMaterial != null)
        {
            myRenderer.sharedMaterial = DirectionFlashEffect.runtimeSharedMaterial;
        }

        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        // 1. 2D 沿着 Z 轴自转
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

        // 2. 独立的呼吸灯闪烁（使用 MaterialPropertyBlock，避免它们闪烁频率和爆闪发生冲突）
        if (myRenderer != null && DirectionFlashEffect.runtimeSharedMaterial != null)
        {
            float baseIntensity = DirectionFlashEffect.runtimeSharedMaterial.GetFloat("_EmissionIntensity");

            float lerpFactor = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            float currentIntensity = Mathf.Lerp(baseIntensity * minBrightnessPercent, baseIntensity, lerpFactor);

            myRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_EmissionIntensity", currentIntensity);
            myRenderer.SetPropertyBlock(mpb);
        }
    }
}