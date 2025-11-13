using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class PulsingLight : MonoBehaviour
{
    [Header("Configurações de Intensidade")]
    [Tooltip("Intensidade mínima da luz (valor base).")]
    public float minIntensity = 200f;

    [Tooltip("Intensidade máxima da luz (pico da pulsação).")]
    public float maxIntensity = 500f;

    [Header("Configurações de Pulsação")]
    [Tooltip("Velocidade da pulsação. Valores maiores piscam mais rápido.")]
    public float pulseSpeed = 1f;

    [Tooltip("Define se a pulsação é suave (suave) ou mais brusca (como um alarme).")]
    public float smoothness = 0.5f;

    private Light pointLight;
    private float timeOffset;

    void Awake()
    {
        pointLight = GetComponent<Light>();

        if (pointLight.type != LightType.Point)
        {
            Debug.LogWarning("O script PulsingLight foi anexado a uma luz que não é Point Light. Funcionalidade garantida para Point Light.", this);
        }

        timeOffset = UnityEngine.Random.Range(0f, 10f);

        pointLight.intensity = minIntensity;
    }

    void Update()
    {
        float sinValue = Mathf.Sin(Time.time * pulseSpeed + timeOffset);

        float normalizedIntensity = (sinValue + 1f) * 0.5f;

        float easedIntensity = Mathf.Pow(normalizedIntensity, smoothness);

        pointLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, easedIntensity);
    }
}