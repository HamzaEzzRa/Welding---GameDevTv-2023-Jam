using UnityEngine;

[System.Serializable]
public struct Vector3Range
{
    [SerializeField] private Vector3 min, max;

    public Vector3 Min => min;

    public Vector3 Max => max;

    public Vector3 RandomInRange => new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

    public Vector3Range(Vector3 value) { min = max = value; }

    public Vector3Range(Vector3 min, Vector3 max)
    {
        this.min = min;
        this.max = new Vector3
        {
            x = max.x < min.x ? min.x : max.x,
            y = max.y < min.y ? min.y : max.y,
            z = max.z < min.z ? min.z : max.z,
        };
    }
}
