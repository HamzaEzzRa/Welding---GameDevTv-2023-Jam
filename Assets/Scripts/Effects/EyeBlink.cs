using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

using System;
using System.Collections;

[RequireComponent(typeof(PostProcessVolume))]
public class EyeBlink : MonoBehaviour
{
    [Serializable]
    public struct VignetteParameters
    {
        public VignetteMode mode;
        public Color color;
        public Vector2 center;
        public float intensity;
        public float smoothness;
        public float roundness;
        public bool rounded;
        public bool enabled;

        public VignetteParameters(Vignette vignette)
        {
            mode = vignette.mode.value;
            color = vignette.color.value;
            center = vignette.center.value;
            intensity = vignette.intensity.value;
            smoothness = vignette.smoothness.value;
            roundness = vignette.roundness.value;
            rounded = vignette.rounded.value;
            enabled = vignette.enabled.value;
        }
    }

    [Serializable]
    public struct EcholocationParameters
    {
        public Color color;
        public int scale;
        public float depthThreshold;
        public float depthNormalThreshold;
        public float depthNormalThresholdScale;
        public float normalThreshold;
        public bool enabled;

        public EcholocationParameters(PostProcessEcholocation echolocation)
        {
            color = echolocation.color.value;
            scale = echolocation.scale.value;
            depthThreshold = echolocation.depthThreshold.value;
            depthNormalThreshold = echolocation.depthNormalThreshold.value;
            depthNormalThresholdScale = echolocation.depthNormalThresholdScale.value;
            normalThreshold = echolocation.normalThreshold.value;
            enabled = echolocation.enabled.value;
        }
    }

    [SerializeField, UnityEngine.Min(0f)] private float activationTime = 0.1f;

    [Header("Targets")]
    [SerializeField] private Vector2 targetCenter;
    [SerializeField, Range(0f, 1f)] private float targetIntensity;
    [SerializeField, Range(0f, 1f)] private float targetSmoothness;

    [Header("Curves")]
    [SerializeField] private AnimationCurve centerCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve smoothnessCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    
    public bool IsActive
    {
        get => isActive;
        private set
        {
            isActive = value;
            if (value)
            {
                if (focus != null)
                {
                    focus.Enable();
                    focus.performed += ToggleFocus;
                }
            }
            else
            {
                if (focus != null)
                {
                    focus.Disable();
                    focus.performed -= ToggleFocus;
                }
            }
        }
    }

    public bool CanFocus { get; private set; }
    public bool IsFocused { get; private set; }

    private PostProcessVolume volume;
    
    private Vignette vignette;
    private VignetteParameters initialVignetteParameters;

    private PostProcessEcholocation echolocation;
    private EcholocationParameters initialEcholocationParameters;

    private InputAction focus;
    private Coroutine toggleCoroutine;

    private bool isActive;

    private void Start()
    {
        InputManager inputManager = InputManager.Instance;
        if (inputManager == null)
        {
            Debug.LogWarning("InputManager instance not found...");
        }
        else
        {
            focus = inputManager.GameInputActions.InGame.Focus;

            IsActive = true;
        }

        volume = GetComponent<PostProcessVolume>();
        if (volume.sharedProfile.TryGetSettings(out vignette))
        {
            initialVignetteParameters = new VignetteParameters(vignette);
        }
        else
        {
            Debug.LogWarning("Vignette component not found...");
        }

        if (volume.sharedProfile.TryGetSettings(out echolocation))
        {
            initialEcholocationParameters = new EcholocationParameters(echolocation);
        }
        else
        {
            Debug.LogWarning("Echolocation component not found...");
        }

        CanFocus = true;
    }

    public void Focus()
    {
        IsFocused = true;
        echolocation.enabled.Override(IsFocused);
        GameEvents.FocusModeInvoke(IsFocused);
    }

    private void ToggleFocus(InputAction.CallbackContext context)
    {
        if (!CanFocus && !IsFocused)
        {
            return;
        }

        if (context.performed && toggleCoroutine == null)
        {
            if (!IsFocused)
            {
                UIFaderManager.Instance?.FadeOutSequence(1);
            }
            else
            {
                UIFaderManager.Instance?.FadeOutSequence(2);
            }

            toggleCoroutine = StartCoroutine(ToggleFocusCoroutine(activationTime));
        }
    }

