using UnityEngine;
using System.Collections;

public class SkyboxColorTrigger : MonoBehaviour
{
    public Material skyboxMaterial;
    public Color targetEquatorColor = Color.red;
    public Color targetGroundColor = Color.blue;
    public float transitionDuration = 2.0f;

    private Color originalEquatorColor;
    private Color originalGroundColor;
    private Coroutine animationCoroutine;

    void Start()
    {
        if (skyboxMaterial != null)
        {
            originalEquatorColor = skyboxMaterial.GetColor("_EquatorColor");
            originalGroundColor = skyboxMaterial.GetColor("_GroundColor");
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
            animationCoroutine = StartCoroutine(AnimateSkyboxColor());
        }
    }

    private IEnumerator AnimateSkyboxColor()
    {
        if (skyboxMaterial == null) yield break;

        float elapsed = 0f;
        Color startEquator = skyboxMaterial.GetColor("_EquatorColor");
        Color startGround = skyboxMaterial.GetColor("_GroundColor");

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            skyboxMaterial.SetColor("_EquatorColor", Color.Lerp(startEquator, targetEquatorColor, t));
            skyboxMaterial.SetColor("_GroundColor", Color.Lerp(startGround, targetGroundColor, t));

            yield return null;
        }

        skyboxMaterial.SetColor("_EquatorColor", targetEquatorColor);
        skyboxMaterial.SetColor("_GroundColor", targetGroundColor);
        animationCoroutine = null;
    }

    private void OnDisable()
    {
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetColor("_EquatorColor", originalEquatorColor);
            skyboxMaterial.SetColor("_GroundColor", originalGroundColor);
        }
    }
}