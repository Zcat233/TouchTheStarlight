using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Õ¨µ¯¿ª¹Ø")]
    public bool isExplosionEnabled = true;
    public void TriggerBomb()
    {
        if (isExplosionEnabled)
        {
            Debug.LogError("Change Direction");
        }
    }
}