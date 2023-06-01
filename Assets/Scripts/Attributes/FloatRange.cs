using UnityEngine;

[System.Serializable]
public struct FloatRange
{
    [SerializeField] private float min, max;

    public float Min => min;

    public float Max => max;

    public float RandomInRange => Random.Range(min, max);

    public FloatRange(float value) { min = max = value; }

    public FloatRange(float min, float max)
    {
        this.min = min;
        this.max = max < min ? min : max;
    }
}

public class FloatRangeSliderAttribute : PropertyAttribute
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public FloatRangeSliderAttribute(float min, float max)
    {
        Min = min;
        Max = max < min ? min : max;
    }
}
