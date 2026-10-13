using UnityEngine;

// 불규칙하게 깜빡이는 조명. glow를 지정하면 발광 머티리얼도 같이 깜빡인다.
[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float maxIntensity = 1f;
    public float interval = 0.08f;
    [Range(0, 1)] public float offChance = 0.3f;
    public Renderer glow;

    Light lightSource;
    Material glowMat;
    Color glowColor;
    float timer;

    void Awake()
    {
        lightSource = GetComponent<Light>();
        if (glow != null)
        {
            glowMat = glow.material;
            glowColor = glowMat.GetColor("_EmissionColor");
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0) return;
        timer = Random.Range(interval * 0.5f, interval * 2f);

        float k = Random.value < offChance ? 0f : Random.Range(0.7f, 1f);
        lightSource.intensity = maxIntensity * k;
        if (glowMat != null) glowMat.SetColor("_EmissionColor", glowColor * k);
    }
}