    private IEnumerator ToggleFocusCoroutine(float toggleTime)
    {
        vignette.enabled.Override(true);

        float progress = 0f;
        if (IsFocused)
        {
            while (progress < 1f)
            {
                progress = Mathf.Min(1f, progress + Time.deltaTime / toggleTime);

                float evaluationTime = progress;
                vignette.center.Override(Vector2.Lerp(new Vector2(0.5f, -targetCenter.y), targetCenter, centerCurve.Evaluate(evaluationTime)));
                vignette.intensity.Override(Mathf.Lerp(0f, targetIntensity, intensityCurve.Evaluate(evaluationTime)));
                vignette.smoothness.Override(Mathf.Lerp(0f, targetSmoothness, smoothnessCurve.Evaluate(evaluationTime)));

                yield return null;
            }

            echolocation.enabled.Override(false);
        }

        progress = 0f;
        while (progress < 1f)
        {
            progress = Mathf.Min(1f, progress + Time.deltaTime / toggleTime);

            float evaluationTime = IsFocused ? 1f - progress : progress;
            vignette.center.Override(Vector2.Lerp(initialVignetteParameters.center, targetCenter, centerCurve.Evaluate(evaluationTime)));
            vignette.intensity.Override(Mathf.Lerp(initialVignetteParameters.intensity, targetIntensity, intensityCurve.Evaluate(evaluationTime)));
            vignette.smoothness.Override(Mathf.Lerp(initialVignetteParameters.smoothness, targetSmoothness, smoothnessCurve.Evaluate(evaluationTime)));

            yield return null;
        }

        IsFocused = !IsFocused;
        echolocation.enabled.Override(IsFocused);

        if (IsFocused)
        {
            progress = 0f;
            while (progress < 1f)
            {
                progress = Mathf.Min(1f, progress + Time.deltaTime / (toggleTime * 2f));

                float evaluationTime = progress;
                vignette.center.Override(Vector2.Lerp(targetCenter, new Vector2(0.5f, -targetCenter.y), centerCurve.Evaluate(evaluationTime)));
                vignette.intensity.Override(Mathf.Lerp(targetIntensity, 0f, intensityCurve.Evaluate(evaluationTime)));
                vignette.smoothness.Override(Mathf.Lerp(targetSmoothness, 0f, smoothnessCurve.Evaluate(evaluationTime)));

                yield return null;
            }

            vignette.enabled.Override(false);
            vignette.center.Override(targetCenter);
            vignette.intensity.Override(targetIntensity);
            vignette.smoothness.Override(targetSmoothness);
        }

        GameEvents.FocusModeInvoke(IsFocused);

        toggleCoroutine = null;
    }

    private void OnEnable()
    {
        IsActive = true;
    }

    private void OnDisable()
    {
        IsActive = false;

        // Reset vignette effect to initial state
        vignette.mode.Override(initialVignetteParameters.mode);
        vignette.color.Override(initialVignetteParameters.color);
        vignette.center.Override(initialVignetteParameters.center);
        vignette.intensity.Override(initialVignetteParameters.intensity);
        vignette.smoothness.Override(initialVignetteParameters.smoothness);
        vignette.roundness.Override(initialVignetteParameters.roundness);
        vignette.rounded.Override(initialVignetteParameters.rounded);
        vignette.enabled.Override(initialVignetteParameters.enabled);

        // Reset outline effect to initial state
        echolocation.color.Override(initialEcholocationParameters.color);
        echolocation.scale.Override(initialEcholocationParameters.scale);
        echolocation.depthThreshold.Override(initialEcholocationParameters.depthThreshold);
        echolocation.depthNormalThreshold.Override(initialEcholocationParameters.depthNormalThreshold);
        echolocation.depthNormalThresholdScale.Override(initialEcholocationParameters.depthNormalThresholdScale);
        echolocation.normalThreshold.Override(initialEcholocationParameters.normalThreshold);
        echolocation.enabled.Override(initialEcholocationParameters.enabled);
    }
}
