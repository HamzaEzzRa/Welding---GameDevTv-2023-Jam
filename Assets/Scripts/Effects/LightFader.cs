using UnityEngine;

using System.Collections;

public class LightFader : MonoBehaviour
{
    public Renderer LightRenderer => lightRenderer;

    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    [SerializeField] private Light[] lights;
    [SerializeField] private Renderer lightRenderer;

    [SerializeField, HideInInspector, ColorUsage(false, true)] private Color emissionFadeInColor = Color.black;
    [SerializeField, HideInInspector, ColorUsage(false, true)] private Color emissionFadeOutColor = Color.black;

    private Coroutine lightFadeCoroutine;

    public void FadeInLights()
    {
        if (lightFadeCoroutine != null)
        {
            return;
        }

        lightFadeCoroutine = StartCoroutine(FadeLightCoroutine(fadeInCurve, emissionFadeOutColor, emissionFadeInColor));
    }

    public void FadeOutLights()
    {
        if (lightFadeCoroutine != null)
        {
            return;
        }

        lightFadeCoroutine = StartCoroutine(FadeLightCoroutine(fadeOutCurve, emissionFadeInColor, emissionFadeOutColor));
    }

    private IEnumerator FadeLightCoroutine(AnimationCurve curve, Color startEmission, Color targetEmission)
    {
        float endTime = curve.keys[curve.length - 1].time;

        float progress = 0f;
        while (progress < 1f)
        {
            progress = Mathf.Min(1f, progress + Time.deltaTime / endTime);

            foreach (Light light in lights)
            {
                float evaluationTime = progress * endTime;
                light.intensity = curve.Evaluate(evaluationTime);
            }

            if (lightRenderer != null)
            {
                Color newEmission = Color.Lerp(startEmission, targetEmission, progress);
                lightRenderer.sharedMaterial.SetColor("_EmissionColor", newEmission);
            }

            yield return null;
        }

        lightFadeCoroutine = null;
    }

    private void OnDisable()
    {
        if (lightRenderer != null)
        {
            lightRenderer.sharedMaterial.SetColor("_EmissionColor", emissionFadeOutColor);
        }
    }
}
