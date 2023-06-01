using UnityEngine;
using UnityEngine.Events;

using System.Collections;

public class TweenTransform : MonoBehaviour
{
    public bool AutoTween => autoTween;
    public bool BackAndForth => backAndForth;

    [Header("General")]
    [SerializeField] private bool autoTween;
    [SerializeField] private bool backAndForth;
    [SerializeField] private bool loop;

    [Header("Position Tweening")]
    [SerializeField] private float positionTweenTime = 1f;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private AnimationCurve xPositionCurve = AnimationCurve.Constant(0f, 1f, 0f);
    [SerializeField] private AnimationCurve yPositionCurve = AnimationCurve.Constant(0f, 1f, 0f);
    [SerializeField] private AnimationCurve zPositionCurve = AnimationCurve.Constant(0f, 1f, 0f);

    [Header("Position Callbacks")]
    [SerializeField] private UnityEvent onStartPositionEnter;
    [SerializeField] private UnityEvent onStartPositionExit;

    [SerializeField] private UnityEvent onEndPositionEnter;
    [SerializeField] private UnityEvent onEndPositionExit;

    [Header("Rotation Tweening")]
    [SerializeField] private float rotationTweenTime = 1f;
    [SerializeField] private Vector3 targetRotation;
    [SerializeField] private AnimationCurve xRotationCurve = AnimationCurve.Constant(0f, 1f, 0f);
    [SerializeField] private AnimationCurve yRotationCurve = AnimationCurve.Constant(0f, 1f, 0f);
    [SerializeField] private AnimationCurve zRotationCurve = AnimationCurve.Constant(0f, 1f, 0f);

    [Header("Tween Callbacks")]
    [SerializeField] private UnityEvent onTweenStart;
    [SerializeField, HideInInspector] private UnityEvent onTweenHalfway;
    [SerializeField, HideInInspector] private UnityEvent onTweenFinish;
    [SerializeField, HideInInspector] private UnityEvent onTweenStop;

    private Vector3 initialPosition;
    private Vector3 initialRotation;

    private Coroutine positionTweenCoroutine;
    private Coroutine rotationTweenCoroutine;

    private bool isActive;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localEulerAngles;

        if (autoTween)
        {
            StartTween();
        }
    }

    private void Update()
    {
        if (isActive && loop)
        {
            TweenPosition();
            TweenRotation();
        }
    }

    public void StartTween()
    {
        isActive = true;
        TweenPosition();
        TweenRotation();
    }

    public void StopTween()
    {
        isActive = false;
        StopAllCoroutines();
        positionTweenCoroutine = null;
        rotationTweenCoroutine = null;

        onTweenStop?.Invoke();
    }

    public void ToggleTween()
    {
        if (!isActive)
        {
            StartTween();
        }
        else
        {
            StopTween();
        }
    }

    private void TweenPosition()
    {
        if (positionTweenCoroutine == null)
        {
            positionTweenCoroutine = StartCoroutine(TweenPositionCoroutine(positionTweenTime));
        }
    }

    private void TweenRotation()
    {
        if (rotationTweenCoroutine == null)
        {
            rotationTweenCoroutine = StartCoroutine(TweenRotationCoroutine(rotationTweenTime));
        }
    }

    private IEnumerator TweenPositionCoroutine(float tweenTime)
    {
        bool hasExitedStartPosition = false;
        bool hasEnteredEndPosition = false;

        Vector3 startPosition = transform.localPosition;

        onTweenStart?.Invoke();

        float progress = 0f;
        while (progress < 1f)
        {
            progress = Mathf.Min(1f, progress + Time.deltaTime / tweenTime);
            float evaluationTime = progress;

            Vector3 newPosition = new Vector3
            {
                x = Mathf.LerpUnclamped(startPosition.x, targetPosition.x, xPositionCurve.Evaluate(evaluationTime)),
                y = Mathf.LerpUnclamped(startPosition.y, targetPosition.y, yPositionCurve.Evaluate(evaluationTime)),
                z = Mathf.LerpUnclamped(startPosition.z, targetPosition.z, zPositionCurve.Evaluate(evaluationTime)),
            };
            transform.localPosition = newPosition;

            if (!hasExitedStartPosition && newPosition != startPosition)
            {
                hasExitedStartPosition = true;
                onStartPositionExit?.Invoke();
            }

            if (!hasEnteredEndPosition && newPosition == targetPosition)
            {
                hasEnteredEndPosition = true;
                onEndPositionEnter?.Invoke();
            }

            yield return null;
        }

        if (backAndForth)
        {
            bool hasExitedEndPosition = false;
            bool hasEnteredStartPosition = false;

            onTweenHalfway?.Invoke();

            progress = 0f;
            while (progress < 1f)
            {
                progress = Mathf.Min(1f, progress + Time.deltaTime / tweenTime);
                float evaluationTime = 1f - progress;

                Vector3 newPosition = new Vector3
                {
                    x = Mathf.LerpUnclamped(initialPosition.x, targetPosition.x, xPositionCurve.Evaluate(evaluationTime)),
                    y = Mathf.LerpUnclamped(initialPosition.y, targetPosition.y, yPositionCurve.Evaluate(evaluationTime)),
                    z = Mathf.LerpUnclamped(initialPosition.z, targetPosition.z, zPositionCurve.Evaluate(evaluationTime))
                };
                transform.localPosition = newPosition;

                if (!hasExitedEndPosition && newPosition != targetPosition)
                {
                    hasExitedEndPosition = true;
                    onEndPositionExit?.Invoke();
                }

                if (!hasEnteredStartPosition && newPosition == startPosition)
                {
                    hasEnteredStartPosition = true;
                    onStartPositionEnter?.Invoke();
                }

                yield return null;
            }
        }

        onTweenFinish?.Invoke();

        positionTweenCoroutine = null;
    }

    private IEnumerator TweenRotationCoroutine(float tweenTime)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress = Mathf.Min(1f, progress + Time.deltaTime / tweenTime);
            float evaluationTime = progress;

            Vector3 newRotation = new Vector3
            {
                x = Mathf.LerpUnclamped(initialRotation.x, targetRotation.x, xRotationCurve.Evaluate(evaluationTime)),
                y = Mathf.LerpUnclamped(initialRotation.y, targetRotation.y, yRotationCurve.Evaluate(evaluationTime)),
                z = Mathf.LerpUnclamped(initialRotation.z, targetRotation.z, zRotationCurve.Evaluate(evaluationTime)),
            };
            transform.localEulerAngles = newRotation;

            yield return null;
        }

        rotationTweenCoroutine = null;
    }
}
