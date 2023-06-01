using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using TMPro;

public class UIFader : MonoBehaviour
{
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.Constant(0f, 1f, 0f);
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.Constant(0f, 1f, 0f);

    public Coroutine ActiveCoroutine => activeCoroutine;

    private Image[] images = default;
    private TMP_Text[] texts = default;

    private float currentAlpha = 0f;

    private Coroutine activeCoroutine = null;

    private void Awake()
    {
        images = GetComponentsInChildren<Image>(true);
        texts = GetComponentsInChildren<TMP_Text>(true);
    }

    public void Hide()
    {
        currentAlpha = 0f;
        UpdateAlphas();
    }

    public void StopActiveCoroutine()
    {
        if (activeCoroutine == null)
        {
            return;
        }

        StopCoroutine(activeCoroutine);
        activeCoroutine = null;
    }

    public void FadeIn()
    {
        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(FadeCoroutine(fadeInCurve));
    }

    public void FadeOut()
    {
        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(FadeCoroutine(fadeOutCurve));
    }

    private IEnumerator FadeCoroutine(AnimationCurve curve)
    {
        float endTime = curve.keys[curve.keys.Length - 1].time;

        float startValue = curve.keys[0].value;
        float endValue = curve.keys[curve.keys.Length - 1].value;

        float progress = Mathf.Clamp01((currentAlpha - startValue) / (endValue - startValue));
        while (progress < 1f)
        {
            progress = Mathf.Min(1f, progress + Time.deltaTime / endTime);
            float evaluationTime = progress * endTime;
            currentAlpha = Mathf.Clamp01(curve.Evaluate(evaluationTime));

            UpdateAlphas();
            yield return null;
        }
    }

    private void UpdateAlphas()
    {
        foreach (Image image in images)
        {
            if (!image.gameObject.activeInHierarchy && currentAlpha > 0f)
            {
                image.gameObject.SetActive(true);
            }
            else if (currentAlpha == 0f && image.gameObject.activeInHierarchy)
            {
                image.gameObject.SetActive(false);
            }

            Color tmp = image.color;
            tmp.a = currentAlpha;
            image.color = tmp;
        }
        foreach (TMP_Text textMesh in texts)
        {
            if (!textMesh.gameObject.activeInHierarchy && currentAlpha > 0f)
            {
                textMesh.gameObject.SetActive(true);
            }
            else if (currentAlpha == 0f && textMesh.gameObject.activeInHierarchy)
            {
                textMesh.gameObject.SetActive(false);
            }

            Color tmp = textMesh.color;
            tmp.a = currentAlpha;
            textMesh.color = tmp;
        }
    }
}
